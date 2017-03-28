using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuickStart
{
	public static class SettingManager
	{
		private static Dictionary<Tuple<string, string>, Setting> SettingDictionary = new Dictionary<Tuple<string, string>, Setting>();
		
		public static void AddSetting(string sTabKey, string sSettingKey, object oValue)
		{
			Tuple<string, string> key = Tuple.Create(sTabKey, sSettingKey);
			Setting value = new Setting(sTabKey, sSettingKey, oValue);
			SettingDictionary.Add(key, value);
		}
		public static void UpdateSetting(string sTabKey, string sSettingKey, object oValue)
		{
			Tuple<string, string> SettingKeyTuple = Tuple.Create(sTabKey, sSettingKey);
			Setting s = SettingDictionary[SettingKeyTuple];
			s.SetVal(oValue);
			SettingDictionary[SettingKeyTuple] = s;
		}
		private static Setting GetSetting(string sTabKey, string sSettingKey)
		{
			Tuple<string,string> SettingKeyTuple = Tuple.Create(sTabKey, sSettingKey);
			return SettingDictionary[SettingKeyTuple];
		}
		public static object GetSettingValue(string sTabKey, string sSettingKey)
		{
			Setting s = GetSetting(sTabKey, sSettingKey);
			return s.GetVal();
		}
		public static object GetSettingDefaultValue(string sTabKey, string sSettingKey)
		{
			Setting s = GetSetting(sTabKey, sSettingKey);
			return s.GetDefaultVal();
		}
		public static void OverwriteDefaultType(string sTabKey, string sSettingKey,Type t)
		{
			Tuple<string,string> SettingKeyTuple = Tuple.Create(sTabKey, sSettingKey);
			Setting s = SettingDictionary[SettingKeyTuple];
			s.OverwriteDefaultType(t);
			SettingDictionary[SettingKeyTuple] = s;
		}
		private class Setting
		{
			private object oDefaultSettingValue;
			private object oSettingValue;
			private Type tDefaultType;
			private Tuple<string, string> SettingKeyTuple;

			public Setting(string sTabKey, string sSettingKey, object oValue)
			{
				this.oDefaultSettingValue = oValue;
				this.oSettingValue = oValue;
				this.tDefaultType = oValue.GetType();
				this.SettingKeyTuple = Tuple.Create(sTabKey, sSettingKey);
			}
			public void OverwriteDefaultType(Type t)
			{
				this.tDefaultType = t;
			}
			public void SetVal(object oValue)
			{
				this.oSettingValue = oValue;
			}
			public object GetVal()
			{
				try
				{
					//Attempt to use stored value using default type
					return Convert.ChangeType(this.oSettingValue, this.tDefaultType);
				}
				catch (Exception)
				{
					//return default value
					return Convert.ChangeType(this.oDefaultSettingValue, this.tDefaultType);
				}
			}
			public object GetDefaultVal()
			{
				return Convert.ChangeType(this.oDefaultSettingValue, this.tDefaultType);
			}

		}
	}
	
	
}
