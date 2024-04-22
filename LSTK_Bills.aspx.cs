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

public partial class LSTK_Bills : System.Web.UI.Page
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
                bindJobNumber();
                bindJobNumberSearch();
            }
            else
            {
                Response.Redirect("Login.aspx");
            }
        }    
    }


    protected void rbNewBill_CheckedChanged(object sender, EventArgs e)
    {
        tblUploadBill.Visible = true;
        bindJobNumber();
        tblSearch.Visible = false;
    }

    protected void rbSearchBill_CheckedChanged(object sender, EventArgs e)
    {
        tblUploadBill.Visible = false;
        tblSearch.Visible = true;
        bindJobNumberSearch();
    }


    protected void bindJobNumber()
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("EMPNO_RCM", Session["USERID"].ToString().ToUpper());
        if ("RCM".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append("SELECT distinct upper(job_no) job_no FROM vw_rab_lstk_jobs where upper(EMPNO_RCM)=:EMPNO_RCM ORDER BY JOB_NO");
        }
        else if ("AC".Equals(Session["ROLE"].ToString()))
        {
            sbQuery.Append("SELECT distinct upper(job_no) job_no FROM RAB_TENDER_USERS where upper(EMPNO)=:EMPNO_RCM and role='AC' ORDER BY JOB_NO");
        }
        objDB.bindDropDownList(ddJobNumber, sbQuery.ToString(), paramList, "JOB_NO", "JOB_NO", "", "--Select Job Number--");
    }

    protected void bindJobNumberSearch()
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("is_active","Y");
        sbQuery.Append("SELECT distinct upper(JOB_NUMBER) job_no FROM RAB_LSTK_BILLS where is_active=:is_active ORDER BY JOB_NO");
        objDB.bindDropDownList(ddJobNumberSearch, sbQuery.ToString(), paramList, "JOB_NO", "JOB_NO", "", "--Select Job Number--");
    }

    protected void bindTenders(string jobNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append(" Select distinct upper(tend_no) tend_no FROM vw_rab_lstk_jobs where upper(job_no)=:JOB_NO and TEND_NO like '%8003%'  ORDER BY tend_no");
        paramList.Add("JOB_NO", jobNumber.ToUpper());
        objDB.bindDropDownList(ddTenderNo, sbQuery.ToString(), paramList, "tend_no", "tend_no", "", "--Select Tender Number--");
    }

    protected void ddJobNumber_SelectedIndexChanged(object sender, EventArgs e)
    {

        if (!"".Equals(ddJobNumber.SelectedValue))
        {
            bindTenders(ddJobNumber.SelectedValue);
            trUploadBills.Visible = false;
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
            bindPartNumber(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue);
            trUploadBills.Visible = false;
        }
        else
        {
            Common.Show("Please select Job Number and Tender Number");
        }
    }

    protected void bindPartNumber(string jobNumber, string tenderNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        sbQuery.Append(" Select distinct upper(part_no) part_no FROM vw_rab_lstk_jobs where upper(job_no)=:JOB_NO and upper(tend_no)=:tend_no ORDER BY part_no");
        paramList.Add("JOB_NO", jobNumber.ToUpper());
        paramList.Add("tend_no", tenderNumber.ToUpper());
        objDB.bindDropDownList(ddPartNumber, sbQuery.ToString(), paramList, "part_no", "part_no", "", "--Select Part Number--");
    }

    protected void ddPartNumber_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) && !"".Equals(ddPartNumber.SelectedValue))
        {
            bindJobDetails(ddJobNumber.SelectedValue, ddTenderNo.SelectedValue, ddPartNumber.SelectedValue);
            trUploadBills.Visible = true;
        }
        else
        {
            trUploadBills.Visible = false;
            Common.Show("Please select Job Number, Tender Number and Part Number");
        }
    }

    protected void bindJobDetails(string jobNumber, string tenderNumber, string partNumber)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();      
        paramList.Add("JOB_NO", jobNumber.ToUpper());
        paramList.Add("tend_no", tenderNumber.ToUpper());
        paramList.Add("part_no", partNumber.ToUpper());
        sbQuery.Append(" Select distinct upper(tend_desc) ||','||upper(SITE_NAME) jobDetails,upper(CONT_NAME) CONT_NAME FROM vw_rab_lstk_jobs where upper(job_no)=:JOB_NO and upper(tend_no)=:tend_no and  upper(part_no)=:part_no ");

        DataTable dataTableJobInfo = new DataTable();
        dataTableJobInfo = objDB.bindDataTable(sbQuery.ToString(), paramList);
        if (dataTableJobInfo.Rows.Count != 0 && dataTableJobInfo.Rows.Count == 1)
        {
            lblJobName.Text = dataTableJobInfo.Rows[0]["jobDetails"].ToString();
            lblContractor.Text = dataTableJobInfo.Rows[0]["CONT_NAME"].ToString();           
        }

        // Check if RCM belongs to selected Job; show Bill uploading screen
        paramList.Add("EMPNO_RCM", Session["USERID"].ToString().ToUpper());
        if (!objDB.executeScalar("select count(*) from vw_rab_lstk_jobs where upper(job_no)=:JOB_NO and upper(tend_no)=:tend_no and  upper(part_no)=:part_no and upper(EMPNO_RCM)=:EMPNO_RCM", paramList).Equals("0"))
        {            
            hdifJobRelatedToRCM.Value = "Y";
            trUploadBills.Visible = true;
        }
        else
        {
            trUploadBills.Visible = false;
            hdifJobRelatedToRCM.Value = "N";
        }
        bindUploadedBills();
    }

    protected void bindUploadedBills()
    {
        if (!"".Equals(ddJobNumber.SelectedValue) && !"".Equals(ddTenderNo.SelectedValue) && !"".Equals(ddPartNumber.SelectedValue))
        {
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("JOB_NUMBER", ddJobNumber.SelectedValue);
            paramList.Add("TENDER_NUMBER", ddTenderNo.SelectedValue);
            paramList.Add("PART_NUMBER", ddPartNumber.SelectedValue);
            paramList.Add("IS_ACTIVE", "Y");
            sbQuery.Append(@"select ID, JOB_NUMBER, TENDER_NUMBER, PART_NUMBER, FILE_NAME, GET_EMP_NAME(UPLOADED_BY) UPLOADED_BY, to_char(UPLOADED_ON,'dd-Mon-YYYY') UPLOADED_ON, IS_ACTIVE, REMARKS, ATTACH_TYPE,CONTRACTOR_NAME,JOB_DESC   from RAB_LSTK_BILLS a where  A.JOB_NUMBER=:JOB_NUMBER and a.TENDER_NUMBER=:TENDER_NUMBER and a.PART_NUMBER=:PART_NUMBER and IS_ACTIVE=:IS_ACTIVE ORDER BY ATTACH_TYPE ");
            objDB.bindGridView(gvAttachments, sbQuery.ToString(), paramList);
            // Hide delete button 
            if (hdifJobRelatedToRCM.Value.Equals("N"))
            {
                gvAttachments.Columns[gvAttachments.Columns.Count - 1].Visible = false;                
            }            
        }
        else
        {
            Response.Redirect("Login");
        }
    }

    protected void gvAttachments_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdID = (e.Row.FindControl("hdID") as HiddenField);
            HiddenField hdAttachmentName = (e.Row.FindControl("hdAttachmentName") as HiddenField);
            HyperLink hlAttachment = (e.Row.FindControl("hlAttachment") as HyperLink);
            string docPath = Constants.lstk_path + "/" + ddJobNumber.SelectedValue + "/" +  hdAttachmentName.Value;

            hlAttachment.Attributes.Add("onClick", "ShowDocumentPopup('" + Common.encrypt(docPath) + "')");
         
                foreach (DataControlField dcf in gvAttachments.Columns)
                {
                    if (dcf.ToString() == "CommandField")
                    {
                        if (((CommandField)dcf).ShowDeleteButton == true)
                        {
                            e.Row.Cells[gvAttachments.Columns.IndexOf(dcf)].Attributes.Add("onclick", "return confirm('Are you sure to delete?');");
                        }
                    }
                }            
        }
    }

    protected void gvAttachments_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        int index = e.RowIndex;
        string id = gvAttachments.DataKeys[index].Value.ToString();        
        paramList.Add("ID", id);
        paramList.Add("IS_ACTIVE", "N");
        paramList.Add("DELETED_BY", Session["USERID"].ToString());
        
        if (objDB.executeNonQuery("UPDATE RAB_LSTK_BILLS SET IS_ACTIVE=:IS_ACTIVE, DELETED_ON=sysdate,DELETED_BY=:DELETED_BY WHERE id=:ID", paramList) > 0)
        {            
            Common.Show("File deleted successfully!");
            bindUploadedBills();            
        }
        else
        {
            Common.Show("Error in deleting the attachment!");
        }
    }
   
    protected void btnSearchBill_Click(object sender, EventArgs e)
    {
        StringBuilder sbQuery = new StringBuilder();
        Dictionary<string, string> paramList = new Dictionary<string, string>();
         sbQuery.Append(@"select ID, JOB_NUMBER, TENDER_NUMBER, PART_NUMBER, FILE_NAME, UPLOADED_BY, to_char(UPLOADED_ON,'dd-Mon-YYYY') UPLOADED_ON, IS_ACTIVE, REMARKS, ATTACH_TYPE,CONTRACTOR_NAME,JOB_DESC  from RAB_LSTK_BILLS a where  IS_ACTIVE=:IS_ACTIVE ");
         paramList.Add("IS_ACTIVE", "Y");
        if (!"".Equals(ddJobNumberSearch.SelectedValue))
        {
            sbQuery.Append(@" and upper(JOB_NUMBER) =:JOB_NUMBER ");   
            paramList.Add("JOB_NUMBER", ddJobNumberSearch.SelectedValue.ToUpper());
        }
        if(txtContractorName.Text.Length >0)
        {
            sbQuery.Append(@" and upper(CONTRACTOR_NAME) like '%'||:CONTRACTOR_NAME||'%' ");
            paramList.Add("CONTRACTOR_NAME", txtContractorName.Text.ToUpper());
        }
         if(txtJobDescription.Text.Length >0)
        {
        sbQuery.Append(@" and upper(JOB_DESC) like '%'||:JOB_DESC||'%' ");
            paramList.Add("JOB_DESC", txtJobDescription.Text.ToUpper());
        }   
         
         sbQuery.Append(@" order by JOB_NUMBER ");           
         objDB.bindGridView(gvSearch, sbQuery.ToString(), paramList);               
    }


    protected void gvSearch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            HiddenField hdID = (e.Row.FindControl("hdIDSearch") as HiddenField);
            HiddenField hdAttachmentName = (e.Row.FindControl("hdAttachmentNameSearch") as HiddenField);
            HyperLink hlAttachment = (e.Row.FindControl("hlAttachmentSearch") as HyperLink);
            string docPath = Constants.lstk_path + "/" + ddJobNumber.SelectedValue + "/" + hdAttachmentName.Value;
            hlAttachment.Attributes.Add("onClick", "ShowDocumentPopup('" + Common.encrypt(docPath) + "')");
        }
    }  


    protected void btFileUpload_Click(object sender, EventArgs e)
    {
        StringBuilder sbError = new StringBuilder();
        if (txtRemarks.Text.Length == 0 )
        {
            sbError.Append("Kindly enter Remarks!\n\n");           
        }
        if ( ddBillType.SelectedValue.Equals(""))
        {
            sbError.Append("Kindly select Bill type !\n\n");
        }

        if (!fuAttachment.HasFile)
        {
            sbError.Append("Attachment is mandatory!\n\n");            
        }

        if (fuAttachment.HasFile && fuAttachment.PostedFile.ContentLength > Constants.maxUploasSizeInBytes)
        {            
            sbError.Append("File size limit exceeded for MoM (Limit is " + Constants.maxUploasSize + "MB).\n\n");
        }
        if (sbError.Length > 0)
        {
            Common.Show(sbError.ToString());
            lblError.Text = sbError.ToString();
        }
        else
        {
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            string attachment_name = "";
            if (!fuAttachment.FileName.Trim().Equals(""))
            {
                int newLength = 0;
                string newFile = fuAttachment.PostedFile.FileName.ToString();

                // Removing full directory path from file name
                if (newFile.Contains(":"))
                {
                    newFile = newFile.Split('\\').Last();
                }

                newLength = newFile.LastIndexOf(".");
                //string documentName = newFile.Substring(0, newLength);
                string newFileExtn = newFile.Substring(newLength, newFile.Length - newLength);
                string fileID = objDB.executeScalar("select RAB_LSTK_ATTACH_SEQ.nextval from dual", paramList);
                attachment_name = fileID + newFileExtn;

                string activeDir = Server.MapPath("~/" + Constants.lstk_path + "");
                string servMoMpath = System.IO.Path.Combine(activeDir, ddJobNumber.SelectedValue);
                if (!System.IO.Directory.Exists(servMoMpath))
                {
                    System.IO.Directory.CreateDirectory(servMoMpath);
                }

                string servpath = servMoMpath;
                servpath = System.IO.Path.Combine(servpath, attachment_name);

                try
                {
                    fuAttachment.SaveAs(servpath);
                    paramList.Add("JOB_NUMBER", ddJobNumber.SelectedValue);
                    paramList.Add("TENDER_NUMBER", ddTenderNo.SelectedValue);
                    paramList.Add("PART_NUMBER", ddPartNumber.SelectedValue);
                    paramList.Add("FILE_NAME", attachment_name);
                    paramList.Add("UPLOADED_BY", Session["USERID"].ToString());
                    paramList.Add("REMARKS", txtRemarks.Text);
                    paramList.Add("ATTACH_Type", ddBillType.SelectedValue);
                    paramList.Add("CONTRACTOR_NAME",lblContractor.Text);
                    paramList.Add("JOB_DESC",lblJobName.Text);
                    if (objDB.executeNonQuery("insert into RAB_LSTK_BILLS ( JOB_NUMBER, TENDER_NUMBER, PART_NUMBER, FILE_NAME, UPLOADED_BY,REMARKS,ATTACH_Type,CONTRACTOR_NAME,JOB_DESC) values (:JOB_NUMBER, :TENDER_NUMBER, :PART_NUMBER, :FILE_NAME, :UPLOADED_BY,:REMARKS,:ATTACH_Type,:CONTRACTOR_NAME,:JOB_DESC)", paramList) > 0)
                    {
                        Common.Show("Attachment uploaded successfully");
                        bindUploadedBills();
                    }
                }
                catch (Exception err)
                {
                    //Common.Show("Error:MoM not added due to some technical issue " + err.Message.ToString());                   
                }
            }
        }
    }  
}