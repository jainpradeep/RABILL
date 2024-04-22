using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;
using System.Collections;

public partial class RA_EditSiteQty : System.Web.UI.Page
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
            if (Session["USERID"] != null && Session["ROLE"] != null && "BE".Equals(Session["ROLE"].ToString()))
            {
                bindJobNumber(Session["USERID"].ToString(), Session["ROLE"].ToString());
            }
            else
            {
                Common.Show("This option is valid for Billing Engineer only");
                Response.Redirect("Default.aspx");
            }           
        }
    }

    protected void bindJobNumber(string userId, string userRole)
    {
        StringBuilder sbQuery = new StringBuilder();
        //sbQuery.Append("SELECT DISTINCT JOB_NO FROM RAB_TENDER_MASTER ");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        if ("BE".Equals(userRole) || "AC".Equals(userRole))
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
        objDB.bindDropDownList(ddJobNumber, sbQuery.ToString(), paramList, "JOB_NO", "JOB_NO", "", "--Select Job Number--");
    }

    //protected void bindTenders(string jobNumber)
    //{
    //    StringBuilder sbQuery = new StringBuilder();
    //    sbQuery.Append("SELECT DISTINCT TENDER_NO FROM RAB_TENDER_MASTER ")
    //        .Append(" WHERE JOB_NO=:JOB_NO ")
    //        .Append(" ORDER BY TENDER_NO ");
    //    Dictionary<string, string> paramList = new Dictionary<string, string>();
    //    paramList.Add("JOB_NO", jobNumber.ToUpper());
    //    objDB.bindDropDownList(ddTenderNo, sbQuery.ToString(), paramList, "TENDER_NO", "TENDER_NO", "", "--Select Job Number--");

    //}

    protected void bindTenders(string jobNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();

        if ("AC".Equals(Session["ROLE"].ToString()))
        {

            sbQuery.Append("SELECT DISTINCT TENDER_NO, tenderno||' ( '||b.title||' '||b.title1||')' description  ")
           .Append(" FROM  RAB_TENDER_USERS a,ppms.tenderschedule b ")
              .Append(" WHERE JOB_NO=:JOB_NO ")
              .Append(" AND EMPNO=:EMPNO ")
              .Append(" AND ROLE=:ROLE ")
              .Append(" and upper(trim(b.projno))=upper(trim(A.JOB_NO))")
             .Append(" and A.TENDER_NO=b.tendersrno")
              .Append(" ORDER BY TENDER_NO ");

            paramList.Add("JOB_NO", jobNumber.ToUpper());
            paramList.Add("EMPNO", Session["USERID"].ToString());
            paramList.Add("ROLE", Session["ROLE"].ToString());

        }
        else
        {
            sbQuery.Append(" Select DISTINCT TENDER_NO, tenderno||' ( '||b.title||' '||b.title1||')' description ")
                    .Append(" FROM RAB_TENDER_MASTER a,ppms.tenderschedule b")
                   .Append(" WHERE JOB_NO=:JOB_NO ")
                   .Append(" and upper(trim(b.projno))=upper(trim(A.JOB_NO))")
                   .Append(" and A.TENDER_NO=b.tendersrno")
                   .Append(" ORDER BY TENDER_NO");
            paramList.Add("JOB_NO", jobNumber.ToUpper());
        }
        objDB.bindDropDownList(ddTenderNo, sbQuery.ToString(), paramList, "TENDER_NO", "description", "", "--Select Tender Number--");
    }


    protected void ddJobNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!"".Equals(ddJobNumber.SelectedValue))
        {
            bindTenders(ddJobNumber.SelectedValue);
            pnlSORItems.Visible = false;
            gvSOR.Visible = false;
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
            bindSor(ddJobNumber.SelectedValue,ddTenderNo.SelectedValue);
            pnlSORItems.Visible = false;
            gvSOR.Visible = true;
        }
        else
        {
            Common.Show("Please select Job Number and Tender Number");
        }
    }    

    protected void bindSor(string jobNumber, string tenderNo)
    {
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.Append("SELECT distinct REF_ID, JOB_NO, SUB_JOB, TENDER_NO, PART_NO, SOR_NO, C_CODE, LOI_NO, LOI_DATE, UNIT_NO,TEND_SOR_ID ")
            .Append(" FROM RAB_TENDER_MASTER ")
            .Append(" where upper(JOB_NO)=:JOB_NO ")
            .Append(" AND upper(TENDER_NO)=:TENDER_NO ")
            .Append(" order by SUB_JOB, TENDER_NO, PART_NO, SOR_NO ");
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("JOB_NO", jobNumber.ToUpper());
        paramList.Add("TENDER_NO", tenderNo.ToUpper());
        objDB.bindGridView(gvSOR, sbQuery.ToString(), paramList);
    }
    protected void gvSOR_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName.Equals("Select"))
            {
                int RowIndex = Convert.ToInt16(e.CommandArgument.ToString());
              //  bindSORDetails();
                gvSOR.SelectedIndex = RowIndex;
                gvSOR.SelectedRow.BackColor = System.Drawing.Color.LightYellow;
            }
        }
        catch (Exception ex)
        {

        }
    }
    protected void gvSOR_SelectedIndexChanged(object sender, EventArgs e)
    {
        GridViewRow row = gvSOR.SelectedRow;
        HiddenField hdReferenceID = new HiddenField();
        hdReferenceID = (HiddenField)row.FindControl("hdReferenceID");
        string referenceId = hdReferenceID.Value;
        ViewState["REF_ID"] = referenceId;
        bindSORItems(referenceId,ddJobNumber.SelectedValue);
        pnlSORItems.Visible = true;
    }

    protected void gvSOR_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
        }
    }

    protected void gvSORItems_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdReferenceID = new HiddenField();
            hdReferenceID = (HiddenField)e.Row.FindControl("hdReferenceID");
            string referenceId = hdReferenceID.Value;
            HiddenField hdItemRate = new HiddenField();
            hdItemRate = (HiddenField)e.Row.FindControl("hdItemRate");
            string itemRate = hdItemRate.Value;

            if (itemRate.Length < 1)
            {
                   e.Row.Cells[7].Text = "";
            }
            
            //if ((e.Row.RowState & DataControlRowState.Edit) > 0 && itemRate.Length < 1)
            //{
            //    e.Row.Cells[6].Visible = false;
            //    e.Row.Cells[7].Text = "";
            //}
        }
    }
    

    protected void bindSORItems(string referenceId,string jobNumber)
    {
    
    StringBuilder sbQuery = new StringBuilder();
    sbQuery.Append(" SELECT distinct REF_ID, SEQ_NO, nvl(a.ITEM_RATE_EDITED,a.ITEM_RATE) ITEM_RATE, a.UOM, a.SORT_NO, ACT_DESC, ACT_PERCENT, ACT_SEQ, ADDED_ON, HO_QTY, SITE_QTY, ACT_PROG, FLAG_HO, a.sorno ")
        .Append(" sdesc,to_char(ldesc) ldesc  ")
        .Append("   FROM RAB_ITEM_BREAKUP a,icms_cba.cba_tender  b ")
        .Append("   where upper(REF_ID)=:REF_ID ")
        .Append("   and A.SEQ_NO=B.SEQNO ")
        .Append("  and b.JOB=:JOBNO ")
        .Append("  and b.jtn=:TENDER_NO ")
        .Append("  order by SORT_NO ");
        
        Dictionary<string, string> paramList = new Dictionary<string, string>();      

        paramList.Add("REF_ID", referenceId.ToUpper());
        paramList.Add("JOBNO", jobNumber);
        paramList.Add("TENDER_NO", ddTenderNo.SelectedValue);
        objDB.bindGridView(gvSORItems, sbQuery.ToString(), paramList);
    }    

    private void ShowMessage(string Msg, bool IsError, string RedirectURL)
    {
        Common.Show(Msg);        
    }

    protected void gvSORItems_RowEditing(object sender, GridViewEditEventArgs e)
    {
        gvSORItems.EditIndex = e.NewEditIndex;
        bindSORItems(ViewState["REF_ID"].ToString(), ddJobNumber.SelectedValue);
    }

    protected void gvSORItems_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
    {
        gvSORItems.EditIndex = -1;
        bindSORItems(ViewState["REF_ID"].ToString(), ddJobNumber.SelectedValue);
    }

    protected void gvSORItems_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        int index = gvSORItems.EditIndex;
        GridViewRow row = gvSORItems.Rows[index];
        TextBox txtSiteQty = (TextBox)row.FindControl("txtSiteQty");
        HiddenField hdReferenceID = (HiddenField)row.FindControl("hdReferenceID");
        HiddenField hdSequenceNo = (HiddenField)row.FindControl("hdSequenceNo");
        HiddenField hdSortNumber = (HiddenField)row.FindControl("hdSortNumber");
        HiddenField hdSORNumber = (HiddenField)row.FindControl("hdSORNumber");
        
        string ref_id = hdReferenceID.Value;
        string site_qty_no = txtSiteQty.Text.ToString();
        string error = "";
        if (site_qty_no.Equals(string.Empty))
        {
            error += "Site Quantity cannot be blank.\\n";
        }
        if (hdReferenceID.Value.Length < 1)
        {
            error += "SOR not selected.\\n";
        }
        if (error.Equals(string.Empty))
        {

            StringBuilder sbUpdateQuery = new StringBuilder();
            sbUpdateQuery.Append("UPDATE RAB_ITEM_BREAKUP SET SITE_QTY=:SITE_QTY ")
                .Append(" WHERE REF_ID=:REF_ID ")
                .Append(" AND SEQ_NO=:SEQ_NO ")
                .Append(" AND SORT_NO=:SORT_NO ")
                .Append(" AND SORNO=:SORNO ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("SITE_QTY", site_qty_no);
            paramList.Add("REF_ID", hdReferenceID.Value);
            paramList.Add("SEQ_NO", hdSequenceNo.Value);
            paramList.Add("SORT_NO", hdSortNumber.Value);
            paramList.Add("SORNO", hdSORNumber.Value);         
            int update = objDB.executeNonQuery(sbUpdateQuery.ToString(), paramList);
            if (update != 0)
            {
                gvSORItems.EditIndex = -1;
                bindSORItems(ViewState["REF_ID"].ToString(), ddJobNumber.SelectedValue);
                Common.Show("Quantity updated successfully.");
            }
            else
            {
                Common.Show("Error while updating Quantity.");
            }
        }
        else
        {
            Common.Show(error);
        }
    }
}