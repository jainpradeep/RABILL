using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;

public partial class RA_Edit_MOP : System.Web.UI.Page
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
            if (Session["USERID"] != null && Session["ROLE"] != null && "RCM".Equals(Session["ROLE"].ToString()) && Request.QueryString["id"]!=null)
            {
                bindMOP(Common.Decrypt(Request.QueryString["id"].ToString()));
                hd_MOPID.Value = Common.Decrypt(Request.QueryString["id"].ToString());
            }            
            else 
            {
                Response.Redirect("Login.aspx");
            }
        }
    }



    protected void bindMOP(string mopId)
    {        
     StringBuilder sbQuery = new StringBuilder();
              sbQuery.Append(@"select a.id mopid,b.id headingid,C.ID subheadingid, B.HEADING_DESC,C.SUB_HEADING_DESC,A.UPTO_PREV_BILL_AMT,A.SINCE_PREV_BILL_AMT,A.TOTAL_UPTO_DATE from RAB_MOP_BILL_DTL a,
                RAB_MOP_HEADING_DTL b,
                RAB_MOP_SUB_HEADINGS_DTL c
                 where a.id=:MOPID
                 and A.HEADING_ID=B.ID
                 and A.SUB_HEADING_ID=C.ID
                 and B.ID=C.HEADING_ID
                 order by B.HEADING_ORDER,C.SUB_HEADING_ORDER");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("MOPID", mopId);
        objDB.bindGridView(gvTenders, sbQuery.ToString(), paramList);
    }    

    protected void gvTenders_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvTenders.EditIndex = e.NewEditIndex;
        bindMOP(hd_MOPID.Value);
    }

    protected void gvTenders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvTenders.EditIndex = -1;
        bindMOP(hd_MOPID.Value);
    }

    protected void gvTenders_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int index = gvTenders.EditIndex;
        GridViewRow row = gvTenders.Rows[index];
        TextBox txtUptoPrevBill = (TextBox)row.FindControl("txtUptoPrevBill");
        TextBox txtSincePrevBill = (TextBox)row.FindControl("txtSincePrevBill");
        
        HiddenField hdMOPID = (HiddenField)row.FindControl("hdMOPID");
        HiddenField hdHeadingID = (HiddenField)row.FindControl("hdHeadingID");
        HiddenField hdTenderNo = (HiddenField)row.FindControl("hdTenderNo");
        HiddenField hdSubHeadingID = (HiddenField)row.FindControl("hdSubHeadingID");
        Label lblSincePrevBill = (Label)row.FindControl("lblSincePrevBill");
        
        string error = "";
    
        float pvalue = 0;

        //float.TryParse(txtUptoPrevBill.Text.ToString(), out pvalue);
        //{
        //        if (pvalue == 0)
        //        {
        //            error += "MOP Value is not correct.\\n";
        //            Common.Show(error);
        //            return;
        //        }
        // }

        if (txtUptoPrevBill.Text.Equals(string.Empty))
        {
            error += "MOP Value cannot be blank.\\n";
        }        
        if (error.Equals(string.Empty))
        {
            double totalValue = 0;
            if (txtSincePrevBill.Text.Length >= 0 && txtUptoPrevBill.Text.Length > 0)
            {
                try
                {
                    totalValue = double.Parse(txtSincePrevBill.Text) + double.Parse(txtUptoPrevBill.Text);
                }
                catch (Exception err)
                {
                    Common.Show("Invalid value");
                    return;
                }
            }
            
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(@" UPDATE RAB_MOP_BILL_DTL 
                                             set UPTO_PREV_BILL_AMT=:UPTO_PREV_BILL_AMT,
                                                SINCE_PREV_BILL_AMT=:SINCE_PREV_BILL_AMT,
                                                TOTAL_UPTO_DATE=:TOTAL_UPTO_DATE,ADDED_ON=SYSDATE,ADDED_BY_ROLE=:ADDED_BY_ROLE
                                                where ID=:ID
                                                and HEADING_ID=:HEADING_ID
                                                and SUB_HEADING_ID=:SUB_HEADING_ID                                   
                                                ");
            Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
            paramListMOP.Add("ID", hdMOPID.Value);
            paramListMOP.Add("HEADING_ID", hdHeadingID.Value);
            paramListMOP.Add("SUB_HEADING_ID", hdSubHeadingID.Value);
            paramListMOP.Add("UPTO_PREV_BILL_AMT", txtUptoPrevBill.Text);
            paramListMOP.Add("SINCE_PREV_BILL_AMT", txtSincePrevBill.Text);
            paramListMOP.Add("TOTAL_UPTO_DATE", totalValue.ToString());                      
            paramListMOP.Add("ADDED_BY_ROLE", Session["ROLE"].ToString());
            int update = objDB.executeNonQuery(sbQuery.ToString(), paramListMOP);
            if (update != 0)
            {  
                gvTenders.EditIndex = -1;
                bindMOP(hd_MOPID.Value);
                Common.Show("MOP Data updated successfully.");            }
            else
            {
                Common.Show("Error while updating data.");
            }
        }
        else
        {
            Common.Show(error);
        }
    }
}