using System.Web;
using System.Text;
using System.Web.UI;

/// <summary>
/// A JavaScript alert
/// </summary>
public static class Alert
{

  /// <summary>
  /// Shows a client-side JavaScript alert in the browser.
  /// </summary>
  /// <param name="message">The message to appear in the alert.</param>
    public static void Show(string error)
  {

      Page page = HttpContext.Current.Handler as Page;
      if (page != null)
      {
          error = error.Replace("'", "\'");
          ScriptManager.RegisterStartupScript(page, page.GetType(), "err_msg", "alert('" + error + "');", true);
      }
        //for redirecting the page to some other page use 
      //ScriptManager.RegisterStartupScript(page, page.GetType(), "err_msg", "alert('" + error + "');window.location='newpage.aspx';", true);
  
  }
	
}
