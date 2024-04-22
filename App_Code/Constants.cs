using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class Constants
{
	public Constants()
	{		
	}
    public const string title = "RA Billing Management System.";
    public const string msheet_path = "Msheets";
    public const string admin_pwd = "test";
   // public const string admin_pwd = "test";
    public const string loginHistoryQuery = "INSERT INTO RAB_AUDIT_TRAIL (USER_ID, ACTION_DATE, MACHINE_IP, MODULE_NAME, ACTION, SITE_CD) VALUES (:USER_ID, SYSDATE, :MACHINE_IP, :MODULE_NAME, :ACTION, :SITE_CD)";
    public const string report_path = "Reports/Temp/";
    public const string lstk_path = "LSTK_Attachments/";
    public const int maxUploasSize = 25;
    public const int maxUploasSizeInBytes = 26214400;
    
}