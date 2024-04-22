using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            if (Session["USERID"] == null)
            {
                Response.Redirect("Login.aspx");
            }

            string guid = Guid.NewGuid().ToString();
            Session["AuthToken"] = guid;
            Response.Cookies.Add(new System.Web.HttpCookie("AuthToken", guid));

            string X = Session["ROLE"].ToString();

            if (Session["ROLE"] != null && "FA".Equals(Session["ROLE"].ToString()))
            {
                Response.Redirect("RA_Generate_Bill_sample.aspx");
            }
            else if (Session["USERID"] != null && Session["ROLE"] != null && !"FA".Equals(Session["ROLE"].ToString()))
            {
               // Response.Redirect("RA_Bill_Entry.aspx");
                Response.Redirect("RA_New_Bill_Entry.aspx");                
            }
            //else if (Session["USERID"] != null && Session["ROLE"] != null && "VEND".Equals(Session["ROLE"].ToString()))
            //{
            //   // Response.Redirect("RA_Bill_Entry.aspx");
            //    Response.Redirect("RA_New_Bill_Entry.aspx");  
            //}             
            else
            {
                Response.Redirect("Login.aspx");
            }
        }
    }
}