using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;
using System.Collections;

public partial class RA_Update_Checklist : System.Web.UI.Page
{
    dbFunction objDB = new dbFunction();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["USERID"] == null)
        {
            Response.Redirect("Login.aspx");
        }

        if (!IsPostBack)
        {
            if (Session["USERID"] != null && Session["ROLE"] != null && ("VEND".Equals(Session["ROLE"].ToString()) || "BE".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()) || "RCM".Equals(Session["ROLE"].ToString())))
            {
                bindJobNumber(Session["USERID"].ToString(), Session["ROLE"].ToString());                
            }
            else
            {
                Common.Show("This option is valid for BE/AC/RCM only");
                Response.Redirect("Default.aspx");
            }
        }    
    }
    protected void bindJobNumber(string userId, string userRole)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        if ("VEND".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_MASTER where C_CODE=:C_CODE");
            paramList.Add("C_CODE", userId);
        }
        else if ("BE".Equals(userRole) || "AC".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_USERS where ROLE=:ROLE and EMPNO=:EMPNO and ACTIVE='Y'  ORDER BY JOB_NO");
            paramList.Add("ROLE", userRole);
            paramList.Add("EMPNO", userId);
        }
        else if ("RCM".Equals(userRole))
        {
            sbQuery.Append("SELECT distinct JOB_NO FROM JOB_DIR WHERE SITE_CD IN (SELECT SITE_CD FROM SITE_DIR WHERE EMPNO_RCM=:EMPNO_RCM) ORDER BY JOB_NO");
            paramList.Add("EMPNO_RCM", userId);
        }

        if (sbQuery.Length > 0)
            objDB.bindDropDownList(ddJobNumber, sbQuery.ToString(), paramList, "JOB_NO", "JOB_NO", "", "--Select Job Number--");
    }

    protected void bindTenders(string jobNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        if ("BE".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()) || "RCM".Equals(Session["ROLE"].ToString()) )
        {
            sbQuery.Append("SELECT DISTINCT b.TENDER_NO||'~'||b.part_no tender_part, b.tender_no||'-'|| b.part_no || ' ( '||b.TITLE||')' description  ")
             .Append(" FROM  RAB_TENDER_USERS a,RAB_TENDER_MASTER b ")
               .Append(" WHERE b.JOB_NO=:JOB_NO ")
               .Append(" AND EMPNO=:EMPNO ")
               .Append(" AND ROLE=:ROLE ")
               .Append(" and b.job_no=A.JOB_NO")
              .Append(" and A.TENDER_NO=b.TENDER_NO")
              .Append(" and A.PART_NO=b.PART_NO")
               .Append(" AND a.ACTIVE = 'Y' ORDER BY tender_part ");

            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("EMPNO", Session["USERID"].ToString());
            paramList.Add("ROLE", Session["ROLE"].ToString());

        }
        else if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(" SELECT DISTINCT ")
              .Append("TENDER_NO||'~'||a.part_no tender_part, ")
              .Append(" tender_no||'-'|| a.part_no||' ( '||A.TITLE||')'  description ")
              .Append(" FROM RAB_TENDER_MASTER a ")
               .Append("WHERE     JOB_NO =:JOB_NO ")
              .Append("  AND C_CODE =:C_CODE ")
               .Append("ORDER BY tender_part ");

            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("C_CODE", Session["USERID"].ToString());
        }
        //else
        //{
        //    sbQuery.Append(" Select DISTINCT TENDER_NO||'~'||a.part_no tender_part, tender_no||'-'|| a.part_no||' ( '||a.TITLE||')' description ")
        //              .Append(" FROM RAB_TENDER_MASTER a ")
        //             .Append(" WHERE JOB_NO=:JOB_NO ")
        //             .Append(" ORDER BY tender_part");
        //    paramList.Add("JOB_NO", jobNumber.ToUpper());
        //}

        objDB.bindDropDownList(ddTenderNo, sbQuery.ToString(), paramList, "tender_part", "description", "", "--Select Tender Number--");
    }

    protected void ddJobNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!"".Equals(ddJobNumber.SelectedValue))
        {
            bindTenders(ddJobNumber.SelectedValue);            
        }
        else
        {
            Common.Show("Please select Job Number");
        }
    }

    protected void ddTenderNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');

        bindCheckListPoints(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
        if (gvChecklist.Rows.Count > 0)
        {
            btnSubmit.Visible = true;
            if ("RCM".Equals(Session["ROLE"].ToString()))
            btnFreeze.Visible = true;
        }
        else
        {
            btnSubmit.Visible = false;
            btnFreeze.Visible = false;
        }
    }
    
    protected void bindCheckListPoints(string jobNumber, string tenderNumber, string partNumber)
    {
        StringBuilder query = new StringBuilder();
        query.Append("  SELECT a.CHECKLIST_ID, ")
         .Append("  ITEMS_NAME, ")
         .Append("  DISPLAY_ORDER, ")
         .Append("  B.VENDER_CODE, ")
         .Append("  B.CHECKED_BY_BE, ")
         .Append("  B.CHECKED_BY_AC, ")
        .Append("   B.CHECKED_BY_RCM, ")
         .Append("  B.IS_FREEZED, ")
         .Append("  B.JOB_NO, ")
         .Append("  B.TENDER_NO,B.BE_CHECK,B.AC_CHECK,B.RCM_CHECK,B.VEND_CHECK,B.part_no ")
    .Append("  FROM RAB_CHECKLIST_master a,  ")
    .Append("  RAB_FINAL_BILL_CHECKLIST b ")
   .Append("  WHERE     a.IS_ACTIVE = 'Y' ")
       .Append("    AND A.CHECKLIST_ID = B.CHECKLIST_ID(+) ")
        .Append("   AND B.JOB_NO(+) =:JOB_NO ")
        .Append("   AND B.TENDER_NO(+) =:TENDER_NO ")
        .Append("   AND B.PART_NO(+) =:PART_NO ")
.Append("  ORDER BY DISPLAY_ORDER ");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("JOB_NO", jobNumber);
        paramList.Add("TENDER_NO", tenderNumber);
        paramList.Add("PART_NO", partNumber);
        objDB.bindGridView(gvChecklist, query.ToString(), paramList);
        //int totalGridColumns = gvChecklist.Columns.Count-1;
        //if("BE".Equals(Session["ROLE"].ToString()))
        //{
        //    gvChecklist.Columns[totalGridColumns].Visible = false;
        //    gvChecklist.Columns[totalGridColumns-1].Visible = false;
        //}
        //if ("AC".Equals(Session["ROLE"].ToString()))
        //{
        //    gvChecklist.Columns[totalGridColumns].Visible = false;
        //}
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();        
        ArrayList lstInsertParam = new ArrayList();
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        foreach (GridViewRow row in gvChecklist.Rows)
        {
            HiddenField hdJobNo = new HiddenField();
            hdJobNo = (HiddenField)row.FindControl("hdJobNo");

            HiddenField hdTenderNo = new HiddenField();
            hdTenderNo = (HiddenField)row.FindControl("hdTenderNo");

            HiddenField hdPartNo = new HiddenField();
            hdPartNo = (HiddenField)row.FindControl("hdPartNo");

            
            HiddenField hdCheckListId = new HiddenField();
            hdCheckListId = (HiddenField)row.FindControl("hdCheckListId");

            HiddenField hdRCM = new HiddenField();
            hdRCM = (HiddenField)row.FindControl("hdRCM");
            
            HiddenField hdBE = new HiddenField();
            hdBE = (HiddenField)row.FindControl("hdBE");

            HiddenField hdAC = new HiddenField();
            hdAC = (HiddenField)row.FindControl("hdAC");

            HiddenField hdVendor = new HiddenField();
            hdVendor = (HiddenField)row.FindControl("hdVendor");

            HiddenField hdIsFrozen = new HiddenField();
            hdIsFrozen = (HiddenField)row.FindControl("hdIsFrozen");


            CheckBox chkVendor = new CheckBox();
            chkVendor = (CheckBox)row.FindControl("chkVendor");

            CheckBox chkBE = new CheckBox();
            chkBE = (CheckBox)row.FindControl("chkBE");

            CheckBox chkAC = new CheckBox();
            chkAC = (CheckBox)row.FindControl("chkAC");

            CheckBox chkRCM = new CheckBox();
            chkRCM = (CheckBox)row.FindControl("chkRCM");

            HiddenField hdBECheck = new HiddenField();
            hdBECheck = (HiddenField)row.FindControl("hdBECheck");

            HiddenField hdVendorCheck = new HiddenField();
            hdVendorCheck = (HiddenField)row.FindControl("hdVendorCheck");

            HiddenField hdACCheck = new HiddenField();
            hdACCheck = (HiddenField)row.FindControl("hdACCheck");

            HiddenField hdRCMCheck = new HiddenField();
            hdRCMCheck = (HiddenField)row.FindControl("hdRCMCheck");

            Dictionary<string, string> insertParam = new Dictionary<string, string>();
            StringBuilder sbInsertQuery = new StringBuilder();
            if ("VEND".Equals(Session["ROLE"].ToString()))
            {
                if ("".Equals(hdVendorCheck.Value))
                {
                    //Insert Query with N
                    sbInsertQuery.Append("INSERT INTO RAB_FINAL_BILL_CHECKLIST ")
                        .Append(" (JOB_NO, TENDER_NO, CHECKLIST_ID, VENDER_CODE,VEND_CHECK,VEND_CHECKED_ON,PART_NO) ")
                        .Append(" VALUES ( ")
                        .Append(" :JOB_NO, :TENDER_NO, :CHECKLIST_ID, :VENDER_CODE,:VEND_CHECK,SYSDATE,:PART_NO ")
                        .Append(" )");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("VENDER_CODE", Session["USERID"].ToString());
                    if (chkVendor.Checked)
                    {
                        insertParam.Add("VEND_CHECK", "Y");
                    }
                    else
                    {
                        insertParam.Add("VEND_CHECK", "N");
                    }

                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else if ("N".Equals(hdVendorCheck.Value) && chkVendor.Checked)
                {
                    //Update query
                    sbInsertQuery.Append("update RAB_FINAL_BILL_CHECKLIST ")
                       .Append(" set VENDER_CODE=:VENDER_CODE, ")
                       .Append(" VEND_CHECKED_ON=sysdate, ")
                       .Append(" VEND_CHECK=:VEND_CHECK ")
                       .Append(" WHERE JOB_NO=:JOB_NO ")
                       .Append(" AND TENDER_NO=:TENDER_NO ")
                       .Append(" AND PART_NO=:PART_NO ")
                       .Append(" AND CHECKLIST_ID=:CHECKLIST_ID ")
                       .Append(" AND VEND_CHECK=:VEND_CHECK_NO ");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("CHECKED_BY_BE", Session["USERID"].ToString());
                    insertParam.Add("VEND_CHECK", "Y");
                    insertParam.Add("VEND_CHECK_NO", "N");
                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else
                {
                    chkVendor.Enabled = false;
                }
            }
            else if ("BE".Equals(Session["ROLE"].ToString()))
            {
                 if ("".Equals(hdBECheck.Value))
                 {
                    //Insert Query with N
                    sbInsertQuery.Append("INSERT INTO RAB_FINAL_BILL_CHECKLIST ")
                        .Append(" (JOB_NO, TENDER_NO, CHECKLIST_ID, CHECKED_BY_BE,BE_CHECK,CHECKED_ON_BE,PART_NO) ")
                        .Append(" VALUES ( ")
                        .Append(" :JOB_NO, :TENDER_NO, :CHECKLIST_ID, :CHECKED_BY_BE,:BE_CHECK,SYSDATE,:PART_NO ")
                        .Append(" )");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("CHECKED_BY_BE", Session["USERID"].ToString());
                    if (chkBE.Checked)
                    {
                        insertParam.Add("BE_CHECK", "Y");
                    }
                    else
                    {
                        insertParam.Add("BE_CHECK", "N");
                    }

                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else if ( "N".Equals(hdBECheck.Value) && chkBE.Checked)
                {
                    //Update query
                    sbInsertQuery.Append("update RAB_FINAL_BILL_CHECKLIST ")
                       .Append(" set CHECKED_BY_BE=:CHECKED_BY_BE, ")
                       .Append(" CHECKED_ON_BE=sysdate, ")
                       .Append(" BE_CHECK=:BE_CHECK ")
                       .Append(" WHERE JOB_NO=:JOB_NO ")
                       .Append(" AND TENDER_NO=:TENDER_NO ")
                       .Append(" AND PART_NO=:PART_NO ")
                       .Append(" AND CHECKLIST_ID=:CHECKLIST_ID ")
                       .Append(" AND BE_CHECK=:BE_CHECK_NO ");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("CHECKED_BY_BE", Session["USERID"].ToString());
                    insertParam.Add("BE_CHECK", "Y");
                    insertParam.Add("BE_CHECK_NO", "N");
                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else
                {
                    chkBE.Enabled = false;                    
                }
            }
            else if ("AC".Equals(Session["ROLE"].ToString()))
            {
                if ("Y".Equals(hdBECheck.Value) && "N".Equals(hdACCheck.Value) && chkAC.Checked)
                {
                    //Update query
                    sbInsertQuery.Append("update RAB_FINAL_BILL_CHECKLIST ")
                       .Append(" set CHECKED_BY_AC=:CHECKED_BY_AC, ")
                       .Append(" CHECKED_ON_AC=sysdate, ")
                       .Append(" AC_CHECK=:AC_CHECK ")
                       .Append(" WHERE JOB_NO=:JOB_NO ")
                       .Append(" AND TENDER_NO=:TENDER_NO ")
                       .Append(" AND PART_NO=:PART_NO ")
                       .Append(" AND CHECKLIST_ID=:CHECKLIST_ID ")
                       .Append(" AND AC_CHECK=:AC_CHECK_NO ")
                       .Append(" AND Be_CHECK=:BE_CHECK ");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("CHECKED_BY_AC", Session["USERID"].ToString());
                    insertParam.Add("AC_CHECK", "Y");
                    insertParam.Add("AC_CHECK_NO", "N");
                    insertParam.Add("BE_CHECK", "Y");
                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else
                {
                    chkAC.Enabled = false;
                }
            }
            else if ("RCM".Equals(Session["ROLE"].ToString()))
            {
                if ("Y".Equals(hdBECheck.Value) && "Y".Equals(hdACCheck.Value) &&  "N".Equals(hdRCMCheck.Value) && chkRCM.Checked)
                {
                    //Update query
                    sbInsertQuery.Append("update RAB_FINAL_BILL_CHECKLIST ")
                       .Append(" set CHECKED_BY_RCM=:CHECKED_BY_RCM, ")
                       .Append(" CHECKED_ON_RCM=sysdate, ")
                       .Append(" RCM_CHECK=:RCM_CHECK ")
                       .Append(" WHERE JOB_NO=:JOB_NO ")
                       .Append(" AND TENDER_NO=:TENDER_NO ")
                       .Append(" AND PART_NO=:PART_NO ")
                       .Append(" AND CHECKLIST_ID=:CHECKLIST_ID ")
                       .Append(" AND Be_CHECK=:BE_CHECK ")
                       .Append(" AND AC_CHECK=:AC_CHECK ")
                        .Append(" AND RCM_CHECK=:RCM_CHECK_NO ");
                    insertParam.Add("JOB_NO", ddJobNumber.SelectedValue);
                    insertParam.Add("TENDER_NO", strArray[0]);
                    insertParam.Add("PART_NO", strArray[1]);
                    insertParam.Add("CHECKLIST_ID", hdCheckListId.Value);
                    insertParam.Add("CHECKED_BY_RCM", Session["USERID"].ToString());
                    insertParam.Add("AC_CHECK", "Y");
                    insertParam.Add("RCM_CHECK_NO", "N");
                    insertParam.Add("RCM_CHECK", "Y");
                    insertParam.Add("BE_CHECK", "Y");
                    lstInsertQueries.Add(sbInsertQuery.ToString());
                    lstInsertParam.Add(insertParam);
                }
                else
                {
                    chkAC.Enabled = false;
                }
            }
        }
        int recordsAffected = 0;
        if (lstInsertQueries.Count > 0)
        {
            string[] queryArray = new String[lstInsertQueries.Count];
            Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[lstInsertParam.Count];
            for (int ii = 0; ii < lstInsertQueries.Count; ii++)
            {
                queryArray[ii] = lstInsertQueries[ii].ToString();
                paramListArray[ii] = (Dictionary<string, string>)lstInsertParam[ii];
            }
            if (queryArray.Length > 0)
                recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
        }

        if (recordsAffected > 0)
        {
            Common.Show("Checklist updated");           
            bindCheckListPoints(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
        }
        else
        {
            Common.Show("Checklist not updated");
        }
    }

    protected void gvChecklist_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdJobNo = new HiddenField();
            hdJobNo = (HiddenField)e.Row.FindControl("hdJobNo");

            HiddenField hdTenderNo = new HiddenField();
            hdTenderNo = (HiddenField)e.Row.FindControl("hdTenderNo");

            HiddenField hdPartNo = new HiddenField();
            hdPartNo = (HiddenField)e.Row.FindControl("hdPartNo");

            HiddenField hdCheckListId = new HiddenField();
            hdCheckListId = (HiddenField)e.Row.FindControl("hdCheckListId");

            HiddenField hdBE = new HiddenField();
            hdBE = (HiddenField)e.Row.FindControl("hdBE");

            HiddenField hdAC = new HiddenField();
            hdAC = (HiddenField)e.Row.FindControl("hdAC");

            HiddenField hdRCM = new HiddenField();
            hdRCM = (HiddenField)e.Row.FindControl("hdRCM");

            HiddenField hdIsFrozen = new HiddenField();
            hdIsFrozen = (HiddenField)e.Row.FindControl("hdIsFrozen");

            CheckBox chkVendor = new CheckBox();
            chkVendor = (CheckBox)e.Row.FindControl("chkVendor");

            CheckBox chkBE = new CheckBox();
            chkBE = (CheckBox)e.Row.FindControl("chkBE");

            CheckBox chkAC = new CheckBox();
            chkAC = (CheckBox)e.Row.FindControl("chkAC");

            CheckBox chkRCM = new CheckBox();
            chkRCM = (CheckBox)e.Row.FindControl("chkRCM");

            HiddenField hdBECheck = new HiddenField();
            hdBECheck = (HiddenField)e.Row.FindControl("hdBECheck");

            HiddenField hdACCheck = new HiddenField();
            hdACCheck = (HiddenField)e.Row.FindControl("hdACCheck");

            HiddenField hdRCMCheck = new HiddenField();
            hdRCMCheck = (HiddenField)e.Row.FindControl("hdRCMCheck");

            HiddenField hdVendorCheck = new HiddenField();
            hdVendorCheck = (HiddenField)e.Row.FindControl("hdVendorCheck");
            
            //TO Do write the logic to enable disable checkboxes based on selection      

            if (hdVendorCheck.Value.Equals("Y"))
            {
                chkVendor.Checked = true;
            }
            else
            {
                chkVendor.Checked = false;
            }
            
            if (hdBECheck.Value.Equals("Y"))
            {
                chkBE.Checked = true;
            }
            else
            {
                chkBE.Checked = false;
            }

            if (hdACCheck.Value.Equals("Y"))
            {
                chkAC.Checked = true;
            }
            else
            {
                chkAC.Checked = false;
            }
            if (hdRCMCheck.Value.Equals("Y"))
            {
                chkRCM.Checked = true;
            }
            else
            {
                chkRCM.Checked = false;
            }

            if ("VEND".Equals(Session["ROLE"]))
            {
                if (hdVendorCheck.Value.Equals("Y"))
                {
                    chkVendor.Enabled = false;
                }
                else
                {
                    chkVendor.Enabled = true;
                }
                chkBE.Enabled = false;
                chkAC.Enabled = false;
                chkRCM.Enabled = false;
            }
            else if ("BE".Equals(Session["ROLE"]))
            {
                if (hdVendorCheck.Value.Equals("Y") && hdBECheck.Value.Equals("N"))
                {
                    chkBE.Enabled = true;
                }
                else
                {
                    chkBE.Enabled = false;
                }
                chkVendor.Enabled = false;
                chkAC.Enabled = false;
                chkRCM.Enabled = false;
            }
            else if ("AC".Equals(Session["ROLE"]))
            {
                if (hdVendorCheck.Value.Equals("Y") && hdBECheck.Value.Equals("Y") && hdACCheck.Value.Equals("N"))
                {
                    chkAC.Enabled = true;
                }
                else
                {
                    chkAC.Enabled = false;
                }
                chkVendor.Enabled = false;
                chkBE.Enabled = false;
                chkRCM.Enabled = false;
            }
            else if ("RCM".Equals(Session["ROLE"]))
            {
                if (hdVendorCheck.Value.Equals("Y") && hdBECheck.Value.Equals("Y") && hdACCheck.Value.Equals("Y") && hdRCMCheck.Value.Equals("N"))
                {
                    chkRCM.Enabled = true;
                }
                else
                {
                    chkRCM.Enabled = false;
                }
                chkVendor.Enabled = false;
                chkBE.Enabled = false;
                chkAC.Enabled = false;
            }
            else
            {
                chkVendor.Enabled = false;
                chkBE.Enabled = false;
                chkAC.Enabled = false;
                chkRCM.Enabled = false;
                btnSubmit.Visible = false;
                btnFreeze.Visible = false;
            }
        }
    }

    protected void btnFreeze_Click(object sender, EventArgs e)
    {
    }
    
}