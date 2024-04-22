using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Collections;
using System.Text;
using AppCode;

public partial class RA_SelectRole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        dbFunction objDB = new dbFunction();
        if (!IsPostBack)
        {
            if (Session["USERID"] != null)
            {                
                    //ArrayList multiRole = (ArrayList)Session["MULTIPLEROLELIST"];
                    //int roleCount = multiRole.Count;
                    //for (int i = 0; i < roleCount; i++)
                    //{
                    //    string[] temp = new string[2];
                    //    temp = multiRole[i].ToString().Split('-');
                    //    ListItem li = new ListItem();
                    //    li.Text = multiRole[i].ToString();
                    //    //
                    //    //li.Value = temp[0].ToString();
                    //    if ("PC".Equals(temp[0].ToString()))
                    //    {
                    //        li.Value = "PEM";
                    //    }
                    //    else
                    //    {
                    //        li.Value = temp[0].ToString();
                    //    }
                    //    ddRole.Items.Add(li);
                    //}

                    if (Session["MULTIROLE"] != null && "Y".Equals(Session["MULTIROLE"].ToString()))
                    {
                        //Bind Roles
                        StringBuilder sbRoleQuery = new StringBuilder();
                        sbRoleQuery.Append(@"   select ROLE,roledesc from (
                                            (select distinct ROLE,decode(ROLE,'AC','Area Coordinator',
                                                'BE','Billing Engineer','RCM', 'RCM' ) roledesc  
                                                from RAB_TENDER_USERS  a 
                                            where empno=:EMPNO 
                                                and A.ACTIVE='Y')
                                           ) order by ROLE");                        
               

                        Dictionary<string, string> paramList = new Dictionary<string, string>();
                        paramList.Add("EMPNO", Session["USERID"].ToString());
                        if (sbRoleQuery.Length > 0)
                        {
                            objDB.bindDropDownList(ddRole, sbRoleQuery.ToString(), paramList, "ROLE", "roledesc", "", "--Select Role--");
                           
                        }
                    }
                }
            else
            {
                Response.Redirect("Login.aspx");
            }
            }
            
        }
  

    protected void ddRole_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddRole.SelectedValue.Length > 0)
        {         
            Session["ROLE"] = ddRole.SelectedValue;
            Session["UserType"] = ddRole.SelectedValue;
            Response.Redirect("Default.aspx");
            //FormsAuthentication.RedirectFromLoginPage(Session["USERID"].ToString(), false);
        }
    }
}