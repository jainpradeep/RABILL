using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class RA_MenuPage : System.Web.UI.MasterPage
{
    private const string AntiXsrfTokenKey = "__AntiXsrfToken";
    private const string AntiXsrfUserNameKey = "__AntiXsrfUserName";
    private string _antiXsrfTokenValue;


    protected void Page_Load(object sender, EventArgs e)
    {

        HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
        HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        HttpContext.Current.Response.Cache.SetNoStore();
        Response.Cache.SetExpires(DateTime.Now.AddSeconds(-1));
        Response.Cache.SetValidUntilExpires(true);

        if (Session["USERID"] != null && Session["AuthToken"] != null
&& Request.Cookies["AuthToken"] != null) //token is there
        {
            if (!Session["AuthToken"].ToString().Equals(
            Request.Cookies["AuthToken"].Value))//check token value
            {
                Response.Redirect("Login.aspx");//invalid token               
            }
        }
        else
        {
            Response.Redirect("Login.aspx");//no token
        }

        if (Session["USERID"] == null)
        {
            Response.Redirect("Login.aspx");
        }
        else
        {
            Label UserName = (Label)HeadLoginView.FindControl("LoginName");
            try
            {
                UserName.Text = Session["NAME"].ToString();
            }
            catch (Exception err)
            {
                // UserName.Text = " ";
            }
            bindMenuItems();
        }
        RefreshAuthToken();
    }

    private void RefreshAuthToken()
    {
        string guid = Guid.NewGuid().ToString();
        Session["AuthToken"] = guid;
        Response.Cookies["AuthToken"].Value = guid;
    }

    protected void bindMenuItems()
    {
        if (Session["USERID"] != null && Session["ROLE"] != null && "VEND".Equals(Session["ROLE"].ToString()))
        {    

            //NavigationMenu.Items.Remove(NavigationMenu.Items[16]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[15]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[14]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[13]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[11]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[8]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[7]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[6]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[5]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[4]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[3]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[2]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[1]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[0]);

        }
        else if (Session["USERID"] != null && Session["ROLE"] != null && "BE".Equals(Session["ROLE"].ToString()))
        {
            //NavigationMenu.Items.Remove(NavigationMenu.Items[16]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[15]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[14]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[13]);     
            //NavigationMenu.Items.Remove(NavigationMenu.Items[11]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[7]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[6]); 
            NavigationMenu.Items.Remove(NavigationMenu.Items[5]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[4]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[3]);            
            //NavigationMenu.Items.Remove(NavigationMenu.Items[1]);            
        }
        else if (Session["USERID"] != null && Session["ROLE"] != null && ("AC".Equals(Session["ROLE"].ToString())))
        {
            // Showing to AC also RALSTKBills email by dhananjay date 04-May-2023
            // NavigationMenu.Items.Remove(NavigationMenu.Items[14]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[15]);      
            //NavigationMenu.Items.Remove(NavigationMenu.Items[13]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[11]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[7]); 
            //NavigationMenu.Items.Remove(NavigationMenu.Items[5]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[4]);
            NavigationMenu.Items.Remove(NavigationMenu.Items[0]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[3]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[2]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[1]);
        }
        else if (Session["USERID"] != null && Session["ROLE"] != null && ("RCM".Equals(Session["ROLE"].ToString())))
        {
            NavigationMenu.Items.Remove(NavigationMenu.Items[0]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[11]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[7]); 
            //NavigationMenu.Items.Remove(NavigationMenu.Items[5]);
            //NavigationMenu.Items.Remove(NavigationMenu.Items[2]);
        }

        //Finance Related login changes
        //else if (Session["USERID"] != null && Session["ROLE"] != null && "FA".Equals(Session["ROLE"].ToString()))
        //{
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[16]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[15]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[14]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[13]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[12]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[11]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[10]);
        //    //NavigationMenu.Items.Remove(NavigationMenu.Items[9]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[8]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[7]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[6]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[5]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[4]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[3]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[2]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[1]);
        //    NavigationMenu.Items.Remove(NavigationMenu.Items[0]);
        //}

        //If multiple role than display switch role screen
        if (Session["MULTIROLE"] != null && "Y".Equals(Session["MULTIROLE"].ToString()))
        {
            trSwitchRole.Visible = true;
            if (Session["ROLE"] != null)
            {
                if (Session["ROLE"].ToString().Equals("BE"))
                {
                    lblCurrentRole.Text = "Role: Billing Engineer";
                }
                else if (Session["ROLE"].ToString().Equals("AC"))
                {
                    lblCurrentRole.Text = "Role: Area Co-ordinator";
                }
                else if (Session["ROLE"].ToString().Equals("RCM"))
                {
                    lblCurrentRole.Text = "Role: RCM";
                }
                else
                {
                    lblCurrentRole.Text = "";
                }
            }
        }
        else
        {
            trSwitchRole.Visible = false;
        }
    }

    protected void clear_cookies(object sender, EventArgs e)
    {
        HttpCookie aCookie;
        string cookieName;
        int limit = Request.Cookies.Count - 1;
        for (int i = 0; i <= limit; i++)
        {
            cookieName = Request.Cookies[i].Name;
            aCookie = new HttpCookie(cookieName);
            aCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(aCookie);
        }

        //if (Request.Cookies["ASP.NET_SessionId"] != null)
        //{
        //    Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
        //    Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
        //}
        //if (Request.Cookies["AuthToken"] != null)
        //{
        //    Response.Cookies["AuthToken"].Value = string.Empty;
        //    Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
        //}



        Response.AddHeader("Pragma", "no-cache");
        Response.CacheControl = "no-cache";
        Response.Cache.SetAllowResponseInBrowserHistory(false);
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        Response.Expires = -1;
        //for AuthCookie
        if (Request.Cookies["ASP.NET_SessionId"] != null)
        {
            Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
            Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
        }
        if (Request.Cookies["AuthToken"] != null)
        {
            Response.Cookies["AuthToken"].Value = string.Empty;
            Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
        }
        if (HttpContext.Current.Request.Cookies["__AntiXsrfToken"] != null)
        {
            HttpContext.Current.Response.Cookies["__AntiXsrfToken"].Value = string.Empty;
            HttpContext.Current.Response.Cookies["__AntiXsrfToken"].Expires =
            DateTime.Now.AddMonths(-20);
        }
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        //First, check for the existence of the Anti-XSS cookie
        var requestCookie = Request.Cookies[AntiXsrfTokenKey];
        Guid requestCookieGuidValue;
        //If the CSRF cookie is found, parse the token from the cookie.
        //Then, set the global page variable and view state user
        //key. The global variable will be used to validate that it matches in the view
        //state form field in the Page.PreLoad
        //method.
        if (requestCookie != null && Guid.TryParse(requestCookie.Value, out
requestCookieGuidValue))
        {
            //Set the global token variable so the cookie value can be
            //validated against the value in the view state form field in
            //the Page.PreLoad method.
            _antiXsrfTokenValue = requestCookie.Value;
            //Set the view state user key, which will be validated by the
            //framework during each request
            Page.ViewStateUserKey = _antiXsrfTokenValue;
        }
        //If the CSRF cookie is not found, then this is a new session.
        else
        {
            //Generate a new Anti-XSRF token
            _antiXsrfTokenValue = Guid.NewGuid().ToString("N");
            //Set the view state user key, which will be validated by the
            //framework during each request
            Page.ViewStateUserKey = _antiXsrfTokenValue;
            //Create the non-persistent CSRF cookie
            var responseCookie = new HttpCookie(AntiXsrfTokenKey)
            {
                //Set the HttpOnly property to prevent the cookie from
                //being accessed by client side script
                HttpOnly = true,
                //Add the Anti-XSRF token to the cookie value
                Value = _antiXsrfTokenValue
            };
            //If we are using SSL, the cookie should be set to secure to
            //prevent it from being sent over HTTP connections
            //if (FormsAuthentication.RequireSSL &&
            //Request.IsSecureConnection)
            //responseCookie.Secure = true;
            //Add the CSRF cookie to the response
            Response.Cookies.Set(responseCookie);
        }
        Page.PreLoad += master_Page_PreLoad;
    }
    protected void master_Page_PreLoad(object sender, EventArgs e)
    {
        if (Session["USERID"] != null)
        {
            string emp_no = string.Empty;
            if (Session["USERID"] != null)
            {
                emp_no = Session["USERID"].ToString();
            }
            //During the initial page load, add the Anti-XSRF token and user
            //name to the ViewState
            if (!IsPostBack)
            {
                //Set Anti-XSRF token
                ViewState[AntiXsrfTokenKey] = Page.ViewStateUserKey;
                //If a user name is assigned, set the user name
                ViewState[AntiXsrfUserNameKey] = emp_no ?? String.Empty;
            }
            //During all subsequent post backs to the page, the token value from
            //the cookie should be validated against the token in the view state
            //form field. Additionally user name should be compared to the
            //authenticated users name
            else
            {
                //Validate the Anti-XSRF token
                if ((string)ViewState[AntiXsrfTokenKey] != _antiXsrfTokenValue ||
               (string)ViewState[AntiXsrfUserNameKey] != (emp_no ?? String.Empty))
                {
                    throw new InvalidOperationException("Validation of Anti-XSRF token failed.");
                }
            }
        }
        else
        {
            //signOut();
            clear_cookies(sender, e);
        }
    }
}
