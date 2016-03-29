using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickStart
{
	public static class StringUtility
	{
		public static List<string> HighMemSplit(this string s, string sDelimiter)
		{
			//Returns a list of delimited strings. 
			//Uses Split() function

			List<string> list = new List<string>();

			string[] splitter = s.Split(new string[] { sDelimiter }, StringSplitOptions.None);
			foreach (string line in splitter)
			{
				list.Add(line);
			}
			return list;
		}
		public static List<string> LowMemSplit(this string s, string sDelimiter)
		{
			//Returns a list of delimited strings. 
			//More effecient when working with large strings
			//Note: If delimiter is found twice in a row, it will not return a blank space in between. Use HighMemSplit instead

			List<string> list = new List<string>();

			int lastPos = 0;
			int pos = s.IndexOf(sDelimiter);

			if (sDelimiter == String.Empty || pos == -1)
			{
				list.Add(s);
				return list;
			}

			while (pos > -1)
			{
				while (pos == lastPos)
				{
					lastPos += sDelimiter.Length;

					if (s.IndexOf(sDelimiter, lastPos) == -1) break;
					else 
						pos = s.IndexOf(sDelimiter, lastPos);
				}

				if (s.IndexOf(sDelimiter, lastPos) == -1) break;

				string tmp = s.Substring(lastPos, pos - lastPos);
				if (tmp.Trim().Length > 0)
					list.Add(tmp);
				lastPos = pos + sDelimiter.Length;
				pos = s.IndexOf(sDelimiter, lastPos);
			}

			if (lastPos < s.Length)
			{
				string tmp = s.Substring(lastPos, s.Length - lastPos);
				if (tmp.Trim().Length > 0)
					list.Add(tmp);		
			}
			return list;
		}		
		public static int NthIndexOf(string sSource, char cFinder, int iNth)
		{
			//This works with char finder
			int count = 0;
			for (int i = 0; i < sSource.Length; i++)
			{
				if (sSource[i] == cFinder)
				{
					count++;
					if (count == iNth)
					{
						return i;
					}
				}
			}
			return -1;
		}
		public static int NthIndexOf(string sSource, string sFinder, int iNth)
		{
			//Works with string finder
			int count = 0;
			for (int i = 0; i < sSource.Length; i++)
			{
				if (sSource.Substring(i, sFinder.Length) == sFinder)
				{
					count++;
					if (count == iNth)
					{
						return i;
					}
				}
			}
			return -1;
		}
		public static int NthIndexOf(string sSource, string sFinder, int iNth, bool bUseRegex)
		{
			//Works with string finder. But uses Regex
			Match m = Regex.Match(sSource, "((" + Regex.Escape(sFinder) + ").*?){" + iNth + "}");

			if (m.Success)
				return m.Groups[2].Captures[iNth - 1].Index;
			else
				return -1;
		}
		public static StringBuilder CleanSB(StringBuilder sb)
		{
			if(sb.ToString().Contains("\n\n\n"))
			{
				return sb.Replace("\n\n\n","\n\n");
			}
			return sb;
		}
		public static string SqlToUpper(string sSource)
		{
			StringBuilder sb = new StringBuilder();
			bool bIterate = true;
			//Capitalize every letter not inside ''
			List<string> SplitterList = LowMemSplit(sSource, "'");
			foreach (string s in SplitterList)
			{
				if (bIterate)
				{
					sb.Append(s.ToUpper());
				}
				else
				{
					sb.Append("'" + s + "'");
				}
				bIterate = !bIterate;
			}
			return sb.ToString();
		}
		public static StringBuilder ReplaceNthOccurrence(StringBuilder sbSource, string sFinder, string sReplacement, int iNth)
		{
			//More effecient passing around a SB object than the string itself. Resolved OOM issues.

			int iOcurrenceCount = StringUtility.LowMemSplit(sbSource.ToString(), sFinder).Count;
			for (int i = iNth; i <= iOcurrenceCount; i += iNth)
			{
				int NthIndex = NthIndexOf(sbSource.ToString(), sFinder, i);
				sbSource = sbSource.Remove(NthIndex, sFinder.Length).Insert(NthIndex, sReplacement);
			}
			return sbSource;
		}
	}
}
