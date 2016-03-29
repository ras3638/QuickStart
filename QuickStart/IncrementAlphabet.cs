using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	public class IncrementAlphabet
	{
		private string m_sCurrentAlphabet { get; set; }
		private string m_sPreviousAlphabet { get; set; }
		Dictionary<string, int> m_dictAlphabetOrder = new Dictionary<string, int>()
		{
	    {"A", 0},
	    {"B", 1},
	    {"C", 2},
	    {"D", 3},
		{"E", 4},
		{"F", 5},
		{"G", 6},
		{"H", 7},
		};

		public IncrementAlphabet(string sTableAlias)
		{
			this.m_sPreviousAlphabet = "A";
			this.m_sCurrentAlphabet = "B";		

			if (!String.IsNullOrEmpty(sTableAlias))
			{
				int iAliasOrder;
				this.m_sPreviousAlphabet = sTableAlias;

				if (m_dictAlphabetOrder.TryGetValue(sTableAlias, out iAliasOrder)) 
				{
					var varKey = m_dictAlphabetOrder.FirstOrDefault(x => x.Value == iAliasOrder + 1).Key;
					this.m_sCurrentAlphabet = (varKey == null) ? "A" : varKey;
				}
				else
				{
					this.m_sCurrentAlphabet = "A";
				}			
			}
		}
		public void Next()
		{
			this.m_sPreviousAlphabet = m_sCurrentAlphabet;
			if (this.m_sCurrentAlphabet == "A") this.m_sCurrentAlphabet = "B";
			else if (this.m_sCurrentAlphabet == "B") this.m_sCurrentAlphabet = "C";
			else if (this.m_sCurrentAlphabet == "C") this.m_sCurrentAlphabet = "D";
			else if (this.m_sCurrentAlphabet == "D") this.m_sCurrentAlphabet = "E";
			else if (this.m_sCurrentAlphabet == "E") this.m_sCurrentAlphabet = "F";
			else if (this.m_sCurrentAlphabet == "F") this.m_sCurrentAlphabet = "G";
			else if (this.m_sCurrentAlphabet == "G") this.m_sCurrentAlphabet = "H";
			else this.m_sCurrentAlphabet = "A";

		}
		public string GetCurrent()
		{
			return this.m_sCurrentAlphabet;
		}
		public string GetPrevious()
		{
			return this.m_sPreviousAlphabet;
		}
	}
}
