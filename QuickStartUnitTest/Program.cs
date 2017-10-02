using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickStart;
namespace QuickStartUnitTest
{
	class Program
	{
		static void Main(string[] args)
		{
			LoadDefaultSettings();
			UnitTest();
			
			Console.ReadLine();
		}
		static void LoadDefaultSettings()
		{

			SettingManager.AddSetting(frmQuickStart.INSERT_GEN, "Max Byte Threshold", (long)12000000);
			SettingManager.AddSetting(frmQuickStart.INSERT_GEN, "Enable Oracle Functionality", false);
			SettingManager.AddSetting(frmQuickStart.INSERT_GEN, "Enable Excel Functionality", false);
			SettingManager.AddSetting(frmQuickStart.CASCADE_DELETE_GEN, "Multiline support for None cascade option", false);
			SettingManager.AddSetting(frmQuickStart.SEARCH_STRING, "Enable Zip Functionality", true);
		}
	
		static void UnitTest()
		{
			string sTest;
			string sActResult;
			string sExpResult;
			string sMethodName;

			sMethodName = "T1: QuickStart.StringUtility.SqlRemoveExtraWhiteSpace()";
			sTest = "HE   LLO";
			sActResult = QuickStart.StringUtility.SqlRemoveExtraWhiteSpace(sTest);
			sExpResult = "HE LLO";
			VerifyString(sMethodName, sTest, sActResult, sExpResult);

			sMethodName = "T2: QuickStart.StringUtility.SqlRemoveExtraWhiteSpace()";
			sTest = "SELECT *  FROM    A";
			sActResult = QuickStart.StringUtility.SqlRemoveExtraWhiteSpace(sTest);
			sExpResult = "SELECT * FROM A";
			VerifyString(sMethodName, sTest, sActResult, sExpResult);

			sMethodName = "T3: QuickStart.StringUtility.SqlRemoveExtraWhiteSpace()";
			sTest = "SELECT *  FROM    A WHERE B = 'AA  AA'";
			sActResult = QuickStart.StringUtility.SqlRemoveExtraWhiteSpace(sTest);
			sExpResult = "SELECT * FROM A WHERE B = 'AA  AA'";
			VerifyString(sMethodName, sTest, sActResult, sExpResult);

			sMethodName = "T4: QuickStart.SqlGenEngine.InsertEngine()";
			sTest = "CHK_RGSTR_SEQ_NO\tOPER_BUS_SEG_CD\tRGSTR_NO\tBANK_ACCT_NO\tPROCESS_QUEUE_ID\tPROCESS_STEP_QUEUE_ID\tBUS_UNIT_CD\tOWNR_NO\tOWNR_SUB\tCHK_NO\tCHK_TYPE_CD\tCW_CHK_STAT_CD\tCW_BANK_STAT_CD\tCHK_DT\tCHK_AMT\tBANK_NO\tACCTG_MTH\tSENT_TO_TREAS_FL\tGRS_VAL_BAL_AMT\tSEV_TAX_BAL_AMT\tOTH_DDUC_BAL_AMT\tNET_VAL_BAL_AMT\tCHK_STAT_DT\tCHK_CLEAR_FL\tCHK_CLEAR_DT\tCW_TIME_CD\tRUN_DT\tSENT_TO_CDEX_FL\tSENT_TO_BANK_FL\tSTG_INPUT_ID\tRYL_PMT_TYPE_CD\tTEMP_FL\tGRS_VAL_AMT_YTD_TOT\tSEV_TAX_AMT_YTD_TOT\tOTH_DDUC_AMT_YTD_TOT\tMKT_DDUC_AMT_YTD_TOT\tNET_VAL_AMT_YTD_TOT\tUPDT_USER\tUPDT_DT\tBA_NM1\tBA_NM2\tBA_NM3\tADDR_LINE1\tADDR_LINE2\tADDR_LINE3\tCTY\tALT_ST_CD\tZIP_CD\tCTRY_DESCR\tPRT_FST_FL\tTAX_ID\tTAX_ID_TYPE_CD\tDIVERT_FL\tBIN1_FL\tBIN2_FL\tBIN7_FL\tDISC_DATA\tINTFC_COMPLETE_FL\tPMT_SENT_TO_BANK_FL\tORIG_SENT_TO_BANK_FL\tCMNT_SEQ_NO\tEXP_TO_PDS_FL\tEXP_VOIDED_CHK_FL\tPMNT_EXP_FL\tOWNR_GMI_DEC\tBANK_RECON_COMPL_FL\tOVRD_RSN_CD\n1\tWOC\tNULL\t002419801018\t87264\t520028\t001\t958483\t2\t2\tSYS\tOS\t2\t2016-07-31 00:00:00.000\t1420.85\t111901302\t2016-11-01 00:00:00.000\tN\t1626.72\t205.87\t0.00\t1420.85\tNULL\tN\tNULL\t3\t2016-07-19 00:00:00.000\tN\tN\t0\tPF\tN\t1626.72\t205.87\t0.00\t0.00\t1420.85\tQC_ALAN_BOUNDS\t2016-12-28 12:59:47.000\tWILLIAM P MILLS III\tNULL\tNULL\t301 E KALISTE SALOON RD\tSUITE 401\tNULL\tLAFAYETTE\tLA   \t70508-0000\tNULL\tN\t437740973\tSSN\tN\tN\tN\tN\tNULL\tY\tY\tN\tNULL\tY\tN\tN\tNULL\tN\tNULL\n2\tWOC\tNULL\t002419801018\t87264\t520028\t001\t958965\t1\t3\tSYS\tOS\t2\t2016-07-31 00:00:00.000\t162.23\t111901302\t2016-11-01 00:00:00.000\tN\t185.74\t23.51\t0.00\t162.23\tNULL\tN\tNULL\t3\t2016-07-19 00:00:00.000\tN\tN\t1\tPF\tN\t185.74\t23.51\t0.00\t0.00\t162.23\tQC_ALAN_BOUNDS\t2016-12-28 12:59:47.000\tSHB LIMITED PARTNERSHIP\tNULL\tNULL\t132 C STEINER RD\tNULL\tNULL\tLAFAYETTE\tLA   \t70508-6002\tNULL\tN\t721298044\tFED\tN\tN\tN\tN\tNULL\tY\tY\tN\tNULL\tY\tN\tN\tNULL\tN\tNULL\n3\tWOC\tNULL\t2419801018\t137536\t621925\t001\t137643\t1\t4\tSYS\tCL\t1\t2016-08-10 00:00:00.000\t9.62\t111901302\t2016-11-01 00:00:00.000\tN\t10.07\t0.45\t0.00\t9.62\tNULL\tY\t2016-08-25 00:00:00.000\t1\t2016-08-10 00:00:00.000\tN\tN\t0\tPP\tN\t10.07\t0.45\t0.00\t0.00\t9.62\tQC_ALAN_BOUNDS\t2016-12-28 12:59:47.000\tAVALON ROYALTY LLC\tNULL\tNULL\tPO BOX 11373\tNULL\tNULL\tMIDLAND\tTX   \t79702-0000\tNULL\tN\t752922053\tFED\tN\tN\tN\tN\tNULL\tY\tY\tN\tNULL\tY\tN\tN\tNULL\tN\tNULL";
			sActResult = QuickStart.SqlGenEngine.InsertEngine(sTest, "", "RTRN_CHK_RGSTR").ToString();
			sExpResult = @"INSERT INTO RTRN_CHK_RGSTR (CHK_RGSTR_SEQ_NO,OPER_BUS_SEG_CD,RGSTR_NO,BANK_ACCT_NO,PROCESS_QUEUE_ID,PROCESS_STEP_QUEUE_ID,BUS_UNIT_CD,OWNR_NO,OWNR_SUB,CHK_NO,CHK_TYPE_CD,CW_CHK_STAT_CD,CW_BANK_STAT_CD,CHK_DT,CHK_AMT,BANK_NO,ACCTG_MTH,SENT_TO_TREAS_FL,GRS_VAL_BAL_AMT,SEV_TAX_BAL_AMT,OTH_DDUC_BAL_AMT,NET_VAL_BAL_AMT,CHK_STAT_DT,CHK_CLEAR_FL,CHK_CLEAR_DT,CW_TIME_CD,RUN_DT,SENT_TO_CDEX_FL,SENT_TO_BANK_FL,STG_INPUT_ID,RYL_PMT_TYPE_CD,TEMP_FL,GRS_VAL_AMT_YTD_TOT,SEV_TAX_AMT_YTD_TOT,OTH_DDUC_AMT_YTD_TOT,MKT_DDUC_AMT_YTD_TOT,NET_VAL_AMT_YTD_TOT,UPDT_USER,UPDT_DT,BA_NM1,BA_NM2,BA_NM3,ADDR_LINE1,ADDR_LINE2,ADDR_LINE3,CTY,ALT_ST_CD,ZIP_CD,CTRY_DESCR,PRT_FST_FL,TAX_ID,TAX_ID_TYPE_CD,DIVERT_FL,BIN1_FL,BIN2_FL,BIN7_FL,DISC_DATA,INTFC_COMPLETE_FL,PMT_SENT_TO_BANK_FL,ORIG_SENT_TO_BANK_FL,CMNT_SEQ_NO,EXP_TO_PDS_FL,EXP_VOIDED_CHK_FL,PMNT_EXP_FL,OWNR_GMI_DEC,BANK_RECON_COMPL_FL,OVRD_RSN_CD) VALUES ('1','WOC',NULL,'002419801018','87264','520028','001','958483','2','2','SYS','OS','2','2016-07-31 00:00:00.000','1420.85','111901302','2016-11-01 00:00:00.000','N','1626.72','205.87','0.00','1420.85',NULL,'N',NULL,'3','2016-07-19 00:00:00.000','N','N','0','PF','N','1626.72','205.87','0.00','0.00','1420.85','QC_ALAN_BOUNDS','2016-12-28 12:59:47.000','WILLIAM P MILLS III',NULL,NULL,'301 E KALISTE SALOON RD','SUITE 401',NULL,'LAFAYETTE','LA   ','70508-0000',NULL,'N','437740973','SSN','N','N','N','N',NULL,'Y','Y','N',NULL,'Y','N','N',NULL,'N',NULL)
INSERT INTO RTRN_CHK_RGSTR (CHK_RGSTR_SEQ_NO,OPER_BUS_SEG_CD,RGSTR_NO,BANK_ACCT_NO,PROCESS_QUEUE_ID,PROCESS_STEP_QUEUE_ID,BUS_UNIT_CD,OWNR_NO,OWNR_SUB,CHK_NO,CHK_TYPE_CD,CW_CHK_STAT_CD,CW_BANK_STAT_CD,CHK_DT,CHK_AMT,BANK_NO,ACCTG_MTH,SENT_TO_TREAS_FL,GRS_VAL_BAL_AMT,SEV_TAX_BAL_AMT,OTH_DDUC_BAL_AMT,NET_VAL_BAL_AMT,CHK_STAT_DT,CHK_CLEAR_FL,CHK_CLEAR_DT,CW_TIME_CD,RUN_DT,SENT_TO_CDEX_FL,SENT_TO_BANK_FL,STG_INPUT_ID,RYL_PMT_TYPE_CD,TEMP_FL,GRS_VAL_AMT_YTD_TOT,SEV_TAX_AMT_YTD_TOT,OTH_DDUC_AMT_YTD_TOT,MKT_DDUC_AMT_YTD_TOT,NET_VAL_AMT_YTD_TOT,UPDT_USER,UPDT_DT,BA_NM1,BA_NM2,BA_NM3,ADDR_LINE1,ADDR_LINE2,ADDR_LINE3,CTY,ALT_ST_CD,ZIP_CD,CTRY_DESCR,PRT_FST_FL,TAX_ID,TAX_ID_TYPE_CD,DIVERT_FL,BIN1_FL,BIN2_FL,BIN7_FL,DISC_DATA,INTFC_COMPLETE_FL,PMT_SENT_TO_BANK_FL,ORIG_SENT_TO_BANK_FL,CMNT_SEQ_NO,EXP_TO_PDS_FL,EXP_VOIDED_CHK_FL,PMNT_EXP_FL,OWNR_GMI_DEC,BANK_RECON_COMPL_FL,OVRD_RSN_CD) VALUES ('2','WOC',NULL,'002419801018','87264','520028','001','958965','1','3','SYS','OS','2','2016-07-31 00:00:00.000','162.23','111901302','2016-11-01 00:00:00.000','N','185.74','23.51','0.00','162.23',NULL,'N',NULL,'3','2016-07-19 00:00:00.000','N','N','1','PF','N','185.74','23.51','0.00','0.00','162.23','QC_ALAN_BOUNDS','2016-12-28 12:59:47.000','SHB LIMITED PARTNERSHIP',NULL,NULL,'132 C STEINER RD',NULL,NULL,'LAFAYETTE','LA   ','70508-6002',NULL,'N','721298044','FED','N','N','N','N',NULL,'Y','Y','N',NULL,'Y','N','N',NULL,'N',NULL)
INSERT INTO RTRN_CHK_RGSTR (CHK_RGSTR_SEQ_NO,OPER_BUS_SEG_CD,RGSTR_NO,BANK_ACCT_NO,PROCESS_QUEUE_ID,PROCESS_STEP_QUEUE_ID,BUS_UNIT_CD,OWNR_NO,OWNR_SUB,CHK_NO,CHK_TYPE_CD,CW_CHK_STAT_CD,CW_BANK_STAT_CD,CHK_DT,CHK_AMT,BANK_NO,ACCTG_MTH,SENT_TO_TREAS_FL,GRS_VAL_BAL_AMT,SEV_TAX_BAL_AMT,OTH_DDUC_BAL_AMT,NET_VAL_BAL_AMT,CHK_STAT_DT,CHK_CLEAR_FL,CHK_CLEAR_DT,CW_TIME_CD,RUN_DT,SENT_TO_CDEX_FL,SENT_TO_BANK_FL,STG_INPUT_ID,RYL_PMT_TYPE_CD,TEMP_FL,GRS_VAL_AMT_YTD_TOT,SEV_TAX_AMT_YTD_TOT,OTH_DDUC_AMT_YTD_TOT,MKT_DDUC_AMT_YTD_TOT,NET_VAL_AMT_YTD_TOT,UPDT_USER,UPDT_DT,BA_NM1,BA_NM2,BA_NM3,ADDR_LINE1,ADDR_LINE2,ADDR_LINE3,CTY,ALT_ST_CD,ZIP_CD,CTRY_DESCR,PRT_FST_FL,TAX_ID,TAX_ID_TYPE_CD,DIVERT_FL,BIN1_FL,BIN2_FL,BIN7_FL,DISC_DATA,INTFC_COMPLETE_FL,PMT_SENT_TO_BANK_FL,ORIG_SENT_TO_BANK_FL,CMNT_SEQ_NO,EXP_TO_PDS_FL,EXP_VOIDED_CHK_FL,PMNT_EXP_FL,OWNR_GMI_DEC,BANK_RECON_COMPL_FL,OVRD_RSN_CD) VALUES ('3','WOC',NULL,'2419801018','137536','621925','001','137643','1','4','SYS','CL','1','2016-08-10 00:00:00.000','9.62','111901302','2016-11-01 00:00:00.000','N','10.07','0.45','0.00','9.62',NULL,'Y','2016-08-25 00:00:00.000','1','2016-08-10 00:00:00.000','N','N','0','PP','N','10.07','0.45','0.00','0.00','9.62','QC_ALAN_BOUNDS','2016-12-28 12:59:47.000','AVALON ROYALTY LLC',NULL,NULL,'PO BOX 11373',NULL,NULL,'MIDLAND','TX   ','79702-0000',NULL,'N','752922053','FED','N','N','N','N',NULL,'Y','Y','N',NULL,'Y','N','N',NULL,'N',NULL)";
			VerifyString(sMethodName, sTest, sActResult, sExpResult);

			sMethodName = "T5: QuickStart.StringUtility.HighMemSplit()";
			sTest = "HELLOFRIENDHELLO";
			List<string> ActResultList = QuickStart.StringUtility.HighMemSplit(sTest,"EL");
			List<string> ExpResultList = new List<string>();
			ExpResultList.Add("H");
			ExpResultList.Add("LOFRIENDH");
			ExpResultList.Add("LO");
			VerifyStringList(sMethodName, sTest, ActResultList, ExpResultList);

			sMethodName = "T6: QuickStart.StringUtility.LowMemSplit()";
			sTest = "HELLOFRIENDHELLO";
			ActResultList = QuickStart.StringUtility.LowMemSplit(sTest, "EL");
			ExpResultList = new List<string>();
			ExpResultList.Add("H");
			ExpResultList.Add("LOFRIENDH");
			ExpResultList.Add("LO");
			VerifyStringList(sMethodName, sTest, ActResultList, ExpResultList);

			sMethodName = "T7: QuickStart.StringUtility.LowMemSplit()";
			sTest = "HELLOFRIENDHELLO";
			ActResultList = QuickStart.StringUtility.LowMemSplit(sTest, "L");
			ExpResultList = new List<string>();
			ExpResultList.Add("HE");
			ExpResultList.Add("OFRIENDHE");
			ExpResultList.Add("O");
			VerifyStringList(sMethodName, sTest, ActResultList, ExpResultList);

			sMethodName = "T8: QuickStart.StringUtility.HighMemSplit()";
			sTest = "HELLOFRIENDHELLO";
			ActResultList = QuickStart.StringUtility.HighMemSplit(sTest, "L");
			ExpResultList = new List<string>();
			ExpResultList.Add("HE");
			ExpResultList.Add("");
			ExpResultList.Add("OFRIENDHE");
			ExpResultList.Add("");
			ExpResultList.Add("O");
			VerifyStringList(sMethodName, sTest, ActResultList, ExpResultList);
		}
		static void VerifyStringList(string sMethodName, string sTest, List<string> ActResultList, List<string> ExpResultList)
		{
			if (ActResultList.SequenceEqual(ExpResultList))
			{
				Console.WriteLine("PASS: " + sMethodName);
			}
			else
			{
				Console.WriteLine("FAIL: " + sMethodName);
			}

		}
		static void VerifyString(string sMethodName,string sTest, string sActResult, string sExpResult)
		{
			if (sActResult == sExpResult)
			{
				Console.WriteLine("PASS: " + sMethodName);
			}
			else
			{
				Console.WriteLine("FAIL: " + sMethodName);
				Console.WriteLine("     Test Parameter: " + sTest);
				Console.WriteLine("     Expected Result: " + sExpResult);
				Console.WriteLine("     Actual Result: " + sActResult);
			}
		}
	}
}
