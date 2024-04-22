using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;

public partial class RA_Edit_Percentage : System.Web.UI.Page
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
            if (Session["USERID"] != null && Session["ROLE"] != null && "RCM".Equals(Session["ROLE"].ToString()))
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
     sbQuery.Append(@"Select DISTINCT a.TENDER_NO, a.tender_no||' ( '||upper(a.TITLE)||')' description,
                        B.ID,nvl(B.IS_FROZEN,'N') is_frozen,
                            nvl(B.PECENTAGE_VALUE,'0') percentage_value,A.PART_NO
                     FROM RAB_TENDER_MASTER a , RAB_TENDER_REBATE b
                    WHERE a.JOB_NO=:JOB_NO
                    and A.JOB_NO=B.JOB_NO(+)
                    and A.TENDER_NO=B.TENDER_NO(+) 
                    and A.PART_NO=B.PART_NO(+)                                        
                    ORDER BY a.TENDER_NO");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("JOB_NO", jobNumber);
        objDB.bindGridView(gvTenders, sbQuery.ToString(), paramList);
    }

    protected void gvTenders_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdID = new HiddenField();
            hdID = (HiddenField)e.Row.FindControl("hdID");
            string id = hdID.Value;
            HiddenField hdFrozen = new HiddenField();
            hdFrozen = (HiddenField)e.Row.FindControl("hdFrozen");
            string isFrozen = hdFrozen.Value;

            if (isFrozen.Equals("Y") )
            {
                e.Row.Cells[4].Text = "";
            }

            //if ((e.Row.RowState & DataControlRowState.Edit) > 0 && itemRate.Length < 1)
            //{
            //    e.Row.Cells[6].Visible = false;
            //    e.Row.Cells[7].Text = "";
            //}
        }
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
        TextBox txtPercentage = (TextBox)row.FindControl("txtPercentage");
        HiddenField hdID = (HiddenField)row.FindControl("hdID");
        HiddenField hdFrozen = (HiddenField)row.FindControl("hdFrozen");
        HiddenField hdTenderNo = (HiddenField)row.FindControl("hdTenderNo");
        HiddenField hdPartNumber = (HiddenField)row.FindControl("hdPartNumber");
        
        string error = "";
    
            float pvalue = 0;

            float.TryParse(txtPercentage.Text.ToString(), out pvalue);
        {
                if (pvalue == 0)
                {
                    error += "Percentage Value is not correct.\\n";
                    Common.Show(error);
                    return;
                }
            }        


        if (txtPercentage.Text.Equals(string.Empty))
        {
            error += "Percentage Value cannot be blank.\\n";
        }
        if (hdTenderNo.Value.Length < 1)
        {
            error += "Tender number not selected.\\n";
        }
        if (error.Equals(string.Empty))
        {
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            if (hdID.Value.Equals(""))
            {
                sbQuery.Append(@"INSERT INTO RAB_TENDER_REBATE ( JOB_NO, TENDER_NO, PECENTAGE_VALUE,  ADDED_BY,part_no)
                            VALUES ( :JOB_NO, :TENDER_NO, :PECENTAGE_VALUE, :ADDED_BY,:part_no)
                           ");
                paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToString());
                paramList.Add("TENDER_NO", hdTenderNo.Value);
                paramList.Add("PECENTAGE_VALUE", pvalue.ToString());
                paramList.Add("ADDED_BY", Session["USERID"].ToString());
                paramList.Add("part_no", hdPartNumber.Value);
            }
            else
            {
                sbQuery.Append("UPDATE RAB_TENDER_REBATE SET PECENTAGE_VALUE=:PECENTAGE_VALUE,ADDED_BY=:ADDED_BY,IS_FROZEN=:IS_FROZEN,ADDED_ON=sysdate ")
                    .Append(" WHERE ID=:ID ")
                    .Append(" AND JOB_NO=:JOB_NO ")
                    .Append(" AND TENDER_NO=:TENDER_NO ")
                    .Append(" AND IS_FROZEN=:IS_FROZEN_NO ")
                     .Append(" AND PART_NO=:PART_NO "); ;

                paramList.Add("ID", hdID.Value);
                paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToString());
                paramList.Add("TENDER_NO", hdTenderNo.Value);
                paramList.Add("IS_FROZEN_NO", "N");
                paramList.Add("IS_FROZEN", "Y");
                paramList.Add("ADDED_BY", Session["USERID"].ToString());
                paramList.Add("PECENTAGE_VALUE", pvalue.ToString());
                paramList.Add("PART_NO", hdPartNumber.Value);
            }
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