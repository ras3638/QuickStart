using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{
	class EnvDefDirectories
	{
		public static string EnvDefBase = "C:\\Users\\robert_salinas\\Desktop\\Environment Definitions";
		public static string Core5QPEC = EnvDefBase + "\\5.0\\QPEC\\CORE_5.0_QPEC.txt";
		public static string Core8QPEC = EnvDefBase + "\\8.0\\QPEC\\CORE_8.0_QPEC.txt";
		public static string Core16QPEC = EnvDefBase + "\\16.0\\QPEC\\CORE_16.0_QPEC.txt";

		public static string Core5QPECConfig = EnvDefBase + "\\5.0\\QPECConfig\\CORE_5.0_QPEC_Config.txt";
		public static string Core8QPECConfig = EnvDefBase + "\\8.0\\QPECConfig\\CORE_8.0_QPEC_Config.txt";
		public static string Core16QPECConfig = EnvDefBase + "\\16.0\\QPECConfig\\CORE_16.0_QPEC_Config.txt";

		public static string Core5QEnvironmentDefinitions = EnvDefBase + "\\5.0\\QEnvironmentDefinitions\\CORE_5.0_QEnvironmentDefinitions.txt";

		public static string Core5UpstreamMainApp = EnvDefBase + "\\5.0\\UpstreamMainApp\\CORE_5.0_UpstreamMainApp.txt";
		public static string Core8UpstreamMainApp = EnvDefBase + "\\8.0\\UpstreamMainApp\\CORE_8.0_UpstreamMainApp.txt";
		public static string Core16UpstreamMainApp = EnvDefBase + "\\16.0\\UpstreamMainApp\\CORE_16.0_UpstreamMainApp.txt";
	}
	class BaseDirectories
	{
		public static string Base5 = "C:\\Devl\\Products\\Upstream\\5.0.00";
		public static string Base8 = "C:\\Devl\\Products\\Upstream\\8.0.00";
		public static string Base16 = "C:\\Devl\\Products\\Upstream\\Main";

		public static string ProcessBase = "\\Packages\\UpstreamSuiteProcess";
		public static string Process16Base = "\\Packages\\Package.UpstreamSuiteProcess";
		public static string GUIBase = "\\Packages\\UpstreamSuiteGUI";
		public static string GUI16Base = "\\Packages\\Package.UpstreamSuiteGUI";

		public static string ProcessQPEC = "\\QPEC.ini";
		public static string ProcessQPECConfig = "\\QPEC.exe.config";
		public static string GUIUpstreamMainApp = "\\UpstreamMainApp.exe.config";
		public static string GUIQEnvironmentDefinitions = "\\QEnvironmentDefinitions.xml";

		public static string Full5QEnvironmentDefinitions = Base5 + GUIBase + GUIQEnvironmentDefinitions;

		public static string Full5QPEC = Base5 + ProcessBase + ProcessQPEC;
		public static string Full8QPEC = Base8 + ProcessBase + ProcessQPEC;
		public static string Full16QPEC = Base16 + Process16Base + ProcessQPEC;

		public static string Full5QPECConfig = Base5 + ProcessBase + ProcessQPECConfig;
		public static string Full8QPECConfig = Base8 + ProcessBase + ProcessQPECConfig;
		public static string Full16QPECConfig = Base16 + Process16Base + ProcessQPECConfig;

		public static string Full5UpstreamMainApp = Base5 + GUIBase + GUIUpstreamMainApp;
		public static string Full8UpstreamMainApp = Base8 + GUIBase + GUIUpstreamMainApp;
		public static string Full16UpstreamMainApp = Base16 + GUI16Base + GUIUpstreamMainApp;
	}
}
