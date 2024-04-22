using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;
using System.Collections;

public partial class RA_New_Bill_Entry : System.Web.UI.Page
{
    dbFunction objDB = new dbFunction();
    public string sortColumn = "";
    public string sortOrder = "";
    Double totalMeasurementQty = 0;
    Double totalMeasurementQtyP = 0;
    StringBuilder sbQuery = new StringBuilder();
    Dictionary<string, string> paramList = new Dictionary<string, string>();
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["USERID"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (ViewState["TXTCOUNT"] == null)
        {
            ViewState["TXTCOUNT"] = 0;
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

    protected void clearParam()
    {
        sbQuery.Clear();
        paramList.Clear();
    }

    protected void bindJobNumber(string userId, string userRole)
    {
        clearParam();
        if ("VEND".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_MASTER where C_CODE=:C_CODE order by JOB_NO");
            paramList.Add("C_CODE", userId);
            lblUpdateBillNote.Visible = false;
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
        clearParam();
    }

    protected void bindTenders(string jobNumber)
    {
        clearParam();
        if ("BE".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()) || "RCM".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(@"SELECT DISTINCT b.TENDER_NO||'~'||b.part_no tender_part, b.tender_no||'-'|| b.part_no || ' ( '||b.TITLE||')' description   FROM  RAB_TENDER_USERS a, RAB_TENDER_MASTER b  WHERE b.JOB_NO=:JOB_NO  AND EMPNO=:EMPNO  AND ROLE=:ROLE  and b.job_no=A.JOB_NO and A.TENDER_NO=b.TENDER_NO and A.PART_NO=b.PART_NO and a.ACTIVE='Y' ORDER BY tender_part ");

            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("EMPNO", Session["USERID"].ToString());
            paramList.Add("ROLE", Session["ROLE"].ToString());

        }
        else if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(@" SELECT DISTINCT TENDER_NO||'~'||a.part_no tender_part, tender_no||'-'|| a.part_no||' ( '||A.TITLE||')'  description  FROM RAB_TENDER_MASTER a WHERE     JOB_NO =:JOB_NO   AND C_CODE =:C_CODE ORDER BY tender_part ");
            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("C_CODE", Session["USERID"].ToString());
        }
        //else
        //{
        //    sbQuery.Append(@" Select DISTINCT TENDER_NO||'~'||a.part_no tender_part, tender_no||'-'|| a.part_no||' ( '||a.TITLE||')' description FROM RAB_TENDER_MASTER a  WHERE JOB_NO=:JOB_NO ORDER BY tender_part");
        //    paramList.Add("JOB_NO", jobNumber.ToUpper());
        //}

        objDB.bindDropDownList(ddTenderNo, sbQuery.ToString(), paramList, "tender_part", "description", "", "--Select Tender Number--");
        clearParam();        
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
            string[] strArray = ddTenderNo.SelectedValue.Split('~');

            bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
            //bindSORNumber(ddJobNumber.SelectedValue, strArray[0]);
            if (ddTenderNo.Items.Count > 0 && ddTenderNo.SelectedValue.Length > 0 && "VEND".Equals(Session["ROLE"].ToString()))
            {
                btnCreateNewBill.Visible = true;
            }
            else
            {
                btnCreateNewBill.Visible = false;
            }
        }
        else
        {
            btnCreateNewBill.Visible = false;
            Common.Show("Please select Job Number and Tender Number");
        }
    }

    protected void bindBills(string jobNumber,string tenderNumber ,string partNumber)
    {
        clearParam();
        sbQuery.Append("select ID, BILL_NUMBER, to_char(PERIOD_FROM,'dd-Mon-yyyy') PERIOD_FROM,to_char(PERIOD_TO,'dd-Mon-yyyy') PERIOD_TO,to_char(PERIOD_FROM,'DD-Mon-yyyy') || ' To ' || to_char(PERIOD_TO,'DD-Mon-yyyy') BILL_PERIOD, to_char(BILL_DATE,'dd-Mon-YYYY') BILL_DATE, CONT_ID, JOB_NO, TENDER_NO, PART_NO, BILL_STATUS, TEND_SOR_ID, RUN_SL_NO, RA_BLL_NO, SUB_JOB from RAB_TENDER_BILL_MST where JOB_NO=:JOB_NO and TENDER_NO=:TENDER_NO and PART_NO=:PART_NO order by to_date(PERIOD_FROM) desc");
        paramList.Add("JOB_NO", jobNumber);
        paramList.Add("TENDER_NO", tenderNumber);
        paramList.Add("PART_NO", partNumber);
        objDB.bindGridView(gvAllBills, sbQuery.ToString(), paramList);
        clearParam();
    }

    protected void gvAllBills_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = gvAllBills.SelectedRow;        
        HiddenField hdTenderSorRefID = new HiddenField();
        hdTenderSorRefID = (HiddenField)row.FindControl("hdTenderSorRefID");
        string tenderSORreferenceId = hdTenderSorRefID.Value;

        HiddenField hdBillRunningSRNo = new HiddenField();
        hdBillRunningSRNo = (HiddenField)row.FindControl("hdBillRunningSRNo");

        HiddenField hdBillingDate = new HiddenField();
        hdBillingDate = (HiddenField)row.FindControl("hdBillingDate");

        HiddenField hdRABillNumber = new HiddenField();
        hdRABillNumber = (HiddenField)row.FindControl("hdRABillNumber");

        HiddenField hdBillStatus = new HiddenField();
        hdBillStatus = (HiddenField)row.FindControl("hdBillStatus");

        HiddenField hdbillID = new HiddenField();
        hdbillID = (HiddenField)row.FindControl("hdbillID");

        HiddenField hdBillPeriodFrom = new HiddenField();
        hdBillPeriodFrom = (HiddenField)row.FindControl("hdBillPeriodFrom");

        HiddenField hdBillPeriodTo = new HiddenField();
        hdBillPeriodTo = (HiddenField)row.FindControl("hdBillPeriodTo");
        
        HiddenField hdRAOverallBillNumber = new HiddenField();
        hdRAOverallBillNumber = (HiddenField)row.FindControl("hdRAOverallBillNumber");
        
        ViewState["TEND_SOR_REF_ID"] = tenderSORreferenceId;
        ViewState["RUN_SL_NO"] = hdBillRunningSRNo.Value;
        ViewState["RUN_SL_DATE"] = hdBillingDate.Value;
        ViewState["RA_FINAL_BILL_NO"] = hdRABillNumber.Value;
        ViewState["RA_BLL_NO"] = hdRAOverallBillNumber.Value;
        ViewState["RA_BILL_ID"] = hdbillID.Value;
        ViewState["PERIOD_FROM"] = hdBillPeriodFrom.Value;
        ViewState["PERIOD_TO"] = hdBillPeriodTo.Value;
        ViewState["BILL_STATUS"] = hdBillStatus.Value;

        //Bind the below grid
        if (hdBillStatus.Value.Equals("ACCEPTED BY RCM"))
        {
            bindSORItems(tenderSORreferenceId, ddJobNumber.SelectedValue, hdBillRunningSRNo.Value);
            trItemsDetails.Visible = true;
            pnlSORItems.Visible = true;
        }
        else
        { 
           // open bill for updation according to the ROLE
             bindBillParameters();
            // string[] strArray = ddTenderNo.SelectedValue.Split('~');
            // bindSORNumber(ddJobNumber.SelectedValue, strArray[0]);
             mpUpdateBill.Show();
        }
    }

    protected void bindBillParameters()
    {
        if (ViewState["RA_FINAL_BILL_NO"] != null)
        {
            lblUpdatedBillNo.Text = ViewState["RA_FINAL_BILL_NO"].ToString();
            lblUpdatedBillDate.Text = ViewState["RUN_SL_DATE"].ToString();
            lblUpdatedBillPeriod.Text = ViewState["PERIOD_FROM"].ToString() + " - " + ViewState["PERIOD_TO"].ToString();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            bindSORNumber(ddJobNumber.SelectedValue, strArray[0]);
            gvBillSeqItems.DataSource = null;
            gvBillSeqItems.DataBind();
            ddSeqNumber.Items.Clear();
            bindAddedMSData(ViewState["RA_BILL_ID"].ToString());
            enableButtons(Session["ROLE"].ToString());
        }
    }

    protected void bindAddedMSData(string billID)
    {
        clearParam();
        sbQuery.Append(@"select a.bill_id,a.ref_id,a.seq_no,a.run_SL_NO,RA_BLL_NO,a.act_seq,cont_qty,to_char(cont_added_on,'dd-Mon-yyyy')                           added_on,a.cont_id,
                               a.tend_sor_id,a.status,B.ACTIVITY_DESC,nvl(C.ITEM_RATE_EDITED,C.ITEM_RATE) item_rate,C.UOM,c.SITE_QTY,b.ACTivity_PERCENT,C.SDESC, ( (nvl(C.ITEM_RATE_EDITED,C.ITEM_RATE) * cont_qty) * b.ACTIVITY_PERCENT / 100) qty_amount
                            from RAB_TENDER_BILL a,
                            RAB_TENDER_DETAILS b, RAB_ITEM_BREAKUP c
                            where a.bill_id=:ID 
                            and A.REF_ID=B.REF_ID
                            and A.ACT_SEQ=B.ACTIVITY_ID
                            and B.REF_ID=C.REF_ID
                            and A.SEQ_NO=B.SEQ_NO
                            and A.SEQ_NO=C.SEQ_NO
                            order by a.seq_no, c.SORT_NO");
        paramList.Add("ID",billID);
        objDB.bindGridView(gvAddedSequences, sbQuery.ToString(), paramList);
        clearParam();    
    }

    protected void gvAddedSequences_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdTenderSorRefID = new HiddenField();
            hdTenderSorRefID = (HiddenField)e.Row.FindControl("hdTenderSorRefID");
            string referenceId = hdTenderSorRefID.Value;
            HiddenField hdBillRunningSRNo = new HiddenField();
            hdBillRunningSRNo = (HiddenField)e.Row.FindControl("hdBillRunningSRNo");
            string runningSrNo = hdBillRunningSRNo.Value;
            //HiddenField hdRABillNumber = new HiddenField();
            //hdRABillNumber = (HiddenField)e.Row.FindControl("hdRABillNumber");
            //string RABillNo = hdRABillNumber.Value;

            HiddenField hdBillSeqNo = new HiddenField();
            hdBillSeqNo = (HiddenField)e.Row.FindControl("hdBillSeqNo");

            HiddenField hdActSeq = new HiddenField();
            hdActSeq = (HiddenField)e.Row.FindControl("hdActSeq");

            HiddenField hdBillRefID = new HiddenField();
            hdBillRefID = (HiddenField)e.Row.FindControl("hdBillRefID");            

            HyperLink hlActivity = new HyperLink();
            hlActivity = (HyperLink)e.Row.FindControl("hlActivity");
            hlActivity.Text = "View";

            string msheetURL = "RAB_Msheet_View.aspx?refId=" + hdBillRefID.Value + "&seqNo=" + hdBillSeqNo.Value + "&actId=" + hdActSeq.Value + "&tendSorId=" + hdTenderSorRefID.Value + "&RSno=" + ViewState["RUN_SL_NO"].ToString() + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0] + "&billID=" + ViewState["RA_BILL_ID"];

            hlActivity.Attributes.Add("onclick", "window.open('" + msheetURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
        }
    }

    protected void bindSORItems(string tenderSORreferenceId, string jobNumber, string runningSerialNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        if (sortColumn.Equals(""))
        {
            sortColumn = "   ORDER BY  sdesc,SORT_NO,SEQ_NO ";
        }
        if (sortOrder.Equals(""))
        {
            sortOrder = " ASC";
        }

        if (sortColumn.Equals("sdesc"))
        {
            sortColumn = " ORDER BY  sdesc ";
            sortOrder = " DESC";
        }
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        sbQuery.Append(@" SELECT distinct a.REF_ID,
                   a.SEQ_NO,
                    nvl(ITEM_RATE_EDITED,a.ITEM_RATE) ITEM_RATE,
                   a.UOM,
                   a.SORT_NO,
                   ACT_DESC,
                   ACT_PERCENT,
                   a.ACT_SEQ,
                   ADDED_ON,
                   SITE_QTY,
                   HO_QTY,
                   ACT_PROG,
                   FLAG_HO,
                   a.sorno sdesc,
                   TO_CHAR (nvl(sdesc,act_desc)) ldesc,
                   C.TEND_SOR_ID
              FROM RAB_ITEM_BREAKUP a, RAB_TENDER_MASTER c, rab_tender_bill e
             WHERE     c.JOB_no =:JOBNO
                   AND c.tender_no =:TENDER_NO
                AND c.part_no =:part_no
                   AND C.SOR_NO = A.SORNO
                   AND C.REF_ID = A.REF_ID 
                    and E.REF_ID=C.REF_ID
                    and E.SEQ_NO=A.SEQ_NO  
                    and E.RUN_SL_NO=:RUN_SL_NO
                ");
        //To restrict vendors to access the part no for which he have access to
        if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(" and part_no in (select distinct part_no from RAB_TENDER_MASTER aa where aa.c_code=:ccode1 and aa.JOB_no =:job_no1 AND aa.tender_no =:tender_no1 AND aa.part_no =:part_no1) ");
            paramList.Add("ccode1", Session["USERID"].ToString());
            paramList.Add("job_no1", jobNumber);
            paramList.Add("tender_no1", strArray[0]);
            paramList.Add("part_no1", strArray[1]);
        }
        sbQuery.Append(" ORDER BY sdesc, SORT_NO ");

        paramList.Add("JOBNO", jobNumber);
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("part_no", strArray[1]);
        paramList.Add("RUN_SL_NO", runningSerialNumber);
        objDB.bindGridView(gvSORItems, sbQuery.ToString(), paramList);

        trItemsDetails.Visible = true;
        pnlSORItems.Visible = true;

        //if (gvSORItems.Rows.Count > 0)
        //{
        //    trRemarksHistory.Visible = true;
        //    bindSORComments(tenderSORreferenceId, ddJobNumber.SelectedValue, runningSerialNumber);
        //    //Enablinfg the buttons forresubmitting the values
        //    enableActionButtons();
        //}
        //else
        //{
        //    trRemarksHistory.Visible = false;
        //}
    }

    protected void gvSORItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdReferenceID = new HiddenField();
            hdReferenceID = (HiddenField)e.Row.FindControl("hdReferenceID");
            string referenceId = hdReferenceID.Value;
            HiddenField hdSequenceNo = new HiddenField();
            hdSequenceNo = (HiddenField)e.Row.FindControl("hdSequenceNo");
            string sequenceNo = hdSequenceNo.Value;

            HiddenField hdItemRate = new HiddenField();
            hdItemRate = (HiddenField)e.Row.FindControl("hdItemRate");
            string itemRate = hdItemRate.Value;

            HiddenField hdItemQuantity = new HiddenField();
            hdItemQuantity = (HiddenField)e.Row.FindControl("hdItemQuantity");
            string itemQty = hdItemQuantity.Value;


            HiddenField hdSORQty = new HiddenField();
            hdSORQty = (HiddenField)e.Row.FindControl("hdSORQty");
            string SORQty = hdSORQty.Value;

            HiddenField hdSORTenderId = new HiddenField();
            hdSORTenderId = (HiddenField)e.Row.FindControl("hdSORTenderId");

            Label lblAllTotalAmount = new Label();
            lblAllTotalAmount = (Label)e.Row.FindControl("lblAllTotalAmount");

            Label lblAllTotalQty = new Label();
            lblAllTotalQty = (Label)e.Row.FindControl("lblAllTotalQty");

            Button btnSplitActivity = new Button();
            btnSplitActivity = (Button)e.Row.FindControl("btnSplitActivity");

            GridView gvChildReport = e.Row.FindControl("gvSORSplits") as GridView;
            StringBuilder query = new StringBuilder();

            if (!"".Equals(SORQty))
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                if ((Session["ROLE"].Equals("VEND") || (!Session["ROLE"].Equals("VEND") && ViewState["RUN_SL_NO"] != null)))
                //     if ((Session["ROLE"].Equals("VEND") && rbUpdateBill.Checked && ViewState["RUN_SL_NO"] != null) || (!Session["ROLE"].Equals("VEND") && ViewState["RUN_SL_NO"] != null))
                {
                    query.Append(@"select distinct c.TEND_SOR_ID,a.REF_ID,a.SEQ_NO,a.ACTIVITY_DESC ,nvl(ITEM_RATE_EDITED,b.ITEM_RATE) ITEM_RATE,  a.ACTIVITY_PERCENT||'%' ACTIVITY_PERCENT,a.IS_BREAKABLE,a.ACTIVITY_ID ,((nvl(ITEM_RATE_EDITED,b.ITEM_RATE) * B.HO_QTY)*a.ACTIVITY_PERCENT/100)  activityAmt,  (B.HO_QTY*a.ACTIVITY_PERCENT/100) activityQty ,nvl(C.AC_IS_FROZEN,'N') frozen ,CONT_IS_FROZEN,round(cont_qty,4) CONT_QTY,BENGG_IS_FROZEN,AC_IS_FROZEN,RCM_IS_FROZEN, round(BENGG_QTY,4) BENGG_QTY, round(AC_QTY,4) AC_QTY,round(RCM_QTY,4) RCM_QTY,RUN_SL_NO  ,nvl(rab_get_previousQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,RUN_SL_NO,'O'),0) previousQty , rab_get_msheetQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,RUN_SL_NO) msheetQty,UOM,STATUS from RAB_TENDER_DETAILS a,RAB_ITEM_BREAKUP b ,RAB_TENDER_BILL c   where a.REF_ID=:REF_ID    and a.SEQ_NO =:SEQ_NO    and a.seq_no=B.SEQ_NO    and A.REF_ID=B.REF_ID  and C.SEQ_NO(+)=A.SEQ_NO  and C.REF_ID(+)=A.REF_ID  and C.ACT_SEQ(+)=A.ACTIVITY_ID  and ( RCM_FROZEN='Y') ");

                    //To check if the bill is rejected from BE , thn allow Contractor to add other items in the bill - 18-09-2017

                    int countAcceptedItems = 0;
                    countAcceptedItems = getAcceptedCount(ViewState["RUN_SL_NO"].ToString(), referenceId, sequenceNo);

                    if (countAcceptedItems != 0 && (Session["ROLE"].Equals("VEND") && ViewState["RUN_SL_NO"] != null))
                    {
                        query.Append(@" and C.RUN_SL_NO=:RUN_SL_NO  and Trunc (C.RA_DATe)=to_date(:RUN_SL_DATE ,'dd-Mon-yyyy')");
                        paramList.Add("RUN_SL_NO", ViewState["RUN_SL_NO"].ToString());
                        paramList.Add("RUN_SL_DATE", ViewState["RUN_SL_DATE"].ToString());
                    }
                    else
                    //else if (!Session["ROLE"].Equals("VEND") )
                    {
                        query.Append(@" and C.RUN_SL_NO=:RUN_SL_NO  and Trunc (C.RA_DATe)=to_date(:RUN_SL_DATE ,'dd-Mon-yyyy')");
                        paramList.Add("RUN_SL_NO", ViewState["RUN_SL_NO"].ToString());
                        paramList.Add("RUN_SL_DATE", ViewState["RUN_SL_DATE"].ToString());
                    }
                }
                else if (Session["ROLE"].Equals("VEND") )//&& rbNewBill.Checked)
                {
                    query.Append(" select distinct C.TEND_SOR_ID,a.REF_ID,a.SEQ_NO,a.ACTIVITY_DESC , nvl(ITEM_RATE_EDITED,b.ITEM_RATE) ITEM_RATE,  ")
                           .Append(" a.ACTIVITY_PERCENT||'%' ACTIVITY_PERCENT,a.IS_BREAKABLE,a.ACTIVITY_ID ,((nvl(ITEM_RATE_EDITED,b.ITEM_RATE) * B.HO_QTY)*a.ACTIVITY_PERCENT/100)  activityAmt,  ")
                           .Append(" (B.HO_QTY*a.ACTIVITY_PERCENT/100) activityQty ,'' frozen ,'' CONT_IS_FROZEN,'' CONT_QTY,'' BENGG_IS_FROZEN,'' AC_IS_FROZEN,'' RCM_IS_FROZEN,")
                    .Append(" '' BENGG_QTY,'' AC_QTY,'' RCM_QTY,'' RUN_SL_NO ")

                          .Append(" ,nvl(rab_get_previousQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,0,'N'),0) previousQty ")
                           .Append(", rab_get_msheetQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,null) msheetQty,UOM,'Not Filled' status ")

                          .Append("  from RAB_TENDER_DETAILS a,RAB_ITEM_BREAKUP b ,rab_tender_master c  ")
                         .Append("   where a.REF_ID=:REF_ID ")
                         .Append("   and a.SEQ_NO =:SEQ_NO  ")
                         .Append("   and a.seq_no=B.SEQ_NO  ")
                           .Append(" and A.REF_ID=B.REF_ID  ")
                        //.Append(" and (BE_FROZEN= 'Y' or  AC_FROZEN='Y' or RCM_FROZEN='Y')           ")
                           .Append(" and ( RCM_FROZEN='Y')  ")
                           .Append(" and C.REF_ID=B.REF_ID  ")
                           .Append(" and C.SOR_NO=B.SORNO   ");
                }
                query.Append("   order by IS_BREAKABLE desc,ACTIVITY_PERCENT desc");

                paramList.Add("REF_ID", referenceId);
                paramList.Add("SEQ_NO", sequenceNo);
                objDB.bindGridView(gvChildReport, query.ToString(), paramList);
            }
            Label lblTotalAmount = new Label();
            lblTotalAmount = (Label)e.Row.FindControl("lblTotalAmount");
            ////if (!"0".Equals(totalValue.ToString()))
            ////    lblTotalAmount.Text = totalValue.ToString();
            //if (rbNewBill.Checked)
            //{
            //    gvChildReport.Columns[8].Visible = false;
            //    gvChildReport.Columns[9].Visible = false;
            //    gvChildReport.Columns[10].Visible = false;
            //}
            //else
            //{
            //    gvChildReport.Columns[8].Visible = true;
            //    gvChildReport.Columns[9].Visible = true;
            //    gvChildReport.Columns[10].Visible = true;
            //}
        }
    }

    protected void gvSORSplits_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdChildReferenceID = new HiddenField();
            hdChildReferenceID = (HiddenField)e.Row.FindControl("hdChildReferenceID");
            string referenceId = hdChildReferenceID.Value;
            HiddenField hdChildSequenceNo = new HiddenField();
            hdChildSequenceNo = (HiddenField)e.Row.FindControl("hdChildSequenceNo");
            string sequenceNo = hdChildSequenceNo.Value;

            HiddenField hdActivityPercent = new HiddenField();
            hdActivityPercent = (HiddenField)e.Row.FindControl("hdActivityPercent");
            string activityPercent = hdActivityPercent.Value;

            HiddenField hdActivityId = new HiddenField();
            hdActivityId = (HiddenField)e.Row.FindControl("hdActivityId");
            string activityID = hdActivityId.Value;

            HiddenField hdIsBreakable = new HiddenField();
            hdIsBreakable = (HiddenField)e.Row.FindControl("hdIsBreakable");
            string isBreakable = hdIsBreakable.Value;

            HiddenField hdContractorFrozen = new HiddenField();
            hdContractorFrozen = (HiddenField)e.Row.FindControl("hdContractorFrozen");
            string ContractorFrozen = hdContractorFrozen.Value;

            HiddenField hdVendorQty = new HiddenField();
            hdVendorQty = (HiddenField)e.Row.FindControl("hdVendorQty");
            string vendorQty = hdVendorQty.Value;

            HiddenField hdBEFrozen = new HiddenField();
            hdBEFrozen = (HiddenField)e.Row.FindControl("hdBEFrozen");

            HiddenField hdACFrozen = new HiddenField();
            hdACFrozen = (HiddenField)e.Row.FindControl("hdACFrozen");

            HiddenField hdRCMFrozen = new HiddenField();
            hdRCMFrozen = (HiddenField)e.Row.FindControl("hdRCMFrozen");

            HiddenField hdRunSrNo = new HiddenField();
            hdRunSrNo = (HiddenField)e.Row.FindControl("hdRunSrNo");

            HiddenField hdTenderSORId = new HiddenField();
            hdTenderSORId = (HiddenField)e.Row.FindControl("hdTenderSORId");

            Label lblQuantity = new Label();
            lblQuantity = (Label)e.Row.FindControl("lblQuantity");

            TextBox txtVenQuantity = new TextBox();
            txtVenQuantity = (TextBox)e.Row.FindControl("txtVenQuantity");

            Label lblVendQuantity = new Label();
            lblVendQuantity = (Label)e.Row.FindControl("lblVendQuantity");

            Label lblAmount = new Label();
            lblAmount = (Label)e.Row.FindControl("lblAmount");

            HiddenField hdInitialItemRate = new HiddenField();
            hdInitialItemRate = (HiddenField)e.Row.FindControl("hdInitialItemRate");

            float unitRate = 0;
            float prevQty = 0;
            float totalAmount = 0;
            float actPercentage = float.Parse(activityPercent.Replace("%", ""));

            if (hdInitialItemRate.Value.Length > 0 && lblQuantity.Text.Length > 0)
            {
                unitRate = float.Parse(hdInitialItemRate.Value);
                prevQty = float.Parse(lblQuantity.Text);
                if (unitRate > 0 && prevQty > 0)
                {
                    totalAmount = (unitRate * prevQty * (actPercentage / 100));
                }
                // lblAmount.Text = totalAmount.ToString();
                lblAmount.Text = totalAmount.ToString("0.00");
            }
            else
            {
                lblAmount.Text = "0";
            }

            Label lblBEReject = new Label();
            lblBEReject = (Label)e.Row.FindControl("lblBEReject");

            Label lblACFrozen = new Label();
            lblACFrozen = (Label)e.Row.FindControl("lblACFrozen");

            Label lblRCMFrozen = new Label();
            lblRCMFrozen = (Label)e.Row.FindControl("lblRCMFrozen");

            CheckBox chkBEReject = new CheckBox();
            chkBEReject = (CheckBox)e.Row.FindControl("chkBEReject");
            TextBox txtBEQuantity = new TextBox();
            txtBEQuantity = (TextBox)e.Row.FindControl("txtBEQuantity");

            CheckBox chkACReject = new CheckBox();
            chkACReject = (CheckBox)e.Row.FindControl("chkACReject");
            TextBox txtACQuantity = new TextBox();
            txtACQuantity = (TextBox)e.Row.FindControl("txtACQuantity");

            CheckBox chkRCMReject = new CheckBox();
            chkRCMReject = (CheckBox)e.Row.FindControl("chkRCMReject");
            TextBox txtRCMQuantity = new TextBox();
            txtRCMQuantity = (TextBox)e.Row.FindControl("txtRCMQuantity");

            Label lblBEQuantity = new Label();
            lblBEQuantity = (Label)e.Row.FindControl("lblBEQuantity");

            Label lblACQuantity = new Label();
            lblACQuantity = (Label)e.Row.FindControl("lblACQuantity");

            Label lblRCMQuantity = new Label();
            lblRCMQuantity = (Label)e.Row.FindControl("lblRCMQuantity");

            Button btnChildSplitActivity = new Button();
            btnChildSplitActivity = (Button)e.Row.FindControl("btnChildSplitActivity");

            chkBEReject.Visible = false;
            chkACReject.Visible = false;
            chkRCMReject.Visible = false;
            txtBEQuantity.Visible = false;
            txtACQuantity.Visible = false;
            txtRCMQuantity.Visible = false;

            Label lblMyTotal = new Label();
            lblMyTotal = (Label)e.Row.FindControl("lblMyTotal");

            Label lblMyAmount = new Label();
            lblMyAmount = (Label)e.Row.FindControl("lblMyAmount");

            Label lblinitPercentage = new Label();
            lblinitPercentage = (Label)e.Row.FindControl("lblinitPercentage");

            HiddenField hdBEQty = new HiddenField();
            hdBEQty = (HiddenField)e.Row.FindControl("hdBEQty");

            HiddenField hdACQty = new HiddenField();
            hdACQty = (HiddenField)e.Row.FindControl("hdACQty");

            HiddenField hdRCMQty = new HiddenField();
            hdRCMQty = (HiddenField)e.Row.FindControl("hdRCMQty");

            if (hdVendorQty.Value.Length > 0 && lblQuantity.Text.Length > 0)
            {
                try
                {
                    float totalQty = 0;
                    float qtyTemp = 0;
                    if (hdRCMQty.Value.Length > 0)
                    {
                        qtyTemp = float.Parse(hdRCMQty.Value);
                    }
                    else if (hdACQty.Value.Length > 0)
                    {
                        qtyTemp = float.Parse(hdACQty.Value);
                    }
                    else if (hdBEQty.Value.Length > 0)
                    {
                        qtyTemp = float.Parse(hdBEQty.Value);
                    }
                    else if (hdVendorQty.Value.Length > 0)
                    {
                        qtyTemp = float.Parse(hdVendorQty.Value);
                    }

                    totalQty = qtyTemp + float.Parse(lblQuantity.Text);
                    float itemInitialRate = float.Parse(hdInitialItemRate.Value);
                    float activityPercentage = actPercentage;
                    lblMyTotal.Text = totalQty.ToString();
                    lblMyAmount.Text = (totalQty * itemInitialRate * activityPercentage / 100).ToString("0.00");
                }
                catch (Exception err)
                {
                    lblMyTotal.Text = "0";
                    lblMyAmount.Text = "0";
                }

                // Showing the link to view measurement sheet details

                HyperLink hlMSheet = new HyperLink();
                hlMSheet = (HyperLink)e.Row.FindControl("hlMSheet");
                hlMSheet.Text = "View";
                //string msheetURL = "RAB_Msheet_View.aspx?refId=" + referenceId + "&seqNo=" + sequenceNo + "&actId=" + activityID + "&tendSorId=" + hdTenderSORId.Value + "&RSno=" + hdRunSrNo.Value + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0];

                string msheetURL = "RAB_Msheet_View.aspx?refId=" + referenceId + "&seqNo=" + sequenceNo + "&actId=" + activityID + "&tendSorId=" + hdTenderSORId.Value + "&RSno=" + ViewState["RUN_SL_NO"].ToString() + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0] + "&billID=" + ViewState["RA_BILL_ID"];
                
                hlMSheet.Attributes.Add("onclick", "window.open('" + msheetURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
            }
            else
            {
                HyperLink hlMSheet = new HyperLink();
                hlMSheet = (HyperLink)e.Row.FindControl("hlMSheet");
                hlMSheet.Text = "";
            }
            //Enabling upload measurement sheet link

            if ("VEND".Equals(Session["ROLE"].ToString()) && ("N".Equals(hdContractorFrozen.Value) || "R".Equals(hdContractorFrozen.Value)))
            {
                txtVenQuantity.Visible = true;
                HyperLink hlUploadMSheet = new HyperLink();
                hlUploadMSheet = (HyperLink)e.Row.FindControl("hlUploadMSheet");
                hlUploadMSheet.Text = "M. Sheet";
                string msheetURL = "RAB_Msheet_Entry.aspx?refId=" + referenceId + "&seqNo=" + sequenceNo + "&actId=" + activityID + "&tendSorId=" + hdTenderSORId.Value + "&RSno=" + ViewState["RUN_SL_NO"].ToString() + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0] + "&billID=" + ViewState["RA_BILL_ID"]; ;
                hlUploadMSheet.Attributes.Add("onclick", "window.open('" + msheetURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
            }

              // 18-09-2017
            // Below condition is added for displaying the link if the complete bill is rejected by BE, 
            // The requirement was to add new items even after the bill is generated and rejected all items by BE.
            else if ("".Equals(hdContractorFrozen.Value) && getAcceptedCount(hdRunSrNo.Value, referenceId, sequenceNo) == 0)
            {
                txtVenQuantity.Visible = true;
                HyperLink hlUploadMSheet = new HyperLink();
                hlUploadMSheet = (HyperLink)e.Row.FindControl("hlUploadMSheet");
                hlUploadMSheet.Text = "M. Sheet";
                string msheetURL = "RAB_Msheet_Entry.aspx?refId=" + referenceId + "&seqNo=" + sequenceNo + "&actId=" + activityID + "&tendSorId=" + hdTenderSORId.Value + "&RSno=" + ViewState["RUN_SL_NO"].ToString() + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0] + "&billID=" + ViewState["RA_BILL_ID"]; ;
                hlUploadMSheet.Attributes.Add("onclick", "window.open('" + msheetURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
            }

            if ("VEND".Equals(Session["ROLE"].ToString()))
            {
                if ("Y".Equals(ContractorFrozen))
                {
                    //lblVendQuantity.Visible = true;
                    lblVendQuantity.Text = vendorQty;
                    txtVenQuantity.Visible = false;

                    if ("Y".Equals(hdBEFrozen.Value) && "N".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";                       
                        lblACFrozen.Text = "";
                        lblACFrozen.Text = "";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "Y".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        // lblACFrozen.Text = "Checked by AC";
                        //  lblACFrozen.Text = "Pending with RCM";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "Y".Equals(hdACFrozen.Value) && "Y".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        // lblACFrozen.Text = "Checked by AC";
                        //  lblRCMFrozen.Text = "Approved by RCM";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "N".Equals(hdACFrozen.Value) && "Y".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        // lblACFrozen.Text = "";
                        // lblRCMFrozen.Text = "Approved by RCM";
                    }
                    else
                    {
                        lblACFrozen.Text = "";
                        lblACFrozen.Text = "";
                    }
                }
                else
                {
                    // lblVendQuantity.Visible = false;
                    txtVenQuantity.Visible = true;
                    txtVenQuantity.Text = vendorQty;
                }
            }

            else if ("VEND".Equals(Session["ROLE"].ToString()) )
            {
                txtVenQuantity.Visible = true;
                HyperLink hlUploadMSheet = new HyperLink();
                hlUploadMSheet = (HyperLink)e.Row.FindControl("hlUploadMSheet");
                hlUploadMSheet.Text = "M. Sheet";
                string msheetURL = "RAB_Msheet_Entry.aspx?refId=" + referenceId + "&seqNo=" + sequenceNo + "&actId=" + activityID + "&tendSorId=" + hdTenderSORId.Value + "&RSno=" + ViewState["RUN_SL_NO"].ToString() + "&jobno=" + ddJobNumber.SelectedValue + "&tNo=" + strArray[0] + "&billID=" + ViewState["RA_BILL_ID"]; ;
                hlUploadMSheet.Attributes.Add("onclick", "window.open('" + msheetURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
            }
            else
            {
                //lblVendQuantity.Visible = true;
                if (vendorQty.Length > 0)
                {
                    lblVendQuantity.Text = vendorQty;
                }
                else
                {
                    // lblVendQuantity.Text = "Not Filled";
                    lblVendQuantity.Text = "";
                }
                if (!"BE".Equals(Session["ROLE"].ToString()))
                {
                    if ("N".Equals(hdBEFrozen.Value) && "N".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value))
                    {
                        if (vendorQty.Length > 0)
                        {
                            if ("N".Equals(hdContractorFrozen.Value))
                            {
                                // lblBEReject.Text = "Sent for Correction";
                            }
                            else
                            {
                                // lblBEReject.Text = "Pending with BE";
                            }
                        }
                        else
                        {
                            lblBEReject.Text = "";
                        }
                        lblACFrozen.Text = "";
                        lblACFrozen.Text = "";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "N".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        lblACFrozen.Text = "";
                        lblACFrozen.Text = "";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "Y".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        // lblACFrozen.Text = "Checked by AC";
                        // lblACFrozen.Text = "Pending with RCM";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "Y".Equals(hdACFrozen.Value) && "Y".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        //  lblACFrozen.Text = "Checked by AC";
                        //  lblRCMFrozen.Text = "Approved by RCM";
                    }
                    else if ("Y".Equals(hdBEFrozen.Value) && "N".Equals(hdACFrozen.Value) && "Y".Equals(hdRCMFrozen.Value))
                    {
                        // lblBEReject.Text = "Checked by BE";
                        lblACFrozen.Text = "";
                        //  lblRCMFrozen.Text = "Approved by RCM";
                    }
                    else
                    {
                        lblACFrozen.Text = "";
                        lblACFrozen.Text = "";
                    }
                }
                else if ("BE".Equals(Session["ROLE"].ToString()))
                {
                    if (vendorQty.Length > 0 && ("N".Equals(hdBEFrozen.Value) || "R".Equals(hdBEFrozen.Value)) && "Y".Equals(hdContractorFrozen.Value))
                    {
                        lblBEReject.Visible = true;
                        lblBEReject.Text = "Reject";
                        chkBEReject.Visible = true;
                        txtBEQuantity.Visible = true;
                    }
                    else if (vendorQty.Length > 0 && "Y".Equals(hdBEFrozen.Value) && "Y".Equals(hdContractorFrozen.Value))
                    {
                        lblBEReject.Visible = true;
                        lblBEReject.Text = "Qty approved";
                        chkBEReject.Visible = false;
                        lblVendQuantity.Text = vendorQty;
                        // lblBEQuantity.Visible = true;
                        //lblBEQuantity.Text = vendorQty;
                        txtBEQuantity.Visible = false;
                    }
                    else
                    {
                        // lblVendQuantity.Text = "Not Filled";
                        lblVendQuantity.Text = "";
                        lblBEReject.Visible = false;
                        chkBEReject.Visible = false;
                        // lblBEQuantity.Visible = false;
                        txtBEQuantity.Visible = false;
                    }
                }
                txtVenQuantity.Visible = false;

                if ("AC".Equals(Session["ROLE"].ToString()) && ("Y".Equals(ContractorFrozen) && "N".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value)))
                {
                    //chkACReject.Visible = true;
                    //lblACFrozen.Text = "Reject";
                    txtBEQuantity.Visible = false;
                    txtACQuantity.Visible = false;
                    txtRCMQuantity.Visible = false;
                }
                else if ("RCM".Equals(Session["ROLE"].ToString()) && ("Y".Equals(ContractorFrozen) && "Y".Equals(hdACFrozen.Value) && "N".Equals(hdRCMFrozen.Value)))
                {
                    // chkRCMReject.Visible = true;
                    //lblRCMFrozen.Text = "Reject";
                    txtBEQuantity.Visible = false;
                    txtACQuantity.Visible = false;
                    //txtRCMQuantity.Visible = true;
                }
            }
            //Logic for enabling Text box/Label based on user Role
        }
    }
    private void ShowMessage(string Msg, bool IsError, string RedirectURL)
    {
        Common.Show(Msg);
    }

    protected void gvSORItems_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        gvSORItems.PageIndex = e.NewPageIndex;

        //if ("VEND".Equals(Session["ROLE"].ToString()) && rbNewBill.Checked)
        //{
        //    //Bind all SOR grid for new bill    
        //    trOldBills.Visible = false;
        //    bindSORItems("", ddJobNumber.SelectedValue, "");
        //    trItemsDetails.Visible = true;
        //    enableButtons(Session["ROLE"].ToString());
        //}
        //else if ("VEND".Equals(Session["ROLE"].ToString()) && rbUpdateBill.Checked)
        //{
        //    //Bind Old bills for updation     
        //    trOldBills.Visible = true;
        //    trItemsDetails.Visible = false;
        //    trBillDetails.Visible = false;
        //    btnSubmit.Visible = false;
        //    bindBillsAdded(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue);
        //}
        //else
        //{
        //    bindSORItems("", ddJobNumber.SelectedValue, "");
        //}
    }

    protected void bindMeasurementSheetP(string referenceId, string sequenceNumber, string activityId, string tenderSorId, string runningSrNo)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append(@"select ID,REF_ID, SEQ_NO, TENDER_SOR_ID, ACT_SEQ, RUN_SL_NO,  RUN_SL_DATE, ACTIVTY_DESC,QUANTITY,JointNo,ReportNo,to_char(InspectionDate,'dd-mm-yyyy') InspectionDate,wpsNo,  welderNo,com1,com2,   CALCULATED_QTY, ACTIVITY_ORDER,unit  from  RAB_TENDER_MSHEET  WHERE  REF_ID=:REF_ID  AND SEQ_NO=:SEQ_NO  AND ACT_SEQ=:ACT_SEQ  AND TENDER_SOR_ID=:TENDER_SOR_ID ");
        if (runningSrNo.Length > 0)
        {
            sbQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", runningSrNo);
        }
        else
        {
            sbQuery.Append(" AND RUN_SL_NO is null ");
        }
        sbQuery.Append(" ORDER BY ACTIVITY_ORDER");

        paramList.Add("REF_ID", referenceId);
        paramList.Add("SEQ_NO", sequenceNumber);
        paramList.Add("ACT_SEQ", activityId);
        paramList.Add("TENDER_SOR_ID", tenderSorId);

        objDB.bindGridView(gvMeasurementSheetP, sbQuery.ToString(), paramList);

        //Calculate Sum and display in Footer Row
    }

    protected void bindUOM()
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select distinct upper(UOM) UOM from RAB_ITEM_BREAKUP where uom is not null order by upper(uom)");
        objDB.bindDropDownList(ddUnit, sbQuery.ToString(), paramList, "UOM", "UOM", "", "--Select UOM--");
    }

    protected void bindLineNumber(string sequenceNumber)
    {
        string condition = "";
        if (sequenceNumber.Substring(9, 1).Equals("1"))
        {
            //condition = " and to_number(line_size) between  to_number('0') AND to_number('1.5') ";
            condition = " and (line_size) between  ('0') AND ('1.5') ";
        }
        else if (sequenceNumber.Substring(9, 1).Equals("2"))
        {
            //condition = " and to_number(line_size) between  to_number('2') AND to_number('6') ";
            condition = " and (line_size) between  ('2') AND ('6') ";
        }
        else if (sequenceNumber.Substring(9, 1).Equals("3"))
        {
            //condition = " and to_number(line_size) between  to_number('8') AND to_number('14') ";
            condition = " and (line_size) between  ('8') AND ('14') ";
        }
        else if (sequenceNumber.Substring(9, 1).Equals("4"))
        {
            // condition = " and to_number(line_size) between  to_number('16') AND to_number('24') ";
            condition = " and (line_size) between  ('8') AND ('14') ";
        }
        else if (sequenceNumber.Substring(9, 1).Equals("5"))
        {
            // condition = " and to_number(line_size) between  to_number('26') AND to_number('36') ";
            condition = " and (line_size) between  ('26') AND ('36') ";
        }
        else if (sequenceNumber.Substring(9, 1).Equals("6"))
        {
            // condition = " and to_number(line_size) between  to_number('38') AND to_number('48') ";
            condition = " and (line_size) between ('38') AND ('48') ";
        }

        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select line_size,line_size||'-'||service||'-'||unit||'-'||lsrno||'-'||class||'-'||isosheet||'-'||areano as LineNumber from cosmas" + ddJobNumber.SelectedValue + ".c_line where line_size <> '*' " + condition + "  order by line_size||'-'||service||'-'||unit||'-'||lsrno||'-'||class||'-'||isosheet||'-'||areano");
        objDB.bindDropDownList(ddLineNumber, sbQuery.ToString(), paramList, "line_size", "LineNumber", "", "--Select Line Number--");
    }

    protected void bindCom1(string UOM)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select distinct symbol,descr from mto.specinp order by descr");
        objDB.bindDropDownList(ddlCom1, sbQuery.ToString(), paramList, "symbol", "descr", "", "--Select --");
        if ("INCH M".Equals(UOM))
        {
            ddlCom1.SelectedValue = "PIP";
            ddlCom1.Enabled = false;
        }
        else if ("INCH DIA".Equals(UOM))
        {
            ddlCom1.Enabled = true;
            ddlCom1.SelectedValue = "PIP";
        }
    }

    protected void bindCom2(string UOM)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select distinct symbol,descr from mto.specinp order by descr");
        objDB.bindDropDownList(ddlCom2, sbQuery.ToString(), paramList, "symbol", "descr", "", "--Select --");
        if ("INCH M".Equals(UOM))
        {
            ddlCom2.SelectedValue = "PIP";
            ddlCom2.Enabled = false;
        }
        else if ("INCH DIA".Equals(UOM))
        {
            ddlCom2.Enabled = true;
        }
    }

    protected void btnCreateNewBill_Click(object sender, EventArgs e)
    {
        // open modal popup to add new bill entry
        mpCreateBillEntry.Show();
    }

    protected void btnMeasurementSheet_Click(object sender, EventArgs e)
    {
        String key = (sender as Button).Attributes["key"];
        String[] keys = key.Split('$');

        if (keys[1].ToString().StartsWith("M109") || keys[1].ToString().StartsWith("M209"))
        {
            lblSequenceNumberP.Text = keys[1].ToString();
            lblUOM.Text = keys[10].ToString().Trim().ToUpper();
            hdReferenceIdP.Value = keys[0].ToString();
            hdSequenceNumberP.Value = keys[1].ToString();
            hdActivityIdP.Value = keys[2].ToString();
            hdBE_FrozenP.Value = keys[5].ToString();
            hdCont_FrozenP.Value = keys[8].ToString();
            hd_tenderSorIdP.Value = keys[9].ToString();
            hd_UOMP.Value = keys[10].ToString();
            string runningSrNoP = keys[7].ToString();
            hdRunningSerailNoP.Value = runningSrNoP;
            bindMeasurementSheetP(keys[0].ToString(), keys[1].ToString(), keys[2].ToString(), keys[9].ToString(), runningSrNoP);
            ViewState["fileID"] = keys[0].ToString();
            bindLineNumber(keys[1].ToString());
            bindCom1(keys[10].ToString().Trim().ToUpper());
            bindCom2(keys[10].ToString().Trim().ToUpper());
            ModalPopupExtenderForMSheetP.Show();
        }
        else
        {
            lblActivityDesc.Text = keys[4].ToString();
            hdReferenceId.Value = keys[0].ToString();
            hdSequenceNumber.Value = keys[1].ToString();
            hdActivityId.Value = keys[2].ToString();
            hdBE_Frozen.Value = keys[5].ToString();
            hdCont_Frozen.Value = keys[8].ToString();
            lblSequenceNumber.Text = keys[1].ToString();
            hd_tenderSorId.Value = keys[9].ToString();
            string runningSrNo = keys[7].ToString();
            hdRunningSerailNo.Value = runningSrNo;
            bindMeasurementSheet(keys[0].ToString(), keys[1].ToString(), keys[2].ToString(), keys[9].ToString(), runningSrNo);
            ViewState["fileID"] = keys[0].ToString();
            bindUOM();
            if (keys[8].ToString().Equals("Y"))
            {
                trMSheetEntry.Visible = false;
            }
            else if (keys[8].ToString().Equals("N"))
            {
                trMSheetEntry.Visible = true;
            }
            else
            {
                trMSheetEntry.Visible = true;
            }

            if (!Session["ROLE"].Equals("VEND"))
            {
                trMSheetEntry.Visible = false;
            }
            ModalPopupExtenderForMSheet.Show();
        }      
    }

    protected void btnMeasurementSheet1_Click(object sender, EventArgs e)
    {
        String key = (sender as Button).Attributes["key"];
        String[] keys = key.Split('$');

        if (keys[1].ToString().StartsWith("M109") || keys[1].ToString().StartsWith("M209"))
        {
            lblSequenceNumberP.Text = keys[1].ToString();
            lblUOM.Text = keys[10].ToString().Trim().ToUpper();
            hdReferenceIdP.Value = keys[0].ToString();
            hdSequenceNumberP.Value = keys[1].ToString();
            hdActivityIdP.Value = keys[2].ToString();
            hdBE_FrozenP.Value = keys[5].ToString();
            hdCont_FrozenP.Value = keys[8].ToString();
            hd_tenderSorIdP.Value = keys[9].ToString();
            hd_UOMP.Value = keys[10].ToString();
            string runningSrNoP = keys[7].ToString();
            hdRunningSerailNoP.Value = runningSrNoP;
            bindMeasurementSheetP(keys[0].ToString(), keys[1].ToString(), keys[2].ToString(), keys[9].ToString(), runningSrNoP);
            ViewState["fileID"] = keys[0].ToString();
            bindLineNumber(keys[1].ToString());
            bindCom1(keys[10].ToString().Trim().ToUpper());
            bindCom2(keys[10].ToString().Trim().ToUpper());
            ModalPopupExtenderForMSheetP.Show();
        }
        else
        {
            lblActivityDesc.Text = keys[4].ToString();
            hdReferenceId.Value = keys[0].ToString();
            hdSequenceNumber.Value = keys[1].ToString();
            hdActivityId.Value = keys[2].ToString();
            hdBE_Frozen.Value = keys[5].ToString();
            hdCont_Frozen.Value = keys[8].ToString();
            lblSequenceNumber.Text = keys[1].ToString();
            hd_tenderSorId.Value = keys[9].ToString();
            string runningSrNo = keys[7].ToString();
            hdRunningSerailNo.Value = runningSrNo;
            bindMeasurementSheet(keys[0].ToString(), keys[1].ToString(), keys[2].ToString(), keys[9].ToString(), runningSrNo);
            ViewState["fileID"] = keys[0].ToString();
            bindUOM();
            if (keys[8].ToString().Equals("Y"))
            {
                trMSheetEntry.Visible = false;
            }
            else if (keys[8].ToString().Equals("N"))
            {
                trMSheetEntry.Visible = true;
            }
            else
            {
                trMSheetEntry.Visible = true;
            }

            if (!Session["ROLE"].Equals("VEND"))
            {
                trMSheetEntry.Visible = false;
            }
            ModalPopupExtenderForMSheet.Show();
        }
    }

    protected void ddLineNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        ModalPopupExtenderForMSheetP.Show();
        txtQuantityP.Text = ddLineNumber.SelectedItem.Value.ToString();
    }

    protected void gvMeasurementSheet_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            totalMeasurementQty += Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "CALCULATED_QTY"));

            //Hiding Delete button from other than Contractor
            HiddenField hdMSheetRSerialNum = new HiddenField();
            hdMSheetRSerialNum = (HiddenField)e.Row.FindControl("hdMSheetRSerialNum");
            string runningSrNumber = hdMSheetRSerialNum.Value;

            if (runningSrNumber.Length > 0 && hdCont_Frozen.Value.Equals("Y"))
            {
                e.Row.Cells[10].Text = "";
            }

            if (!Session["ROLE"].Equals("VEND"))
            {
                e.Row.Cells[10].Visible = false;
            }

            AddConfirmDelete((GridView)sender, e);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotalQty = (Label)e.Row.FindControl("lblTotalQty");
            lblTotalQty.Text = totalMeasurementQty.ToString();
        }
    }

    protected void gvMeasurementSheetP_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            totalMeasurementQtyP += Convert.ToDouble(DataBinder.Eval(e.Row.DataItem, "CALCULATED_QTY"));

            //Hiding Delete button from other than Contractor
            HiddenField hdMSheetRSerialNumP = new HiddenField();
            hdMSheetRSerialNumP = (HiddenField)e.Row.FindControl("hdMSheetRSerialNumP");
            string runningSrNumber = hdMSheetRSerialNumP.Value;

            if (runningSrNumber.Length > 0 && hdCont_FrozenP.Value.Equals("Y"))
            {
                e.Row.Cells[10].Text = "";
            }

            if (!Session["ROLE"].Equals("VEND"))
            {
                e.Row.Cells[10].Visible = false;
            }

            AddConfirmDelete((GridView)sender, e);
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotalQtyP = (Label)e.Row.FindControl("lblTotalQtyP");
            lblTotalQtyP.Text = totalMeasurementQtyP.ToString();
        }
    }

    public static void AddConfirmDelete(GridView gv, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            foreach (DataControlField dcf in gv.Columns)
            {
                if (dcf.ToString() == "CommandField")
                {
                    if (((CommandField)dcf).ShowDeleteButton == true)
                    {
                        e.Row.Cells[gv.Columns.IndexOf(dcf)].Attributes
                        .Add("onclick", "return confirm(\"Are you sure?\")");
                    }
                }
            }
        }
    }

    protected void gvMeasurementSheet_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow row = (GridViewRow)gvMeasurementSheet.Rows[e.RowIndex];
        HiddenField hdMSheetRSerialNum = new HiddenField();
        hdMSheetRSerialNum = (HiddenField)row.FindControl("hdMSheetRSerialNum");
        HiddenField hdMSheetID = new HiddenField();
        hdMSheetID = (HiddenField)row.FindControl("hdMSheetID");
        HiddenField hdMSheetRefId = new HiddenField();
        hdMSheetRefId = (HiddenField)row.FindControl("hdMSheetRefId");
        HiddenField hdMSheetSeqNo = new HiddenField();
        hdMSheetSeqNo = (HiddenField)row.FindControl("hdMSheetSeqNo");
        HiddenField hdMSheetTSorId = new HiddenField();
        hdMSheetTSorId = (HiddenField)row.FindControl("hdMSheetTSorId");
        HiddenField hdMSheetActSeq = new HiddenField();
        hdMSheetActSeq = (HiddenField)row.FindControl("hdMSheetActSeq");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        StringBuilder sbDeleteQuery = new StringBuilder();
        sbDeleteQuery.Append("DELETE FROM RAB_TENDER_MSHEET WHERE id=:id ");
        paramList.Add("id", hdMSheetID.Value);
        if (hdMSheetRSerialNum.Value.Length > 0)
        {
            sbDeleteQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", hdMSheetRSerialNum.Value);
        }
        if (objDB.executeNonQuery(sbDeleteQuery.ToString(), paramList) > 0)
        {
            //Updating tender_ra_bill table
            try
            {
                Dictionary<string, string> paramUpdateList = new Dictionary<string, string>();
                paramUpdateList.Add("t_refid", hdMSheetRefId.Value);
                paramUpdateList.Add("t_ccode", Session["USERID"].ToString());
                paramUpdateList.Add("t_tend_sor_id", hdMSheetTSorId.Value);
                paramUpdateList.Add("t_act_seq", hdMSheetActSeq.Value);
                paramUpdateList.Add("t_SEQ_NO", hdMSheetSeqNo.Value);
                paramUpdateList.Add("t_RUN_SL_NO", hdMSheetRSerialNum.Value);
                objDB.executeProcedure("RABILLING.rab_msheet_TBill_Update", paramUpdateList);
            }
            catch (Exception err)
            { }
            gvMeasurementSheet.EditIndex = -1;
            Common.Show("Deleted Successfully");
            //Update RAB_TENDER_BILL 
            bindMeasurementSheet(hdMSheetRefId.Value, hdMSheetSeqNo.Value, hdMSheetActSeq.Value, hdMSheetTSorId.Value, hdMSheetRSerialNum.Value);           
            ModalPopupExtenderForMSheet.Show();
        }
    }

    protected void gvMeasurementSheetP_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        GridViewRow row = (GridViewRow)gvMeasurementSheetP.Rows[e.RowIndex];
        HiddenField hdMSheetRSerialNum = new HiddenField();
        hdMSheetRSerialNum = (HiddenField)row.FindControl("hdMSheetRSerialNumP");
        HiddenField hdMSheetID = new HiddenField();
        hdMSheetID = (HiddenField)row.FindControl("hdMSheetIDP");
        HiddenField hdMSheetRefId = new HiddenField();
        hdMSheetRefId = (HiddenField)row.FindControl("hdMSheetRefIdP");
        HiddenField hdMSheetSeqNo = new HiddenField();
        hdMSheetSeqNo = (HiddenField)row.FindControl("hdMSheetSeqNoP");
        HiddenField hdMSheetTSorId = new HiddenField();
        hdMSheetTSorId = (HiddenField)row.FindControl("hdMSheetTSorIdP");
        HiddenField hdMSheetActSeq = new HiddenField();
        hdMSheetActSeq = (HiddenField)row.FindControl("hdMSheetActSeqP");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        StringBuilder sbDeleteQuery = new StringBuilder();
        sbDeleteQuery.Append("DELETE FROM RAB_TENDER_MSHEET WHERE id=:id ");
        paramList.Add("id", hdMSheetID.Value);
        if (hdMSheetRSerialNum.Value.Length > 0)
        {
            sbDeleteQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", hdMSheetRSerialNum.Value);
        }

        if (objDB.executeNonQuery(sbDeleteQuery.ToString(), paramList) > 0)
        {
            //Updating tender_ra_bill table
            try
            {
                Dictionary<string, string> paramUpdateList = new Dictionary<string, string>();
                paramUpdateList.Add("t_refid", hdMSheetRefId.Value);
                paramUpdateList.Add("t_ccode", Session["USERID"].ToString());

                paramUpdateList.Add("t_tend_sor_id", hdMSheetTSorId.Value);
                paramUpdateList.Add("t_act_seq", hdMSheetActSeq.Value);

                paramUpdateList.Add("t_SEQ_NO", hdMSheetSeqNo.Value);
                paramUpdateList.Add("t_RUN_SL_NO", hdMSheetRSerialNum.Value);
                objDB.executeProcedure("RABILLING.rab_msheet_TBill_Update", paramUpdateList);
            }
            catch (Exception err)
            { }
            gvMeasurementSheetP.EditIndex = -1;
            Common.Show("Deleted Successfully");
            //Update RAB_TENDER_BILL 
            bindMeasurementSheetP(hdMSheetRefId.Value, hdMSheetSeqNo.Value, hdMSheetActSeq.Value, hdMSheetTSorId.Value, hdMSheetRSerialNum.Value);          
            ModalPopupExtenderForMSheetP.Show();
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        // rbNewBill_CheckedChanged(sender, e);
    }

    protected void btnCancelP_Click(object sender, EventArgs e)
    {
        // rbNewBill_CheckedChanged(sender, e);
    }

    protected void btnCanceNewBill_Click(object sender, EventArgs e)
    {
        mpCreateBillEntry.Hide();
    }

   protected void btnAddNewBill_Click(object sender, EventArgs e)
    {
       // check bill period and bill date (Future date not allowed) 
        string error = "";
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        clearParam();
        if (txtNewBillNumber.Text.Trim().Length.Equals(0) && txtNewBillDate.Text.Trim().Length.Equals(0) && txtPeriodFrom.Text.Trim().Length.Equals(0) && txtPeriodTo.Text.Trim().Length.Equals(0))
        {
            error = "Bill Number, Bill Date and Bill Period cannot be blank\\n";
        }
        // To check future bill date
        paramList.Add("BILL_DATE", txtNewBillDate.Text);
        if (int.Parse(objDB.executeScalar("select to_date(:BILL_DATE,'dd-mm-yyyy')- trunc(sysdate) datediff from dual ", paramList)) > 0)
        {
            error = error + "Bill date cannot be future date\\n";
        }
        clearParam();

        // To check period to shall be >= period from
        paramList.Add("PERIOD_FROM", txtPeriodFrom.Text);
        paramList.Add("PERIOD_TO", txtPeriodTo.Text);
        if (int.Parse(objDB.executeScalar("select to_date(:PERIOD_TO,'dd-mm-yyyy') - to_date(:PERIOD_FROM,'dd-mm-yyyy') datediff from dual ", paramList)) <= 0)
        {
            error = error + "Invalid Billing period\\n";
        }
        clearParam();

        // To check if billing period already exists
        paramList.Add("PERIOD_FROM", txtPeriodFrom.Text);
        paramList.Add("PERIOD_TO", txtPeriodTo.Text);
        paramList.Add("JOBNO", ddJobNumber.SelectedValue);
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        if (!objDB.executeScalar("select count(*) from RAB_TENDER_BILL_MST where PERIOD_FROM=to_date(:PERIOD_FROM,'dd-mm-yyyy') and  PERIOD_TO=to_date(:PERIOD_TO,'dd-mm-yyyy')  and TEND_SOR_ID=(select distinct tend_sor_id from rab_tender_master where job_no=:JOBNO and tender_no=:TENDER_NO  and part_no=:PART_NO) ", paramList).Equals("0"))
        {
            error = error + "Billing period already exists\\n";
        }
        clearParam();

        // Checking if any of the bill is pending, do not allow if any of the bill is pending
        

        paramList.Add("JOBNO", ddJobNumber.SelectedValue);
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        paramList.Add("BILL_STATUS", "ACCEPTED BY RCM");
        if (!objDB.executeScalar("select count(*) from RAB_TENDER_BILL_MST where TEND_SOR_ID=(select distinct tend_sor_id from rab_tender_master where job_no=:JOBNO and tender_no=:TENDER_NO  and part_no=:PART_NO) and UPPER(BILL_STATUS)!=:BILL_STATUS ", paramList).Equals("0"))
        {
            error = error + "A bill is still pending, first get it approved before raising the new bill.\\n";
        }

        clearParam();

        // To check uniquenesss of bill number tender_no wise
        if (txtNewBillNumber.Text.Length > 0)
        {
            StringBuilder sbQueryRABillNo = new StringBuilder();
            sbQueryRABillNo.Append(@"select distinct upper(BILL_NUMBER) 
                                 from RAB_TENDER_BILL_MST 
                                where  
                                BILL_NUMBER=:RA_FINAL_BILL_NO
                                and TEND_SOR_ID=(                                
                                select distinct tend_sor_id from rab_tender_master 
                                where job_no=:JOBNO and tender_no=:TENDER_NO  and part_no=:PART_NO)");
           
            paramList.Add("JOBNO", ddJobNumber.SelectedValue);
            paramList.Add("TENDER_NO", strArray[0]);
            paramList.Add("PART_NO", strArray[1]);
            paramList.Add("RA_FINAL_BILL_NO", txtNewBillNumber.Text.Trim().ToUpper());
            string billNumber = objDB.executeScalar(sbQueryRABillNo.ToString(), paramList);
            if (billNumber.Length > 0)
            {
                error = error +"Bill Number already exists\\n";                
            }
        }

        clearParam();

        if (error.Length > 0)
        {
            Common.Show("Error : " + error);
            lblErrorNewBill.Text = "Error : " + error.Replace("\\n","<br/>");
            mpCreateBillEntry.Show();
        }
        else
        { 
            // Get tender_SOR_ID and sub job number
            clearParam();
            paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
            paramList.Add("TENDER_NO", strArray[0]);
            paramList.Add("PART_NO", strArray[1]);
            string tenderSORIDAndSubJob = objDB.executeScalar(@"SELECT distinct TEND_SOR_ID||'~'||sub_job
                        FROM RAB_TENDER_MASTER
                        WHERE     job_no =:job_no
                            AND tender_no =:tender_no
                            AND PART_NO =:PART_NO", paramList);             
            clearParam();
            string[] strArrayTenderSOR = tenderSORIDAndSubJob.Split('~');
            

        // Insert record and create the bill
            sbQuery.Append(@"INSERT INTO RAB_TENDER_BILL_MST (BILL_NUMBER, PERIOD_FROM, PERIOD_TO, BILL_DATE, CONT_ID, JOB_NO, TENDER_NO, PART_NO, BILL_STATUS, TEND_SOR_ID, RUN_SL_NO, RA_BLL_NO, SUB_JOB) VALUES (:BILL_NUMBER, to_date(:PERIOD_FROM,'dd-mm-yyyy'), to_date(:PERIOD_TO,'dd-mm-yyyy'), to_date(:BILL_DATE,'dd-mm-yyyy'), :CONT_ID, :JOB_NO, :TENDER_NO, :PART_NO, :BILL_STATUS, :TEND_SOR_ID, :RUN_SL_NO, :RA_BLL_NO, :SUB_JOB)");
            paramList.Add("BILL_NUMBER", txtNewBillNumber.Text.ToUpper());
            paramList.Add("PERIOD_FROM", txtPeriodFrom.Text);
            paramList.Add("PERIOD_TO", txtPeriodTo.Text);
            paramList.Add("BILL_DATE", txtNewBillDate.Text);
            paramList.Add("CONT_ID", Session["USERID"].ToString());
            paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToUpper());
            paramList.Add("TENDER_NO", strArray[0]);
            paramList.Add("PART_NO", strArray[1]);
            paramList.Add("BILL_STATUS", "DRAFT");
            paramList.Add("TEND_SOR_ID",strArrayTenderSOR[0]);
            paramList.Add("SUB_JOB",strArrayTenderSOR[1]);

            //generate single number
            Dictionary<string, string> paramRunSrNo = new Dictionary<string, string>();
            string runSerialNo = "";
            StringBuilder sbRunSrNoQry = new StringBuilder();
            sbRunSrNoQry.Append(@"select nvl(max(run_sl_no),0)  + 1 from rab_tender_bill where TEND_SOR_ID=:TEND_SOR_ID");
            paramRunSrNo.Add("TEND_SOR_ID", strArrayTenderSOR[0]);          
            runSerialNo = objDB.executeScalar(sbRunSrNoQry.ToString(), paramRunSrNo);
            if (runSerialNo.Equals(""))
            {
                runSerialNo = "1";
            }
            paramList.Add("RUN_SL_NO",runSerialNo);

            //generate overall number
             Dictionary<string, string> paramOverallSrNo = new Dictionary<string, string>();
            string ovrallSerialNo = "";
            StringBuilder sbOverallSrNoQry = new StringBuilder();
            sbOverallSrNoQry.Append(@"select nvl(max(RA_BLL_NO),0)  + 1 from RAB_TENDER_BILL_MST ");                   
            ovrallSerialNo = objDB.executeScalar(sbOverallSrNoQry.ToString(), paramOverallSrNo);
            if (ovrallSerialNo.Equals(""))
            {
                ovrallSerialNo = "1";
            }

            paramList.Add("RA_BLL_NO", ovrallSerialNo);
            if(objDB.executeNonQuery(sbQuery.ToString(),paramList) >0)
            {
                Common.Show("Bill added successfully!");
                mpCreateBillEntry.Hide();
                bindBills(ddJobNumber.SelectedValue,strArray[0],strArray[1]);
            }
            else
            {
                mpCreateBillEntry.Show();
                Common.Show("Error : Bill not added!");            
            }
        }
    }

   protected void btnCancelUpdateBill_Click(object sender, EventArgs e)
   {
       gvBillSeqItems.DataSource = null;
       gvBillSeqItems.DataBind();
       ddSeqNumber.Items.Clear();       
       mpUpdateBill.Hide();
   }

   protected void btnUpdateNewBill_Click(object sender, EventArgs e)
   {
   }    

    protected void btnAddItem_Click(object sender, EventArgs e)
    {
        string errorMsg = "";
        if (txtActDesc.Text.Length < 1)
        {
            errorMsg = errorMsg + "Activity description is mandatory \\n";
        }

        if (ddUnit.SelectedValue.Equals(""))
        {
            errorMsg = errorMsg + "Select Unit of measurement \\n";
        }

        if (txtQuantity.Text.Length < 1)
        {
            errorMsg = errorMsg + "Quantity is mandatory \\n";
        }
        if (errorMsg.Length > 0)
        {
            Common.Show(errorMsg);
            ModalPopupExtenderForMSheet.Show();
        }
        else
        {
            StringBuilder sbInsertQry = new StringBuilder();
            int recordInserted = 0;
            sbInsertQry.Append(@"INSERT INTO RAB_TENDER_MSHEET  (  REF_ID, SEQ_NO,  ACT_SEQ, RUN_SL_NO,  ACTIVTY_DESC, UNIT, REMARKS, QUANTITY, LENGTH, BREADTH, HEIGHT,  ADDED_BY, ADDED_ON,TENDER_SOR_ID,unit_Weight  ) values  ( :REF_ID, :SEQ_NO, :ACT_SEQ, :RUN_SL_NO,  :ACTIVTY_DESC, :UNIT, :REMARKS, :QUANTITY, :LENGTH, :BREADTH, :HEIGHT,  :ADDED_BY, sysdate,:TENDER_SOR_ID,:unit_Weight ) ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("REF_ID", hdReferenceId.Value);
            paramList.Add("SEQ_NO", hdSequenceNumber.Value);
            paramList.Add("ACT_SEQ", hdActivityId.Value);
            paramList.Add("RUN_SL_NO", hdRunningSerailNo.Value);
            paramList.Add("ACTIVTY_DESC", txtActDesc.Text);
            paramList.Add("UNIT", ddUnit.SelectedValue.ToString());
            paramList.Add("REMARKS", txtItemRemarks.Text);
            paramList.Add("QUANTITY", txtQuantity.Text);
            paramList.Add("LENGTH", txtLength.Text);
            paramList.Add("BREADTH", txtBreadth.Text);
            paramList.Add("HEIGHT", txtHeight.Text);
            paramList.Add("ADDED_BY", Session["USERID"].ToString());
            paramList.Add("TENDER_SOR_ID", hd_tenderSorId.Value);
            paramList.Add("unit_Weight", txtUnitWeight.Text);
            recordInserted = objDB.executeNonQuery(sbInsertQry.ToString(), paramList);

            if (recordInserted > 0)
            {
                //Updating RAB_TENDER_BILL table
                try
                {
                    Dictionary<string, string> paramUpdateList = new Dictionary<string, string>();
                    paramUpdateList.Add("t_refid", hdReferenceId.Value);
                    paramUpdateList.Add("t_ccode", Session["USERID"].ToString());
                    paramUpdateList.Add("t_tend_sor_id", hd_tenderSorId.Value);
                    paramUpdateList.Add("t_act_seq", hdActivityId.Value);
                    paramUpdateList.Add("t_SEQ_NO", hdSequenceNumber.Value);
                    paramUpdateList.Add("t_RUN_SL_NO", hdRunningSerailNo.Value);
                    objDB.executeProcedure("RABILLING.rab_msheet_TBill_Update", paramUpdateList);
                }
                catch (Exception err)
                { }
                Common.Show("Activity added succesfully");
                bindMeasurementSheet(hdReferenceId.Value, hdSequenceNumber.Value, hdActivityId.Value, hd_tenderSorId.Value, hdRunningSerailNo.Value);
               // rbNewBill_CheckedChanged(sender, e);
                ModalPopupExtenderForMSheet.Show();
            }
        }
    }

    protected void btnAddItemP_Click(object sender, EventArgs e)
    {
        string errorMsg = "";
        if (ddLineNumber.SelectedValue.Equals(""))
        {
            errorMsg = errorMsg + "Select Line number \\n";
        }
        if (txtQuantityP.Text.Length < 1)
        {
            errorMsg = errorMsg + "ID is mandatory \\n";
        }
        if (lblUOM.Text.Trim().ToUpper().Equals("INCH DIA"))
        {
            if (ddlCom1.SelectedValue.Equals(""))
            {
                errorMsg = errorMsg + "Please select Com1 \\n";
            }
            if (ddlCom2.SelectedValue.Equals(""))
            {
                errorMsg = errorMsg + "Please select Com2 \\n";
            }
            if (ddlCom1.SelectedValue.Equals("PIP") && ddlCom2.SelectedValue.Equals("PIP"))
            {
                errorMsg = errorMsg + "Com1 and Com2 can not be same \\n";
            }
        }
        if (ddlCom1.SelectedValue.Equals("") && ddlCom2.SelectedValue.Equals(""))
        {
            errorMsg = errorMsg + "Com1 and Com2 can not be blank \\n \\n";
        }
        if (errorMsg.Length > 0)
        {
            Common.Show(errorMsg);
            ModalPopupExtenderForMSheetP.Show();
        }
        else
        {
            StringBuilder sbInsertQry = new StringBuilder();
            int recordInserted = 0;
            sbInsertQry.Append(@"INSERT INTO RAB_TENDER_MSHEET  (  REF_ID, SEQ_NO,  ACT_SEQ, RUN_SL_NO,  ACTIVTY_DESC,  REMARKS, QUANTITY,   ADDED_BY, ADDED_ON,TENDER_SOR_ID,JointNo,ReportNo,InspectionDate,wpsNo,welderNo,com1,com2,unit  )  values  ( :REF_ID, :SEQ_NO, :ACT_SEQ, :RUN_SL_NO,  :ACTIVTY_DESC,  :REMARKS, :QUANTITY, :ADDED_BY, sysdate,:TENDER_SOR_ID,:JointNo,:ReportNo,to_date(:InspectionDate,'DD-MM-YYYY'),:wpsNo,:welderNo,:com1,:com2,:unit ) ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("REF_ID", hdReferenceIdP.Value);
            paramList.Add("SEQ_NO", hdSequenceNumberP.Value);
            paramList.Add("ACT_SEQ", hdActivityIdP.Value);
            paramList.Add("RUN_SL_NO", hdRunningSerailNoP.Value);
            paramList.Add("ACTIVTY_DESC", ddLineNumber.SelectedItem.Text.ToString());
            paramList.Add("QUANTITY", txtQuantityP.Text);
            paramList.Add("REMARKS", txtItemRemarksP.Text);
            paramList.Add("JointNo", txtJointNo.Text);
            paramList.Add("ReportNo", txtReportNo.Text);
            paramList.Add("InspectionDate", txtInspectionDate.Text);
            paramList.Add("ADDED_BY", Session["USERID"].ToString());
            paramList.Add("TENDER_SOR_ID", hd_tenderSorIdP.Value);
            paramList.Add("wpsNo", txtWPSNo.Text);
            paramList.Add("welderNo", txtWelderNo.Text);
            paramList.Add("com1", ddlCom1.SelectedValue);
            paramList.Add("com2", ddlCom2.SelectedValue);
            paramList.Add("unit", lblUOM.Text);

            recordInserted = objDB.executeNonQuery(sbInsertQry.ToString(), paramList);

            if (recordInserted > 0)
            {
                //Updating RAB_TENDER_BILL table
                try
                {
                    Dictionary<string, string> paramUpdateList = new Dictionary<string, string>();
                    paramUpdateList.Add("t_refid", hdReferenceIdP.Value);
                    paramUpdateList.Add("t_ccode", Session["USERID"].ToString());
                    paramUpdateList.Add("t_tend_sor_id", hd_tenderSorIdP.Value);
                    paramUpdateList.Add("t_act_seq", hdActivityIdP.Value);
                    paramUpdateList.Add("t_SEQ_NO", hdSequenceNumberP.Value);
                    paramUpdateList.Add("t_RUN_SL_NO", hdRunningSerailNoP.Value);
                    objDB.executeProcedure("RABILLING.rab_msheet_TBill_Update", paramUpdateList);
                }
                catch (Exception err)
                { }
                Common.Show("Activity added succesfully");
                bindMeasurementSheetP(hdReferenceIdP.Value, hdSequenceNumberP.Value, hdActivityIdP.Value, hd_tenderSorIdP.Value, hdRunningSerailNoP.Value);
               // rbNewBill_CheckedChanged(sender, e);
                ModalPopupExtenderForMSheetP.Show();
            }
        }
    }

    protected void bindMeasurementSheet(string referenceId, string sequenceNumber, string activityId, string tenderSorId, string runningSrNo)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append(@"select ID,REF_ID, SEQ_NO, TENDER_SOR_ID, ACT_SEQ, RUN_SL_NO,  RUN_SL_DATE, ACTIVTY_DESC, UNIT, REMARKS, QUANTITY, LENGTH,  BREADTH, HEIGHT, UNIT4, UNIT5, UNIT6, unit_Weight, CALCULATED_QTY, ACTIVITY_ORDER,CALCULATED_QTY  FROM RAB_TENDER_MSHEET  WHERE  REF_ID=:REF_ID  AND SEQ_NO=:SEQ_NO  AND ACT_SEQ=:ACT_SEQ  AND TENDER_SOR_ID=:TENDER_SOR_ID ");
        if (runningSrNo.Length > 0)
        {
            sbQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", runningSrNo);
        }
        else
        {
            sbQuery.Append(" AND RUN_SL_NO is null ");
        }
        sbQuery.Append(" ORDER BY ACTIVITY_ORDER");

        paramList.Add("REF_ID", referenceId);
        paramList.Add("SEQ_NO", sequenceNumber);
        paramList.Add("ACT_SEQ", activityId);
        paramList.Add("TENDER_SOR_ID", tenderSorId);
        objDB.bindGridView(gvMeasurementSheet, sbQuery.ToString(), paramList);
        //Calculate Sum and display in Footer Row
    }
    

    protected int getAcceptedCount(string runningSerialNumber, string referenceId, string sequenceNo)
    {
        int countAcceptedItems = 0;
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.Append(@"select 
                        count(*)
                        from rab_tender_bill 
                        where RUN_SL_NO=:RUN_SL_NO 
                        and CONT_ID=:CONT_ID
                        and ref_id=:ref_id
                        and CONT_IS_FROZEN in ('Y')");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("RUN_SL_NO", runningSerialNumber);
        paramList.Add("CONT_ID", Session["USERID"].ToString());
        paramList.Add("ref_id", referenceId);
        countAcceptedItems = int.Parse(objDB.executeScalar(sbQuery.ToString(), paramList));
        return countAcceptedItems;
    }

    protected void gvAllBills_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdTenderSorRefID = new HiddenField();
            hdTenderSorRefID = (HiddenField)e.Row.FindControl("hdTenderSorRefID");
            string referenceId = hdTenderSorRefID.Value;
            HiddenField hdBillRunningSRNo = new HiddenField();
            hdBillRunningSRNo = (HiddenField)e.Row.FindControl("hdBillRunningSRNo");
            string runningSrNo = hdBillRunningSRNo.Value;
            HiddenField hdRABillNumber = new HiddenField();
            hdRABillNumber = (HiddenField)e.Row.FindControl("hdRABillNumber");
            string RABillNo = hdRABillNumber.Value;
            HyperLink hlActivity = new HyperLink();
            hlActivity = (HyperLink)e.Row.FindControl("hlActivity");
            hlActivity.Text = "View";
            string activityURL = "RAB_Activity_View.aspx?jobNo=" + ddJobNumber.SelectedValue + "&tenderNo=" + strArray[0] + "&rsno=" + runningSrNo + "&raBillNo=" + RABillNo + "&tendRefId=" + referenceId;
            hlActivity.Attributes.Add("onclick", "window.open('" + activityURL + "','window','center=yes,resizable=no,Height=700px,Width=750px,status =no,toolbar=no,menubar=no,location=no');");
            HiddenField hdBillStatus = new HiddenField();
            hdBillStatus = (HiddenField)e.Row.FindControl("hdBillStatus");
           
            // Renaming the view bill link according to the bill status          

            LinkButton bt = (LinkButton)e.Row.Cells[6].Controls[0];   
            if ((hdBillStatus.Value.ToUpper().Equals("DRAFT") || hdBillStatus.Value.ToUpper().Equals("REJECTED BY BE")) && "VEND".Equals(Session["ROLE"].ToString()))
            {
                bt.Text = "Update Items";
            }else
            {
                bt.Text="View Items";
            }            
        }
    }
    
    protected void ddSORNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) && !"".Equals(ddSORNumber.SelectedValue))
        {
            string[] strArray1 = ddTenderNo.SelectedValue.Split('~');
            string[] strArray2 = ddSORNumber.SelectedValue.Split('~');
            bindSeqNumber(ddJobNumber.SelectedValue, strArray1[0], strArray2[0]);
            mpUpdateBill.Show();
        }
        else
        {
            ddSeqNumber.Items.Clear();
            Common.Show("Please select Job Number and Tender Number");
        }
    }

    protected void bindSORNumber(string jobNumber, string tenderNo)
    {
        clearParam();
        if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(@"SELECT distinct REF_ID||'~'||SOR_NO SOR_NO_refid,  SOR_NO
                                FROM RAB_TENDER_MASTER 
                                where upper(JOB_NO)=:JOB_NO 
                                    AND upper(TENDER_NO)=:TENDER_NO 
                                    AND C_CODE=:C_CODE  
                                order by SOR_NO ");
            paramList.Add("C_CODE", Session["USERID"].ToString());
            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("TENDER_NO", tenderNo.ToUpper());
        }
        else if ("BE".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(@"SELECT DISTINCT b.REF_ID||'~'||b.SOR_NO SOR_NO_refid,  SOR_NO   
                                FROM  RAB_TENDER_USERS a,RAB_TENDER_MASTER b  
                                WHERE b.JOB_NO=:JOB_NO  AND EMPNO=:EMPNO  AND ROLE=:ROLE  and b.job_no=A.JOB_NO and A.TENDER_NO=b.TENDER_NO and A.PART_NO=b.PART_NO 
                                ORDER BY tender_part ");

            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("EMPNO", Session["USERID"].ToString());
            paramList.Add("ROLE", Session["ROLE"].ToString());

        }
        else
        {
            sbQuery.Append(@"SELECT distinct REF_ID||'~'||SOR_NO SOR_NO_refid,  SOR_NO
                        FROM RAB_TENDER_MASTER 
                        where upper(JOB_NO)=:JOB_NO 
                                AND upper(TENDER_NO)=:TENDER_NO                                 
                        order by SOR_NO ");
            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("TENDER_NO", tenderNo.ToUpper());
        }
        objDB.bindDropDownList(ddSORNumber, sbQuery.ToString(), paramList, "SOR_NO_refid", "SOR_NO", "", "--Select SOR Number--");
        if (ddSORNumber.Items.Count > 0)
        {
            //bindAddedBills();
        }

        clearParam();
    }

    protected void bindSeqNumber(string jobNumber, string tenderNo, string referenceID)
    {
        clearParam();
        if (!ddSORNumber.SelectedValue.Equals(""))
        {
            sbQuery.Append(@"select distinct ref_id ||'~'||seq_no ref_seq,'['||sort_no||'] '|| seq_no|| ' ['||substr(sdesc,0,50)||']'  seq_no_desc,sort_no 
                                from RAB_ITEM_BREAKUP 
                                where ref_id=:ref_id and uom is not null
                                order by sort_no ");
            paramList.Add("ref_id", referenceID);
            objDB.bindDropDownList(ddSeqNumber, sbQuery.ToString(), paramList, "ref_seq", "seq_no_desc", "", "--Select Sequence Number--");
        }
        clearParam();
        mpUpdateBill.Show();
    }

    protected void ddSeqNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if(!"".Equals(ddSeqNumber.SelectedValue))
        {
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            bindAddedSequenceGrid(strArray[0], strArray[1]);
            mpUpdateBill.Show();
        }
    }

    protected void bindAddedSequenceGrid(string refID, string sequenceNumber)
    {
        clearParam();
        if (sortColumn.Equals(""))
        {
            sortColumn = "   ORDER BY  sdesc,SORT_NO,SEQ_NO ";
        }
        if (sortOrder.Equals(""))
        {
            sortOrder = " ASC";
        }

        if (sortColumn.Equals("sdesc"))
        {
            sortColumn = " ORDER BY  sdesc ";
            sortOrder = " DESC";
        }       

        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        sbQuery.Append(@" SELECT distinct a.REF_ID,  a.SEQ_NO,
                    nvl(ITEM_RATE_EDITED,a.ITEM_RATE) ITEM_RATE,
                   a.UOM, a.SORT_NO,
                   ACT_DESC,
                   ACT_PERCENT,
                   a.ACT_SEQ,
                   ADDED_ON,
                   SITE_QTY,
                   HO_QTY,
                   ACT_PROG,
                   FLAG_HO,
                   a.sorno sdesc,
                   TO_CHAR (nvl(sdesc,act_desc)) ldesc,
                   C.TEND_SOR_ID
              FROM RAB_ITEM_BREAKUP a, RAB_TENDER_MASTER c
             WHERE      C.SOR_NO = A.SORNO
                   AND C.REF_ID = A.REF_ID 
                   and a.SEQ_NO=:SEQ_NO
                   and c.REF_ID=:REF_ID
                ");
        //To restrict vendors to access the part no for which he have access to
        if ("VEND".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append(" and part_no in (select distinct part_no from RAB_TENDER_MASTER aa where aa.c_code=:ccode1 and aa.JOB_no =:job_no1 AND aa.tender_no =:tender_no1 AND aa.part_no =:part_no1) ");
            paramList.Add("ccode1", Session["USERID"].ToString());
            paramList.Add("job_no1", ddJobNumber.SelectedValue);
            paramList.Add("tender_no1", strArray[0]);
            paramList.Add("part_no1", strArray[1]);
        }

        sbQuery.Append(" ORDER BY sdesc, SORT_NO ");
        string[] strArray1 = ddSeqNumber.SelectedValue.Split('~');
        paramList.Add("SEQ_NO", strArray1[1]);
        paramList.Add("REF_ID", strArray1[0]);       

       
        objDB.bindGridView(gvBillSeqItems, sbQuery.ToString(), paramList);

        trUpdateItemsDetails.Visible = true;
        pnlUpdateItems.Visible = true;
        clearParam();
        //if (gvSORItems.Rows.Count > 0)
        //{
        //    trRemarksHistory.Visible = true;
        //    bindSORComments(tenderSORreferenceId, ddJobNumber.SelectedValue, runningSerialNumber);
        //    //Enablinfg the buttons forresubmitting the values
        //    enableActionButtons();
        //}
        //else
        //{
        //    trRemarksHistory.Visible = false;
        //}
    }

    protected void gvBillSeqItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdReferenceID = new HiddenField();
            hdReferenceID = (HiddenField)e.Row.FindControl("hdReferenceID");
            string referenceId = hdReferenceID.Value;
            HiddenField hdSequenceNo = new HiddenField();
            hdSequenceNo = (HiddenField)e.Row.FindControl("hdSequenceNo");
            string sequenceNo = hdSequenceNo.Value;
            HiddenField hdItemRate = new HiddenField();
            hdItemRate = (HiddenField)e.Row.FindControl("hdItemRate");
            string itemRate = hdItemRate.Value;
            HiddenField hdItemQuantity = new HiddenField();
            hdItemQuantity = (HiddenField)e.Row.FindControl("hdItemQuantity");
            string itemQty = hdItemQuantity.Value;

            HiddenField hdSORQty = new HiddenField();
            hdSORQty = (HiddenField)e.Row.FindControl("hdSORQty");
            string SORQty = hdSORQty.Value;
            HiddenField hdSORTenderId = new HiddenField();
            hdSORTenderId = (HiddenField)e.Row.FindControl("hdSORTenderId");
            Label lblAllTotalAmount = new Label();
            lblAllTotalAmount = (Label)e.Row.FindControl("lblAllTotalAmount");
            Label lblAllTotalQty = new Label();
            lblAllTotalQty = (Label)e.Row.FindControl("lblAllTotalQty");
            Button btnSplitActivity = new Button();
            btnSplitActivity = (Button)e.Row.FindControl("btnSplitActivity");
            GridView gvChildReport = e.Row.FindControl("gvSORSplits") as GridView;
            StringBuilder query = new StringBuilder();

            if (!"".Equals(SORQty))
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                if (Session["ROLE"].Equals("VEND"))
                {
                    query.Append(" select C.TEND_SOR_ID,a.REF_ID,a.SEQ_NO,a.ACTIVITY_DESC , nvl(ITEM_RATE_EDITED,b.ITEM_RATE) ITEM_RATE,  ")
                           .Append(" a.ACTIVITY_PERCENT||'%' ACTIVITY_PERCENT,a.IS_BREAKABLE,a.ACTIVITY_ID ,((nvl(ITEM_RATE_EDITED,b.ITEM_RATE)  * B.HO_QTY)*a.ACTIVITY_PERCENT/100)  activityAmt,  ")
                           .Append(" (B.HO_QTY*a.ACTIVITY_PERCENT/100) activityQty ,'' frozen ,'' CONT_IS_FROZEN,'' CONT_QTY,'' BENGG_IS_FROZEN,'' AC_IS_FROZEN,'' RCM_IS_FROZEN,")
                    .Append(" '' BENGG_QTY,'' AC_QTY,'' RCM_QTY,'' RUN_SL_NO ")

                          .Append(" ,nvl(rab_get_previousQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,d.RUN_SL_NO,'N'),0) previousQty ")
                           .Append(", rab_get_msheetQty(a.REF_ID,C.TEND_SOR_ID,a.SEQ_NO,a.ACTIVITY_ID,d.RUN_SL_NO) msheetQty,UOM,'Not Filled' status ")

                          .Append("  from RAB_TENDER_DETAILS a,RAB_ITEM_BREAKUP b ,rab_tender_master c, RAB_TENDER_BILL_MST d  ")
                         .Append("   where a.REF_ID=:REF_ID ")
                         .Append("   and a.SEQ_NO =:SEQ_NO  ")
                         .Append("   and a.seq_no=B.SEQ_NO  ")
                           .Append(" and A.REF_ID=B.REF_ID  ")
                        //.Append(" and (BE_FROZEN= 'Y' or  AC_FROZEN='Y' or RCM_FROZEN='Y')           ")
                           .Append(" and ( RCM_FROZEN='Y')  ")
                           .Append(" and C.REF_ID=B.REF_ID  ")
                           .Append(" and C.SOR_NO=B.SORNO  and D.ID=:BILL_ID ");
                }
                query.Append("   order by IS_BREAKABLE desc,ACTIVITY_PERCENT desc");

                paramList.Add("REF_ID", referenceId);
                paramList.Add("SEQ_NO", sequenceNo);
                paramList.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                objDB.bindGridView(gvChildReport, query.ToString(), paramList);
            }
            Label lblTotalAmount = new Label();
            lblTotalAmount = (Label)e.Row.FindControl("lblTotalAmount");
            ////if (!"0".Equals(totalValue.ToString()))
            ////    lblTotalAmount.Text = totalValue.ToString();
            //if (rbNewBill.Checked)
            //{
            //    gvChildReport.Columns[8].Visible = false;
            //    gvChildReport.Columns[9].Visible = false;
            //    gvChildReport.Columns[10].Visible = false;
            //}
            //else
            //{
            //    gvChildReport.Columns[8].Visible = true;
            //    gvChildReport.Columns[9].Visible = true;
            //    gvChildReport.Columns[10].Visible = true;
            //}
        }
    }

    protected void disableButons()
    {
        btnSubmitCont.Visible = false;
        btnBESubmit.Visible = false;
        btnRCMSubmit.Visible = false;
        btnACSubmit.Visible = false;
        //btnRCMGenerateBill.Visible = false;
        trRemarks.Visible = false;        
        btnACReject.Visible = false;
        btnBERejectAll.Visible = false;
        btnBERejectPartial.Visible = false;
        btnACReject.Visible = false;
        btnRCMRejectAll.Visible = false;

        trSelectSOR.Visible = false;
        trSelectSequence.Visible = false;
        trUpdateItemsDetails.Visible = false;
    }

    //Enabling Disabling buttons
    public void enableButtons(string role)
    {
        disableButons();
        trRemarks.Visible = true;
        if ("VEND".Equals(role) && ViewState["BILL_STATUS"] != null && (ViewState["BILL_STATUS"].ToString().Equals("DRAFT") || ViewState["BILL_STATUS"].ToString().Equals("REJECTED BY BE")))
        {
            btnSubmitCont.Visible = true;
            enableAddSequence();            
            //if (gvAddedSequences.Rows.Count >= 0)
            //{
            //    btnSubmitCont.Visible = true;
            //    enableAddSequence();                
            //}           
            //else
            //{
            //    btnSubmitCont.Visible = false;                
            //}
        }
        else if ("BE".Equals(role) && ViewState["BILL_STATUS"] != null && (ViewState["BILL_STATUS"].ToString().Equals("SUBMITTED TO BE") || ViewState["BILL_STATUS"].ToString().Equals("REJECTED BY AC")))
        {
            btnBESubmit.Visible = true;
            btnBERejectAll.Visible = true;
            btnBERejectPartial.Visible = false;
           
        }
        else if ("RCM".Equals(role) && ViewState["BILL_STATUS"] != null && (ViewState["BILL_STATUS"].ToString().Equals("ACCEPTED BY AC") ))
        {
            trRemarks.Visible = true;
            btnRCMSubmit.Visible = true;
            btnRCMRejectAll.Visible = true;                       
        }
        else if ("AC".Equals(role) && ViewState["BILL_STATUS"] != null && (ViewState["BILL_STATUS"].ToString().Equals("ACCEPTED BY BE") || ViewState["BILL_STATUS"].ToString().Equals("REJECTED BY RCM")))
        {
            btnACSubmit.Visible = true;
            btnACReject.Visible = true;
        }
        else
        {
            disableButons();
        }
    }

    protected void enableAddSequence()
    {
        trSelectSOR.Visible = true;
        trSelectSequence.Visible = true;
    }

    protected void btnBESubmit_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            ArrayList lstArrayInsertQueries = new ArrayList();
            ArrayList updateParamList = new ArrayList();
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "ACCEPTED BY BE");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                updateParamList.Add(param);
                foreach (GridViewRow row in gvAddedSequences.Rows)
                {
                    StringBuilder sbUpdateQry = new StringBuilder();
                    Dictionary<string, string> updateParam = new Dictionary<string, string>();
                    HiddenField hdbillID = new HiddenField();
                    hdbillID = (HiddenField)row.FindControl("hdbillID");
                    HiddenField hdTenderSorRefID = new HiddenField();
                    hdTenderSorRefID = (HiddenField)row.FindControl("hdTenderSorRefID");
                    HiddenField hdBillRunningSRNo = new HiddenField();
                    hdBillRunningSRNo = (HiddenField)row.FindControl("hdBillRunningSRNo");
                    HiddenField hdRAOverallBillNumber = new HiddenField();
                    hdRAOverallBillNumber = (HiddenField)row.FindControl("hdRAOverallBillNumber");
                    HiddenField hdBillStatus = new HiddenField();
                    hdBillStatus = (HiddenField)row.FindControl("hdBillStatus");
                    HiddenField hdBillSeqNo = new HiddenField();
                    hdBillSeqNo = (HiddenField)row.FindControl("hdBillSeqNo");
                    HiddenField hdContQty = new HiddenField();
                    hdContQty = (HiddenField)row.FindControl("hdContQty");

                    HiddenField hdActSeq = new HiddenField();
                    hdActSeq = (HiddenField)row.FindControl("hdActSeq");                    

                    sbUpdateQry.Append(@"UPDATE RAB_TENDER_BILL 
                            SET BENGG_IS_FROZEN=:BENGG_IS_FROZEN, 
                             BENGG_ADDED_ON=sysdate, 
                             BENGG_QTY=:BENGG_QTY, 
                             CONT_IS_FROZEN=:CONT_IS_FROZEN,
                             STATUS=:STATUS 
                             WHERE                               
                                BILL_ID=:BILL_ID
                               and BILL_ID is not null and ACT_SEQ=:ACT_SEQ ");
                    updateParam.Add("BENGG_IS_FROZEN", "Y");
                    updateParam.Add("CONT_IS_FROZEN", "Y");
                    updateParam.Add("BENGG_QTY", hdContQty.Value);
                    updateParam.Add("STATUS", "ACCEPTED by BE");
                    updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                    updateParam.Add("ACT_SEQ", hdActSeq.Value);

                    if (sbUpdateQry.Length > 0)
                    {
                        lstArrayInsertQueries.Add(sbUpdateQry.ToString());
                        updateParamList.Add(updateParam);
                    }
                }
            }

            int recordsAffected = 0;
            if (lstArrayInsertQueries.Count > 0)
            {
                string[] queryArray = new String[lstArrayInsertQueries.Count];
                Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[updateParamList.Count];
                for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                {
                    queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                    paramListArray[ii] = (Dictionary<string, string>)updateParamList[ii];
                }
                if (queryArray.Length > 0)
                    recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
            }
            if (recordsAffected > 0)
            {
                Common.Show("Items updated succesfully");

                //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                Hashtable htEmail = new Hashtable();
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                StringBuilder sbMessageAC = new StringBuilder();
                if (htEmail["AC"] != null)
                {
                    try
                    {
                        sbMessageAC = getMailMessage("AC");
                        if (htEmail["RCM"] != null)
                        {
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["AC"], "RAB - Bill approved by billing Engineer, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageAC.ToString(), (ArrayList)htEmail["RCM"]);
                        }
                        else
                        {
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["AC"], "RAB - Bill approved by billing Engineer, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageAC.ToString(), null);
                        }
                    }
                    catch (Exception err)
                    { }
                }

                //Inserting the remarks and audit history
                try
                {
                    insertActivity(txtRemarks.Text.ToString().Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Approved by BE", ViewState["TEND_SOR_REF_ID"].ToString());
                }
                catch (Exception err)
                { }

                bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                mpUpdateBill.Hide();
            }
            else
            {
                Common.Show("Error in updating,please try after some time");
            }
        }
    }

    protected void btnSubmitCont_Click(object sender, EventArgs e)
    {       
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                ArrayList lstArrayInsertQueries = new ArrayList();
                ArrayList insertParamList = new ArrayList();

                // Update RAB_TENDER_BILL_MST, RAB_TENDER_BILL and RAB_BILL_ACTIVITY            

                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "SUBMITTED TO BE");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(param);
                
                sbQuery.Clear();               
                
                sbQuery.Append(@"UPDATE  RAB_TENDER_BILL 
                                 SET CONT_IS_FROZEN=:CONT_IS_FROZEN,                                 
                                 STATUS=:STATUS 
                                 WHERE 
                                 BILL_ID=:BILL_ID
                                 and BILL_ID is not null 
                                 ");
                Dictionary<string, string> param2 = new Dictionary<string, string>();
                param2.Add("CONT_IS_FROZEN", "Y");
                param2.Add("STATUS", "SUBMITTED TO BE");
                param2.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(param2);

                clearParam();

                int recordsAffected = 0;
                if (lstArrayInsertQueries.Count > 0)
                {
                    string[] queryArray = new String[lstArrayInsertQueries.Count];
                    Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[insertParamList.Count];
                    for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                    {
                        queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                        paramListArray[ii] = (Dictionary<string, string>)insertParamList[ii];
                    }
                    if (queryArray.Length > 0)
                        recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
                }
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                if (recordsAffected > 0)
                {
                    //Generate Email alert to BE (Feedback no 1891 Email dated 15-Nov-2017)
                    Hashtable htEmail = new Hashtable();
                    htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                    StringBuilder sbMessageBE = new StringBuilder();
                    if (htEmail["BE"] != null)
                    {
                        try
                        {
                            sbMessageBE = getMailMessage("BE");
                            if (htEmail["AC"] != null)
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - New Bill submitted by contractor, Bill No: " + lblUpdatedBillNo.Text.ToString(), sbMessageBE.ToString(), (ArrayList)htEmail["AC"]);
                            }
                            else
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - New Bill submitted by contractor, Bill No: " + lblUpdatedBillNo.Text.ToString(), sbMessageBE.ToString(), null);
                            }
                        }
                        catch (Exception err){ }
                    }
                    //Inserting the audit history
                    try
                    {
                        insertActivity(txtRemarks.Text.Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Sent for approval to BE by Contractor", ViewState["TEND_SOR_REF_ID"].ToString());
                    }
                    catch (Exception err)
                    { }
                    Common.Show("Bill submitted to BE succesfully");
                    trItemsDetails.Visible = false;
                    btnSubmitCont.Visible = false;
                    mpUpdateBill.Hide();
                    bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                }
                else
                {
                    Common.Show("Error in adding,please try again");
                }
            }
            else
            {
                Common.Show("Error: Bill can not be added!");
                mpUpdateBill.Show();
            }
        
    }

    protected void btnBERejectPartial_Click(object sender, EventArgs e)
    {
        string referenceId = "";
        ArrayList lstArrayInsertQueries = new ArrayList();
        ArrayList updateParamList = new ArrayList();
        string runningSrNo = "";
        foreach (GridViewRow row in gvSORItems.Rows)
        {
            GridView gvSORSplits = (GridView)row.FindControl("gvSORSplits");
            foreach (GridViewRow rowChild in gvSORSplits.Rows)
            {
                HiddenField hdChildReferenceID = new HiddenField();
                hdChildReferenceID = (HiddenField)rowChild.FindControl("hdChildReferenceID");
                referenceId = hdChildReferenceID.Value;

                HiddenField hdChildSequenceNo = new HiddenField();
                hdChildSequenceNo = (HiddenField)rowChild.FindControl("hdChildSequenceNo");
                string sequenceNo = hdChildSequenceNo.Value;

                HiddenField hdActivityId = new HiddenField();
                hdActivityId = (HiddenField)rowChild.FindControl("hdActivityId");

                HiddenField hdRunSrNo = new HiddenField();
                hdRunSrNo = (HiddenField)rowChild.FindControl("hdRunSrNo");

                CheckBox chkBEReject = new CheckBox();
                chkBEReject = (CheckBox)rowChild.FindControl("chkBEReject");

                Label lblBEReject = new Label();
                lblBEReject = (Label)rowChild.FindControl("lblBEReject");

                Label lblVendQuantity = new Label();
                lblVendQuantity = (Label)rowChild.FindControl("lblVendQuantity");

                TextBox txtBEQuantity = new TextBox();
                txtBEQuantity = (TextBox)rowChild.FindControl("txtBEQuantity");

                HiddenField hdTenderSORId = new HiddenField();
                hdTenderSORId = (HiddenField)rowChild.FindControl("hdTenderSORId");
                ViewState["TEND_SOR_ID"] = hdTenderSORId.Value;

                StringBuilder sbUpdateQry = new StringBuilder();
                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                ViewState["REF_ID"] = referenceId;
                ViewState["SEQ_NO"] = sequenceNo;
                ViewState["ACTIVITY_SEQ"] = hdActivityId.Value.ToString();
                runningSrNo = hdRunSrNo.Value.ToString();
                //Update the value of Billing Engineer as per entry
                float BEQuantity = 0;
                if (txtBEQuantity.Text.Trim().Length > 0 && (float.Parse(txtBEQuantity.Text.Trim()) > 0 || float.Parse(txtBEQuantity.Text.Trim()) < 0))
                {
                    BEQuantity = float.Parse(txtBEQuantity.Text.Trim());
                }
                else if (lblVendQuantity.Text.Trim().Length > 0 && (float.Parse(lblVendQuantity.Text.Trim()) > 0 || float.Parse(lblVendQuantity.Text.Trim()) < 0))
                {
                    BEQuantity = float.Parse(lblVendQuantity.Text.Trim());
                }
                //Reject if checked else send approval to AC/RCM
                if (chkBEReject.Checked)
                {
                    sbUpdateQry.Append("UPDATE  RAB_TENDER_BILL ")
                         .Append(" SET CONT_IS_FROZEN=:CONT_IS_FROZEN, ")
                         .Append(" BENGG_ADDED_ON=sysdate, ")
                         .Append(" BENGG_QTY=:BENGG_QTY, ")
                         .Append(" STATUS=:STATUS, ")
                         .Append(" BENGG_IS_FROZEN=:BENGG_IS_FROZEN ")
                         .Append(" WHERE ")
                         .Append(" REF_ID=:REF_ID ")
                         .Append(" AND SEQ_NO=:SEQ_NO ")
                         .Append(" AND ACT_SEQ=:ACT_SEQ ")
                         .Append(" AND CONT_IS_FROZEN=:CONT_IS_FROZEN_YES ")
                         .Append(" AND RUN_SL_NO=:RUN_SL_NO ");
                    updateParam.Add("REF_ID", referenceId);
                    updateParam.Add("SEQ_NO", sequenceNo);
                    updateParam.Add("ACT_SEQ", hdActivityId.Value.ToString());
                    updateParam.Add("CONT_IS_FROZEN", "R");
                    updateParam.Add("CONT_IS_FROZEN_YES", "Y");
                    updateParam.Add("BENGG_QTY", "");
                    updateParam.Add("RUN_SL_NO", runningSrNo);
                    updateParam.Add("BENGG_IS_FROZEN", "N");
                    updateParam.Add("STATUS", "REJECTED by BE");
                }
                //Send for approval
                else if (chkBEReject.Checked == false)
                {
                    sbUpdateQry.Append("UPDATE RAB_TENDER_BILL ")
                         .Append(" SET BENGG_IS_FROZEN=:BENGG_IS_FROZEN, ")
                         .Append(" BENGG_ADDED_ON=sysdate, ")
                         .Append(" BENGG_QTY=:BENGG_QTY, ")
                         .Append(" STATUS=:STATUS ")
                         .Append(" WHERE ")
                         .Append(" REF_ID=:REF_ID ")
                         .Append(" AND SEQ_NO=:SEQ_NO ")
                         .Append(" AND ACT_SEQ=:ACT_SEQ ")
                         .Append(" AND BENGG_IS_FROZEN=:BENGG_IS_FROZEN_NO ")
                    .Append(" AND RUN_SL_NO=:RUN_SL_NO ");
                    updateParam.Add("REF_ID", referenceId);
                    updateParam.Add("SEQ_NO", sequenceNo);
                    updateParam.Add("ACT_SEQ", hdActivityId.Value.ToString());
                    updateParam.Add("BENGG_IS_FROZEN", "Y");
                    updateParam.Add("BENGG_IS_FROZEN_NO", "N");
                    updateParam.Add("BENGG_QTY", BEQuantity.ToString());
                    updateParam.Add("RUN_SL_NO", runningSrNo);
                    updateParam.Add("STATUS", "ACCEPTED by BE");
                }
                if (sbUpdateQry.Length > 0)
                {
                    lstArrayInsertQueries.Add(sbUpdateQry.ToString());
                    updateParamList.Add(updateParam);
                }
            }
        }

        int recordsAffected = 0;
        if (lstArrayInsertQueries.Count > 0)
        {
            string[] queryArray = new String[lstArrayInsertQueries.Count];
            Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[updateParamList.Count];
            for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
            {
                queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                paramListArray[ii] = (Dictionary<string, string>)updateParamList[ii];
            }
            if (queryArray.Length > 0)
                recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
        }
        if (recordsAffected > 0)
        {
            Common.Show(" Items updated succesfully");
            //Inserting the remarks and audit history
            try
            {
                insertRemarks(txtRemarks.Text.ToString().Trim(), runningSrNo);
                insertActivity(txtRemarks.Text.ToString().Trim(), runningSrNo, ViewState["RA_FINAL_BILL_NO"].ToString(), "Rejected partial by BE", ViewState["TEND_SOR_ID"].ToString());
            }
            catch (Exception err)
            { }

            bindSORItems(referenceId, ddJobNumber.SelectedValue, runningSrNo);
        }
        else
        {
            Common.Show("Error in updating,please try after some time");
        }
    }

    protected void btnBERejectAll_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                ArrayList lstArrayInsertQueries = new ArrayList();
                ArrayList insertParamList = new ArrayList();

                // Update RAB_TENDER_BILL_MST, RAB_TENDER_BILL and RAB_BILL_ACTIVITY            

                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "REJECTED BY BE");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(param);

                sbQuery.Clear();

                sbQuery.Append(@" UPDATE  RAB_TENDER_BILL 
                     SET CONT_IS_FROZEN=:CONT_IS_FROZEN, 
                     BENGG_ADDED_ON=sysdate, 
                     BENGG_QTY=:BENGG_QTY, 
                     STATUS=:STATUS, 
                     BENGG_IS_FROZEN=:BENGG_IS_FROZEN 
                     WHERE 
                     BILL_ID=:BILL_ID
                     and BILL_ID is not null  ");
                Dictionary<string, string> updateParam = new Dictionary<string, string>();               
                updateParam.Add("CONT_IS_FROZEN", "R");
                updateParam.Add("BENGG_QTY", "");
                updateParam.Add("STATUS", "REJECTED BY BE");
                updateParam.Add("BENGG_IS_FROZEN", "N");
                updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());               

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(updateParam);

                clearParam();

                int recordsAffected = 0;
                if (lstArrayInsertQueries.Count > 0)
                {
                    string[] queryArray = new String[lstArrayInsertQueries.Count];
                    Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[insertParamList.Count];
                    for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                    {
                        queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                        paramListArray[ii] = (Dictionary<string, string>)insertParamList[ii];
                    }
                    if (queryArray.Length > 0)
                        recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
                }
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                if (recordsAffected > 0)
                {
                    //Inserting the audit history
                    try
                    {
                        insertActivity(txtRemarks.Text.Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(),
                            "Rejected all by BE", ViewState["TEND_SOR_REF_ID"].ToString());
                    }
                    catch (Exception err)
                    { }
                                     
                    mpUpdateBill.Hide();
                    bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                    Common.Show("Bill rejected by BE succesfully"); 
                    //bindBillsAdded(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue);
                }
                else
                {
                    Common.Show("Error in rejecting the bill, please try again");
                }
            }
            else
            {
               mpUpdateBill.Show();
               Common.Show("Error: Bill can not be rejected!");
            }
        }       
    }

    protected void btnACSubmit_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            ArrayList lstArrayInsertQueries = new ArrayList();
            ArrayList updateParamList = new ArrayList();
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "ACCEPTED BY AC");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                updateParamList.Add(param);
                foreach (GridViewRow row in gvAddedSequences.Rows)
                {
                    StringBuilder sbUpdateQry = new StringBuilder();
                    Dictionary<string, string> updateParam = new Dictionary<string, string>();
                    HiddenField hdbillID = new HiddenField();
                    hdbillID = (HiddenField)row.FindControl("hdbillID");
                    HiddenField hdTenderSorRefID = new HiddenField();
                    hdTenderSorRefID = (HiddenField)row.FindControl("hdTenderSorRefID");
                    HiddenField hdBillRunningSRNo = new HiddenField();
                    hdBillRunningSRNo = (HiddenField)row.FindControl("hdBillRunningSRNo");
                    HiddenField hdRAOverallBillNumber = new HiddenField();
                    hdRAOverallBillNumber = (HiddenField)row.FindControl("hdRAOverallBillNumber");
                    HiddenField hdBillStatus = new HiddenField();
                    hdBillStatus = (HiddenField)row.FindControl("hdBillStatus");
                    HiddenField hdBillSeqNo = new HiddenField();
                    hdBillSeqNo = (HiddenField)row.FindControl("hdBillSeqNo");
                    HiddenField hdContQty = new HiddenField();
                    hdContQty = (HiddenField)row.FindControl("hdContQty");
                    
                    HiddenField hdActSeq = new HiddenField();
                    hdActSeq = (HiddenField)row.FindControl("hdActSeq");

                    sbUpdateQry.Append(@"UPDATE RAB_TENDER_BILL 
                     SET AC_IS_FROZEN=:AC_IS_FROZEN, 
                     AC_ADDED_ON=sysdate, 
                     AC_QTY=:AC_QTY,STATUS=:STATUS 
                     WHERE BILL_ID=:BILL_ID
                               and BILL_ID is not null and ACT_SEQ=:ACT_SEQ");
                    updateParam.Add("AC_IS_FROZEN", "Y");                   
                    updateParam.Add("AC_QTY", hdContQty.Value);                   
                    updateParam.Add("STATUS", "ACCEPTED BY AC");
                    updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                    updateParam.Add("ACT_SEQ", hdActSeq.Value);

                    if (sbUpdateQry.Length > 0)
                    {
                        lstArrayInsertQueries.Add(sbUpdateQry.ToString());
                        updateParamList.Add(updateParam);
                    }
                }
            }

            int recordsAffected = 0;
            if (lstArrayInsertQueries.Count > 0)
            {
                string[] queryArray = new String[lstArrayInsertQueries.Count];
                Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[updateParamList.Count];
                for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                {
                    queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                    paramListArray[ii] = (Dictionary<string, string>)updateParamList[ii];
                }
                if (queryArray.Length > 0)
                    recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
            }
            if (recordsAffected > 0)
            {
                Common.Show("Items approved succesfully");

                //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                Hashtable htEmail = new Hashtable();
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                StringBuilder sbMessageRCM = new StringBuilder();
                if (htEmail["RCM"] != null)
                {
                    try
                    {
                        sbMessageRCM = getMailMessage("RCM");
                        if (htEmail["AC"] != null)
                        {
                            //To RCM and CC email to both AC and BE
                            ArrayList lstCCEmails = new ArrayList();
                            lstCCEmails.AddRange((ArrayList)htEmail["AC"]);
                            if (htEmail["BE"] != null)
                                lstCCEmails.AddRange((ArrayList)htEmail["BE"]);
                            //Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - Bill approved by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM.ToString(), (ArrayList)htEmail["AC"]);
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - Bill approved by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM.ToString(), lstCCEmails);
                        }
                        else
                        {
                            //To RCM and CC email to both AC and BE
                            ArrayList lstCCEmails = new ArrayList();
                            if (htEmail["BE"] != null)
                                lstCCEmails.AddRange((ArrayList)htEmail["BE"]);
                            //Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - Bill approved by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM.ToString(), null);
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["RCM"], "RAB - Bill approved by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM.ToString(), lstCCEmails);
                        }
                    }
                    catch (Exception err)
                    { }
                }
                //Inserting the remarks and audit history
                try
                {
                    insertActivity(txtRemarks.Text.ToString().Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Action taken by AC, bill accepted", ViewState["TEND_SOR_REF_ID"].ToString());
                }
                catch (Exception err)
                { }
                bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                enableActionButtons();
            }
            else
            {
                Common.Show("Error in updating,please try after some time");
            }
        }
    }

    protected void btnACReject_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                ArrayList lstArrayInsertQueries = new ArrayList();
                ArrayList insertParamList = new ArrayList();

                // Update RAB_TENDER_BILL_MST, RAB_TENDER_BILL and RAB_BILL_ACTIVITY            

                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "REJECTED BY AC");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(param);

                sbQuery.Clear();

                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                sbQuery.Append(@"UPDATE  RAB_TENDER_BILL 
                                     SET  
                                BENGG_IS_FROZEN=:BENGG_IS_FROZEN, 
                                AC_ADDED_ON=sysdate,
                                AC_QTY=:AC_QTY,STATUS=:STATUS 
                                 WHERE 
                                 BILL_ID=:BILL_ID
                                 and BILL_ID is not null ");

                updateParam.Add("BENGG_IS_FROZEN", "R");
                updateParam.Add("AC_QTY", "0");
                updateParam.Add("STATUS", "REJECTED BY AC");
                updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(updateParam);

                clearParam();

                int recordsAffected = 0;
                if (lstArrayInsertQueries.Count > 0)
                {
                    string[] queryArray = new String[lstArrayInsertQueries.Count];
                    Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[insertParamList.Count];
                    for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                    {
                        queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                        paramListArray[ii] = (Dictionary<string, string>)insertParamList[ii];
                    }
                    if (queryArray.Length > 0)
                        recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
                }
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                if (recordsAffected > 0)
                {
                    //Inserting the audit history
                    Common.Show("Items rejected succesfully");

                    //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                    Hashtable htEmail = new Hashtable();

                    htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                    StringBuilder sbMessageBE_Reject = new StringBuilder();
                    if (htEmail["BE"] != null)
                    {
                        try
                        {
                            sbMessageBE_Reject = getMailMessage("BE");
                            if (htEmail["AC"] != null)
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - Bill rejected by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageBE_Reject.ToString(), (ArrayList)htEmail["AC"]);
                            }
                            else
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - Bill rejected by Area Cordinator, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageBE_Reject.ToString(), null);
                            }
                        }
                        catch (Exception err)
                        { }
                    }

                    //Inserting the remarks and audit history
                    try
                    {
                        insertActivity(txtRemarks.Text.ToString().Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Action taken by AC, bill rejected", ViewState["TEND_SOR_REF_ID"].ToString());
                    }
                    catch (Exception err)
                    { }

                    mpUpdateBill.Hide();
                    bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                }
                else
                {
                    Common.Show("Error: Bill can not be rejected!");
                    mpUpdateBill.Show();
                }
            }
        }
    }

    protected void insertRemarks(string remarks, string runningSrNo)
    {
        StringBuilder sbRemarksQuery = new StringBuilder();
        Dictionary<string, string> remarksParam = new Dictionary<string, string>();
        if (remarks.Trim().Length > 0)
        {
            sbRemarksQuery.Append("INSERT INTO RAB_TENDER_BILL_REMARKS ")
                .Append(" (REF_ID, SEQ_NO, REMARKS,  REMARKS_BY, ROLE, ACTIVITY_SEQ,RUN_SL_NO) ")
                .Append(" VALUES (")
                .Append(" :REF_ID, :SEQ_NO, :REMARKS, :REMARKS_BY, :ROLE, :ACTIVITY_SEQ, :RUN_SL_NO")
                .Append(" )");
            remarksParam.Add("REF_ID", ViewState["REF_ID"].ToString());
            remarksParam.Add("SEQ_NO", ViewState["SEQ_NO"].ToString());
            remarksParam.Add("ACTIVITY_SEQ", ViewState["ACTIVITY_SEQ"].ToString());
            remarksParam.Add("REMARKS", remarks);
            remarksParam.Add("REMARKS_BY", Session["USERID"].ToString());
            remarksParam.Add("ROLE", Session["ROLE"].ToString());
            remarksParam.Add("RUN_SL_NO", runningSrNo);

            int recordsInserted = objDB.executeNonQuery(sbRemarksQuery.ToString(), remarksParam);
        }
    }

    //Adding the activities history by users after clicking on the buttons
    protected void insertActivity(string remarks, string runningSrNo, string RABillNo, string activityDesc, string tenderSORId)
    {
        StringBuilder sbActivityQuery = new StringBuilder();
        Dictionary<string, string> remarksParam = new Dictionary<string, string>();
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        sbActivityQuery.Append("INSERT INTO RAB_BILL_ACTIVITY ")
            .Append(" (JOB_NO, SUB_JOB_NO, TENDER_NO, RUN_SL_NO, RA_BLL_NO, REMARKS,  REMARKS_BY, ROLE, ACTIVITY_DESC,TEND_SOR_ID) ")
            .Append(" VALUES (")
            .Append(" :JOB_NO, :SUB_JOB_NO, :TENDER_NO, :RUN_SL_NO, :RA_BLL_NO, :REMARKS, :REMARKS_BY, :ROLE, :ACTIVITY_DESC,:TEND_SOR_ID")
            .Append(" )");
        remarksParam.Add("JOB_NO", ddJobNumber.SelectedValue);
        remarksParam.Add("SUB_JOB_NO", "");
        remarksParam.Add("TENDER_NO", strArray[0]);
        remarksParam.Add("REMARKS", remarks);
        remarksParam.Add("REMARKS_BY", Session["USERID"].ToString());
        remarksParam.Add("ROLE", Session["ROLE"].ToString());
        remarksParam.Add("RUN_SL_NO", runningSrNo);
        remarksParam.Add("RA_BLL_NO", RABillNo);
        remarksParam.Add("ACTIVITY_DESC", activityDesc);
        remarksParam.Add("TEND_SOR_ID", tenderSORId);
        int recordsInserted = objDB.executeNonQuery(sbActivityQuery.ToString(), remarksParam);
    }

    protected void btnRCMSubmit_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            ArrayList lstArrayInsertQueries = new ArrayList();
            ArrayList updateParamList = new ArrayList();
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "ACCEPTED BY RCM");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                updateParamList.Add(param);
                foreach (GridViewRow row in gvAddedSequences.Rows)
                {
                    StringBuilder sbUpdateQry = new StringBuilder();
                    Dictionary<string, string> updateParam = new Dictionary<string, string>();
                    HiddenField hdbillID = new HiddenField();
                    hdbillID = (HiddenField)row.FindControl("hdbillID");
                    HiddenField hdTenderSorRefID = new HiddenField();
                    hdTenderSorRefID = (HiddenField)row.FindControl("hdTenderSorRefID");
                    HiddenField hdBillRunningSRNo = new HiddenField();
                    hdBillRunningSRNo = (HiddenField)row.FindControl("hdBillRunningSRNo");
                    HiddenField hdRAOverallBillNumber = new HiddenField();
                    hdRAOverallBillNumber = (HiddenField)row.FindControl("hdRAOverallBillNumber");
                    HiddenField hdBillStatus = new HiddenField();
                    hdBillStatus = (HiddenField)row.FindControl("hdBillStatus");
                    HiddenField hdBillSeqNo = new HiddenField();
                    hdBillSeqNo = (HiddenField)row.FindControl("hdBillSeqNo");
                    HiddenField hdContQty = new HiddenField();
                    hdContQty = (HiddenField)row.FindControl("hdContQty");

                    HiddenField hdActSeq = new HiddenField();
                    hdActSeq = (HiddenField)row.FindControl("hdActSeq");

                    sbUpdateQry.Append(@"UPDATE RAB_TENDER_BILL  
                            SET RCM_IS_FROZEN=:RCM_IS_FROZEN,  
                                RCM_ADDED_ON=sysdate, 
                                RCM_QTY=:RCM_QTY,
                                STATUS=:STATUS                     
                           WHERE BILL_ID=:BILL_ID
                               and BILL_ID is not null and ACT_SEQ=:ACT_SEQ ");                   
                   
                    updateParam.Add("RCM_IS_FROZEN", "Y");
                    updateParam.Add("RCM_QTY", hdContQty.Value);                    
                    updateParam.Add("STATUS", "ACCEPTED BY RCM");
                    updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                    updateParam.Add("ACT_SEQ", hdActSeq.Value);                    

                    if (sbUpdateQry.Length > 0)
                    {
                        lstArrayInsertQueries.Add(sbUpdateQry.ToString());
                        updateParamList.Add(updateParam);
                    }
                }
            }

            int recordsAffected = 0;
            if (lstArrayInsertQueries.Count > 0)
            {
                string[] queryArray = new String[lstArrayInsertQueries.Count];
                Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[updateParamList.Count];
                for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                {
                    queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                    paramListArray[ii] = (Dictionary<string, string>)updateParamList[ii];
                }
                if (queryArray.Length > 0)
                    recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
            }

            string[] strArray = ddTenderNo.SelectedValue.Split('~');

            if (recordsAffected > 0)
            {
                Common.Show("Bill approved succesfully");
                //Generate Email alert to BE, CC to AC (Feedback no 1891 Email dated 15-Nov-2017)
                Hashtable htEmail = new Hashtable();               
                htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                StringBuilder sbMessageRCM_Approved = new StringBuilder();
                if (htEmail["BE"] != null)
                {
                    try
                    {
                        sbMessageRCM_Approved = getMailMessage("RCM_Approved");
                        if (htEmail["AC"] != null && htEmail["RCM"] != null)
                        {
                            //To BE and CC email to both AC and RCM
                            ArrayList lstCCEmails = new ArrayList();
                            lstCCEmails.AddRange((ArrayList)htEmail["RCM"]);
                            lstCCEmails.AddRange((ArrayList)htEmail["AC"]);                       
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - Bill approved by RCM, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM_Approved.ToString(), lstCCEmails);
                        }
                        else
                        {
                            Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - Bill approved by RCM, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageRCM_Approved.ToString(), (ArrayList)htEmail["RCM"]);
                        }
                    }
                    catch (Exception err)
                    { }
                }

                //Inserting the remarks and audit history
                try
                {
                    insertActivity(txtRemarks.Text.ToString().Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Bill approved by RCM", ViewState["TEND_SOR_REF_ID"].ToString());
                }
                catch (Exception err)
                { }                
                bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                enableActionButtons();
            }
            else
            {
                Common.Show("Error in updating,please try after some time");
            }
        } 
    }

    protected void btnRCMRejectAll_Click(object sender, EventArgs e)
    {
        if (txtRemarks.Text.Trim().Length == 0)
        {
            Common.Show("Remarks is mandatory!");
            mpUpdateBill.Show();
        }
        else
        {
            if (ViewState["RA_BILL_ID"] != null && ViewState["RA_BILL_ID"].ToString().Length > 0)
            {
                ArrayList lstArrayInsertQueries = new ArrayList();
                ArrayList insertParamList = new ArrayList();

                // Update RAB_TENDER_BILL_MST, RAB_TENDER_BILL and RAB_BILL_ACTIVITY            

                clearParam();
                sbQuery.Append(@"UPDATE RAB_TENDER_BILL_MST SET BILL_STATUS=:BILL_STATUS WHERE ID=:BILL_ID");
                Dictionary<string, string> param = new Dictionary<string, string>();
                param.Add("BILL_STATUS", "REJECTED BY RCM");
                param.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());

                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(param);

                sbQuery.Clear();

                Dictionary<string, string> updateParam = new Dictionary<string, string>();
                sbQuery.Append(@"UPDATE  RAB_TENDER_BILL 
                     SET RCM_IS_FROZEN=:RCM_IS_FROZEN, 
                     AC_IS_FROZEN=:AC_IS_FROZEN, 
                     RCM_ADDED_ON=sysdate, 
                     RCM_QTY=:RCM_QTY, 
                     STATUS=:STATUS 
                    WHERE                     
                         BILL_ID=:BILL_ID
                       and BILL_ID is not null ");                
               
                updateParam.Add("AC_IS_FROZEN", "R");
                updateParam.Add("RCM_IS_FROZEN", "N");
                updateParam.Add("RCM_QTY", "0");
                updateParam.Add("STATUS", "REJECTED BY RCM");
                updateParam.Add("BILL_ID", ViewState["RA_BILL_ID"].ToString());
                lstArrayInsertQueries.Add(sbQuery.ToString());
                insertParamList.Add(updateParam);

                clearParam();

                int recordsAffected = 0;
                if (lstArrayInsertQueries.Count > 0)
                {
                    string[] queryArray = new String[lstArrayInsertQueries.Count];
                    Dictionary<string, string>[] paramListArray = new Dictionary<string, string>[insertParamList.Count];
                    for (int ii = 0; ii < lstArrayInsertQueries.Count; ii++)
                    {
                        queryArray[ii] = lstArrayInsertQueries[ii].ToString();
                        paramListArray[ii] = (Dictionary<string, string>)insertParamList[ii];
                    }
                    if (queryArray.Length > 0)
                        recordsAffected = objDB.executeTransaction(queryArray, paramListArray);
                }
                string[] strArray = ddTenderNo.SelectedValue.Split('~');
                if (recordsAffected > 0)
                {
                    Common.Show("Items rejected succesfully");
                    //Inserting the remarks and audit history

                    //Generate Email alert to AC (Feedback no 1891 Email dated 15-Nov-2017)
                    Hashtable htEmail = new Hashtable();                    
                    htEmail = objDB.getJobEmails(ddJobNumber.SelectedValue, strArray[0], strArray[1]);
                    StringBuilder sbMessageAC_Reject = new StringBuilder();
                    if (htEmail["RCM"] != null)
                    {
                        try
                        {
                            sbMessageAC_Reject = getMailMessage("AC");
                            if (htEmail["AC"] != null)
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["AC"], "RAB - Bill rejected by RCM, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageAC_Reject.ToString(), (ArrayList)htEmail["RCM"]);
                            }
                            else
                            {
                                Common.SendRABMail("itsapp@eil.co.in", (ArrayList)htEmail["BE"], "RAB - Bill rejected by RCM, Bill No: " + ViewState["RA_FINAL_BILL_NO"].ToString(), sbMessageAC_Reject.ToString(), (ArrayList)htEmail["RCM"]);
                            }
                        }
                        catch (Exception err)
                        { }
                    }

                    try
                    {
                        insertActivity(txtRemarks.Text.ToString().Trim(), ViewState["RUN_SL_NO"].ToString(), ViewState["RA_FINAL_BILL_NO"].ToString(), "Action taken by RCM", ViewState["TEND_SOR_REF_ID"].ToString());                     
                    }
                    catch (Exception err)
                    { }

                    mpUpdateBill.Hide();
                    bindBills(ddJobNumber.SelectedValue, strArray[0], strArray[1]);                   
                    
                    //Inserting the audit history
                    Common.Show("Items rejected succesfully");                    
                }
                else
                {
                    Common.Show("Error: Bill can not be rejected!");
                    mpUpdateBill.Show();
                }
            }
        }       
    }

    protected string getTotalActivityCompleted(string refId, string seqNo, string tenderSorId)
    {
        string qty = "0";
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.Append("select nvl(sum(rcm_qty),0) totalQty ")
        .Append(" from rab_tender_bill a where REF_ID=:REF_ID  ")
        .Append(" and SEQ_NO=:SEQ_NO and TEND_SOR_ID=:TEND_SOR_ID ")
        .Append(" and act_seq in ")
        .Append(" (select activity_id from rab_tender_details ")
        .Append(" where REF_ID=a.ref_id    and SEQ_NO=a.SEQ_NO) ")
        .Append(" and RCM_IS_FROZEN='Y' ")
        .Append("group by REF_ID, SEQ_NO, TEND_SOR_ID");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("REF_ID", refId);
        paramList.Add("SEQ_NO", seqNo);
        paramList.Add("TEND_SOR_ID", tenderSorId);
        qty = objDB.executeScalar(sbQuery.ToString(), paramList);
        return qty;
    }

    protected void enableActionButtons()
    {
        //int vendorPendingCount = 0;
        //int BEPendingCount = 0;
        //int ACPendingCount = 0;
        //int RCMPendingCount = 0;
        foreach (GridViewRow row in gvSORItems.Rows)
        {
            GridView gvSORSplits = (GridView)row.FindControl("gvSORSplits");
            foreach (GridViewRow rowChild in gvSORSplits.Rows)
            {
                HiddenField hdContractorFrozen = new HiddenField();
                hdContractorFrozen = (HiddenField)rowChild.FindControl("hdContractorFrozen");
                string ContractorFrozen = hdContractorFrozen.Value;

                HiddenField hdBEFrozen = new HiddenField();
                hdBEFrozen = (HiddenField)rowChild.FindControl("hdBEFrozen");

                HiddenField hdACFrozen = new HiddenField();
                hdACFrozen = (HiddenField)rowChild.FindControl("hdACFrozen");

                HiddenField hdRCMFrozen = new HiddenField();
                hdRCMFrozen = (HiddenField)rowChild.FindControl("hdRCMFrozen");

                HiddenField hdRunSrNo = new HiddenField();
                hdRunSrNo = (HiddenField)rowChild.FindControl("hdRunSrNo");

                HiddenField hdTenderSORId = new HiddenField();
                hdTenderSORId = (HiddenField)rowChild.FindControl("hdTenderSORId");


                HiddenField hdChildSequenceNo = new HiddenField();
                hdChildSequenceNo = (HiddenField)rowChild.FindControl("hdChildSequenceNo");

                if ("VEND".Equals(Session["ROLE"].ToString()))
                {
                    if (hdContractorFrozen.Value.Equals("R") && ((hdBEFrozen.Value.Equals("N") && hdACFrozen.Value.Equals("N") && hdRCMFrozen.Value.Equals("N"))))
                    {
                        disableButons();
                        btnSubmitCont.Visible = true;
                        return;
                    }
                }
                else if ("BE".Equals(Session["ROLE"].ToString()))
                {
                    // if ((hdContractorFrozen.Value.Equals("Y") || hdContractorFrozen.Value.Equals("R")) && hdBEFrozen.Value.Equals("N") && hdACFrozen.Value.Equals("N") && hdRCMFrozen.Value.Equals("N"))
                    //if ( hdContractorFrozen.Value.Equals("R") && hdBEFrozen.Value.Equals("N") && hdACFrozen.Value.Equals("N") && hdRCMFrozen.Value.Equals("N"))
                    if (hdContractorFrozen.Value.Equals("R") || hdContractorFrozen.Value.Equals("N"))
                    {
                        disableButons();
                        // btnBESubmit.Visible = true;
                        // btnBERejectAll.Visible = true;
                        // btnBERejectPartial.Visible = true;
                        return;
                    }
                    //else if (hdContractorFrozen.Value.Equals("Y") && hdBEFrozen.Value.Equals("N") && hdACFrozen.Value.Equals("N") && hdRCMFrozen.Value.Equals("N"))
                    else if (hdContractorFrozen.Value.Equals("Y") && (hdBEFrozen.Value.Equals("N") || hdBEFrozen.Value.Equals("R")))
                    {
                        disableButons();
                        btnBESubmit.Visible = true;
                        btnBERejectAll.Visible = true;
                        btnBERejectPartial.Visible = true;
                        trRemarks.Visible = true;
                        return;
                    }
                    // Commented on 23-Jun-2017 because loop was not completing
                    //else
                    //{
                    //    disableButons();                        
                    //    return;
                    //}
                }
                else if ("AC".Equals(Session["ROLE"].ToString()))
                {
                    if (hdBEFrozen.Value.Equals("R") && hdBEFrozen.Value.Equals("N"))
                    {
                        disableButons();
                        return;
                    }
                    else if (hdBEFrozen.Value.Equals("Y") && (hdACFrozen.Value.Equals("N") || hdACFrozen.Value.Equals("R")))
                    {
                        disableButons();
                        btnACSubmit.Visible = true;
                        btnACReject.Visible = true;
                        trRemarks.Visible = true;
                        return;
                    }
                    // Commented on 23-Jun-2017 because loop was not completing
                    //else
                    //{
                    //    disableButons();
                    //    return;
                    //}
                }
                else if ("RCM".Equals(Session["ROLE"].ToString()))
                {
                    if (hdACFrozen.Value.Equals("R") && hdACFrozen.Value.Equals("N"))
                    {
                        disableButons();
                        return;
                    }
                    else if (hdACFrozen.Value.Equals("Y") && hdRCMFrozen.Value.Equals("N"))
                    {
                        disableButons();
                        btnRCMSubmit.Visible = true;
                        //btnRCMGenerateBill.Visible = true;
                        btnRCMRejectAll.Visible = true;
                        trRemarks.Visible = true;
                        return;
                    }
                    // Commented on 23-Jun-2017 because loop was not completing
                    //else 
                    //{
                    //    disableButons();                        
                    //    return;
                    //}
                }
                else
                {
                    disableButons();
                    
                }
            }
        }

    }
    public StringBuilder getMailMessage(string mailType)
    {
        StringBuilder sbMessage = new StringBuilder();
        sbMessage.Append(@"Dear Sir/Ma'am <br/<br/>");
        if ("BE".Equals(mailType))
        {
            sbMessage.Append(@"A new bill has been submitted by Contractor for your action/information.<br/<br/>");
        }
        else if ("AC".Equals(mailType))
        {
            sbMessage.Append(@"A bill has been submitted by Billing Engineer for your action/information.<br/<br/>");
        }
        else if ("RCM".Equals(mailType))
        {
            sbMessage.Append(@"A bill has been submitted by Area Cordinator for your action/information.<br/<br/>");
        }
        else if ("RCM_Approved".Equals(mailType))
        {
            sbMessage.Append(@"A bill has been approved by RCM for your action/information.<br/<br/>");
        }
        else
        {
            sbMessage.Append(@"A bill has been submitted for your action/information.<br/<br/>");
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