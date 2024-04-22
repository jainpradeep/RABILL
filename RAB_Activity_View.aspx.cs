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

public partial class RAB_Activity_View : System.Web.UI.Page
{
    dbFunction objDB = new dbFunction();
    string runningSrNo, jobno, tNo, billNo,tenderReferenceId;
   
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
                jobno=Request.QueryString["jobNo"].ToString();
                tNo=Request.QueryString["tenderNo"].ToString();
                runningSrNo=Request.QueryString["rsno"].ToString();                
                billNo = Request.QueryString["raBillNo"].ToString();
                tenderReferenceId = Request.QueryString["tendRefId"].ToString();


                bindActivities(jobno, tNo, runningSrNo, billNo, tenderReferenceId);                
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void bindActivities(string jobno, string tNo, string runningSrNo, string billNo, string tenderReferenceId)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append(@"SELECT JOB_NO, SUB_JOB_NO, TENDER_NO, RUN_SL_NO, 
                        RA_BLL_NO, REMARKS, REMARKS_ON, REMARKS_BY, ROLE, ACTIVITY_DESC 
                        FROM RAB_BILL_ACTIVITY
                       WHERE 
                        JOB_NO=:JOB_NO AND TENDER_NO=:TENDER_NO
                        AND RUN_SL_NO=:RUN_SL_NO AND 
                        RA_BLL_NO=:RA_BLL_NO  
                        and TEND_SOR_ID=:TEND_SOR_ID        
                       ORDER BY REMARKS_ON");

        paramList.Add("JOB_NO", jobno);
        paramList.Add("TENDER_NO", tNo);
        paramList.Add("RUN_SL_NO", runningSrNo);
        paramList.Add("RA_BLL_NO", billNo);
        paramList.Add("TEND_SOR_ID", tenderReferenceId);        
      
        objDB.bindGridView(gvActivities, sbQuery.ToString(), paramList);        
    }
    
    protected void btnSimpleClose_Click(object sender, EventArgs e)
    {
        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "Close", "window.close()", true);
    } 
   
}
