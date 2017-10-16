using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	public static class SqlGenEngine
	{
		public static HashSet<string> QueryFKHashSet(DataTable dtDatabaseSchema, string sFK)
		{
			HashSet<string> FKResultsHashSet = new HashSet<string>();
			HashSet<string> TableResultsHashSet = new HashSet<string>();
			HashSet<string> RefTableResultsHashSet = new HashSet<string>();
			//SQL:
			//SELECT [table] FROM SchemaDataset WHERE [fk_name] = sFKName and [constraint_type] = "fk"
			//put results in TableResultsHashSet

			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
                //(p => (p.Field<string>("constraint_type") == "fk");
            (p => (p.Field<string>("constraint_name") == sFK.Trim().ToUpper())
            && (p.Field<string>("constraint_type") == "fk"));

            foreach (DataRow row in Query1)
			{
				string sTable = row["table"].ToString();
				TableResultsHashSet.Add(sTable.Trim());
			}

			//Avoid circular reference

			foreach (DataRow row in Query1)
			{
				string sRefTable = row["referenced_table"].ToString();
				RefTableResultsHashSet.Add(sRefTable.Trim());
			}
			if (RefTableResultsHashSet.Overlaps(TableResultsHashSet))
			{
				//This is a circular reference
				return FKResultsHashSet;
			}
			//SQL:
			//SELECT [fk_name] FROM SchemaDataset WHERE [referenced_table] IN TableList
			//put results in FKResultsHashSet

			var Query2 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				(p => TableResultsHashSet.Contains(p.Field<string>("referenced_table")));

			foreach (DataRow row in Query2)
			{
				string sFKName2 = row["constraint_name"].ToString();
				FKResultsHashSet.Add(sFKName2.Trim());				
			}

			return FKResultsHashSet;
		}
        public static StringBuilder GenerateUpdateStatement(List<string> ColumnList, string sTableName, List<string> PKList)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbWhereClause = new StringBuilder();

            bool bWhereClauseFirstIter = true;
            bool bSetClauseFirstIter = true;

            for (int i = 0; i < ColumnList.Count; i++)
            {
                if (PKList.Contains(ColumnList[i]) && bWhereClauseFirstIter)
                {
                    sbWhereClause.Append(" WHERE " + ColumnList[i] + " = '{" + i + "}'");
                    bWhereClauseFirstIter = false;
                }
                else if (PKList.Contains(ColumnList[i]) && !bWhereClauseFirstIter)
                {
                    sbWhereClause.Append(" AND " + ColumnList[i] + " = '{" + i + "}'");
                }
                else
                {
                    if (bSetClauseFirstIter)
                    {
                        sb.Append("UPDATE " + sTableName + " SET " + ColumnList[i] + " = '{" + i + "}'");
                        bSetClauseFirstIter = false;
                    }
                    else
                    {
                        //last statement
                        sb.Append(", " + ColumnList[i] + " = '{" + i + "}'");
                    }
                }

            }
            return sb.Append(sbWhereClause);
        }
        public static bool IsUpdateColumnPK(string sColumn, string sTableName)
        {
            return true;
        }

        public static StringBuilder GenerateInsertStatement(List<string> ColumnList, string sTableName)
		{
			StringBuilder sb = new StringBuilder();
	
			sb.Append("INSERT INTO " + sTableName + " (");

			for (int i = 0; i < ColumnList.Count; i++)
			{
				if (i == ColumnList.Count - 1)
				{
					//last statement
					sb.Append(ColumnList[i] + ") VALUES ('");
				}
				else
				{
					sb.Append(ColumnList[i] + ",");		
				}
			}
			return sb;
		}
		public static StringBuilder FKRecursionEngine(DataTable dtDatabaseSchema, bool bUseIncrements, string sSource, HashSet<string> FKToUseHashSet, StringBuilder sb)
		{
			HashSet<string> FKToQueryHashSet = new HashSet<string>();
			StringBuilder temp1 = new StringBuilder();

			if (FKToUseHashSet.Count() == 0) return sb;
			
			foreach (string s in FKToUseHashSet)
			{	
				temp1 = CascadeDeleteSingleEngine(dtDatabaseSchema, s, bUseIncrements, sSource);

				sb.Insert(0, "\n");
				sb.Insert(0, temp1);
				
				FKToQueryHashSet = QueryFKHashSet(dtDatabaseSchema, s);
				FKRecursionEngine(dtDatabaseSchema, bUseIncrements, temp1.ToString(), FKToQueryHashSet, sb);
			}
			return sb;
		}		
		public static StringBuilder CascadeDeleteFullEngine(DataTable dtDatabaseSchema, string sFKName, bool bUseIncrements, string sSource)
		{
			StringBuilder sb = new StringBuilder();
			HashSet<string> FKResultsHashSet = new HashSet<string>();
		
			FKResultsHashSet = QueryFKHashSet(dtDatabaseSchema, sFKName);

			//bUseIncrements will be false in this case. Use temp0 as the starting point
			StringBuilder temp0 = CascadeDeleteSingleEngine(dtDatabaseSchema, sFKName, false, sSource);

			//Exit out if there is an error
			if(temp0.ToString().Contains("@@Error:")) return temp0;
			
			//bUseIncrements will be false in this case.
			sb.Append(FKRecursionEngine(dtDatabaseSchema, false, temp0.ToString(), FKResultsHashSet, new StringBuilder()));
			sb.Append(temp0);

			if (bUseIncrements)
			{
				List<string> DeleteStatementList = StringUtility.HighMemSplit(sb.ToString(), "\n");
				string sWhileExists = "WHILE EXISTS (SELECT 1 ";
				sb.Clear();

				foreach (string sDelStatement in DeleteStatementList)
				{
					int iIndexFrom = sDelStatement.IndexOf("FROM ");
					string sTemp1 = sDelStatement.Substring(iIndexFrom);
					sb.Append(sWhileExists + sTemp1);
					sb.Append(")\n");

					//Get the alias being used
					string sAlias = GetAlias(sDelStatement);
					string sDeleteTop = "DELETE TOP (100000) " + sAlias + " ";
					sb.Append(sDeleteTop + sTemp1);
					sb.Append("\n\n");
				}
			}

			sb.Insert(0,"--Foriegn Key Cascade: " + sFKName + "\n");
			
			return sb;
		}
		public static string GetTableToDelete(string sSource)
		{
			sSource = StringUtility.SqlToUpper(sSource);
			if (!sSource.Contains("DELETE FROM "))
			{
				return String.Empty;
			}
			else if (StringUtility.HighMemSplit(sSource, "DELETE ").Count > 2)
			{
				return String.Empty;
			}

			int indexDelete = sSource.IndexOf("DELETE FROM ") + 12;
			string temp = sSource.Substring(indexDelete);

			List<string> Splitter = StringUtility.HighMemSplit(temp, " ");
			if (Splitter.Count == 1)
			{
				return Splitter[0].Trim();
			}
			else if (Splitter.Count >= 1)
			{
				if (temp.Contains(")"))
				{
					int indexRightPara = temp.IndexOf(")") + 1;
					//return temp.Substring(indexRightPara); ??
				}

				return Splitter[0].Trim();
			}
			else
			{
				return String.Empty;
			}
		}
		public static string GetWhereClause(string sSource)
		{
			sSource = StringUtility.SqlToUpper(sSource);
			if(!sSource.Contains("WHERE "))
			{
				return String.Empty;
			}
			int iWhereIndex = sSource.IndexOf("WHERE ");
			return " " + sSource.Substring(iWhereIndex).Trim();
		}
		public static StringBuilder CascadeDeleteNoneEngine(DataTable dtDatabaseSchema, string sFKName, bool bUseIncrements, string sSource)
		{		
			StringBuilder sb3 = new StringBuilder();
			sSource = StringUtility.SqlToUpper(sSource);
			sSource = StringUtility.SqlRemoveExtraWhiteSpace(sSource);
			sSource = sSource.TrimStart(new char[] { '\n' });
			sSource = sSource.TrimEnd(new char[] { '\n' });
			string sAlias = String.Empty;
			string sWhileExists = String.Empty;
			string sDeleteTop = String.Empty;
			string sDeleteAll = String.Empty;
			List<string> Splitter = new List<string> {sSource};

			if ((bool)SettingManager.GetSettingValue("Cascade Delete Gen", "Multiline support for None cascade option"))
			{
				if (sSource.Contains("\n"))
				{
					//Multiline
					Splitter = StringUtility.HighMemSplit(sSource, "\n");
				}
			}
			
			foreach (string s in Splitter)
			{
				StringBuilder sb = new StringBuilder();
				if (!s.Contains("DELETE "))
				{
					sb.Clear();
					sb.Append("@@Error: Delete keyword not found.");
					return sb;
				}
				else if (StringUtility.HighMemSplit(s, "DELETE ").Count > 2)
				{
					sb.Clear();
					sb.Append("@@Error: Multiple delete keywords are not supported.");
					return sb;
				}
				else if (s.Contains("TOP "))
				{
					sb.Clear();
					sb.Append("@@Error: Top keyword not supported.");
					return sb;
				}
				else if (s.Contains("WHILE "))
				{
					sb.Clear();
					sb.Append("@@Error: While keyword not supported.");
					return sb;
				}
				else
				{
					sAlias = GetAlias(s);
				}
				IncrementAlphabet A = new IncrementAlphabet(sAlias);

				sWhileExists = "WHILE EXISTS (SELECT 1";
				sDeleteTop = "DELETE TOP (100000) " + A.GetCurrent();
				sDeleteAll = "DELETE " + A.GetCurrent();

				sb.Append(" FROM " + GetTableToDelete(s) + " " + A.GetCurrent() + GetWhereClause(s));

				if (bUseIncrements)
				{
					//Lets manage two copies of sb.
					StringBuilder sb2 = new StringBuilder(sb.ToString());
					sb.Insert(0, sWhileExists);
					sb.Append(")");
					sb2.Insert(0, sDeleteTop);
					sb.Append(sb2);
				}
				else
				{
					sb.Insert(0, sDeleteAll);
				}
				if(Splitter.Count > 1) sb.Append("\n");
				sb3.Append(sb);
			}
			
			return sb3;
		}
		public static StringBuilder CascadeDeleteAllEngine(DataTable dtDatabaseSchema, string sFKName, bool bUseIncrements, string sSource)
		{
			StringBuilder sb = new StringBuilder();
			HashSet<string> RefTableHashSet = new HashSet<string>();
			HashSet<string> FKNameHashSet = new HashSet<string>();

			//Allow FK name to be optional when selecting cascade "All"
			if (String.IsNullOrEmpty(sFKName))
			{
				//Locate the table to delete in sSource
				string sReferencedTable = GetTableToDelete(sSource);
				RefTableHashSet.Add(sReferencedTable);
			}
			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
					//(p => p.Field<string>("fk_name") == sFKName.Trim());
                    (p => (p.Field<string>("constraint_name") == sFKName.Trim().ToUpper())
                    && (p.Field<string>("constraint_type") == "fk"));

            foreach (DataRow row in Query1)
			{
				string sReferenced_Table = row["referenced_table"].ToString();
				RefTableHashSet.Add(sReferenced_Table);
			}

			if (RefTableHashSet.Count() > 1)
			{
				//Each single FK should only have one referenced table. Look at Sql.sLoadSchema
				throw new NotImplementedException();
			}

			var Query2 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				(p => RefTableHashSet.Contains(p.Field<string>("referenced_table")));

			foreach (DataRow row in Query2)
			{
                //string sFK_Name = row["fk_name"].ToString();
                string sFK_Name = row["constraint_name"].ToString();
				FKNameHashSet.Add(sFK_Name);
			}

			if (FKNameHashSet.Count == 0)
			{
				sb.Clear();
				sb.Append("@@Error: The specified FK was not found in the schema.");
				return sb;
			}

			foreach (string sFK in FKNameHashSet)
			{
				sb.Append(CascadeDeleteFullEngine(dtDatabaseSchema, sFK, bUseIncrements, sSource));
				sb.Append("\n");
			}
			//Remove last \n
			int indexNewLine = sb.ToString().LastIndexOf("\n");
			sb.Remove(indexNewLine, 1);

			return sb;
		}
		public static StringBuilder CascadeDeleteSingleEngine(DataTable dtDatabaseSchema, string sFKName, bool bUseIncrements, string sSource)
		{
			StringBuilder sb = new StringBuilder();
			sSource = StringUtility.SqlToUpper(sSource);
			sSource = StringUtility.SqlRemoveExtraWhiteSpace(sSource);
			string sAlias = String.Empty;
			string sWhileExists = String.Empty;
			string sDeleteTop = String.Empty;
			string sDeleteAll = String.Empty;

			bool bFirstIteration = true;
			int iIterationCount = 0;

			if (!sSource.Contains("DELETE "))
			{
				sb.Clear();
				sb.Append("@@Error: Delete keyword not found.");
				return sb;
			}
			else if (StringUtility.HighMemSplit(sSource, "DELETE ").Count > 2)
			{
				sb.Clear();
				sb.Append("@@Error: Multiple delete keywords are not supported.");
				return sb;
			}
			else if (sSource.Contains("TOP "))
			{
				sb.Clear();
				sb.Append("@@Error: Top keyword not supported.");
				return sb;
			}
			else if (sSource.Contains("WHILE "))
			{
				sb.Clear();
				sb.Append("@@Error: While keyword not supported.");
				return sb;
			}
			else
			{
				sAlias = GetAlias(sSource);
			}

			IncrementAlphabet A = new IncrementAlphabet(sAlias);

			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				//(p => p.Field<string>("fk_name") == sFKName.Trim());
                (p => (p.Field<string>("constraint_name") == sFKName.Trim().ToUpper())
                && (p.Field<string>("constraint_type") == "fk"));

            foreach (DataRow row in Query1)
			{
				string sColumn = row["column"].ToString();
				string sReferenced_Column = row["referenced_column"].ToString();
				string sTable = row["table"].ToString();
				string sReferenced_Table = row["referenced_table"].ToString();

				if (bFirstIteration)
				{
					sWhileExists = "WHILE EXISTS (SELECT 1";
					sDeleteTop = "DELETE TOP (100000) " + A.GetCurrent();
					sDeleteAll = "DELETE " + A.GetCurrent();

					sb.Append(" FROM " + sTable + " " + A.GetCurrent() + " WHERE EXISTS");
					
					sb.Append("(SELECT * FROM " + sReferenced_Table + " " + A.GetPrevious() + " WHERE ");
					sb.Append(A.GetPrevious() + "." + sReferenced_Column + " = " + A.GetCurrent() + "." + sColumn);
				}
				else
				{
					sb.Append(" AND " + A.GetPrevious() + "." + sReferenced_Column + " = " + A.GetCurrent() + "." + sColumn);
				}

				bFirstIteration = false;
				iIterationCount++;
			}

			if (iIterationCount == 0)
			{
				sb.Clear();
				sb.Append("@@Error:The specified FK was not found in the schema.");
				return sb;
			}

			//Handle the first where clause

			sSource = sSource.Replace(@"\", string.Empty).Trim();

			if (sSource.Contains("JOIN"))
			{
				sb.Clear();
				sb.Append("@@Error:Join keyword not supported.");
				return sb;
			}

			//Added additional parantheses to handle OR cases
			sb.Append(SetWhereClause(sSource));
			sb.Append(")");

			if (bUseIncrements)
			{
				//Lets manage two copies of sb.
				StringBuilder sb2 = new StringBuilder(sb.ToString());
				sb.Insert(0, sWhileExists);
				sb.Append(")\n");
				sb2.Insert(0, sDeleteTop);
				sb.Append(sb2);
			}
			else
			{
				sb.Insert(0, sDeleteAll);
			}

			return sb;
		}

        public static StringBuilder UpdateGenEngine(DataTable dtDatabaseSchema, string sTableName, string sSource)
        {
            StringBuilder sb = new StringBuilder();
            string sRecordSet = sSource;

            //Remove the first line. These are columns
            int iFirstNewLineIndex = sRecordSet.IndexOf("\n");
            sRecordSet = sRecordSet.Substring(iFirstNewLineIndex);

            //Handles Apostrophes
            sRecordSet = new StringBuilder(sRecordSet).Replace("'", "''").ToString();

            //Get PK columns
            List<string> PKList = new List<string>();
            var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
                (p => (p.Field<string>("table") == sTableName.Trim())
                && (p.Field<string>("constraint_type") == "pk"));

            foreach (DataRow row in Query1)
            {
                string sColumn = row["column"].ToString();
                PKList.Add(sColumn);

            }

            string sFirstLine = sSource.Substring(0, iFirstNewLineIndex);
            List<string> ColumnList = StringUtility.LowMemSplit(sFirstLine, "\t");
            string sUpdateStatement = SqlGenEngine.GenerateUpdateStatement(ColumnList, sTableName, PKList).ToString();
            sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "\r\n" + sUpdateStatement).ToString();


            //Handle ending and beginning
            sRecordSet = sRecordSet.Remove(0, 2);
            List<string> Splitter = StringUtility.LowMemSplit(sRecordSet, "\r\n");
            foreach(string s in Splitter)
            {
                int iLast = s.LastIndexOf("}'");
                string sLast = s.Substring(iLast + 2);

                string sFirst = s.Substring(0, iLast + 2);

                List<string> InnerSplitter = StringUtility.LowMemSplit(sLast, "\t");
                for (int i = 0; i < InnerSplitter.Count(); i++)
                {
                    sFirst = sFirst.Replace("{" + i + "}", InnerSplitter[i]);
                }
                sb.Append(sFirst + "\r\n");
                
            }
            sb.Replace("'NULL'", "NULL");

            if(PKList.Count == 0)
            {
                sb.Insert(0, "--@@Warning: Schema did not contain any primary keys for table " + sTableName + ". Where clause could not be generated \r\n");
                return sb;
            }
            if (!StringUtility.ContainsAllItems(ColumnList, PKList))
            {
                sb.Insert(0,"--@@Warning: Not all primary keys found for table " + sTableName + ". Verify your where clause \r\n");
                return sb;
            }

            return sb;
        }
		public static StringBuilder InsertEngine(string sSource, string sIdentityInsert, string sTableName)
		{
			StringBuilder sb = new StringBuilder();
			string sRecordSet = sSource;
			bool bUseOracle = (bool)SettingManager.GetSettingValue("Insert Gen", "Enable Oracle Functionality");
			bool bUseExcel = (bool)SettingManager.GetSettingValue("Insert Gen", "Enable Excel Functionality");

			if (!string.IsNullOrWhiteSpace(sIdentityInsert))
			{
				sb.AppendLine(SetIdentityInsert(sIdentityInsert, sTableName));
			}

			//Allows copy/paste from Excel
			if (bUseExcel)
			{
				sRecordSet = sRecordSet.Trim().Replace("\t\n", "\n"); 
			}

			//Remove the first line. These are columns
			int iFirstNewLineIndex = sRecordSet.IndexOf("\n");
			sRecordSet = sRecordSet.Substring(iFirstNewLineIndex);

			//Handles Apostrophes
			sRecordSet = new StringBuilder(sRecordSet).Replace("'", "''").ToString();

			//Handle insert statements and general formatting
			sRecordSet = new StringBuilder(sRecordSet).Replace("\t", "','").ToString();

			//Handle semicolon for Oracle
			if (bUseOracle)
				sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "');\n").ToString();
			else
				sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "')\n").ToString();

			int iFirstNewLineIndex2 = sSource.IndexOf('\n');
			string sFirstLine = sSource.Substring(0, iFirstNewLineIndex2);
			List<string> ColumnList = StringUtility.LowMemSplit(sFirstLine, "\t");
			string sInsertStatement = SqlGenEngine.GenerateInsertStatement(ColumnList, sTableName).ToString();
			sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "\r\n" + sInsertStatement).ToString();

			//Handle ending and beginning
			sRecordSet = sRecordSet.Remove(0, 4);

			//Handle semicolon for Oracle for last statement
			if (bUseOracle)
				sRecordSet = sRecordSet.Insert(sRecordSet.Length, "');");
			else
				sRecordSet = sRecordSet.Insert(sRecordSet.Length, "')");			

			//Handle Nulls
			sRecordSet = new StringBuilder(sRecordSet).Replace("'NULL'", "NULL").ToString();

			//Handle GO statements every 5000 rows for MSSQL
			if (!bUseOracle)
			{
				sRecordSet = StringUtility.ReplaceNthOccurrence(new StringBuilder(sRecordSet), "\n", "\nGO\r\n", 5000).ToString();
			}
			sb.Append(sRecordSet);

			//Handle Identity Insert for MSSQL
			if (!string.IsNullOrWhiteSpace(sIdentityInsert) && !bUseOracle)
			{
				sb.Append("\r\n" + SetIdentityInsertInverse(sIdentityInsert, sTableName));
			}

			//RecordSet = RecordSet.Replace("\n", "')\n");
			//Add Begin/End for Oracle
			if (bUseOracle)
			{
				sb.Insert(0, "BEGIN");
				sb.Append("\nEND;");	
			}
			
			return sb;
		}
		public static string SetWhereClause(string sSource)
		{
			string sTemp = String.Empty;

			if (sSource.Contains("WHERE"))
			{
				int iWhereIndex = sSource.IndexOf("WHERE") + 5;
				string CustomWhereClause = sSource.Substring(iWhereIndex).Trim();
				//Add paranthesis for cases when 'OR' is used. 
				sTemp = " AND (" + CustomWhereClause + ")";
			}
			return sTemp;
		}
		public static string GetAlias(string sSource)
		{
			if (!sSource.Contains("DELETE ") || !sSource.Contains("FROM "))
			{
				return String.Empty;
			}

			int indexDelete = sSource.IndexOf("DELETE ") + 7;
			int indexFrom = sSource.IndexOf("FROM");
			string temp = sSource.Substring(indexDelete, indexFrom - indexDelete);

			if (temp.Contains(")"))
			{
				int indexRightPara = temp.IndexOf(")") + 1;
				temp = temp.Substring(indexRightPara);
			}

			return temp.Trim();
		}
		public static string SetIdentityInsert(string sIDInsert, string sTableName)
		{
			return "SET IDENTITY_INSERT " + sTableName + " " + sIDInsert;
		}
		public static string SetIdentityInsertInverse(string sIDInsert, string sTableName)
		{
			sIDInsert = (sIDInsert == "OFF") ? "ON" : "OFF";
			return "SET IDENTITY_INSERT " + sTableName + " " + sIDInsert;
		}
		public static string AppendOriginalDelStatement(string sSource)
		{
			sSource = sSource.Replace(@"\", string.Empty).Trim();
			sSource = "\n" + "--Original delete statement\n" + StringUtility.SqlToUpper(sSource);
			return sSource;
		}
	}
}
