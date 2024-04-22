using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;
using System.Collections;
using ClosedXML.Excel;
using System.Data;
using System.IO;

public partial class RAB_Msheet_View : System.Web.UI.Page
{
    dbFunction objDB = new dbFunction();
    Double totalMeasurementQty = 0;
    string referenceId, sequenceNumber, activityId, tenderSorId, runningSrNo, jobno, tNo;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (Session["USERID"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        if (!IsPostBack)
        {
            if (Session["USERID"] != null && (Session["ROLE"] != null))
            {
                //Get the value from Request and bind the measurement sheet detail                
                referenceId=Request.QueryString["refId"].ToString();
                sequenceNumber=Request.QueryString["seqNo"].ToString();
                lblSeqNumber.Text = sequenceNumber;
                activityId=Request.QueryString["actId"].ToString();
                tenderSorId=Request.QueryString["tendSorId"].ToString();
                runningSrNo=Request.QueryString["RSno"].ToString();
                jobno = Request.QueryString["jobno"].ToString();
                tNo = Request.QueryString["tNo"].ToString();
                bindMeasurementSheet(referenceId, sequenceNumber, activityId, tenderSorId, runningSrNo);
                //Setting Hidden values
                hdreferenceId.Value=referenceId;
                hdactivityId.Value=activityId;
                hdtenderSorId.Value=tenderSorId;
                hdrunningSrNo.Value = runningSrNo;
                hdJobNumber.Value = jobno;
                hdTenderNo.Value = tNo;
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void bindMeasurementSheet_08052017(string referenceId, string sequenceNumber, string activityId, string tenderSorId, string runningSrNo)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select ID,REF_ID, SEQ_NO, TENDER_SOR_ID, ACT_SEQ, RUN_SL_NO, ")
        .Append(" RUN_SL_DATE, ACTIVTY_DESC, UNIT, REMARKS, QUANTITY, LENGTH, ")
        .Append(" BREADTH, HEIGHT, UNIT4, UNIT5, UNIT6, unit_Weight, CALCULATED_QTY, ACTIVITY_ORDER,CALCULATED_QTY ")
        .Append(" FROM RAB_TENDER_MSHEET ")
        .Append(" WHERE ")
        .Append(" REF_ID=:REF_ID ")
        .Append(" AND SEQ_NO=:SEQ_NO ")
        .Append(" AND ACT_SEQ=:ACT_SEQ ")
        .Append(" AND TENDER_SOR_ID=:TENDER_SOR_ID ");
        if (runningSrNo.Length > 0)
        {
            sbQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", runningSrNo);
        }       
        sbQuery.Append(" ORDER BY id");

        paramList.Add("REF_ID", referenceId);
        paramList.Add("SEQ_NO", sequenceNumber);
        paramList.Add("ACT_SEQ", activityId);
        paramList.Add("TENDER_SOR_ID", tenderSorId);

        objDB.bindGridView(gvMeasurementSheet, sbQuery.ToString(), paramList);
        
    }

    protected void bindMeasurementSheet(string referenceId, string sequenceNumber, string activityId, string tenderSorId, string runningSrNo)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append("select ID,REF_ID, SEQ_NO, TENDER_SOR_ID, ACT_SEQ, RUN_SL_NO, ")
        .Append(" RUN_SL_DATE, ACTIVTY_DESC, UNIT, REMARKS, round(QUANTITY,4) QUANTITY, round(LENGTH,4) LENGTH, ")
        .Append(" round(BREADTH,4) BREADTH, round(HEIGHT,4) HEIGHT, round(UNIT4,4) UNIT4, round(UNIT5,4) UNIT5, round(UNIT6,4) UNIT6, round(unit_Weight,4) unit_Weight, round(CALCULATED_QTY,4) CALCULATED_QTY, ACTIVITY_ORDER,round(CALCULATED_QTY,4) ")
        .Append(" FROM RAB_TENDER_MSHEET ")
        .Append(" WHERE ")
        .Append(" REF_ID=:REF_ID ")
        .Append(" AND SEQ_NO=:SEQ_NO ")
        .Append(" AND ACT_SEQ=:ACT_SEQ ")
        .Append(" AND TENDER_SOR_ID=:TENDER_SOR_ID ");
        if (runningSrNo.Length > 0)
        {
            sbQuery.Append(" AND RUN_SL_NO=:RUN_SL_NO ");
            paramList.Add("RUN_SL_NO", runningSrNo);
        }
        sbQuery.Append(" ORDER BY id");

        paramList.Add("REF_ID", referenceId);
        paramList.Add("SEQ_NO", sequenceNumber);
        paramList.Add("ACT_SEQ", activityId);
        paramList.Add("TENDER_SOR_ID", tenderSorId);

        objDB.bindGridView(gvMeasurementSheet, sbQuery.ToString(), paramList);

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


            if (!Session["ROLE"].Equals("VEND"))
            {
                e.Row.Cells[10].Visible = false;
            }
            
        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {
            Label lblTotalQty = (Label)e.Row.FindControl("lblTotalQty");
            lblTotalQty.Text = totalMeasurementQty.ToString();
        }
    }

    
    protected void btnSimpleClose_Click(object sender, EventArgs e)
    {
        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
    } 
   
}
