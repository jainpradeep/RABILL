using System;
using System.Web;
using System.Data;
//using Oracle.DataAccess;
//using Oracle.DataAccess.Types;
//using Oracle.DataAccess.Client;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Collections;
using System.Text;
using System.Data.OleDb;

namespace OracleDataAccess
{
    public class CreateOracleDataAccessObjects
    {
        protected string strDsn, strSql, ErrDesc, strORADsn;
        //static OracleDependency depObj;
        protected DataSet objDataSet;


        private OleDbConnection objConnection;
        private OleDbDataAdapter objAdapter;
        private OleDbCommand objCommand;
        private OleDbDataReader objDatareader;
        private OleDbCommandBuilder objCommandBuilder;

        private bool commit_trans = true;
        private OleDbTransaction objTransaction;

        //private OleDbTransaction objTransaction;
        public bool open_con;
        public bool grid_bind, list_bind, gridView_bind;
        public string ErrorStr;
        public bool err_flag;
        public string strSchStart, strSchComp, strExpComp, strCtrlMHrs, strActualProgress, strActualData;
        public Int32 ctobj;


        public CreateOracleDataAccessObjects(string strORADsn)
        {
            //strORADsn = ConfigurationManager.ConnectionStrings["Conn"].ConnectionString;

            //strORADsn = ConfigurationManager.AppSettings["DSN1"] + HttpContext.Current.Session["sJOBNO"] + ";password=" + HttpContext.Current.Session["sJOBNO"] + ";" + System.Configuration.ConfigurationManager.AppSettings["DSN2"];
            //string str1 = ConfigurationManager.AppSettings["DSN1"] + HttpContext.Current.Session["sJOBNO"];
            //string str2 = "password=" + HttpContext.Current.Session["sJOBNO"] + "";
            //string str3 = System.Configuration.ConfigurationManager.AppSettings["DSN2"];
        
        }

        /// <summary>
        /// OPEN CONNECTION
        /// RETURN NULL IF THERE IS SOME PROBLEM IN OPENING THE CONNECTION 
        /// </summary>
        /// <returns></returns>
        public OleDbConnection open_connection()
        {
            try
            {
                objConnection = new OleDbConnection(strORADsn);
                if (objConnection.State != ConnectionState.Open)
                {
                    objConnection.Open();
                }
                open_con = true;
                return objConnection;
            }
            catch (OleDbException objError)
            {
                if (objError.Message.Substring(0, 9) == "ORA-12154")
                {
                    ErrorStr = "Connection Failed : System Could not resolve Service Name";
                }
                else if (objError.Message.Substring(0, 9) == "ORA-01017")
                {
                    ErrorStr = "Invalid user Name/Password";
                }
                else if (objError.Message.Substring(0, 9) == "ORA-12203")
                {
                    ErrorStr = "Unable to Connect To Destination";
                }

                open_con = false;
                return null;
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                open_con = false;
                return null;
            }
        }



        public bool IsDataAvailable(string strSql)
        {
            try
            {
                objConnection = open_connection();
                objCommand = new OleDbCommand(strSql, objConnection);
                objDatareader = objCommand.ExecuteReader();
                if (objDatareader.HasRows)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (OleDbException objError)
            {
                ErrorStr = objError.Message;
                //                    ErrorMsg(objConnection, "MSG1107");
                err_flag = true;
                return false;
            }
            finally
            {
                objConnection.Close();
                objDatareader.Close();
                objCommand.Dispose();

            }
        }



        public string ErrorMsg(OleDbConnection objConnection, string ErrCode)
        {	//			objConnection=open_connection();
            strSql = "Select error_desc from STD_ERROR_DICTIONARY where error_code='" + ErrCode + "'";
            objCommand = new OleDbCommand(strSql, objConnection);
            objDatareader = objCommand.ExecuteReader();
            while (objDatareader.Read() == true)
            {
                ErrDesc = objDatareader["error_desc"].ToString();
            }
            objCommand.Dispose();
            objDatareader.Close();
            return ErrDesc;
        }

        public string ErrorMsg(string ErrCode)
        {
            objConnection = open_connection();
            strSql = "Select error_desc from STD_ERROR_DICTIONARY where error_code='" + ErrCode + "'";
            objCommand = new OleDbCommand(strSql, objConnection);
            objDatareader = objCommand.ExecuteReader();
            while (objDatareader.Read() == true)
            {
                ErrDesc = objDatareader["error_desc"].ToString();
            }
            objDatareader.Close();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return ErrDesc;
        }



        public OleDbDataReader DataReader(string strSql, OleDbConnection objConnection)
        {
            //objConnection=open_connection();
            //strSql="Select error_desc_eng from STD_ERROR_DICTIONARY where error_cd='" + ErrCode + "'";
            objCommand = new OleDbCommand(strSql, objConnection);
            objDatareader = objCommand.ExecuteReader();
            //objDatareader.Close();
            //objConnection.Close();
            objCommand.Dispose();
            return objDatareader;
        }

        public OleDbDataReader DataReader(string strSql)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            objDatareader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
            //objDatareader.Close();
            //objConnection.Close();
            //objCommand.Dispose();
            return objDatareader;
        }

        //public IDataReader DataReader(string strSql)
        //{
        //    objConnection = open_connection();
        //    objCommand = new OleDbCommand(strSql, objConnection);
        //    objDatareader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    objCommand.Dispose();
        //    return objDatareader;
        //}

        //public string ExecuteStatementString(string strSql)
        //{
        //    objConnection = open_connection();
        //    objCommand = new OleDbCommand(strSql, objConnection);
        //    objDatareader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    objCommand.Dispose();
        //    if (objDatareader.Read())
        //    {
        //        return Convert.ToString(objDatareader.GetValue(0));

        //        objConnection.Close();
        //        objConnection.Dispose();
        //    }
        //    else
        //    {
        //        return "";
        //        objConnection.Close();
        //        objConnection.Dispose();
        //    }


        //    //return objDatareader.GetString(0).ToString();
        //}

        //public string ExecuteStatementStringWithColumn(string strSql, string strColumnName)
        //{
        //    objConnection = open_connection();
        //    objCommand = new OleDbCommand(strSql, objConnection);
        //    objDatareader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    objCommand.Dispose();
        //    if (objDatareader.Read())
        //    {
        //        return Convert.ToString(objDatareader[strColumnName]);

        //        objConnection.Close();
        //        objConnection.Dispose();
        //    }
        //    else
        //    {
        //        return "";
        //        objConnection.Close();
        //        objConnection.Dispose();
        //    }


        //    //return objDatareader.GetString(0).ToString();
        //}

        public Int32 ExecuteStatementCount(string strSql)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            object obj = objCommand.ExecuteScalar();
            objCommand.Dispose();
            ctobj = Convert.ToInt32(obj);
            objConnection.Close();
            objConnection.Dispose();
            return ctobj;
        }

        //public IDataReader DataReader(string strSql, string strDSN)
        //{
        //    objConnection = open_connection(strDSN);
        //    objCommand = new OleDbCommand(strSql, objConnection);
        //    objDatareader = objCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    objCommand.Dispose();
        //    return objDatareader;
        //}


        //public IDataReader StoredDataReader(string strSP, params OleDbParameter[] paramObj)
        //{
        //    objConnection = open_connection();
        //    objCommand = new OleDbCommand(strSP, objConnection);
        //    objCommand.CommandType = CommandType.StoredProcedure;
        //    if (paramObj != null)
        //    {
        //        foreach (OleDbParameter paramVal in paramObj)
        //        {
        //            objCommand.Parameters.Add(paramVal);
        //        }
        //    }
        //    OleDbParameter oparam = objCommand.Parameters.Add("cur_user", OleDbType.RefCursor);
        //    oparam.Direction = ParameterDirection.Output;
        //    //depObj.AddCommandDependency(objCommand);
        //    //depObj.OnChange+=new OnChangeEventHandler(CreateDataAccessObjects.OnNotification);
        //    objCommand.ExecuteNonQuery();
        //    objDatareader = (OleDbDataReader)((OracleRefCursor)(oparam.Value)).GetDataReader();
        //    //objDatareader=objCommand.ExecuteReader(CommandBehavior.CloseConnection);
        //    //objDatareader.Close();
        //    //objConnection.Close();
        //    objCommand.Dispose();
        //    return objDatareader;
        //}

        //public DataSet Fetch_StoredDataSet(string strSP, string tablename, params OleDbParameter[] paramObj)
        //{
        //    objConnection = open_connection();
        //    objDataSet = new DataSet();
        //    objCommand = new OleDbCommand(strSP, objConnection);
        //    objCommand.CommandType = CommandType.StoredProcedure;
        //    if (paramObj != null)
        //    {
        //        foreach (OleDbParameter paramVal in paramObj)
        //        {
        //            objCommand.Parameters.Add(paramVal);
        //        }
        //    }
        //    OleDbParameter oparam = objCommand.Parameters.Add("cur_user", OleDbType.RefCursor);
        //    oparam.Direction = ParameterDirection.Output;
        //    objAdapter = new OleDbDataAdapter(objCommand);
        //    objAdapter.Fill(objDataSet, tablename);
        //    //objDatareader.Close();
        //    objAdapter.Dispose();
        //    objConnection.Close();
        //    objConnection.Dispose();
        //    return objDataSet;
        //}

        public void ExcecuteStoredProcedure(string strSP, params OleDbParameter[] paramObj)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSP, objConnection);
            objCommand.CommandType = CommandType.StoredProcedure;
            if (paramObj != null)
            {
                foreach (OleDbParameter paramVal in paramObj)
                {
                    objCommand.Parameters.Add(paramVal);
                }
            }
            objCommand.ExecuteNonQuery();
            objCommand.Dispose();
            objConnection.Close();
            objConnection.Dispose();
        }

        public DataSet Fetch_DataSet(string strSql)
        {
            objConnection = open_connection();
            objDataSet = new DataSet();
            objAdapter = new OleDbDataAdapter(strSql, objConnection);
            objAdapter.Fill(objDataSet);
            //objDatareader.Close();
            objAdapter.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return objDataSet;
        }

        public DataSet Fetch_DataSet(string strSql, string tablename)
        {
            objConnection = open_connection();
            objDataSet = new DataSet();
            objAdapter = new OleDbDataAdapter(strSql, objConnection);
            objAdapter.Fill(objDataSet, tablename);
            //objDatareader.Close();
            objAdapter.Dispose();
            objConnection.Close();
            objConnection.Dispose();
            return objDataSet;
        }


        public string execute(string strSql, string strType, params OleDbParameter[] paramObj)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            if (strType == "Text")
            {
                objCommand.CommandType = CommandType.Text;
            }
            else if (strType == "Proc")
            {
                objCommand.CommandType = CommandType.StoredProcedure;
            }
            if (paramObj != null)
            {
                foreach (OleDbParameter paramVal in paramObj)
                {
                    objCommand.Parameters.Add(paramVal);
                }
            }
            try
            {
                objCommand.ExecuteNonQuery();
                err_flag = false;
                ErrorStr = "";
            }
            catch (OleDbException objError)
            {
                if (objError.Message.Substring(0, 21) == "Table does not exist.")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 28) == "ORA-00001: unique constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1103");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 31) == "ORA-02292: integrity constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1107");
                    err_flag = true;
                }
                else if ((objError.Message.Length >= 87) && (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name"))
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    err_flag = true;
                }
                else
                {
                    ErrorStr = objError.Message;
                    err_flag = true;
                }
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;
            }
            finally
            {
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            return ErrorStr;
        }

        public string execute_sql(string strSql)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            objCommand.CommandType = CommandType.Text;
            try
            {
                objCommand.ExecuteNonQuery();
                ErrorStr = "";
                err_flag = false;
            }
            //catch (OleDbException objError)
            //{
            //    if (objError.Message.Substring(0, 21) == "Table does not exist.")
            //    {
            //        ErrorStr = ErrorMsg(objConnection, "MSG1201");
            //        err_flag = true;
            //    }
            //    else if (objError.Message.Substring(0, 28) == "ORA-00001: unique constraint")
            //    {
            //        ErrorStr = ErrorMsg(objConnection, "MSG1103");
            //        err_flag = true;
            //    }
            //    else if (objError.Message.Substring(0, 31) == "ORA-02292: integrity constraint")
            //    {
            //        ErrorStr = ErrorMsg(objConnection, "MSG1107");
            //        err_flag = true;
            //    }
            //    else if ((objError.Message.Length >= 87) && (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name"))
            //    {
            //        ErrorStr = ErrorMsg(objConnection, "MSG1202");
            //        err_flag = true;
            //    }
            //    else
            //    {
            //        ErrorStr = objError.Message;
            //        err_flag = true;
            //    }
            //}
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;
            }
            finally
            {
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            return ErrorStr;
        }

        public string execute_sql(string strSql, string strType)
        {
            objConnection = open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            objCommand.CommandType = CommandType.Text;
            try
            {
                objCommand.ExecuteNonQuery();
                ErrorStr = ErrorMsg(objConnection, "MSG-0007");
                err_flag = false;
                return ErrorStr;
            }
            catch (OleDbException objError)
            {
                if (objError.Message.Substring(0, 21) == "Table does not exist.")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 28) == "ORA-00001: unique constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1103");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 31) == "ORA-02292: integrity constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1107");
                    err_flag = true;
                }
                else if ((objError.Message.Length >= 87) && (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name"))
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    err_flag = true;
                }
                else
                {
                    ErrorStr = objError.Message;
                    err_flag = true;
                }
                return ErrorStr;
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;
                return ErrorStr;
            }
            finally
            {

                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
        }

        public void trans_execute_sql(string strSql, OleDbConnection objConnection, OleDbTransaction objTransaction)
        {
            //objConnection=open_connection();
            objCommand = new OleDbCommand(strSql, objConnection);
            objCommand.CommandType = CommandType.Text;
            try
            {
                //objCommand.Transaction=objTransaction;
                objCommand.ExecuteNonQuery();
                err_flag = false;
            }
            catch (OleDbException objError)
            {
                if (objError.Message.Substring(0, 21) == "Table does not exist.")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 28) == "ORA-00001: unique constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1103");
                    err_flag = true;
                }
                else if (objError.Message.Substring(0, 31) == "ORA-02292: integrity constraint")
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1107");
                    err_flag = true;
                }
                else if ((objError.Message.Length >= 87) && (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name"))
                {
                    ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    err_flag = true;
                }
                else
                {
                    ErrorStr = objError.Message;
                    err_flag = true;
                }
                objTransaction.Rollback();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();

            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                objTransaction.Rollback();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
                err_flag = true;
            }
            /*finally
            {
                //objConnection.Close(); 			
                null;
            }*/
        }

        //public void SetFocus(Control control)
        //{
        //    StringBuilder sb = new StringBuilder();

        //    sb.Append("\r\n<script language='JavaScript'>\r\n");
        //    sb.Append("<!--\r\n");
        //    sb.Append("function SetFocus()\r\n");
        //    sb.Append("{\r\n");
        //    sb.Append("\tdocument.");

        //    Control p = control.Parent;
        //    while (!(p is System.Web.UI.HtmlControls.HtmlForm)) p = p.Parent;

        //    sb.Append(p.ClientID);
        //    sb.Append("['");
        //    sb.Append(control.UniqueID);
        //    sb.Append("'].focus();\r\n");
        //    sb.Append("}\r\n");
        //    sb.Append("window.onload = SetFocus;\r\n");
        //    sb.Append("// -->\r\n");
        //    sb.Append("</script>");

        //    control.Page.RegisterClientScriptBlock("SetFocus", sb.ToString());
        //}

        public DataGrid BindGrid(DataGrid dgrdGen, string strSql)
        {
            objConnection = open_connection();
            if (open_con == true)
            {
                try
                {
                    objDataSet = new DataSet();
                    objAdapter = new OleDbDataAdapter(strSql, objConnection);
                    objAdapter.Fill(objDataSet, "role_id");
                    dgrdGen.DataSource = objDataSet.Tables["role_id"].DefaultView;
                    dgrdGen.DataBind();
                    grid_bind = true;
                    return dgrdGen;
                }
                catch (OleDbException objError)
                {
                    if (objError.Message.Substring(0, 21) == "Table does not exist.")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    }
                    if (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    }
                    grid_bind = false;
                    return null;
                }
                finally
                {
                    objConnection.Close();
                    objConnection.Dispose();
                    objAdapter.Dispose();
                }
            }
            else
            {
                return null;
            }
        }


        public GridView BindGridView(GridView grdVGenIn, string strSqlIn)
        {
            objConnection = open_connection();
            if (open_con == true)
            {
                try
                {
                    objDataSet = new DataSet();
                    objAdapter = new OleDbDataAdapter(strSqlIn, objConnection);
                    objAdapter.Fill(objDataSet, "tblGrdV");
                    grdVGenIn.DataSource = objDataSet.Tables["tblGrdV"].DefaultView;
                    grdVGenIn.DataBind();
                    gridView_bind = true;
                    return grdVGenIn;
                }
                catch (OleDbException objError)
                {
                    if (objError.Message.Substring(0, 21) == "Table does not exist.")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    }
                    else if (objError.Message.Substring(59, 25) == "ORA-00904: invalid column name")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    }
                    else
                    {
                        ErrorStr = objError.Message;
                        err_flag = true;
                    }
                    gridView_bind = false;
                    return null;
                }
                finally
                {
                    objConnection.Close();
                    objConnection.Dispose();
                    objAdapter.Dispose();
                }
            }
            else
            {
                return null;
            }
        }


        public GridView ClearGridView(GridView grdVGenIn)
        {
            grdVGenIn.DataSource = null;
            grdVGenIn.DataBind();
            return null;
        }

        public FormView BindFormView(FormView grdVGenIn, string strSqlIn)
        {
            objConnection = open_connection();
            if (open_con == true)
            {
                try
                {
                    objDataSet = new DataSet();
                    objAdapter = new OleDbDataAdapter(strSqlIn, objConnection);
                    objAdapter.Fill(objDataSet, "tblGrdV");
                    grdVGenIn.DataSource = objDataSet.Tables["tblGrdV"].DefaultView;
                    grdVGenIn.DataBind();
                    gridView_bind = true;
                    return grdVGenIn;
                }
                catch (OleDbException objError)
                {
                    if (objError.Message.Substring(0, 21) == "Table does not exist.")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    }
                    else if (objError.Message.Substring(59, 25) == "ORA-00904: invalid column name")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    }
                    else
                    {
                        ErrorStr = objError.Message;
                        err_flag = true;
                    }
                    gridView_bind = false;
                    return null;
                }
                finally
                {
                    objConnection.Close();
                    objConnection.Dispose();
                    objAdapter.Dispose();
                }
            }
            else
            {
                return null;
            }
        }


        public FormView ClearFormView(FormView grdVGenIn)
        {
            grdVGenIn.DataSource = null;
            grdVGenIn.DataBind();
            return null;
        }

        public ListBox fill_listbox(ListBox lstGen, string strSql, string strValue, string strText, string strExtraValue, string strExtraText)
        {
            objConnection = open_connection();
            if (open_con == true)
            {
                try
                {
                    objCommand = new OleDbCommand(strSql, objConnection);
                    objDatareader = objCommand.ExecuteReader();
                    lstGen.DataSource = objDatareader;
                    lstGen.DataTextField = strText;
                    lstGen.DataValueField = strValue;
                    lstGen.DataBind();
                    objDatareader.Close();
                    objConnection.Close();
                    if (strExtraText != "")
                    {
                        lstGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
                    }
                    if (lstGen.Items.Count > 0)
                    {
                        lstGen.SelectedIndex = 0;
                    }
                    list_bind = true;
                    return lstGen;
                }
                catch (OleDbException objError)
                {
                    if (objError.Message.Substring(0, 21) == "Table does not exist.")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    }
                    list_bind = false;
                    return null;
                }
                finally
                {
                    objCommand.Dispose();
                    objConnection.Close();
                    objConnection.Dispose();
                }
            }
            else
            {
                return null;
            }
        }

        public void MoveItems(bool isAdd, ListBox ListBox1, ListBox ListBox2)
        {
            if (isAdd)// means if you add items to the right box
            {
                for (int i = ListBox1.Items.Count - 1; i >= 0; i--)
                {
                    if (ListBox1.Items[i].Selected)
                    {
                        ListBox2.Items.Add(ListBox1.Items[i]);
                        ListBox2.ClearSelection();
                        ListBox1.Items.Remove(ListBox1.Items[i]);
                    }
                }
            }
            else // means if you remove items from the right box and add it back to the left box
            {
                for (int i = ListBox2.Items.Count - 1; i >= 0; i--)
                {
                    if (ListBox2.Items[i].Selected)
                    {
                        ListBox1.Items.Add(ListBox2.Items[i]);
                        ListBox1.ClearSelection();
                        ListBox2.Items.Remove(ListBox2.Items[i]);
                    }
                }
            }
            if (ListBox1.Items.Count > 0)
            {
                ListBox1.SelectedIndex = 0;
            }
            if (ListBox2.Items.Count > 0)
            {
                ListBox2.SelectedIndex = 0;
            }
        }

        public void MoveAllItems(bool isAddAll, ListBox ListBox1, ListBox ListBox2)
        {
            if (isAddAll)// means if you add ALL items to the right box
            {
                for (int i = ListBox1.Items.Count - 1; i >= 0; i--)
                {
                    ListBox2.Items.Add(ListBox1.Items[i]);
                    ListBox2.ClearSelection();
                    ListBox1.Items.Remove(ListBox1.Items[i]);
                }
            }
            else // means if you remove ALL items from the right box and add it back to the left box
            {
                for (int i = ListBox2.Items.Count - 1; i >= 0; i--)
                {
                    ListBox1.Items.Add(ListBox2.Items[i]);
                    ListBox1.ClearSelection();
                    ListBox2.Items.Remove(ListBox2.Items[i]);
                }
            }
            if (ListBox1.Items.Count > 0)
            {
                ListBox1.SelectedIndex = 0;
            }
            if (ListBox2.Items.Count > 0)
            {
                ListBox2.SelectedIndex = 0;
            }
        }

        public DropDownList populate_list(DropDownList ddlistGen, string strSql, string strValue, string strText, string strExtraValue, string strExtraText)
        {
            objConnection = open_connection();
            if (open_con == true)
            {
                try
                {

                    objCommand = new OleDbCommand(strSql, objConnection);
                    objDatareader = objCommand.ExecuteReader();
                    ddlistGen.DataSource = objDatareader;

                    ddlistGen.DataTextField = strText;
                    ddlistGen.DataValueField = strValue;
                    ddlistGen.DataBind();
                    objDatareader.Close();
                    objConnection.Close();
                    if (strExtraText != "")
                    {
                        ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
                        ddlistGen.SelectedIndex = 0;
                    }

                    list_bind = true;
                    return ddlistGen;
                }
                catch (OleDbException objError)
                {
                    if (objError.Message.Substring(0, 21) == "Table does not exist.")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1201");
                    }
                    if (objError.Message.Substring(59, 30) == "ORA-00904: invalid column name")
                    {
                        ErrorStr = ErrorMsg(objConnection, "MSG1202");
                    }
                    list_bind = false;
                    return null;
                }
                finally
                {
                    objCommand.Dispose();
                    objConnection.Close();
                    objConnection.Dispose();
                }
            }
            else
            {
                return null;
            }
        }


        public string execute_transaction(string[] strSql)
        {
            objConnection = open_connection();
            try
            {
                if (open_con)
                {
                    objTransaction = objConnection.BeginTransaction();

                    Int32 i = 0;
                    string strQuery = "";
                    for (i = 0; i <= strSql.Length - 1; i++)
                    {
                        strQuery = "";
                        strQuery = strSql[i];

                        if (strQuery.Length != 0)
                        {
                            objCommand = new OleDbCommand(strQuery, objConnection);
                            objCommand.CommandType = CommandType.Text;
                            try
                            {
                                objCommand.ExecuteNonQuery();
                                err_flag = false;
                            }
                            catch (Exception objError)
                            {
                                ErrorStr = objError.Message;
                                err_flag = true;

                                objTransaction.Rollback();
                                objCommand.Dispose();
                                objConnection.Close();
                                objConnection.Dispose();
                            }
                        }
                    }

                    if (!err_flag)
                    {
                        objTransaction.Commit();
                        objCommand.Dispose();
                        objConnection.Close();
                        objConnection.Dispose();
                    }
                    else
                    {
                        objTransaction.Rollback();
                        objCommand.Dispose();
                        objConnection.Close();
                        objConnection.Dispose();
                    }
                }
                else
                {
                    ErrorStr = "Error : Unable to connect to database.";
                    err_flag = true;
                }
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;

                objTransaction.Rollback();
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            finally
            {
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            return ErrorStr;
        }

        public DataSet createDataSet(string strQuery, string strTableName)
        {
            DataSet ds = new DataSet();
            objConnection = open_connection();
            try
            {
                if (open_con)
                {
                    objAdapter = new OleDbDataAdapter(strQuery, objConnection);
                    objCommandBuilder = new OleDbCommandBuilder(objAdapter);
                    objAdapter.Fill(ds, strTableName);

                    err_flag = false;
                }
                else
                {
                    ErrorStr = "Error : Unable to connect to database.";
                    err_flag = true;
                }
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;

                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            finally
            {
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            return ds;
        }


        public string saveDataTable(string strQuery, DataSet ds)
        {
            objConnection = open_connection();
            try
            {
                if (open_con)
                {
                    objAdapter.SelectCommand = new OleDbCommand(strQuery, objConnection);
                    objCommandBuilder = new OleDbCommandBuilder(objAdapter);

                    Int32 i = 0;
                    for (i = 0; i <= ds.Tables.Count - 1; i++)
                    {
                        string strTableName = "";
                        strTableName = ds.Tables[i].TableName;

                        objAdapter.Update(ds, strTableName);
                    }

                    err_flag = false;
                }
                else
                {
                    ErrorStr = "Error : Unable to connect to database.";
                    err_flag = true;
                }
            }
            catch (Exception objError)
            {
                ErrorStr = objError.Message;
                err_flag = true;

                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            finally
            {
                objCommand.Dispose();
                objConnection.Close();
                objConnection.Dispose();
            }
            return ErrorStr;
        }
    }
}

