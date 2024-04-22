using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;

public partial class RA_Edit_Tender : System.Web.UI.Page
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
            if (Session["USERID"] != null && Session["ROLE"] != null && ("RCM".Equals(Session["ROLE"].ToString()) || "AC".Equals(Session["ROLE"].ToString())))
            {
                bindJobNumber(Session["USERID"].ToString(), Session["ROLE"].ToString());
            }            
            else {
                Response.Redirect("Login.aspx");
            }
        }
    }

    protected void bindJobNumber(string userId, string userRole)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        if ("RCM".Equals(userRole))
        {
            sbQuery.Append("SELECT distinct JOB_NO FROM JOB_DIR WHERE SITE_CD IN (SELECT SITE_CD FROM SITE_DIR WHERE EMPNO_RCM=:EMPNO_RCM) ORDER BY JOB_NO");
            paramList.Add("EMPNO_RCM", userId);
        }
        else if ("AC".Equals(userRole))
        {
            sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_USERS where ROLE=:ROLE and EMPNO=:EMPNO and ACTIVE='Y'  ORDER BY JOB_NO");
            paramList.Add("ROLE", Session["ROLE"].ToString());
            paramList.Add("EMPNO", userId);
        }

        if (sbQuery.Length > 0)
            objDB.bindDropDownList(ddJobNumber, sbQuery.ToString(), paramList, "JOB_NO", "JOB_NO", "", "--Select Job Number--");
    }

    protected void ddJobNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddJobNumber.SelectedValue.ToString().Length > 0)
        bindTenders(ddJobNumber.SelectedValue.ToString());
    }

    protected void bindTenders(string jobNumber)
    {        
     StringBuilder sbQuery = new StringBuilder();
     sbQuery.Append(@"Select DISTINCT a.TENDER_NO,A.PART_NO, a.tender_no||' ( '||upper(a.TITLE)||')' description,LOI_No,    
to_char(Date_AWARDED,'dd-Mon-yyyy') Date_AWARDED,    
to_char(Completion_Date,'dd-Mon-yyyy') Completion_Date,amount
                        
                     FROM RAB_TENDER_MASTER a 
                    WHERE a.JOB_NO=:JOB_NO                                                           
                    ORDER BY a.TENDER_NO");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("JOB_NO", jobNumber);
        objDB.bindGridView(gvTenders, sbQuery.ToString(), paramList);
    }

    protected void gvTenders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //if (e.Row.RowType == DataControlRowType.DataRow)
        //{
        //    HiddenField hdID = new HiddenField();
        //    hdID = (HiddenField)e.Row.FindControl("hdID");
        //    string id = hdID.Value;
        //    HiddenField hdFrozen = new HiddenField();
        //    hdFrozen = (HiddenField)e.Row.FindControl("hdFrozen");
        //    string isFrozen = hdFrozen.Value;

        //    if (isFrozen.Equals("Y") )
        //    {
        //        e.Row.Cells[4].Text = "";
        //    }
            
        //}
    }

    protected void gvTenders_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvTenders.EditIndex = e.NewEditIndex;
        bindTenders( ddJobNumber.SelectedValue);
    }

    protected void gvTenders_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvTenders.EditIndex = -1;
        bindTenders(ddJobNumber.SelectedValue);
    }

    protected void gvTenders_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int index = gvTenders.EditIndex;
        GridViewRow row = gvTenders.Rows[index];
        TextBox txtLOI = (TextBox)row.FindControl("txtLOI");
        TextBox txtAwardedDate = (TextBox)row.FindControl("txtAwardedDate");
        TextBox txtCompletionDate = (TextBox)row.FindControl("txtCompletionDate");
        TextBox txtAmount = (TextBox)row.FindControl("txtAmount");

        
        HiddenField hdTenderNo = (HiddenField)row.FindControl("hdTenderNo");
        HiddenField hdPartNumber = (HiddenField)row.FindControl("hdPartNumber");
        
        string error = "";
    
            float pvalue = 0;

        //float.TryParse(txtLOI.Text.ToString(), out pvalue);
        //{
        //        if (pvalue == 0)
        //        {
        //            error += "Percentage Value is not correct.\\n";
        //            Common.Show(error);
        //            return;
        //        }
        //    }


        if (txtLOI.Text.Equals(string.Empty))
        {
            error += "LOI Number Value cannot be blank.\\n";
        }
        if (hdTenderNo.Value.Length < 1)
        {
            error += "Tender number not selected.\\n";
        }

        if (txtAwardedDate.Text.Equals(string.Empty))
        {
            error += "Awarded Date cannot be blank.\\n";
        }

        if (txtCompletionDate.Text.Equals(string.Empty))
        {
            error += "Completion Date cannot be blank.\\n";
        }

        if (txtAmount.Text.Equals(string.Empty))
        {
            error += "Amount cannot be blank.\\n";
        }

        if (error.Equals(string.Empty))
        {
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();

            sbQuery.Append(@"UPDATE RAB_TENDER_MASTER
                           SET LOI_No = :LOI_No,
                               T_ADDED_BY = :ADDED_BY,
                               Date_AWARDED = TO_DATE (:Date_AWARDED, 'dd-MM-yyyy'),
                               T_ADDED_ON = SYSDATE,
                               Completion_Date = TO_DATE (:Completion_Date, 'dd-MM-yyyy'),
                               amount = :amount
                         WHERE JOB_NO = :JOB_NO AND TENDER_NO = :TENDER_NO AND PART_NO = :PART_NO"); 
               
                paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToString());
                paramList.Add("TENDER_NO", hdTenderNo.Value);
                paramList.Add("Date_AWARDED", txtAwardedDate.Text);
                paramList.Add("Completion_Date", txtCompletionDate.Text);
                paramList.Add("ADDED_BY", Session["USERID"].ToString());
                paramList.Add("LOI_No", txtLOI.Text);
                paramList.Add("PART_NO", hdPartNumber.Value);
                paramList.Add("amount", txtAmount.Text);
            
            int update = objDB.executeNonQuery(sbQuery.ToString(), paramList);
            if (update != 0)
            {
                gvTenders.EditIndex = -1;
                bindTenders( ddJobNumber.SelectedValue);
                Common.Show("Data updated successfully.");
            }
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