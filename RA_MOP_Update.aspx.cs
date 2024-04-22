using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AppCode;
using System.Text;

public partial class RA_MOP_Update : System.Web.UI.Page
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
            bindMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue);
        }
    }

    protected void bindMop(string jobNumber,string tenderNo, string billNumber)
    {
        string[] strArray = tenderNo.Split('~');
        string[] strBill = billNumber.Split('~');
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        Dictionary<string, string> paramListJob = new Dictionary<string, string>();
        paramListJob.Add("JOB_NO", jobNumber);
        paramListJob.Add("TENDER_NO", strArray[0]);
        paramListJob.Add("PART_NO", strArray[1]);
        paramListJob.Add("BILL_NO", strBill[0]);
        
        objDB.bindGridView(gvJobMop, @"select A.JOB_NO,A.TENDER_NO,A.ID,A.HEADING_ORDER, A.HEADING_DESC 
                                        from RAB_MOP_HEADING_DTL a  
                                        where a.IS_VALID='Y' 
                                       and a.JOB_NO=:JOB_NO and a.TENDER_NO=:TENDER_NO and a.PART_NO=:PART_NO and BILL_NO=:BILL_NO
                                       and  A.HEADING_ORDER in (11,13) 
                                        order by A.HEADING_ORDER", paramListJob);
        bool dataExists = false;
        if (gvJobMop.Rows.Count > 0)
        {
            pnlJobMOP.Visible = true;
            trJobMOP.Visible = true;
            pnlMasterMop.Visible = false;           
        }
        //else if (gvJobMop.Rows.Count == 0)
        //{
        //    pnlJobMOP.Visible = false;
        //    //objDB.bindGridView(gvMopHeadings, @"select A.ID,A.HEADING_ORDER, A.HEADING_DESC from RAB_MOP_HEADING_DTL a  where a.JOB_NO='0000' and a.TENDER_NO='0' and a.part_NO='0' and a.IS_VALID='Y' order by A.HEADING_ORDER", paramList);
        //    //pnlMasterMop.Visible = true;
        //    //trPopulateMop.Visible = true;
        //    //lblMopError.ForeColor = System.Drawing.Color.Red;
        //    //lblMopError.Text = "MOP fields are not present, Click the button to populate ";
        //    //Populate MOP values


        //    Dictionary<string, string> paramListMOP = new Dictionary<string, string>();
        //    paramListMOP.Add("T_SOURCE_JOBNO", "0000");
        //    paramListMOP.Add("T_DEST_JOBNO", ddJobNumber.SelectedValue.ToString());
        //    paramListMOP.Add("T_TENDER_NO", strArray[0]);
        //    paramListMOP.Add("T_PART_NO", strArray[1]);
        //    paramListMOP.Add("T_USER", Session["USERID"].ToString());
        //    paramListMOP.Add("T_RA_BILL_NO", strBill[0]);
        //    try
        //    {
        //        objDB.executeProcedure("RABILLING.RAB_COPY_MOP", paramListMOP);               
        //        dataExists = true;
        //    }
        //    catch (Exception err)
        //    {
        //        Common.Show("Error:Kindly select Job Number and Tender Number");
        //        dataExists = false;
        //    }
        //}
       //Bind MOP again
        //if (dataExists)
        //{
        //    objDB.bindGridView(gvJobMop, @"select A.JOB_NO,A.TENDER_NO,A.ID,A.HEADING_ORDER, A.HEADING_DESC from RAB_MOP_HEADING_DTL a  where a.IS_VALID='Y' and a.JOB_NO=:JOB_NO and a.TENDER_NO=:TENDER_NO and a.PART_NO=:PART_NO and BILL_NO=:BILL_NO  order by A.HEADING_ORDER", paramListJob);
        //    pnlJobMOP.Visible = true;
        //    trJobMOP.Visible = true;
        //    pnlMasterMop.Visible = false;
        //}

        // Check if MOP Freezed than do not show the button
        StringBuilder sbQuery = new StringBuilder();
        sbQuery.Append(@"select VENDOR_FROZEN from RAB_MOP_BILL_ACTION where MOP_ID =
                (select distinct a.id 
                         from RAB_MOP_BILL_DTL a
                            where a.JOB_NO=:JOB_NO and a.TENDER_NO=:TENDER_NO and a.PART_NO=:PART_NO and RA_BILL_NO=:BILL_NO)");
        string vedorFrozen ="";
        vedorFrozen = objDB.executeScalar(sbQuery.ToString(),paramListJob);
        if ("Y".Equals(vedorFrozen))
        {
            btnAddNewFields.Visible = false;            
        }
        else
        {
            btnAddNewFields.Visible = true;
        }
       
    }
    protected void gvMopHeadings_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdID = new HiddenField();
            hdID = (HiddenField)e.Row.FindControl("hdID");
            string headingID = hdID.Value;
            HiddenField hdOrder = new HiddenField();
            hdOrder = (HiddenField)e.Row.FindControl("hdOrder");
            string headingOrder = hdOrder.Value;

            GridView gvMopSubHeadings = e.Row.FindControl("gvMopSubHeadings") as GridView;
            StringBuilder query = new StringBuilder();

            if (!"".Equals(headingID))
            {
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                query.Append(@"select A.ID,B.HEADING_ID,B.SUB_HEADING_ORDER, B.SUB_HEADING_DESC
                                from RAB_MOP_HEADING_dtl a,RAB_MOP_SUB_HEADINGS_dtl b where B.HEADING_ID =:HEADING_ID and A.ID=B.HEADING_ID
                                order by B.SUB_HEADING_ORDER");
                paramList.Add("HEADING_ID", headingID);                
                objDB.bindGridView(gvMopSubHeadings, query.ToString(), paramList);
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

    protected void btnPopulateMopData_Click(object sender, EventArgs e)
    {
       if(!"".Equals(ddJobNumber.SelectedValue.ToString()) && !"".Equals(ddTenderNo.SelectedValue.ToString()))
       {
        string[] strArray = ddTenderNo.SelectedValue.Split('~');
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("T_SOURCE_JOBNO", "0000");
        paramList.Add("T_DEST_JOBNO", ddJobNumber.SelectedValue.ToString());
        paramList.Add("T_TENDER_NO", strArray[0]);
        paramList.Add("T_PART_NO", strArray[1]);        
        paramList.Add("T_USER", Session["USERID"].ToString());
        try{
            objDB.executeProcedure("RABILLING.RAB_COPY_MOP", paramList);
               Common.Show("Data Populated Successfully");
            bindMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue);
        }catch(Exception err)
        {
           Common.Show("Error:Kindly select Job Number and Tender Number");
        }
       }
       else
       {
           Common.Show("Error:Kindly select Job Number and Tender Number");
       }
    }


    protected void btnCheckMOP_Click(object sender, EventArgs e)
    {
       if(!"".Equals(ddJobNumber.SelectedValue.ToString()) && !"".Equals(ddTenderNo.SelectedValue.ToString()))
       {
           bindMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue);
           pnlAddExtraMOPFields.Visible = false;           
       }
     }
   
    protected void ddHeadings_SelectedIndexChanged(object sender, EventArgs e)
    {
        btnPopulateMopData.Visible = false;

        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) && !"".Equals(ddHeadings.SelectedValue))
        {
            bindSubheadings(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddHeadings.SelectedValue);
        }
    }

    protected void bindHeadings(string jobNumber, string tenderNo, string billNo)
    {
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        StringBuilder sbQuery = new StringBuilder();
        string[] strArray = tenderNo.Split('~');
        sbQuery.Append(@" Select DISTINCT ID, HEADING_DESC,HEADING_ORDER
                     FROM RAB_MOP_HEADING_DTL  
                   WHERE JOB_NO=:JOB_NO 
                    and TENDER_NO=:TENDER_NO 
                    and PART_NO=:PART_NO 
                    and BILL_NO=:BILL_NO 
                    and  CHILD_EXISTS='Y' 
                    and    HEADING_ORDER in (11,13)
                    ORDER BY HEADING_ORDER");
        paramList.Add("JOB_NO", jobNumber);
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        paramList.Add("BILL_NO", billNo);

        objDB.bindDropDownList(ddHeadings, sbQuery.ToString(), paramList, "ID", "HEADING_DESC", "", "--Select Heading--");
     }

    protected void bindSubheadings(string jobNumber, string tenderNo, string headingId)
    {
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        StringBuilder sbQuery = new StringBuilder();
        string[] strArray = tenderNo.Split('~');
        sbQuery.Append(@"SELECT SUB_HEADING_ORDER SNo, SUB_HEADING_DESC Description
                        fROM RAB_MOP_SUB_HEADINGS_DTL
                        WHERE HEADING_ID=:HEADING_ID
                        AND JOB_NO=:JOB_NO 
                        and TENDER_NO=:TENDER_NO  
                         and PART_NO=:PART_NO       
                     ORDER BY SUB_HEADING_ORDER");
        paramList.Add("HEADING_ID", headingId);
        paramList.Add("JOB_NO", jobNumber);
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        objDB.bindGridView(gvSubHeading, sbQuery.ToString(), paramList);
    }
    protected void btnAddNewFields_Click(object sender, EventArgs e)
    {
        pnlJobMOP.Visible = false;
        pnlMasterMop.Visible = false;
        pnlAddExtraMOPFields.Visible = true;
        btnAddNewFields.Visible = false;
        btnPopulateMopData.Visible = false;
        string[] strArray = ddBillNumber.SelectedValue.Split('~');
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) && !"".Equals(ddBillNumber.SelectedValue))
        {
            bindHeadings(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, strArray[0]);
        }
        else
        {
            Common.Show("Error: Selct Job Number and Tender Number");
        }
    }

    protected void btnSaveField_Click(object sender, EventArgs e)
    {
        if("".Equals(ddJobNumber.SelectedValue.ToString()))
        {
        Common.Show("Error: Select Job Number ");
        }
        else if( "".Equals(ddTenderNo.SelectedValue.ToString()) )
        {
        Common.Show("Error: Select Tender Number ");
        }
        else if( "".Equals(ddHeadings.SelectedValue.ToString()))
        {
        Common.Show("Error: Select  Heading");
        }
        else if(txtSubHeading.Text.Trim().Length == 0)
        {
        Common.Show("Error: Enter Sub Heading");
        }
        else
        {            
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            string[] strArray = ddTenderNo.SelectedValue.Split('~');
            string[] strArrayBill = ddBillNumber.SelectedValue.Split('~');
            sbQuery.Append(@"INSERT INTO RAB_MOP_SUB_HEADINGS_DTL
                        (HEADING_ID, SUB_HEADING_DESC, SUB_HEADING_ORDER, UPDATED_BY,  JOB_NO, TENDER_NO,PART_NO,VALUE_EXISTS,BILL_NO)
                        VALUES (:HEADING_ID, :SUB_HEADING_DESC, (select max(nvl(sub_heading_order,0))+1 from RAB_MOP_SUB_HEADINGS_DTL where HEADING_ID =:HEADING_ID1 group by HEADING_ID), :UPDATED_BY, :JOB_NO, :TENDER_NO,:PART_NO,'Y',:BILL_NO)");
        
            paramList.Add("HEADING_ID1", ddHeadings.SelectedValue);            
            paramList.Add("HEADING_ID", ddHeadings.SelectedValue);            
            paramList.Add("SUB_HEADING_DESC", txtSubHeading.Text.Trim().ToString());
            paramList.Add("UPDATED_BY", Session["USERID"].ToString() );
        paramList.Add("JOB_NO", ddJobNumber.SelectedValue.ToString());
        paramList.Add("TENDER_NO", strArray[0]);
        paramList.Add("PART_NO", strArray[1]);
        paramList.Add("BILL_NO", strArrayBill[0]);
            if(objDB.executeNonQuery(sbQuery.ToString(),paramList) > 0)
            {
               Common.Show("Subheading added succesfully");
               bindMop(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddBillNumber.SelectedValue);
               btnAddNewFields.Visible = true;
               pnlAddExtraMOPFields.Visible = false;
            }
            else
            {
             Common.Show("Error: Subheading not added");
            }
        }
    }
}