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
		{"I", 8},
		{"J", 9},
		{"K",10},
		{"L",11},
		{"M",12},
		{"N",13},
		{"O",14},
	    {"P",15},
	    {"Q",16},
	    {"R",17},
		{"S",18},
		{"T",19},
		{"U",20},
		{"V",21},
		{"W",22},
		{"X",23},
		{"Y",24},
		{"Z",25}
		};

		public IncrementAlphabet(string sTableAlias)
		{
			this.m_sPreviousAlphabet = "A";
			this.m_sCurrentAlphabet = "B";		

			if (!String.IsNullOrEmpty(sTableAlias))
			{
				SetNext(sTableAlias);
			}
		}
		public void SetNext(string sInput)
		{
			int iAliasOrder;
			this.m_sPreviousAlphabet = sInput;

			if (m_dictAlphabetOrder.TryGetValue(sInput, out iAliasOrder))
			{
				var varKey = m_dictAlphabetOrder.FirstOrDefault(x => x.Value == iAliasOrder + 1).Key;
				this.m_sCurrentAlphabet = (varKey == null) ? "A" : varKey;
			}
			else
			{
				this.m_sCurrentAlphabet = "A";
			}
		}
		public void Next()
		{
			SetNext(m_sCurrentAlphabet);
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
