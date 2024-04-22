<%@ Application Language="C#" %>

<script runat="server">

    void Application_BeginRequest()
    {
        //HttpContext.Current.Response.Cache.SetAllowResponseInBrowserHistory(false);
        //HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
        //HttpContext.Current.Response.Cache.SetNoStore();
        //Response.Cache.SetExpires(DateTime.Now);
        //Response.Cache.SetValidUntilExpires(true);
    }
    
    void Application_Start(object sender, EventArgs e) 
    {
        // Code that runs on application startup

    }
    
    void Application_End(object sender, EventArgs e) 
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs

    }

    void Session_Start(object sender, EventArgs e) 
    {
        // Code that runs when a new session is started

    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

    }

    protected void Application_PreSendRequestHeaders()
    {
        Response.Headers.Remove("Server");
        Response.Headers.Remove("X-AspNet-Version");
        Response.Headers.Remove("X-AspNetMvc-Version");
        Response.Headers.Remove("X-Powered-By");
        Response.Headers.Remove("X-SourceFiles");

    }
       
</script>
