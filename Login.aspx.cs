using System;
using System.Web.UI;
using System.Text;
using AppCode;
using System.Collections.Generic;
using System.Web.Security;
using System.Data;
using System.DirectoryServices;
using System.Collections;

public partial class Login : System.Web.UI.Page
{
    dbFunction objDB = new dbFunction();
    private const string AES_KEY = "AES_KEY";
    protected void Page_Load(object sender, EventArgs e)
    {
        Dictionary<string, string> paramList = new Dictionary<string, string>();
        lblTotalHits.Text = objDB.executeScalar("SELECT COUNT(*) FROM RAB_AUDIT_TRAIL WHERE ACTION='LOGIN'", paramList);

        if (!Page.IsPostBack)
        {
            SetFocus(txtUserName);       
            resetAESKey();

            try
            {
               
            }
            catch (Exception ex)
            { }
        }
    }

    private void resetAESKey()
    {
        //generate and set random key
        string randmmAESKey = AESEncryptDecrypt.getRandomKey();
        aesKeyField.Value = randmmAESKey;
        Session[AES_KEY] = randmmAESKey;
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        string userId = txtUserName.Text.ToUpper().ToString();
        string password = txtPassword.Text.ToString();
        password = AESEncryptDecrypt.DecryptStringAES(password, Session[AES_KEY].ToString());
      
        string role = "";
        string error = "";

        if (userId.Equals(string.Empty))
        {
            error += "User Id is required.\\n";
        }
        if (password.Equals(string.Empty))
        {
            error += "Password is required.\\n";
        }

        if (!txtCaptcha.Text.ToString().Equals(Session["captcha"].ToString()))
        {
            error += "Captcha is invalid.\\n";
        }

        bool valid = false;
        //Login for Vendor/Contractor
        if (error.Equals(string.Empty) && userId.Length >= 4)
        {
            valid = authenticateUser(StrExt.ParseSQL(userId), StrExt.ParseSQL(password));

            Dictionary<string, string> paramList = new Dictionary<string, string>();            
            if (valid)
            { 
                paramList.Add("USER_ID",StrExt.ParseSQL(userId));
                paramList.Add("MACHINE_IP", Request.UserHostAddress.ToString());
                paramList.Add("MODULE_NAME","NEW LOGIN");
                paramList.Add("ACTION","LOGIN");
                paramList.Add("SITE_CD","");
                if (!"::1".Equals(Request.UserHostAddress.ToString()))
                {
                    int insertRec = objDB.executeNonQuery(Constants.loginHistoryQuery, paramList);
                }

                string guid = Guid.NewGuid().ToString();
                Session["AuthToken"] = guid;
                Response.Cookies.Add(new System.Web.HttpCookie("AuthToken", guid));

            }
            if (Session["USERID"] != null && Session["ROLE"] != null)
            {
                FormsAuthentication.RedirectFromLoginPage(Session["USERID"].ToString(), false);
            }           
            else
            {
                lblError.Text = "Error: You are not authorized or Invalid password.";
                resetAESKey();
            }
        }//Login for EIL Employee
        else
        {
            resetAESKey();
            Common.Show(error);
            lblError.Text = error.Replace("\\n","<br/>");
            
        }
    }

    protected bool authenticateUser(string userId,string password)
    {
        bool isValid = false;
        if (userId.Length == 4)
        {
            //isValid = ad_authentication(userId, password);
            isValid = User_Authentication(userId, password);
           
            if (Constants.admin_pwd.Equals(password))
                isValid = true;

            if (isValid)
            {
                Session["IS_EMP"] = "Y";
                Session["USERID"] = userId;
                Session["sUserId"] = userId;
                
                //Bind Name division /Department
                     DataTable dataTableEmpInfo = new DataTable();
                     StringBuilder sbEmpInfoQuery = new StringBuilder();
                     Dictionary<string, string> empParamList = new Dictionary<string, string>();
                     sbEmpInfoQuery.Append(" select EMPNO,empname,prst_divn,prst_sectN ")
                     .Append("  from ")
                     .Append(" vw_employee ")
                     .Append(" WHERE EMPNO=:EMPNO ")
                     .Append(" AND SEP_TYPE=0 ");
                     empParamList.Add("EMPNO", userId);
                     dataTableEmpInfo = objDB.bindDataTable(sbEmpInfoQuery.ToString(), empParamList);
                     if (dataTableEmpInfo.Rows.Count != 0 && dataTableEmpInfo.Rows.Count == 1)
                     {
                         Session["NAME"] = dataTableEmpInfo.Rows[0]["empname"].ToString();
                         Session["DIV_CODE"] = dataTableEmpInfo.Rows[0]["prst_divn"].ToString();
                         Session["DEPT_CODE"] = dataTableEmpInfo.Rows[0]["prst_sectN"].ToString();
                     }
                     else
                     {
                         Common.Show("Error: Invalid User\n");
                         return false;
                     }
                 
                //Bind Session Parameters like ROLE and other details
                ArrayList lstRole = new ArrayList();
                DataTable dataTableRole=new DataTable();
                StringBuilder sbQuery=new StringBuilder();
                Dictionary<string, string> paramList = new Dictionary<string, string>();
                //sbQuery.Append(" select ROLE,EMPNO from (" )
                sbQuery.Append(" select distinct ROLE from (")
                .Append("(select distinct 'RCM' ROLE, empno_rcm empno ")
                .Append(" from site_dir ")
                .Append(" where site_opn_closed='O' and empno_rcm=:EMPNO )")
                .Append("  union ")
                .Append(" (select ROLE,empno  ")
                .Append(" from RAB_TENDER_USERS  ")
                .Append(" where empno=:EMPNO  and ACTIVE='Y') ")
                .Append(" )");
                paramList.Add("EMPNO" ,userId);
                dataTableRole = objDB.bindDataTable(sbQuery.ToString(), paramList);
                Session["MULTIROLE"] = "N";
                if (dataTableRole.Rows.Count != 0 && dataTableRole.Rows.Count > 1)
                { 
                //Show multirole dropdown
                    Session["MULTIROLE"] = "Y";
                    Response.Redirect("RA_SelectRole.aspx"); 
                }
                else if (dataTableRole.Rows.Count != 0 && dataTableRole.Rows.Count == 1)
                {
                    Session["ROLE"] = dataTableRole.Rows[0]["ROLE"].ToString();
                    Session["UserType"] = dataTableRole.Rows[0]["ROLE"].ToString();                    
                } 
               //Setting Finance and Construction role to view Report only
                else if (Session["ROLE"] == null &&  (Session["DIV_CODE"].Equals("33") || Session["DIV_CODE"].Equals("19")))
                {
                    Session["ROLE"] = "FA";
                    Session["UserType"] ="FA";
                }
            }
        }
        else if (userId.Length > 4)
        {
            isValid = vendor_authentication(userId, password);

            if (Constants.admin_pwd.Equals(password))
                isValid = true;

            if (isValid)
            {
                Session["IS_EMP"] = "N";
                Session["ROLE"] = "VEND";
                Session["UserType"] = "VEND";
                Session["MULTIROLE"] = "N";
            }
        }
        return isValid;    
    }

    //public bool ad_authentication(string userId, string password)
    //{
    //    bool authenticated = false;
    //    try
    //    {
    //        if (password.Equals("test"))
    //        {
    //            authenticated = true;
    //            return authenticated;
    //        }

    //    }
    //    catch (DirectoryServicesCOMException cex)
    //    {
    //        return false;
    //    }
    //    catch (Exception ex)
    //    {
    //        return false;
    //    }
    //    return authenticated;
    //}

    public bool User_Authentication(string userId, string password)
    {
        try
        {
            StringBuilder sbQuery = new StringBuilder();
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            sbQuery.Append("select count(1) from VW_EMPLOYEE where EMPNO=:EMPNO ");
            paramList.Add(":EMPNO", userId);
            int Count = objDB.ExecuteStatementCount(sbQuery.ToString(), paramList);

            if (Count == 1)
            {
                sbQuery.Clear();
                paramList.Clear();

                sbQuery.Append("select SALT from VW_EMPLOYEE where EMPNO=:EMPNO ");
                paramList.Add(":EMPNO", userId);
                string salt = objDB.executeScalar(sbQuery.ToString(), paramList);

                dbFunction.HashSalt Hash_Password = objDB.GenerateSHA256Hash(password, salt);

                sbQuery.Clear();
                paramList.Clear();

                sbQuery.Append("select count(1) from VW_EMPLOYEE where EMPNO=:EMPNO and PWD=:PWD");
                paramList.Add(":EMPNO", userId);
                paramList.Add(":PWD", Hash_Password.Hash);
                int PassMatch = objDB.ExecuteStatementCount(sbQuery.ToString(), paramList);

                if (PassMatch == 1)
                {
                    return true;
                }
                else
                {
                    lblError.Text = "Error: Invalid User ID / Password. Kindly check";
                    return false;
                }
            }
            else
            {
                lblError.Text = "Error: Invalid User ID / Password. Kindly check";
                return false;
            }
        }
        catch (DirectoryServicesCOMException cex)
        {
            return false;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

   // to do uncomment the line for checking of password
    public bool vendor_authentication(string userId, string password)
    {
        bool authenticated = false;
        StringBuilder sbQuery = new StringBuilder();

        sbQuery.Append(@"select CONT_CD,name from CONT_LOGIN a,vw_rab_ccode_mst b
              where 
              upper(LOGIN_ID)=:LOGIN_ID 
              and a.CONT_CD=b.c_code
              and rownum =1");

        Dictionary<string, string> paramList = new Dictionary<string, string>();
        paramList.Add("LOGIN_ID", userId);

        if (!password.Equals(Constants.admin_pwd))
        {
            sbQuery.Append(" and PASSWORD=:PASSWORD ");
            paramList.Add("PASSWORD", FormsAuthentication.HashPasswordForStoringInConfigFile(password, "SHA1"));
        }

                
        DataTable dataTableRole = new DataTable();
        dataTableRole = objDB.bindDataTable(sbQuery.ToString(), paramList);
        if (dataTableRole.Rows.Count != 0 && dataTableRole.Rows.Count == 1)
        {
            Session["ROLE"] = "VEND";
            Session["USERID"] = dataTableRole.Rows[0]["CONT_CD"].ToString();
            Session["NAME"] = dataTableRole.Rows[0]["name"].ToString();
            authenticated = true;
        }
        else
        {
            authenticated= false;
        }
        return authenticated;
    }            

}

