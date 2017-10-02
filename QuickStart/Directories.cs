using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickStart
{

	class BaseDirectories
	{


        public static string QuickStartBase = Directory.GetCurrentDirectory() + "\\SettingFiles";
		public static string Base5 = "C:\\Devl\\Products\\Upstream\\5.0.00";
		public static string Base8 = "C:\\Devl\\Products\\Upstream\\8.0.00";
		public static string Base16 = "C:\\Devl\\Products\\Upstream\\Main";

		public static string CacheClientBase = QuickStartBase + "\\CachedClient.txt";

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

		public static string GeneratedSchemas = QuickStartBase + "\\Generated Schemas";
		public static string GeneratedScriptFile = QuickStartBase + "\\GeneratedScript.txt";

		public static string Core5QPECBase = QuickStartBase + "\\5.0\\QPEC";
		public static string Core8QPECBase = QuickStartBase + "\\8.0\\QPEC";
		public static string Core16QPECBase = QuickStartBase + "\\16.0\\QPEC";

		public static string Core5QPEC = Core5QPECBase + "\\CORE_5.0_QPEC.txt";
		public static string Core8QPEC = Core8QPECBase + "\\CORE_8.0_QPEC.txt";
		public static string Core16QPEC = Core16QPECBase + "\\CORE_16.0_QPEC.txt";

		public static string Core5QPECConfigBase = QuickStartBase + "\\5.0\\QPECConfig";
		public static string Core8QPECConfigBase = QuickStartBase + "\\8.0\\QPECConfig";
		public static string Core16QPECConfigBase = QuickStartBase + "\\16.0\\QPECConfig";

		public static string Core5QPECConfig = Core5QPECConfigBase + "\\CORE_5.0_QPEC_Config.txt";
		public static string Core8QPECConfig = Core8QPECConfigBase + "\\CORE_8.0_QPEC_Config.txt";
		public static string Core16QPECConfig = Core16QPECConfigBase + "\\CORE_16.0_QPEC_Config.txt";

		public static string Core5QEnvironmentDefinitionsBase = QuickStartBase + "\\5.0\\QEnvironmentDefinitions";
		public static string Core5QEnvironmentDefinitions = Core5QEnvironmentDefinitionsBase + "\\CORE_5.0_QEnvironmentDefinitions.txt";

		public static string Core5UpstreamMainAppBase = QuickStartBase + "\\5.0\\UpstreamMainApp";
		public static string Core8UpstreamMainAppBase = QuickStartBase + "\\8.0\\UpstreamMainApp";
		public static string Core16UpstreamMainAppBase = QuickStartBase + "\\16.0\\UpstreamMainApp";

		public static string Core5UpstreamMainApp = Core5UpstreamMainAppBase + "\\CORE_5.0_UpstreamMainApp.txt";
		public static string Core8UpstreamMainApp = Core8UpstreamMainAppBase + "\\CORE_8.0_UpstreamMainApp.txt";
		public static string Core16UpstreamMainApp = Core16UpstreamMainAppBase + "\\CORE_16.0_UpstreamMainApp.txt";
	}
}
