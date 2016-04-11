using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	public static class DataTableHelperUtility
	{
		public static DataTable AddColumns(DataTable dt, string sLine)
		{
			List<String> Splitter = StringUtility.LowMemSplit(sLine, "\t");
			foreach (string s in Splitter)
			{
				dt.Columns.Add(s);
			}
			return dt;
		}
		public static DataTable AddRows(DataTable dt, string sLine)
		{
			List<String> Splitter = StringUtility.LowMemSplit(sLine, "\t");
			dt.Rows.Add(Splitter.ToArray());
			return dt;
		}

	}
}
