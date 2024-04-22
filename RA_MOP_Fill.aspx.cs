using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;
using System.Data;
using System.Collections;

public partial class RA_MOP_Fill : System.Web.UI.Page
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
            if (Session["USERID"] != null && Session["ROLE"] != null)
            {                
                bindJobNumber(Session["USERID"].ToString(), Session["ROLE"].ToString());
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }          
    }

    protected void bindJobNumber(string userId, string userRole)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        if ("VEND".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_MASTER where C_CODE=:C_CODE ORDER BY JOB_NO");
            paramList.Add("C_CODE", userId);
        }
        else if ("BE".Equals(userRole) || "AC".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_USERS where ROLE=:ROLE and EMPNO=:EMPNO and ACTIVE='Y'  ORDER BY JOB_NO" );
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
        if ("BE".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()) || "RCM".Equals(Session["ROLE"].ToString()))
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
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue))
        {            
            bindBills(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue);
           // bindMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue);
        }
    }

    protected void bindBills(string jobNumber, string tenderNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        string[] strArray = tenderNumber.Split('~');

        sbQuery.Append(@"select distinct ra_final_bill_no||'~'||ra_date RA_BILL_DATE,ra_final_bill_no||' [Bill Date- '||ra_date||']' ra_final_bill_no  from RAB_TENDER_BILL a where tend_sor_id in (
                select tend_sor_id from RAB_TENDER_MASTER aa where AA.JOB_NO=:JOB_NO AND AA.TENDER_NO=:TENDER_NO AND AA.PART_NO=:PART_NO  )
                and RCM_IS_FROZEN='Y' ORDER BY  ra_final_bill_no");
        paramList.Add("JOB_NO", jobNumber.ToUpper());
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        objDB.bindDropDownList(ddBillNumber, sbQuery.ToString(), paramList, "RA_BILL_DATE", "ra_final_bill_no", "", "--Select RA Bill Number--");
    }

    protected void ddBillNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) &&  !"".Equals(ddBillNumber.SelectedValue))
        {
            
            DataTable dt = new DataTable();
            dt = getMOPStatus();
            enableActionButtons(dt);
            
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"select count(*) from RAB_MOP_BILL_DTL a where  a.JOB_NO =:JOB_NO
                        AND a.TENDER_NO =:TENDER_NO
                        AND a.PART_NO =:PART_NO
                        AND RA_BILL_NO =:RA_BILL_NO
                        ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            string[] strBill = ddBillNumber.SelectedValue.Split('~');
            paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
            paramList.Add("TENDER_NO", strArray[0]);
            paramList.Add("PART_NO", strArray[1]);
            paramList.Add("RA_BILL_NO", strBill[0]);

            string countRecord = objDB.executeScalar(sbQuery.ToString(), paramList);

            if (countRecord.Length > 0 && int.Parse(countRecord) > 0)
            {
                // If MoP exist, find if it is the latest MoP so that RCM may edit upto previous bill data
                if ("RCM".Equals(Session["ROLE"].ToString()))
                {
                    StringBuilder sbQueryMaxMop = new StringBuilder();
                    Dictionary<string, string> paramMoPList = new Dictionary<string, string>();                   
                    paramMoPList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
                    paramMoPList.Add("TENDER_NO", strArray[0]);
                    paramMoPList.Add("PART_NO", strArray[1]);
                    paramMoPList.Add("RCM_IS_FROZEN", "Y");                  
                  
                    sbQueryMaxMop.Append(@"SELECT NVL (MAX (id), 0)
                              FROM RAB_MOP_BILL_DTL a,RAB_MOP_BILL_ACTION b
                             WHERE     a.JOB_NO =:JOB_NO
                                   AND a.TENDER_NO =:TENDER_NO
                                   AND a.PART_NO =:PART_NO    
                                   and B.MOP_ID=A.ID
                                   and B.RCM_IS_FROZEN=:RCM_IS_FROZEN");
                    string maxMOPID = objDB.executeScalar(sbQueryMaxMop.ToString(), paramMoPList);
                    if (maxMOPID.Length > 0 && maxMOPID != "0" && hdMOPId.Value.Equals(maxMOPID))
                    {
                        ViewState["IfmaxMoPID"] = "Y";
                        ViewState["MaxMoPID"] = maxMOPID;
                        btnEditRecoveries.Visible = true;
                    }
                    else
                    {
                        ViewState["IfmaxMoPID"] = "N";
                        btnEditRecoveries.Visible = false;
                    }
                }
                else
                {
                    ViewState["IfmaxMoPID"] = "N";
                    btnEditRecoveries.Visible = false;
                }

                // bindMOPHeader(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue,dt);
                bindRecoveriesMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                bindPaymentReccomendation(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                // to display total in the grid
                populateData();
            }
            else
            {
                StringBuilder sbQueryHeading = new StringBuilder();
                sbQueryHeading.Append(@"select count(*) from RAB_MOP_HEADING_DTL a where  a.JOB_NO =:JOB_NO
                 AND a.TENDER_NO =:TENDER_NO
                AND a.PART_NO =:PART_NO  
                AND a.BILL_NO =:BILL_NO            
                ");
                Dictionary<string, string> paramHeadingList = new Dictionary<string, string>();
                
                paramHeadingList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
                paramHeadingList.Add("TENDER_NO", strArray[0]);
                paramHeadingList.Add("PART_NO", strArray[1]);
                paramHeadingList.Add("BILL_NO", strBill[0]); 
                bool dataPopulated = false;
                string countHeadingRecord = objDB.executeScalar(sbQueryHeading.ToString(), paramHeadingList);
                if (countHeadingRecord.Equals("0"))
                {
                    Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                    paramListMOP.Add("T_SOURCE_JOBNO", "0000");
                    paramListMOP.Add("T_DEST_JOBNO", ddJobNumber.SelectedValue.ToString());
                    paramListMOP.Add("T_TENDER_NO", strArray[0]);
                    paramListMOP.Add("T_USER", Session["USERID"].ToString());
                    paramListMOP.Add("T_PART_NO", strArray[1]);                    
                    paramListMOP.Add("T_BILL_NO", strBill[0]);
                    try
                    {
                        objDB.executeProcedure("RABILLING.RAB_COPY_MOP", paramListMOP);
                        dataPopulated = true;
                    }
                    catch (Exception err)
                    {
                        Common.Show("Error:Kindly select Job Number and Tender Number");
                        dataPopulated = false;
                    }

                    if (dataPopulated)
                    {
                        dt = getMOPStatus();
                        enableActionButtons(dt);
                        // bindMOPHeader(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue,dt);
                        bindRecoveriesMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                        bindPaymentReccomendation(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                        // to display total in the grid
                        populateData();
                    }

                }
                else {
                    dt = getMOPStatus();
                    enableActionButtons(dt);
                    // bindMOPHeader(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue,dt);
                    bindRecoveriesMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                    bindPaymentReccomendation(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue, dt);
                    // to display total in the grid
                    populateData();
                }
            }
           
        }
    }

    protected DataTable getMOPStatus()
    {
        DataTable dt = new DataTable();
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.Append(@"select MOP_ID, BE_IS_FROZEN, BE_EMPNO,  
                        AC_IS_FROZEN, AC_EMPNO, RCM_IS_FROZEN, RCM_EMPNO ,VENDOR_FROZEN,
                        nvl(ACTUAL_COMPLETION_REMARKS,to_char(ACTUAL_COMPLETION_DT,'dd-Mon-yyyy')) ACTUAL_COMPLETION_DT,
                        EXTENSION_PERIOD,
                        PERCENT_PROGRESS ,enhanced_value 
                        from RAB_MOP_BILL_ACTION
                        where MOP_ID =  (select distinct id from RAB_MOP_BILL_DTL
                        where JOB_NO=:JOB_NO
                        and  TENDER_NO=:TENDER_NO
                        and  RA_BILL_NO=:RA_BILL_NO
                        and  PART_NO=:PART_NO
                        )");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        string[] strBill = ddBillNumber.SelectedValue.Split('~');
        paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        paramList.Add("RA_BILL_NO", strBill[0]);
        dt = objDB.bindDataTable(sbQuery.ToString(), paramList);
        return dt;
    }


    protected void enableActionButtons(DataTable dt)
    {
        trShowMOPEdit.Visible = true;
        lnlMOPEdit.Text = "Click to Edit MOP Fields";
        lnlMOPEdit.NavigateUrl = "RA_MOP_Update.aspx";
       
        //To enable disable button based on the MOP status pending at the user role
        trActionButtons.Visible = true;

        if (dt.Rows.Count == 0)
        {
            hdMOPId.Value = "0";
            hdMOPVendStatus.Value = "N";
            hdMOPBEStatus.Value = "N";
            hdMOPACStatus.Value = "N";
            hdMOPRCMStatus.Value = "N";            
            
            lblMOPStatus.Text = "Status: MOP not yet filled by Contractor/BE";
            if ("VEND".Equals(Session["ROLE"].ToString()) || "BE".Equals(Session["ROLE"].ToString()))
            {
                txtTimeExtension.Visible = true;
                txtPercentProgress.Visible = true;
                txtCompletionDate.Visible = true;
                txtEnhancedValue.Visible = true;
                //btnCal1.Visible = true;
                lblActualCompletionDate.Visible = false;
                lblEnhancedValue.Visible = false;
                lblTimeExtension.Visible = false;
                lblPercentProgress.Visible = false;            
            }
            else if ("RCM".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()))
            {
                txtTimeExtension.Visible = false;
                txtPercentProgress.Visible = false;
                txtCompletionDate.Visible = false;
                txtEnhancedValue.Visible = false;
                //btnCal1.Visible = false;
                lblActualCompletionDate.Visible = true;
                lblEnhancedValue.Visible = true;
                lblTimeExtension.Visible = true;
                lblPercentProgress.Visible = true;                
            }
        }
        else if (dt.Rows.Count > 0)
        {
            hdMOPId.Value = dt.Rows[0]["MOP_ID"].ToString();
            hdMOPVendStatus.Value = dt.Rows[0]["VENDOR_FROZEN"].ToString();
            hdMOPBEStatus.Value = dt.Rows[0]["BE_IS_FROZEN"].ToString();
            hdMOPACStatus.Value = dt.Rows[0]["AC_IS_FROZEN"].ToString();
            hdMOPRCMStatus.Value = dt.Rows[0]["RCM_IS_FROZEN"].ToString();

            if ("RCM".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()))
            {
            txtTimeExtension.Visible = false;
            txtPercentProgress.Visible = false;
            txtCompletionDate.Visible = false;
            txtEnhancedValue.Visible = false;
            //btnCal1.Visible = false;
            lblActualCompletionDate.Visible = true;
            lblEnhancedValue.Visible = true;
            lblTimeExtension.Visible = true;
            lblPercentProgress.Visible = true;

            lblActualCompletionDate.Text=dt.Rows[0]["ACTUAL_COMPLETION_DT"].ToString();
            lblTimeExtension.Text=dt.Rows[0]["EXTENSION_PERIOD"].ToString();
            lblPercentProgress.Text = dt.Rows[0]["PERCENT_PROGRESS"].ToString();
            lblEnhancedValue.Text = dt.Rows[0]["enhanced_value"].ToString();
                
            }
            else if ("VEND".Equals(Session["ROLE"].ToString()) || "BE".Equals(Session["ROLE"].ToString()))
            {
                txtTimeExtension.Visible = false;
                txtPercentProgress.Visible = false;
                txtCompletionDate.Visible = false;
                txtEnhancedValue.Visible = false;
                //btnCal1.Visible = false;

                lblActualCompletionDate.Visible = true;
                lblEnhancedValue.Visible = true;
                lblTimeExtension.Visible = true;
                lblPercentProgress.Visible = true;
                lblActualCompletionDate.Text = dt.Rows[0]["ACTUAL_COMPLETION_DT"].ToString();
                lblTimeExtension.Text = dt.Rows[0]["EXTENSION_PERIOD"].ToString();
                lblPercentProgress.Text = dt.Rows[0]["PERCENT_PROGRESS"].ToString();
                lblEnhancedValue.Text = dt.Rows[0]["enhanced_value"].ToString();
            }
            if ("Y".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString()))
            {
                lblMOPStatus.Text = "Status: MOP Submitted by Vendor";
            }
            else if ("Y".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString()))
            {
                lblMOPStatus.Text = "Status: MOP approved by RCM";
            }
            else if ("Y".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString()))
            {
                lblMOPStatus.Text = "Status: MOP approved by AC and pending with RCM";
            }
            else if ("Y".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString()))
            {
                lblMOPStatus.Text = "Status: MOP approved by BE and pending with AC";
            }
            else if ("N".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString()))
            {
                lblMOPStatus.Text = "Status: MOP is yet to be submitted by Vendor";
            }
            else
            {
                lblMOPStatus.Text = "";
            }
        }       

        if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            if (dt.Rows.Count == 0)
            {
                btnAddMOPVend.Visible = true;
            }
            else if (dt.Rows.Count > 0 && "Y".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()))
            {
                btnAddMOPVend.Visible = false;
            }
            else if (dt.Rows.Count > 0 && "N".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()))
            {
                btnAddMOPVend.Visible = true;
            }
            btnRejectMOPBE.Visible = false;
            btnAddMopData.Visible = false;
            btnApproveAC.Visible = false;
            btnRejectAC.Visible = false;
            btnApproveRCM.Visible = false;
            btnRejectRCM.Visible = false;            
            
        }
        else if ("BE".Equals(Session["ROLE"].ToString()) )
        {            
            btnAddMOPVend.Visible = false;
            if (dt.Rows.Count > 0 && ("Y".Equals(hdMOPVendStatus.Value) && "N".Equals(hdMOPBEStatus.Value)))
            {
                btnAddMopData.Visible = true;
                btnRejectMOPBE.Visible = true;
            }
            else if (dt.Rows.Count == 0 && ("N".Equals(hdMOPVendStatus.Value) && "N".Equals(hdMOPBEStatus.Value)))
            {
                btnAddMopData.Visible = true;
                btnRejectMOPBE.Visible = false;
            }
            else
            {
                btnAddMopData.Visible = false;
                btnRejectMOPBE.Visible = false;
            }
            btnApproveAC.Visible = false;
            btnRejectAC.Visible = false;
            btnApproveRCM.Visible = false;
            btnRejectRCM.Visible = false;
        }
        else if ("AC".Equals(Session["ROLE"].ToString()) )
        {            
            btnAddMOPVend.Visible = false;
            btnAddMopData.Visible = false;
            btnRejectMOPBE.Visible = false;
            if (dt.Rows.Count > 0 && ("Y".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString())))
            {
                btnApproveAC.Visible = true;
                btnRejectAC.Visible = true;
            }
            else {
                btnApproveAC.Visible = false;
                btnRejectAC.Visible = false;
            }
            btnApproveRCM.Visible = false;
            btnRejectRCM.Visible = false;
        }
        else if ("RCM".Equals(Session["ROLE"].ToString()))
        {
            btnAddMOPVend.Visible = false;
            btnAddMopData.Visible = false;
            btnApproveAC.Visible = false;
            btnRejectAC.Visible = false;
            btnRejectMOPBE.Visible = false;
            if (dt.Rows.Count > 0 && ("Y".Equals(dt.Rows[0]["VENDOR_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["BE_IS_FROZEN"].ToString()) && "Y".Equals(dt.Rows[0]["AC_IS_FROZEN"].ToString()) && "N".Equals(dt.Rows[0]["RCM_IS_FROZEN"].ToString())))
            {
                btnApproveRCM.Visible = true;
                btnRejectRCM.Visible = true;
            }
            else {
                btnApproveRCM.Visible = false;
                btnRejectRCM.Visible = false;
            }
        }
        else
        {
            btnAddMOPVend.Visible = false;
            btnAddMopData.Visible = false;
            btnApproveAC.Visible = false;
            btnRejectAC.Visible = false;
            btnApproveRCM.Visible = false;
            btnRejectRCM.Visible = false;
            btnRejectMOPBE.Visible = false;
        }
    }

    protected void populateData()
    {
        //Calculating Work done
        double workDoneUP = 0;
        double workDoneUPExtra = 0;
        double workDoneSP = 0;
        double workDoneSPExtra = 0;
        double workDoneTU = 0;

        Dictionary<string, string> paramList = new Dictionary<string, string>();
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        string[] strBill = ddBillNumber.SelectedValue.Split('~');
        paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        paramList.Add("RA_BILL_NO", strBill[0]);


        Dictionary<string, string> paramListWUP = new Dictionary<string, string>();
        paramListWUP.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
        paramListWUP.Add("TENDER_NO", strArray[0]);
        paramListWUP.Add("PART_NO", strArray[1]);
        paramListWUP.Add("BILL_NO", strBill[0]);

        paramListWUP.Add("JOB_NO2", ddJobNumber.SelectedValue.ToUpper());
        paramListWUP.Add("TENDER_NO2", strArray[0]);
        paramListWUP.Add("PART_NO2", strArray[1]);

//        workDoneUP = double.Parse( objDB.executeScalar(@"SELECT nvl(ROUND (SUM (QTY_VAL)),0)
//                        FROM (SELECT ACTIVITY_PERCENT,
//               ACTIVITYAMT,
//               RCM_QTY,
//                 (  (ACTIVITYAMT / ACTIVITYQTY)
//                  * RCM_QTY
//                  * REPLACE (ACTIVITY_PERCENT, '%', ''))
//               / 100
//                  QTY_VAL
//          FROM VW_RA_BILL_ITEMS_DETAIL A
//         WHERE     A.TEND_SOR_ID IN
//                      (SELECT TEND_SOR_ID
//                         FROM RAB_TENDER_MASTER AA
//                        WHERE     Aa.JOB_NO =:JOB_NO
//                              AND Aa.TENDER_NO =:TENDER_NO
//                              AND Aa.PART_NO =:PART_NO)
//               AND A.RCM_IS_FROZEN = 'Y'
//               AND A.RUN_SL_NO BETWEEN 0 AND ( select distinct (A.RUN_SL_NO -1) from VW_RA_BILL_ITEMS_DETAIL a 
//                WHERE A.TEND_SOR_ID in ( SELECT TEND_SOR_ID
//                         FROM RAB_TENDER_MASTER Ab
//                        WHERE     Ab.JOB_NO =:JOB_NO2
//                              AND Ab.TENDER_NO =:TENDER_NO2
//                              AND Ab.PART_NO =:PART_NO2) and A.RA_FINAL_BILL_NO=:BILL_NO and a.RCM_IS_FROZEN='Y') )", paramListWUP));

           workDoneUP = double.Parse( objDB.executeScalar(@"SELECT nvl(ROUND (SUM (QTY_VAL),3),0)
                        FROM (SELECT ACTIVITY_PERCENT,
               ACTIVITYAMT,
               RCM_QTY,
                 (  (ACTIVITYAMT / ACTIVITYQTY)
                  * (case when RCM_QTY = 0 then CONT_QTY else  RCM_QTY end)
                  * REPLACE (ACTIVITY_PERCENT, '%', ''))
               / 100
                  QTY_VAL
          FROM VW_RA_BILL_ITEMS_DETAIL A
         WHERE  A.FLAG_HO='N' and A.RCM_IS_FROZEN='Y' AND A.CONT_IS_FROZEN = 'Y' and   A.TEND_SOR_ID IN
                      (SELECT TEND_SOR_ID
                         FROM RAB_TENDER_MASTER AA
                        WHERE     Aa.JOB_NO =:JOB_NO
                              AND Aa.TENDER_NO =:TENDER_NO
                              AND Aa.PART_NO =:PART_NO)
               AND A.RCM_IS_FROZEN = 'Y'
               AND A.RUN_SL_NO BETWEEN 0 AND ( select distinct (A.RUN_SL_NO -1) from VW_RA_BILL_ITEMS_DETAIL a 
                WHERE A.TEND_SOR_ID in ( SELECT TEND_SOR_ID
                         FROM RAB_TENDER_MASTER Ab
                        WHERE     Ab.JOB_NO =:JOB_NO2
                              AND Ab.TENDER_NO =:TENDER_NO2
                              AND Ab.PART_NO =:PART_NO2) and A.RA_FINAL_BILL_NO=:BILL_NO and a.RCM_IS_FROZEN='Y') )", paramListWUP));


           try
           {
               workDoneUPExtra = double.Parse(objDB.executeScalar(@"SELECT nvl(ROUND (SUM (QTY_VAL),3),0)
                        FROM (SELECT ACTIVITY_PERCENT,
               ACTIVITYAMT,
               RCM_QTY,
                 (  (ACTIVITYAMT / ACTIVITYQTY)
                  * (case when RCM_QTY = 0 then CONT_QTY else  RCM_QTY end)
                  * REPLACE (ACTIVITY_PERCENT, '%', ''))
               / 100
                  QTY_VAL
          FROM VW_RA_BILL_ITEMS_DETAIL A
         WHERE  A.FLAG_HO='Y' and A.RCM_IS_FROZEN='Y' AND A.CONT_IS_FROZEN = 'Y' and   A.TEND_SOR_ID IN
                      (SELECT TEND_SOR_ID
                         FROM RAB_TENDER_MASTER AA
                        WHERE     Aa.JOB_NO =:JOB_NO
                              AND Aa.TENDER_NO =:TENDER_NO
                              AND Aa.PART_NO =:PART_NO)
               AND A.RCM_IS_FROZEN = 'Y'
               AND A.RUN_SL_NO BETWEEN 0 AND ( select distinct (A.RUN_SL_NO -1) from VW_RA_BILL_ITEMS_DETAIL a 
                WHERE A.TEND_SOR_ID in ( SELECT TEND_SOR_ID
                         FROM RAB_TENDER_MASTER Ab
                        WHERE     Ab.JOB_NO =:JOB_NO2
                              AND Ab.TENDER_NO =:TENDER_NO2
                              AND Ab.PART_NO =:PART_NO2) and A.RA_FINAL_BILL_NO=:BILL_NO and a.RCM_IS_FROZEN='Y') )", paramListWUP));

           }
           catch (Exception err)
           {
               workDoneUPExtra = 0;
           }

       //Finding the rebate if any and modifying the value based on the rebate
       // double rebate = 0;
        Dictionary<string, string> paramListRebate = new Dictionary<string, string>();
        
        paramListRebate.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
        paramListRebate.Add("TENDER_NO", strArray[0]);
        paramListRebate.Add("PART_NO", strArray[1]);

        string percentageRebate = "";
        percentageRebate = objDB.executeScalar(@"select PECENTAGE_VALUE from RAB_TENDER_REBATE where job_no=:JOB_NO and tender_no=:TENDER_NO and part_no=:PART_NO and is_frozen='Y'", paramListRebate);

        //if (!string.Equals(percentageRebate, ""))
        //{

        //    decimal NewPercentageValue = decimal.Parse(percentageRebate.Replace("-", ""));
        //}
        double NewPercentageValue = 0;
        if (percentageRebate.Length > 0)
        {
            NewPercentageValue = double.Parse(percentageRebate.Replace("-", ""));
        }        
        
//        workDoneSP = double.Parse( objDB.executeScalar(@"SELECT round(SUM(QTY_VAL)) FROM
//                    (
//                SELECT ACTIVITY_PERCENT, ACTIVITYAMT,RCM_QTY , ((ACTIVITYAMT/ACTIVITYQTY) * RCM_QTY * REPLACE(ACTIVITY_PERCENT,'%',''))/100 QTY_VAL FROM VW_RA_BILL_ITEMS_DETAIL A WHERE A.TEND_SOR_ID IN (
//                SELECT TEND_SOR_ID FROM RAB_TENDER_MASTER AA WHERE AA.JOB_NO=:JOB_NO AND AA.TENDER_NO=:TENDER_NO AND AA.PART_NO=:PART_NO
//                ) AND A.RA_FINAL_BILL_NO=:RA_BILL_NO AND A.RCM_IS_FROZEN='Y'
//                )", paramList));

        try
        {
          workDoneSP = double.Parse( objDB.executeScalar(@"SELECT round(SUM(QTY_VAL),3) FROM
                    (
                SELECT ACTIVITY_PERCENT, ACTIVITYAMT,RCM_QTY , ((ACTIVITYAMT/ACTIVITYQTY) * (case when RCM_QTY = 0 then CONT_QTY else  RCM_QTY end) * REPLACE(ACTIVITY_PERCENT,'%',''))/100 QTY_VAL FROM VW_RA_BILL_ITEMS_DETAIL A WHERE A.TEND_SOR_ID IN (
                SELECT TEND_SOR_ID FROM RAB_TENDER_MASTER AA WHERE AA.JOB_NO=:JOB_NO AND AA.TENDER_NO=:TENDER_NO AND AA.PART_NO=:PART_NO
                ) AND A.RA_FINAL_BILL_NO=:RA_BILL_NO AND A.RCM_IS_FROZEN='Y' AND A.CONT_IS_FROZEN = 'Y' and a.FLAG_HO='N'
                )", paramList));
         }
        catch (Exception err) { }
        
        if (percentageRebate.Length > 0 && percentageRebate.Contains("-"))
        {
            //workDoneUP = Math.Round(workDoneUP - (workDoneUP * (Math.Round(NewPercentageValue, 2))/100));
            //workDoneSP = Math.Round(workDoneSP - (workDoneSP * (Math.Round(NewPercentageValue, 2))/100));

            workDoneUP = Math.Round(workDoneUP - (workDoneUP * (NewPercentageValue) / 100),2);
            workDoneSP = Math.Round(workDoneSP - (workDoneSP * (NewPercentageValue) / 100),2);
        }
        else
        {
            //workDoneUP = Math.Round(workDoneUP + (workDoneUP * (Math.Round(NewPercentageValue, 2))/100));
            //workDoneSP = Math.Round(workDoneSP + (workDoneSP * (Math.Round(NewPercentageValue, 2))/100));
            workDoneUP = Math.Round(workDoneUP + (workDoneUP * (NewPercentageValue) / 100),2);
            workDoneSP = Math.Round(workDoneSP + (workDoneSP * (NewPercentageValue) / 100),2);
        }

        workDoneUP = Math.Round(workDoneUP + workDoneUPExtra, 2);

        try
        {
        // Rebate is not applicable on Extra items so added it on 30-Sep-2020
        workDoneSPExtra = double.Parse(objDB.executeScalar(@"SELECT round(SUM(QTY_VAL),3) FROM
                    (
                SELECT ACTIVITY_PERCENT, ACTIVITYAMT,RCM_QTY , ((ACTIVITYAMT/ACTIVITYQTY) * (case when RCM_QTY = 0 then CONT_QTY else  RCM_QTY end) * REPLACE(ACTIVITY_PERCENT,'%',''))/100 QTY_VAL FROM VW_RA_BILL_ITEMS_DETAIL A WHERE A.TEND_SOR_ID IN (
                SELECT TEND_SOR_ID FROM RAB_TENDER_MASTER AA WHERE AA.JOB_NO=:JOB_NO AND AA.TENDER_NO=:TENDER_NO AND AA.PART_NO=:PART_NO
                ) AND A.RA_FINAL_BILL_NO=:RA_BILL_NO AND A.RCM_IS_FROZEN='Y' AND A.CONT_IS_FROZEN = 'Y' and a.FLAG_HO='Y'
                )", paramList));
       
            workDoneSP = workDoneSP + workDoneSPExtra;
        }
        catch (Exception err) { }

        lblUptoPreviousBill.Text = workDoneUP.ToString();
        
        lblSincePreviousBill.Text = workDoneSP.ToString();
        lblUptoPreviousBill.Visible = true;
        lblSincePreviousBill.Visible = true;
        lblTotalUptodate.Visible = true;
                 
        workDoneTU = workDoneUP + workDoneSP;
        lblTotalUptodate.Text = workDoneTU.ToString();
        lblUptoPreviousBill.Text = workDoneUP.ToString();

        // Work done calculation Complete

        // Calculation for Recoveries and Payment Recommendations start

        double totalRecommendedZ_UP = 0;
        double totalRecommendedZ_SP = 0;
        double totalRecommendedZ_TU = 0;

        double totalRecoveryY_UP = 0;
        double totalRecoveryY_SP = 0;
        double totalRecoveryY_TU = 0;

        foreach (GridViewRow row in gvMopRecoveriesValues.Rows)
                    {                
                        Label lbluptoPrevBill = new Label();
                        lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                        Label lblSincePrevBill = new Label();
                        lblSincePrevBill = (Label)row.FindControl("lblSincePrevBill");

                        Label lbltotalBill = new Label();
                        lbltotalBill = (Label)row.FindControl("lbltotalBill");

                        TextBox txtSincePrevBill = new TextBox();
                        txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");
                                    
                        if (lbluptoPrevBill.Text.Length > 0 )
                        {
                            totalRecoveryY_UP = totalRecoveryY_UP + double.Parse(lbluptoPrevBill.Text);
                        }

                        if (lblSincePrevBill.Text.Length > 0)
                        {
                            totalRecoveryY_SP = totalRecoveryY_SP + double.Parse(lblSincePrevBill.Text);
                        }
                        if (lbltotalBill.Text.Length > 0)
                        {
                            totalRecoveryY_TU = totalRecoveryY_TU + double.Parse(lbltotalBill.Text);
                        }
                }

        foreach (GridViewRow row in gvMopPaymentRecommendations.Rows)
        {
            Label lbluptoPrevBill = new Label();
            lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

            Label lblSincePrevBill = new Label();
            lblSincePrevBill = (Label)row.FindControl("lblSincePrevBill");

            Label lbltotalBill = new Label();
            lbltotalBill = (Label)row.FindControl("lbltotalBill");

            TextBox txtSincePrevBill = new TextBox();
            txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

            if (lbluptoPrevBill.Text.Length > 0)
            {
                totalRecommendedZ_UP = totalRecommendedZ_UP + double.Parse(lbluptoPrevBill.Text);
            }

            if (lblSincePrevBill.Text.Length > 0)
            {
                totalRecommendedZ_SP = totalRecommendedZ_SP + double.Parse(lblSincePrevBill.Text);
            }
            if (lbltotalBill.Text.Length > 0)
            {
                totalRecommendedZ_TU = totalRecommendedZ_TU + double.Parse(lbltotalBill.Text);
            }
        }

        lblTotalRecovPB.Text = totalRecoveryY_UP.ToString();
        lblTotalRecovSPB.Text = totalRecoveryY_SP.ToString();
        lblTotalRecovTotal.Text = totalRecoveryY_TU.ToString();

        lblPaymentRecUP.Text = totalRecommendedZ_UP.ToString();
        lblPaymentRecSP.Text = totalRecommendedZ_SP.ToString();
        lblPaymentRecTU.Text = totalRecommendedZ_TU.ToString();

        lblNetAmountUP.Text = (workDoneUP - totalRecoveryY_UP + totalRecommendedZ_UP).ToString();
        lblNetAmountUP.Visible = true;

        lblNetAmountSP.Text = (workDoneSP - totalRecoveryY_SP + totalRecommendedZ_SP).ToString();
        lblNetAmountSP.Visible = true;

        lblNetAmountTU.Text = (workDoneTU - totalRecoveryY_TU + totalRecommendedZ_TU).ToString();
        lblNetAmountTU.Visible = true;
}

    protected void bindMOPHeader(string jobNumber, string tenderNo, string billNumber, DataTable dt)
    {
        string[] strArray = tenderNo.Split('~');
        string[] strBill = billNumber.Split('~');
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        Dictionary<string, string> paramListJob = new Dictionary<string, string>();
        paramListJob.Add("JOB_NO", jobNumber);
        paramListJob.Add("TENDER_NO", strArray[0]);
        paramListJob.Add("PART_NO", strArray[1]);
        paramListJob.Add("BILL_NO", strBill[0]);

        objDB.bindGridView(gvMOPHeading, @"select A.ID heading_id,'0' sub_heading,A.HEADING_DESC                                                         description,A.VALUE_EXISTS,A.HEADING_ORDER,B.HEADING_VALUE from
                                            RAB_MOP_HEADING_DTL a,RAB_MOP_BILL_DTL b
                                            where a.job_no=:JOB_NO 
                                            and a.part_no=:PART_NO 
                                            and a.tender_no=:TENDER_NO
                                            and a.bill_no=:BILL_NO 
                                            and A.HEADING_ORDER in (7,8,9) 
                                            and A.JOB_NO=B.JOB_NO(+)
                                            and A.part_no=B.part_no(+)
                                            and A.tender_no=B.tender_no(+)
                                            and A.bill_no=B.RA_BILL_NO(+)
                                            
                                order by A.HEADING_ORDER", paramListJob);

        if (gvMOPHeading.Rows.Count > 0)
        {
            bindMOPMasterDetails();
            pnlJobMOP.Visible = true;
        }
    }

protected void bindMOPMasterDetails()
{
    string[] strArray = ddTenderNo.SelectedValue.Split('~');
    string[] strBill = ddBillNumber.SelectedValue.Split('~');
    StringBuilder sbQuery = new StringBuilder();
//    sbQuery.Append(@"SELECT DISTINCT TENDER_DESCR,FOA_NO,C_NAME,ra_bll_no,to_char(ra_date,'dd-Mon-yyyy') ra_date,to_char(PERIOD_FROM,'dd-Mon-yyyy') PERIOD_FROM,
//                    to_char(PERIOD_TO,'dd-Mon-yyyy') PERIOD_TO ,net_contr_amount,
//                    contr_compl || ' ' ||  contr_compl_unit cont_comp,
//                    contr_from,to_char(foa_date,'dd-Mon-yyyy') foa_date
//                    FROM VW_RAB_MOP_mst where
//                    JOB_NO=:JOB_NO
//                    AND TENDER_NO=:TENDER_NO
//                    AND PART_NO=:PART_NO
//                    AND RA_BLL_NO=:BILL_NO");

    sbQuery.Append(@"SELECT DISTINCT TENDER_DESCR,nvl(tenderno_long,FOA_NO ) FOA_NO,C_NAME,ra_bll_no,to_char(ra_date,'dd-Mon-yyyy') ra_date,to_char(PERIOD_FROM,'dd-Mon-yyyy')                          PERIOD_FROM,
                    to_char(PERIOD_TO,'dd-Mon-yyyy') PERIOD_TO ,net_contr_amount,
                    contr_compl || ' ' ||  contr_compl_unit cont_comp,
                    nvl(to_char(foa_date),to_char(contr_from)) contr_from,to_char(foa_date,'dd-Mon-yyyy') foa_date
                    FROM VW_RAB_MOP_mst where
                    JOB_NO=:JOB_NO
                    AND TENDER_NO=:TENDER_NO
                    AND PART_NO=:PART_NO
                    AND RA_BLL_NO=:BILL_NO");
    Dictionary<string, string> paramListJob = new Dictionary<string, string>();
    paramListJob.Add("JOB_NO", ddJobNumber.SelectedValue);
    paramListJob.Add("TENDER_NO", strArray[0]);
    paramListJob.Add("PART_NO", strArray[1]);
    paramListJob.Add("BILL_NO", strBill[0]);
    DataTable dtTbl = new DataTable();
    dtTbl = objDB.bindDataTable(sbQuery.ToString(), paramListJob);
    if (dtTbl.Rows.Count > 0)
    {
        lblBillNumber.Text = dtTbl.Rows[0]["ra_bll_no"].ToString();
        lblBillDate.Text = dtTbl.Rows[0]["ra_date"].ToString();
        lblPeriod.Text = "From - " + dtTbl.Rows[0]["PERIOD_FROM"].ToString() + " To - " + dtTbl.Rows[0]["PERIOD_TO"].ToString();
        lblWork.Text = dtTbl.Rows[0]["TENDER_DESCR"].ToString();
        lblContractor.Text = dtTbl.Rows[0]["C_NAME"].ToString();
        lblFOI.Text = dtTbl.Rows[0]["FOA_NO"].ToString();
        lblDateAwarded.Text = dtTbl.Rows[0]["foa_date"].ToString();
        lblCompletionDate.Text = dtTbl.Rows[0]["cont_comp"].ToString();
        lblContractValue.Text = dtTbl.Rows[0]["net_contr_amount"].ToString();
    }
}

    protected void gvMOPHeading_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void bindRecoveriesMop(string jobNumber, string tenderNo, string billNumber, DataTable dt)
    {
        string[] strArray = tenderNo.Split('~');
        string[] strBill = billNumber.Split('~');
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        Dictionary<string, string> paramListJob = new Dictionary<string, string>();
        paramListJob.Add("JOB_NO", jobNumber);
        paramListJob.Add("TENDER_NO", strArray[0]);
        paramListJob.Add("PART_NO", strArray[1]);
        paramListJob.Add("BILL_NO", strBill[0]);

//        objDB.bindGridView(gvMopRecoveriesValues, @"select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER
//                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
//                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
//                and a.BILL_NO =:BILL_NO
//                and a.BILL_NO =b.BILL_NO(+)
//                and a.job_no=b.job_no(+)
//                and a.tender_no=b.tender_no(+)
//                and a.part_no=b.part_no(+)
//                and A.ID=B.HEADING_ID(+)
//                and A.HEADING_ORDER in (11) 
//                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER", paramListJob);


//        objDB.bindGridView(gvMopRecoveriesValues, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
//                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
//                        from
//                        (
//                        select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
//                       getMOPValUptoPrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) uptoPrevBill,getMOPValSincePrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) sincePrevBill
//                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
//                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
//                and a.BILL_NO =:BILL_NO
//                and a.BILL_NO =b.BILL_NO(+)
//                and a.job_no=b.job_no(+)
//                and a.tender_no=b.tender_no(+)
//                and a.part_no=b.part_no(+)
//                and A.ID=B.HEADING_ID(+)
//                and A.HEADING_ORDER in (11) 
//                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
//                )", paramListJob);

        if (hdMOPVendStatus.Value.Equals("Y"))
        {
            
//            objDB.bindGridView(gvMopRecoveriesValues, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
//                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
//                        from
//                        (
//                        select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
//                       UPTO_PREV_BILL_AMT uptoPrevBill,
//                       SINCE_PREV_BILL_AMT sincePrevBill
//                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b, RAB_MOP_BILL_DTL c
//                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
//                and a.BILL_NO =:BILL_NO
//                and a.BILL_NO =b.BILL_NO
//                and a.job_no=b.job_no
//                and a.tender_no=b.tender_no
//                and a.part_no=b.part_no
//                and A.ID=B.HEADING_ID
//                and c.ID = :ID
//                and c.HEADING_ID = a.id
//               and c.HEADING_ID = b.heding_id
//                and c.HEADING_ID = a.id
//               and c.HEADING_ID = b.heding_id
//                and A.HEADING_ORDER in (11) 
//                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
//                )", paramListJob);

            paramListJob.Add("ID", hdMOPId.Value);
             objDB.bindGridView(gvMopRecoveriesValues, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
                        from
                        (
              select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
                       UPTO_PREV_BILL_AMT uptoPrevBill,
                       SINCE_PREV_BILL_AMT sincePrevBill
                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b, RAB_MOP_BILL_DTL c
                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
                and a.BILL_NO =:BILL_NO
                and a.BILL_NO =b.BILL_NO
                and a.job_no=b.job_no
                and a.tender_no=b.tender_no
                and a.part_no=b.part_no
                and A.ID=B.HEADING_ID                
                and c.HEADING_ID = a.id
               and c.HEADING_ID = b.heAding_id
                and c.HEADING_ID = a.id
               and c.HEADING_ID = b.heAding_id
                and A.HEADING_ORDER in (11) 
                 and c.ID =:ID
                and c.HEADING_ID = B.HEADING_ID               
                and c.sub_HEADING_ID = B.ID
                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
 )", paramListJob);
        }
        else
        {

            objDB.bindGridView(gvMopRecoveriesValues, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
                        from
                        (
                        select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
                       getMOPValUptoPrevBill(a.job_no,a.tender_no,a.part_no,A.HEADING_ORDER,B.SUB_HEADING_order) uptoPrevBill,
                       getMOPValSincePrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) sincePrevBill
                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
                and a.BILL_NO =:BILL_NO
                and a.BILL_NO =b.BILL_NO(+)
                and a.job_no=b.job_no(+)
                and a.tender_no=b.tender_no(+)
                and a.part_no=b.part_no(+)
                and A.ID=B.HEADING_ID(+)
                and A.HEADING_ORDER in (11) 
                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
                )", paramListJob);

        }
        if (gvMopRecoveriesValues.Rows.Count > 0)
        {
            bindMOPMasterDetails();
            pnlJobMOP.Visible = true;
        }
       
    }
    protected void gvMopRecoveriesValues_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {            
            Label lblSincePrevBill = new Label();
            lblSincePrevBill = (Label)e.Row.FindControl("lblSincePrevBill");
            TextBox txtSincePrevBill = new TextBox();
            txtSincePrevBill = (TextBox)e.Row.FindControl("txtSincePrevBill");

           
            if ("RCM".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()))
            {
                lblSincePrevBill.Visible = true;
                txtSincePrevBill.Visible = false;
            }
            else if ("VEND".Equals(Session["ROLE"].ToString()) || "BE".Equals(Session["ROLE"].ToString()))
            {
                if ("Y".Equals(hdMOPVendStatus.Value))
                {
                    lblSincePrevBill.Visible = true;
                    txtSincePrevBill.Visible = false;
                }
                else
                {
                    lblSincePrevBill.Visible = false;
                    txtSincePrevBill.Visible = true;
                }
            }           
        }
    }

    protected void bindPaymentReccomendation(string jobNumber, string tenderNo, string billNumber,DataTable dt)
    {
        string[] strArray = tenderNo.Split('~');
        string[] strBill = billNumber.Split('~');
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        Dictionary<string, string> paramListJob = new Dictionary<string, string>();
        paramListJob.Add("JOB_NO", jobNumber);
        paramListJob.Add("TENDER_NO", strArray[0]);
        paramListJob.Add("PART_NO", strArray[1]);
        paramListJob.Add("BILL_NO", strBill[0]);

//        objDB.bindGridView(gvMopPaymentRecommendations, @"select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,A.HEADING_ORDER
//                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
//                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
//                and a.BILL_NO =:BILL_NO
//                and a.BILL_NO =b.BILL_NO(+)
//                and a.job_no=b.job_no(+)
//                and a.tender_no=b.tender_no(+)
//                and a.part_no=b.part_no(+)
//                and A.ID=B.HEADING_ID(+)
//                and A.HEADING_ORDER in (13) 
//                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER", paramListJob);        

//        objDB.bindGridView(gvMopPaymentRecommendations, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
//                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
//                        from
//                        (
//                        select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
//                       getMOPValUptoPrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) uptoPrevBill,getMOPValSincePrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) sincePrevBill
//                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
//                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
//                and a.BILL_NO =:BILL_NO
//                and a.BILL_NO =b.BILL_NO(+)
//                and a.job_no=b.job_no(+)
//                and a.tender_no=b.tender_no(+)
//                and a.part_no=b.part_no(+)
//                and A.ID=B.HEADING_ID(+)
//                and A.HEADING_ORDER in (13) 
//                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
//                )", paramListJob);

        if (hdMOPVendStatus.Value.Equals("Y"))
        {
            paramListJob.Add("ID", hdMOPId.Value);
            objDB.bindGridView(gvMopPaymentRecommendations, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
                        from
                        (
              select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
                       UPTO_PREV_BILL_AMT uptoPrevBill,
                       SINCE_PREV_BILL_AMT sincePrevBill
                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b, RAB_MOP_BILL_DTL c
                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
                and a.BILL_NO =:BILL_NO
                and a.BILL_NO =b.BILL_NO
                and a.job_no=b.job_no
                and a.tender_no=b.tender_no
                and a.part_no=b.part_no
                and A.ID=B.HEADING_ID                
                and c.HEADING_ID = a.id
               and c.HEADING_ID = b.heAding_id
                and c.HEADING_ID = a.id
               and c.HEADING_ID = b.heAding_id
                and A.HEADING_ORDER in (13) 
                 and c.ID =:ID
                and c.HEADING_ID = B.HEADING_ID               
                and c.sub_HEADING_ID = B.ID
                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
 )", paramListJob);
        }
        else
        {
            objDB.bindGridView(gvMopPaymentRecommendations, @"select heading_id,sub_heding_id, description,VALUE_EXISTS,SUB_HEADING_ORDER,
                        uptoPrevBill, sincePrevBill , (uptoPrevBill + sincePrevBill) total
                        from
                        (
                        select A.ID heading_id,B.ID sub_heding_id,B.SUB_HEADING_DESC description,B.VALUE_EXISTS,B.SUB_HEADING_ORDER,
                       getMOPValUptoPrevBill(a.job_no,a.tender_no,a.part_no,A.HEADING_ORDER,B.SUB_HEADING_order) uptoPrevBill,
                       getMOPValSincePrevBill(a.job_no,a.tender_no,a.part_no,a.BILL_NO,B.SUB_HEADING_DESC) sincePrevBill
                from RAB_MOP_HEADING_DTL a,RAB_MOP_SUB_HEADINGS_DTL b
                where a.job_no=:JOB_NO and a.part_no=:PART_NO and a.tender_no=:TENDER_NO
                and a.BILL_NO =:BILL_NO
                and a.BILL_NO =b.BILL_NO(+)
                and a.job_no=b.job_no(+)
                and a.tender_no=b.tender_no(+)
                and a.part_no=b.part_no(+)
                and A.ID=B.HEADING_ID(+)
                and A.HEADING_ORDER in (13) 
                order by A.HEADING_ORDER,B.SUB_HEADING_ORDER
                )", paramListJob);
        }
    }

    protected void gvMopPaymentRecommendations_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblSincePrevBill = new Label();
            lblSincePrevBill = (Label)e.Row.FindControl("lblSincePrevBill");
            TextBox txtSincePrevBill = new TextBox();
            txtSincePrevBill = (TextBox)e.Row.FindControl("txtSincePrevBill");

            
            if ("RCM".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()))
            {
                lblSincePrevBill.Visible = true;
                txtSincePrevBill.Visible = false;
            }
            else if ("VEND".Equals(Session["ROLE"].ToString()) || "BE".Equals(Session["ROLE"].ToString()))
            {
                if ("Y".Equals(hdMOPVendStatus.Value))
                {
                    lblSincePrevBill.Visible = true;
                    txtSincePrevBill.Visible = false;
                }
                else
                {
                    lblSincePrevBill.Visible = false;
                    txtSincePrevBill.Visible = true;
                }
            }

        }
    }

    protected void gvJobMop_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdID = new HiddenField();
            hdID = (HiddenField)e.Row.FindControl("hdJobID");
            string headingID = hdID.Value;
            HiddenField hdOrder = new HiddenField();
            hdOrder = (HiddenField)e.Row.FindControl("hdJobOrder");
            string headingOrder = hdOrder.Value;

            GridView gvJobMopSubHeadings = e.Row.FindControl("gvJobMopSubHeadings") as GridView;
            StringBuilder query = new StringBuilder();

            if (!"".Equals(headingID))
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                query.Append(@"select A.ID,B.HEADING_ID,B.SUB_HEADING_ORDER, B.SUB_HEADING_DESC
                                from RAB_MOP_HEADING_dtl a,RAB_MOP_SUB_HEADINGS_dtl b where B.HEADING_ID =:HEADING_ID and A.ID=B.HEADING_ID
                                order by B.SUB_HEADING_ORDER");
                paramList.Add("HEADING_ID", headingID);
                objDB.bindGridView(gvJobMopSubHeadings, query.ToString(), paramList);
            }
        }
    }


    //Adding MOP values by Vendor
    protected void btnAddMOPVend_Click(object sender, EventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        string[] strBill = ddBillNumber.SelectedValue.Split('~');
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        //Insert MOP
        if (hdMOPId.Value.Equals("0"))
        {
            if (txtCompletionDate.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter Actual Completion Date");
            }
            else if (txtTimeExtension.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter Extension of Time Period, If any");
            }
            else if (txtPercentProgress.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter % Progress");
            }
            else
            {
                //GET MOP ID from sequence and populate the data using transaction
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                string mopID = objDB.executeScalar("SELECT RAB_MOP_BILL_DTL_SEQ.NEXTVAL FROM DUAL", paramList);

                Dictionary<string, string> paramListMOPDtl = new Dictionary<string, string>();
                StringBuilder sbMasterQry = new StringBuilder();
//                sbMasterQry.Append(@"INSERT INTO RAB_MOP_BILL_ACTION (MOP_ID, VENDOR_FROZEN, ACTUAL_COMPLETION_DT, EXTENSION_PERIOD, PERCENT_PROGRESS,ENHANCED_VALUE)
//                            VALUES (:MOP_ID, :VENDOR_FROZEN, to_date(:ACTUAL_COMPLETION_DT,'DD-MM-YYYY'), :EXTENSION_PERIOD, :PERCENT_PROGRESS,:ENHANCED_VALUE)");
                sbMasterQry.Append(@"INSERT INTO RAB_MOP_BILL_ACTION (MOP_ID, VENDOR_FROZEN, ACTUAL_COMPLETION_REMARKS, EXTENSION_PERIOD, PERCENT_PROGRESS,ENHANCED_VALUE)
                            VALUES (:MOP_ID, :VENDOR_FROZEN, :ACTUAL_COMPLETION_DT, :EXTENSION_PERIOD, :PERCENT_PROGRESS,:ENHANCED_VALUE)");
                paramListMOPDtl.Add("MOP_ID", mopID);
                paramListMOPDtl.Add("VENDOR_FROZEN", "Y");
                paramListMOPDtl.Add("ACTUAL_COMPLETION_DT", txtCompletionDate.Text);
                paramListMOPDtl.Add("EXTENSION_PERIOD", txtTimeExtension.Text);
                paramListMOPDtl.Add("PERCENT_PROGRESS", txtPercentProgress.Text);
                paramListMOPDtl.Add("ENHANCED_VALUE", txtEnhancedValue.Text);
                
                lstInsertQueries.Add(sbMasterQry);
                lstInsertParam.Add(paramListMOPDtl);

                //ACTION DETAILS
                Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
                StringBuilder sbQryAction = new StringBuilder();
                sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
                paramListMOPAction.Add("MOP_ID", mopID);
                paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
                paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
                paramListMOPAction.Add("ACTIVITY_DESC", "MOP Added by Vendor");

                lstInsertQueries.Add(sbQryAction);
                lstInsertParam.Add(paramListMOPAction);

                if (mopID.Length > 0)
                {
                    //Iterate the 2 gridviews and get the details to insert
                    foreach (GridViewRow row in gvMopRecoveriesValues.Rows)
                    {
                        HiddenField hdHeadingID = new HiddenField();
                        hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                        HiddenField hdSubHeadingID = new HiddenField();
                        hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                        Label lbluptoPrevBill = new Label();
                        lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                        TextBox txtSincePrevBill = new TextBox();
                        txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                        double totalValue = 0;
                        if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                        {
                            totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                        }

                        StringBuilder sbInsertQuery = new StringBuilder();
                        sbInsertQuery.Append(@" INSERT INTO RAB_MOP_BILL_DTL (ID, HEADING_ID,SUB_HEADING_ID, UPTO_PREV_BILL_AMT, SINCE_PREV_BILL_AMT, TOTAL_UPTO_DATE, ADDED_BY, ADDED_ON, JOB_NO, TENDER_NO, RA_BILL_NO, RA_DATE, PART_NO,  ADDED_BY_ROLE, IS_FROZEN)
                                                VALUES (:ID, :HEADING_ID, :SUB_HEADING_ID, :UPTO_PREV_BILL_AMT, :SINCE_PREV_BILL_AMT, :TOTAL_UPTO_DATE, :ADDED_BY, SYSDATE, :JOB_NO, :TENDER_NO, :RA_BILL_NO, to_date(:RA_DATE,'DD-MON-YY'), :PART_NO,  :ADDED_BY_ROLE, 'Y')");
                        Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                        paramListMOP.Add("ID", mopID);
                        paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                        paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                        paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                        paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                        paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                        paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                        paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                        paramListMOP.Add("TENDER_NO", strArray[0]);
                        paramListMOP.Add("PART_NO", strArray[1]);
                        paramListMOP.Add("RA_BILL_NO", strBill[0]);
                        paramListMOP.Add("RA_DATE", strBill[1].ToUpper());
                        paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                        lstInsertQueries.Add(sbInsertQuery);
                        lstInsertParam.Add(paramListMOP);
                    }


                    foreach (GridViewRow row in gvMopPaymentRecommendations.Rows)
                    {
                        HiddenField hdHeadingID = new HiddenField();
                        hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                        HiddenField hdSubHeadingID = new HiddenField();
                        hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                        Label lbluptoPrevBill = new Label();
                        lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                        TextBox txtSincePrevBill = new TextBox();
                        txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                        double totalValue = 0;
                        if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                        {
                            totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                        }

                        StringBuilder sbInsertQuery = new StringBuilder();
                        sbInsertQuery.Append(@" INSERT INTO RAB_MOP_BILL_DTL (ID, HEADING_ID,SUB_HEADING_ID, UPTO_PREV_BILL_AMT, SINCE_PREV_BILL_AMT, TOTAL_UPTO_DATE, ADDED_BY, ADDED_ON, JOB_NO, TENDER_NO, RA_BILL_NO, RA_DATE, PART_NO,  ADDED_BY_ROLE, IS_FROZEN)
                                                VALUES (:ID, :HEADING_ID, :SUB_HEADING_ID, :UPTO_PREV_BILL_AMT, :SINCE_PREV_BILL_AMT, :TOTAL_UPTO_DATE, :ADDED_BY, SYSDATE, :JOB_NO, :TENDER_NO, :RA_BILL_NO, to_date(:RA_DATE,'DD-MON-YY'), :PART_NO,  :ADDED_BY_ROLE, 'Y')");
                        Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                        paramListMOP.Add("ID", mopID);
                        paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                        paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                        paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                        paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                        paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                        paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                        paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                        paramListMOP.Add("TENDER_NO", strArray[0]);
                        paramListMOP.Add("PART_NO", strArray[1]);
                        paramListMOP.Add("RA_BILL_NO", strBill[0]);
                        paramListMOP.Add("RA_DATE", strBill[1].ToUpper());
                        paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                        lstInsertQueries.Add(sbInsertQuery);
                        lstInsertParam.Add(paramListMOP);
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

                        //Generate Email alert to BE (Feedback no 1891 Email dated 15-Nov-2017)
                        Hashtable htEmail = new Hashtable();
                        htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                        StringBuilder sbMessage = new StringBuilder();
                        if (htEmail["BE"] != null)
                        {
                            try
                            {
                                sbMessage = getMailMessage("BE");
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - New MOP submitted by contractor, Bill No: " + strBill[0], sbMessage.ToString(), null);
                            }
                            catch (Exception err)
                            { }
                        }


                        Common.Show("MOP added successfully ");
                        ddBillNumber_SelectedIndexChanged(sender, e);
                    }
                    else
                    {
                        Common.Show("MOP not updated");
                    }
                }
            }
        }//Update MOP
        else
        {
            string mopID = hdMOPId.Value;
            Dictionary<string, string> paramListMOPDtl = new Dictionary<string, string>();
            StringBuilder sbMasterQry = new StringBuilder();
            sbMasterQry.Append(@"UPDATE RAB_MOP_BILL_ACTION SET VENDOR_FROZEN='Y' WHERE MOP_ID=:MOP_ID");
            paramListMOPDtl.Add("MOP_ID", mopID);                      
            lstInsertQueries.Add(sbMasterQry);
            lstInsertParam.Add(paramListMOPDtl);            
            
            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Updated by Vendor");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);
            
            if (mopID.Length > 0)
            {
                //Iterate the 2 gridviews and get the details to insert
                foreach (GridViewRow row in gvMopRecoveriesValues.Rows)
                {
                    HiddenField hdHeadingID = new HiddenField();
                    hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                    HiddenField hdSubHeadingID = new HiddenField();
                    hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                    Label lbluptoPrevBill = new Label();
                    lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                    TextBox txtSincePrevBill = new TextBox();
                    txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                    double totalValue = 0;
                    if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                    {
                        totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                    }

                    StringBuilder sbInsertQuery = new StringBuilder();
                    sbInsertQuery.Append(@" UPDATE RAB_MOP_BILL_DTL 
                                             set UPTO_PREV_BILL_AMT=:UPTO_PREV_BILL_AMT,
                                                SINCE_PREV_BILL_AMT=:SINCE_PREV_BILL_AMT,
                                                TOTAL_UPTO_DATE=:TOTAL_UPTO_DATE,ADDED_ON=SYSDATE,ADDED_BY_ROLE=:ADDED_BY_ROLE
                                                where ID=:ID
                                                and HEADING_ID=:HEADING_ID
                                                and SUB_HEADING_ID=:SUB_HEADING_ID
                                                and ADDED_BY=:ADDED_BY
                                                and JOB_NO=:JOB_NO
                                                and TENDER_NO=:TENDER_NO
                                                and RA_BILL_NO=:RA_BILL_NO
                                                and PART_NO=:PART_NO
                                                 
                                                ");
                    Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                    paramListMOP.Add("ID", mopID);
                    paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                    paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                    paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                    paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                    paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                    paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                    paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                    paramListMOP.Add("TENDER_NO", strArray[0]);
                    paramListMOP.Add("PART_NO", strArray[1]);
                    paramListMOP.Add("RA_BILL_NO", strBill[0]);                    
                    paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                    lstInsertQueries.Add(sbInsertQuery);
                    lstInsertParam.Add(paramListMOP);
                }


                foreach (GridViewRow row in gvMopPaymentRecommendations.Rows)
                {
                    HiddenField hdHeadingID = new HiddenField();
                    hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                    HiddenField hdSubHeadingID = new HiddenField();
                    hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                    Label lbluptoPrevBill = new Label();
                    lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                    TextBox txtSincePrevBill = new TextBox();
                    txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                    double totalValue = 0;
                    if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                    {
                        totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                    }

                    StringBuilder sbInsertQuery = new StringBuilder();
                    sbInsertQuery.Append(@" UPDATE RAB_MOP_BILL_DTL 
                                             set UPTO_PREV_BILL_AMT=:UPTO_PREV_BILL_AMT,
                                                SINCE_PREV_BILL_AMT=:SINCE_PREV_BILL_AMT,
                                                TOTAL_UPTO_DATE=:TOTAL_UPTO_DATE,ADDED_ON=SYSDATE, ADDED_BY_ROLE=:ADDED_BY_ROLE
                                                where ID=:ID
                                                and HEADING_ID=:HEADING_ID
                                                and SUB_HEADING_ID=:SUB_HEADING_ID
                                                and ADDED_BY=:ADDED_BY                                                
                                                and JOB_NO=:JOB_NO
                                                and TENDER_NO=:TENDER_NO
                                                and RA_BILL_NO=:RA_BILL_NO
                                                and PART_NO=:PART_NO
                                                
                                                ");
                    Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                    paramListMOP.Add("ID", mopID);
                    paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                    paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                    paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                    paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                    paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                    paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                    paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                    paramListMOP.Add("TENDER_NO", strArray[0]);
                    paramListMOP.Add("PART_NO", strArray[1]);
                    paramListMOP.Add("RA_BILL_NO", strBill[0]);
                    paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                    lstInsertQueries.Add(sbInsertQuery);
                    lstInsertParam.Add(paramListMOP);
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
                    //Generate Email alert to BE (Feedback no 1891 Email dated 15-Nov-2017)
                    Hashtable htEmail = new Hashtable();
                    htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                    StringBuilder sbMessage = new StringBuilder();
                    if (htEmail["BE"] != null)
                    {
                        try
                        {
                            sbMessage = getMailMessage("BE");
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - New MOP submitted by contractor, Bill No: " + strBill[0], sbMessage.ToString(), null);
                        }
                        catch (Exception err)
                        { }
                    }

                    Common.Show("MOP added successfully ");
                    ddBillNumber_SelectedIndexChanged(sender, e);
                }
                else
                {
                    Common.Show("MOP not updated");
                }
            }

        }
    }


    protected void btnEditRecoveries_Click(object sender, EventArgs e)
    {
        Response.Redirect("RAeditMOP?id=" + Common.encrypt(hdMOPId.Value));
    }

    //BE addition
    protected void btnAddMopData_Click(object sender, EventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        string[] strBill = ddBillNumber.SelectedValue.Split('~');
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        //Insert MOP
        if (hdMOPId.Value.Equals("0"))
        {
            if (txtCompletionDate.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter Actual Completion Date");
            }
            else if (txtTimeExtension.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter Extension of Time Period, If any");
            }
            else if (txtPercentProgress.Text.Length == 0)
            {
                Common.Show("Error: Kindly enter % Progress");
            }
            else
            {
                //GET MOP ID from sequence and populate the data using transaction
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                string mopID = objDB.executeScalar("SELECT RAB_MOP_BILL_DTL_SEQ.NEXTVAL FROM DUAL", paramList);

                Dictionary<string, string> paramListMOPDtl = new Dictionary<string, string>();
                StringBuilder sbMasterQry = new StringBuilder();
//                sbMasterQry.Append(@"INSERT INTO RAB_MOP_BILL_ACTION (MOP_ID,VENDOR_FROZEN, BE_IS_FROZEN,BE_EMPNO, ACTUAL_COMPLETION_DT, EXTENSION_PERIOD, PERCENT_PROGRESS,ENHANCED_VALUE)
//                            VALUES (:MOP_ID, :VENDOR_FROZEN, :BE_IS_FROZEN,:BE_EMPNO, to_date(:ACTUAL_COMPLETION_DT,'DD-MM-YYYY'), :EXTENSION_PERIOD, :PERCENT_PROGRESS,:ENHANCED_VALUE)");
                sbMasterQry.Append(@"INSERT INTO RAB_MOP_BILL_ACTION (MOP_ID,VENDOR_FROZEN, BE_IS_FROZEN,BE_EMPNO, ACTUAL_COMPLETION_REMARKS, EXTENSION_PERIOD, PERCENT_PROGRESS,ENHANCED_VALUE)
                            VALUES (:MOP_ID, :VENDOR_FROZEN, :BE_IS_FROZEN,:BE_EMPNO, :ACTUAL_COMPLETION_DT, :EXTENSION_PERIOD, :PERCENT_PROGRESS,:ENHANCED_VALUE)");
                paramListMOPDtl.Add("MOP_ID", mopID);
                paramListMOPDtl.Add("BE_IS_FROZEN", "Y");
                paramListMOPDtl.Add("VENDOR_FROZEN", "Y");                
                paramListMOPDtl.Add("BE_EMPNO", Session["USERID"].ToString());
                paramListMOPDtl.Add("ACTUAL_COMPLETION_DT", txtCompletionDate.Text);
                paramListMOPDtl.Add("EXTENSION_PERIOD", txtTimeExtension.Text);
                paramListMOPDtl.Add("PERCENT_PROGRESS", txtPercentProgress.Text);
                paramListMOPDtl.Add("ENHANCED_VALUE", txtEnhancedValue.Text);
                
                lstInsertQueries.Add(sbMasterQry);
                lstInsertParam.Add(paramListMOPDtl);

                //ACTION DETAILS
                Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
                StringBuilder sbQryAction = new StringBuilder();
                sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
                paramListMOPAction.Add("MOP_ID", mopID);
                paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
                paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
                paramListMOPAction.Add("ACTIVITY_DESC", "MOP Added by Billing Engineer");

                lstInsertQueries.Add(sbQryAction);
                lstInsertParam.Add(paramListMOPAction);

                if (mopID.Length > 0)
                {
                    //Iterate the 2 gridviews and get the details to insert
                    foreach (GridViewRow row in gvMopRecoveriesValues.Rows)
                    {
                        HiddenField hdHeadingID = new HiddenField();
                        hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                        HiddenField hdSubHeadingID = new HiddenField();
                        hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                        Label lbluptoPrevBill = new Label();
                        lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                        TextBox txtSincePrevBill = new TextBox();
                        txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                        double totalValue = 0;
                        if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                        {
                            totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                        }

                        StringBuilder sbInsertQuery = new StringBuilder();
                        sbInsertQuery.Append(@" INSERT INTO RAB_MOP_BILL_DTL (ID, HEADING_ID,SUB_HEADING_ID, UPTO_PREV_BILL_AMT, SINCE_PREV_BILL_AMT, TOTAL_UPTO_DATE, ADDED_BY, ADDED_ON, JOB_NO, TENDER_NO, RA_BILL_NO, RA_DATE, PART_NO,  ADDED_BY_ROLE, IS_FROZEN)
                                                VALUES (:ID, :HEADING_ID, :SUB_HEADING_ID, :UPTO_PREV_BILL_AMT, :SINCE_PREV_BILL_AMT, :TOTAL_UPTO_DATE, :ADDED_BY, SYSDATE, :JOB_NO, :TENDER_NO, :RA_BILL_NO, to_date(:RA_DATE,'DD-MON-YY'), :PART_NO,  :ADDED_BY_ROLE, 'Y')");
                        Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                        paramListMOP.Add("ID", mopID);
                        paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                        paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                        paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                        paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                        paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                        paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                        paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                        paramListMOP.Add("TENDER_NO", strArray[0]);
                        paramListMOP.Add("PART_NO", strArray[1]);
                        paramListMOP.Add("RA_BILL_NO", strBill[0]);
                        paramListMOP.Add("RA_DATE", strBill[1].ToUpper());
                        paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                        lstInsertQueries.Add(sbInsertQuery);
                        lstInsertParam.Add(paramListMOP);
                    }


                    foreach (GridViewRow row in gvMopPaymentRecommendations.Rows)
                    {
                        HiddenField hdHeadingID = new HiddenField();
                        hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");

                        HiddenField hdSubHeadingID = new HiddenField();
                        hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
                        Label lbluptoPrevBill = new Label();
                        lbluptoPrevBill = (Label)row.FindControl("lbluptoPrevBill");

                        TextBox txtSincePrevBill = new TextBox();
                        txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");

                        double totalValue = 0;
                        if (lbluptoPrevBill.Text.Length > 0 && txtSincePrevBill.Text.Length > 0)
                        {
                            totalValue = double.Parse(lbluptoPrevBill.Text) + double.Parse(txtSincePrevBill.Text);
                        }

                        StringBuilder sbInsertQuery = new StringBuilder();
                        sbInsertQuery.Append(@" INSERT INTO RAB_MOP_BILL_DTL (ID, HEADING_ID,SUB_HEADING_ID, UPTO_PREV_BILL_AMT, SINCE_PREV_BILL_AMT, TOTAL_UPTO_DATE, ADDED_BY, ADDED_ON, JOB_NO, TENDER_NO, RA_BILL_NO, RA_DATE, PART_NO,  ADDED_BY_ROLE, IS_FROZEN)
                                                VALUES (:ID, :HEADING_ID, :SUB_HEADING_ID, :UPTO_PREV_BILL_AMT, :SINCE_PREV_BILL_AMT, :TOTAL_UPTO_DATE, :ADDED_BY, SYSDATE, :JOB_NO, :TENDER_NO, :RA_BILL_NO, to_date(:RA_DATE,'DD-MON-YY'), :PART_NO,  :ADDED_BY_ROLE, 'Y')");
                        Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
                        paramListMOP.Add("ID", mopID);
                        paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
                        paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
                        paramListMOP.Add("UPTO_PREV_BILL_AMT", lbluptoPrevBill.Text);
                        paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
                        paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());
                        paramListMOP.Add("ADDED_BY", Session["USERID"].ToString());
                        paramListMOP.Add("JOB_NO", ddJobNumber.SelectedValue);
                        paramListMOP.Add("TENDER_NO", strArray[0]);
                        paramListMOP.Add("PART_NO", strArray[1]);
                        paramListMOP.Add("RA_BILL_NO", strBill[0]);
                        paramListMOP.Add("RA_DATE", strBill[1].ToUpper());
                        paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
                        lstInsertQueries.Add(sbInsertQuery);
                        lstInsertParam.Add(paramListMOP);
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
                        //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                        Hashtable htEmail = new Hashtable();
                        htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                        StringBuilder sbMessage = new StringBuilder();
                        if (htEmail["AC"] != null)
                        {
                            try
                            {
                                sbMessage = getMailMessage("AC");
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["AC"], "RAB - MOP submitted by Billing Engineer, Bill No: " + strBill[0], sbMessage.ToString(), null);
                            }
                            catch (Exception err)
                            { }
                        }
                        
                        Common.Show("MOP added successfully");
                        ddBillNumber_SelectedIndexChanged(sender, e);
                    }
                    else
                    {
                        Common.Show("MOP not updated");
                    }
                }
            }
        }//Update MOP
        else
        {            
            if (!hdMOPId.Value.Equals("0"))
            {
                StringBuilder sbQuery = new StringBuilder();
                sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET BE_IS_FROZEN='Y', BE_EMPNO=:BE_EMPNO, BE_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID
                        AND BE_IS_FROZEN='N'                                             
                        AND VENDOR_FROZEN='Y'");
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                paramList.Add("MOP_ID", hdMOPId.Value);
                paramList.Add("BE_EMPNO", Session["USERID"].ToString());

                lstInsertQueries.Add(sbQuery);
                lstInsertParam.Add(paramList);

                //ACTION DETAILS
                Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
                StringBuilder sbQryAction = new StringBuilder();
                sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
                paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
                paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
                paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
                paramListMOPAction.Add("ACTIVITY_DESC", "MOP Approved by BE");

                lstInsertQueries.Add(sbQryAction);
                lstInsertParam.Add(paramListMOPAction);
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
                //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                Hashtable htEmail = new Hashtable();
                htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                StringBuilder sbMessage = new StringBuilder();
                if (htEmail["AC"] != null)
                {
                    try
                    {
                        sbMessage = getMailMessage("AC");
                        Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["AC"], "RAB - MOP submitted by Billing Engineer, Bill No: " + strBill[0], sbMessage.ToString(), null);
                    }
                    catch (Exception err)
                    { }
                }
                
                Common.Show("MOP approved successfully");
                ddBillNumber_SelectedIndexChanged(sender, e);
            }
            else
            {
                Common.Show("MOP not approved");
            }

        }
    }

    protected void btnRejectMOPBE_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        if (!hdMOPId.Value.Equals("0"))
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET VENDOR_FROZEN='N', BE_EMPNO=:BE_EMPNO, BE_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID 
                        AND AC_IS_FROZEN='N'                       
                        AND RCM_IS_FROZEN='N'
                        AND VENDOR_FROZEN='Y'");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("MOP_ID", hdMOPId.Value);
            paramList.Add("BE_EMPNO", Session["USERID"].ToString());

            lstInsertQueries.Add(sbQuery);
            lstInsertParam.Add(paramList);

            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Rejected by BE");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);
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
            Common.Show("MOP rejected successfully");
            ddBillNumber_SelectedIndexChanged(sender, e);
        }
        else
        {
            Common.Show("MOP not rejected");
        }
    }

    protected void btnApproveAC_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        if (!hdMOPId.Value.Equals("0"))
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET AC_IS_FROZEN='Y', AC_EMPNO=:AC_EMPNO, AC_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID
                        AND BE_IS_FROZEN='Y'
                        AND AC_IS_FROZEN='N'
                        AND RCM_IS_FROZEN='N'
                        AND VENDOR_FROZEN='Y'");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("MOP_ID", hdMOPId.Value);
            paramList.Add("AC_EMPNO", Session["USERID"].ToString());

            lstInsertQueries.Add(sbQuery);
            lstInsertParam.Add(paramList);      

            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Approved by Area Cordinator");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);           
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
            //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
            Hashtable htEmail = new Hashtable();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
            StringBuilder sbMessage = new StringBuilder();
            if (htEmail["RCM"] != null)
            {
                try
                {
                    sbMessage = getMailMessage("RCM");
                    //To RCM and CC email to both AC and BE
                    ArrayList lstCCEmails = new ArrayList();
                    if (htEmail["AC"] != null)
                        lstCCEmails.AddRange((ArrayList)htEmail["AC"]);
                    if (htEmail["BE"] != null)
                        lstCCEmails.AddRange((ArrayList)htEmail["BE"]);

                    // Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - MOP approved by Area Cordinator", //sbMessage.ToString(), null);
                    Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - MOP approved by Area Cordinator", sbMessage.ToString(), lstCCEmails);
                }
                catch (Exception err)
                { }
            }
            
            Common.Show("MOP approved successfully");
            ddBillNumber_SelectedIndexChanged(sender, e);
        }
        else
        {
            Common.Show("MOP not approved");
        }
    }

    protected void btnRejectAC_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        if (!hdMOPId.Value.Equals("0"))
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET BE_IS_FROZEN='N', AC_EMPNO=:AC_EMPNO, AC_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID
                        AND BE_IS_FROZEN='Y'
                        AND RCM_IS_FROZEN='N'
                        AND VENDOR_FROZEN='Y'");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("MOP_ID", hdMOPId.Value);
            paramList.Add("AC_EMPNO", Session["USERID"].ToString());

            lstInsertQueries.Add(sbQuery);
            lstInsertParam.Add(paramList);

            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Rejected by Area Cordinator");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);
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
            //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
            Hashtable htEmail = new Hashtable();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
            StringBuilder sbMessage = new StringBuilder();
            if (htEmail["BE"] != null)
            {
                try
                {
                    sbMessage = getMailMessage("BE");
                    Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - MOP rejected by Area Cordinator", sbMessage.ToString(), null);
                }
                catch (Exception err)
                { }
            }
            
            Common.Show("MOP rejected successfully");
            ddBillNumber_SelectedIndexChanged(sender, e);
        }
        else
        {
            Common.Show("MOP not rejected");
        }
    }

    protected void btnApproveRCM_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        if (!hdMOPId.Value.Equals("0"))
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET RCM_IS_FROZEN='Y', RCM_EMPNO=:RCM_EMPNO, RCM_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID
                        AND BE_IS_FROZEN='Y'
                        AND AC_IS_FROZEN='Y'
                        AND RCM_IS_FROZEN='N'
                        AND VENDOR_FROZEN='Y'");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("MOP_ID", hdMOPId.Value);
            paramList.Add("RCM_EMPNO", Session["USERID"].ToString());

            lstInsertQueries.Add(sbQuery);
            lstInsertParam.Add(paramList);

            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Approved by RCM");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);
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
            //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
            Hashtable htEmail = new Hashtable();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
            StringBuilder sbMessage = new StringBuilder();
            if (htEmail["BE"] != null)
            {
                try
                {
                    sbMessage = getMailMessage("BE");
                    if ((ArrayList)htEmail["AC"] != null)
                    {
                        Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - MOP aopproved by RCM", sbMessage.ToString(), (ArrayList)htEmail["AC"]);
                    }
                    else
                    {
                        Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - MOP aopproved by RCM", sbMessage.ToString(), null);
                    }
                }
                catch (Exception err)
                { }
            }            
            Common.Show("MOP approved successfully");
            ddBillNumber_SelectedIndexChanged(sender, e);
        }
        else
        {
            Common.Show("MOP not approved");
        }
    }

    protected void btnRejectRCM_Click(object sender, EventArgs e)
    {
        ArrayList lstInsertQueries = new ArrayList();
        ArrayList lstInsertParam = new ArrayList();
        if (!hdMOPId.Value.Equals("0"))
        {
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(@"UPDATE RAB_MOP_BILL_ACTION SET AC_IS_FROZEN='N', RCM_EMPNO=:RCM_EMPNO, RCM_ACTION_ON=SYSDATE
                        WHERE MOP_ID=:MOP_ID
                        AND BE_IS_FROZEN='Y'
                        AND AC_IS_FROZEN='Y'
                        AND RCM_IS_FROZEN='N'
                        AND VENDOR_FROZEN='Y'");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("MOP_ID", hdMOPId.Value);
            paramList.Add("RCM_EMPNO", Session["USERID"].ToString());

            lstInsertQueries.Add(sbQuery);
            lstInsertParam.Add(paramList);

            //ACTION DETAILS
            Dictionary<string, string> paramListMOPAction = new Dictionary<string, string>();
            StringBuilder sbQryAction = new StringBuilder();
            sbQryAction.Append(@"INSERT INTO RAB_MOP_ACTIVITY (REMARKS_BY, ROLE, ACTIVITY_DESC, MOP_ID)
                            VALUES (:REMARKS_BY, :ROLE, :ACTIVITY_DESC, :MOP_ID)");
            paramListMOPAction.Add("MOP_ID", hdMOPId.Value);
            paramListMOPAction.Add("REMARKS_BY", Session["USERID"].ToString());
            paramListMOPAction.Add("ROLE", Session["ROLE"].ToString());
            paramListMOPAction.Add("ACTIVITY_DESC", "MOP Rejected by RCM");

            lstInsertQueries.Add(sbQryAction);
            lstInsertParam.Add(paramListMOPAction);
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
            //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
            Hashtable htEmail = new Hashtable();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
            StringBuilder sbMessage = new StringBuilder();
            if (htEmail["BE"] != null)
            {
                try
                {
                    sbMessage = getMailMessage("BE");
                    if ((ArrayList)htEmail["AC"] != null)
                    {
                        Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - MOP rejected by RCM", sbMessage.ToString(), (ArrayList)htEmail["AC"]);
                    }
                    else
                    {
                        Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - MOP rejected by RCM", sbMessage.ToString(), null);
                    }
                }
                catch (Exception err)
                { }
            }
            
            Common.Show("MOP rejected successfully");
            ddBillNumber_SelectedIndexChanged(sender, e);
        }
        else
        {
            Common.Show("MOP not rejected");
        }
    }

    public StringBuilder getMailMessage(string mailType)
    {
        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append(@"Dear Sir/Ma'am <br/<br/>");
        if ("BE".Equals(mailType))
        {
            sbMessage.Append(@"A new MOP has been submitted by Contractor for your action/information.<br/<br/>");
        }
        if ("AC".Equals(mailType))
        {
            sbMessage.Append(@"A MOP has been submitted by Billing Engineer for your action/information.<br/<br/>");
        }
        if ("RCM".Equals(mailType))
        {
            sbMessage.Append(@"A MOP has been submitted by Area Cordinator for your action/information.<br/<br/>");
        }

        try
        {
            sbMessage.Append("<b>Job No - ").Append(ddJobNumber.SelectedValue);
            sbMessage.Append("<br/>Tender - ").Append(ddTenderNo.SelectedItem.ToString());
        }
        catch (Exception err) { }

        sbMessage.Append(@"</b><br/><br/><br/>Kindly Login in <a href='www3.eil.co.in/rabilling' target='_Blank'>RA Billing Management System</a> and take your action accordingly.<br/<br/>");
        sbMessage.Append(@"<br/><br/><b>System generated email, Please do not reply.</b><br/<br/>");
        sbMessage.Append(@"<br/><br/>Thanks");
        return sbMessage;
    }
}