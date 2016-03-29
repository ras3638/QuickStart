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
			
			//SQL:
			//SELECT [table] FROM SchemaDataset WHERE [fk_name] = sFKName
			//put results in TableResultsHashSet

			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				(p => p.Field<string>("fk_name") == sFK.Trim().ToUpper());

			foreach (DataRow row in Query1)
			{
				string sTable = row["table"].ToString();
				TableResultsHashSet.Add(sTable.Trim());
			}

			//SQL:
			//SELECT [fk_name] FROM SchemaDataset WHERE [referenced_table] IN TableList
			//put results in FKResultsHashSet

			var Query2 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				(p => TableResultsHashSet.Contains(p.Field<string>("referenced_table")));

			foreach (DataRow row in Query2)
			{
				string sFKName2 = row["fk_name"].ToString();
				FKResultsHashSet.Add(sFKName2.Trim());				
			}

			return FKResultsHashSet;
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
			if(temp0.ToString().Contains("@@Error:")) return sb;
			
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
		public static StringBuilder CascadeDeleteAllEngine(DataTable dtDatabaseSchema, string sFKName, bool bUseIncrements, string sSource)
		{
			StringBuilder sb = new StringBuilder();
			HashSet<string> RefTableHashSet = new HashSet<string>();
			HashSet<string> FKNameHashSet = new HashSet<string>();

			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
					(p => p.Field<string>("fk_name") == sFKName.Trim());

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
				string sFK_Name = row["fk_name"].ToString();
				FKNameHashSet.Add(sFK_Name);
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
			StringBuilder sb = new StringBuilder(String.Empty);
			sSource = StringUtility.SqlToUpper(sSource);
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
			else
			{
				sAlias = GetAlias(sSource);
			}

			IncrementAlphabet A = new IncrementAlphabet(sAlias);

			var Query1 = dtDatabaseSchema.Rows.Cast<DataRow>().Where
				(p => p.Field<string>("fk_name") == sFKName.Trim().ToUpper());

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
					sb.Append(A.GetPrevious() + "." + sColumn + " = " + A.GetCurrent() + "." + sReferenced_Column);
				}
				else
				{
					sb.Append(" AND " + A.GetPrevious() + "." + sColumn + " = " + A.GetCurrent() + "." + sReferenced_Column);
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

			sb.Append(SetWhereClause(sSource));
			sb.Append(")");

			if (bUseIncrements)
			{
				//Lets manage two copies of sb.
				StringBuilder sb2 = new StringBuilder(sb.ToString());
				sb.Insert(0, sWhileExists);
				sb.Append(")\n");
				sb2.Insert(0, sDeleteTop);
				sb.Append(sb2.ToString());
			}
			else
			{
				sb.Insert(0, sDeleteAll);
			}

			return sb;
		}
		public static StringBuilder InsertEngine(string sSource, string sIdentityInsert, string sTableName)
		{
			StringBuilder sb = new StringBuilder();

			if (!string.IsNullOrWhiteSpace(sIdentityInsert))
			{
				sb.AppendLine(SetIdentityInsert(sIdentityInsert, sTableName));
			}

			string sRecordSet = sSource;
			//Remove the first line. These are columns
			int iFirstNewLineIndex = sRecordSet.IndexOf("\n");
			sRecordSet = sRecordSet.Substring(iFirstNewLineIndex);

			//Handles Apostrophes
			//RecordSet = RecordSet.Replace("'", "''");
			sRecordSet = new StringBuilder(sRecordSet).Replace("'", "''").ToString();

			//Handle insert statements and general formatting
			//RecordSet = RecordSet.Replace("\t", "','");
			sRecordSet = new StringBuilder(sRecordSet).Replace("\t", "','").ToString();

			//RecordSet = RecordSet.Replace("\n", "')\n");
			sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "')\n").ToString();

			int iFirstNewLineIndex2 = sSource.IndexOf('\n');
			string sFirstLine = sSource.Substring(0, iFirstNewLineIndex2);
			List<string> ColumnList = StringUtility.LowMemSplit(sFirstLine, "\t");
			string sInsertStatement = SqlGenEngine.GenerateInsertStatement(ColumnList, sTableName).ToString();
			sRecordSet = new StringBuilder(sRecordSet).Replace("\n", "\r\n" + sInsertStatement).ToString();

			//Handle ending and beginning
			sRecordSet = sRecordSet.Remove(0, 4);
			sRecordSet = sRecordSet.Insert(sRecordSet.Length, "')");

			//Handle Nulls
			//RecordSet = RecordSet.Replace("'NULL'", "NULL");
			sRecordSet = new StringBuilder(sRecordSet).Replace("'NULL'", "NULL").ToString();

			//Handle GO statements every 5000 rows
			sRecordSet = StringUtility.ReplaceNthOccurrence(new StringBuilder(sRecordSet), "\n", "\nGO\r\n", 5000).ToString();
			sb.Append(sRecordSet);

			if (!string.IsNullOrWhiteSpace(sIdentityInsert))
			{
				sb.Append("\r\n" + SetIdentityInsertInverse(sIdentityInsert, sTableName));
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
				sTemp = " AND " + CustomWhereClause;
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
