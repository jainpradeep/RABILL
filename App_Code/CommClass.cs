using System;
using System.Data;
using System.Data.OleDb;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Configuration;
using System.Text;
namespace CommClass
{
	/// <summary>
	/// Summary description for CommClass.
	/// </summary>
	public class ContComClass
	{
		private string strDsn,strSql,ErrDesc;
		private OleDbConnection objConnection;
		private OleDbDataAdapter objAdapter;
		private DataSet objDataSet;
		private OleDbCommand objCommand;
		private System.Data.OleDb.OleDbDataReader objDatareader;
		public bool open_con;
		public string ErrorStr;
		public bool err_flag;
		public bool grid_bind,list_bind;
		//private OleDbDataReader objDatareader;
	
		public ContComClass()
		{
            //strDsn =  new OleDbConnection(System.Configuration.ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString);
            strDsn = System.Configuration.ConfigurationManager.ConnectionStrings["raConnection1"].ConnectionString;
            //strDsn = System.Configuration.ConfigurationManager.AppSettings["raConnection"];
		}

		public string ErrorMsg(string ErrCode)
		{
			objConnection=open_connection();
			strSql="Select error_desc_eng from ERROR_DICTIONARY where error_cd='" + ErrCode + "'";
			try
			{
				objCommand=new OleDbCommand(strSql,objConnection);
				objDatareader=objCommand.ExecuteReader();
				while(objDatareader.Read()==true)
				{
					ErrDesc=objDatareader["error_desc_eng"].ToString();
				}
				return ErrDesc;
			}
			finally
			{
				objDatareader.Close();
				objCommand.Dispose();
				objConnection.Close();
				objConnection.Dispose();	
			}
		}
		//
			public DataSet Fetch_DataSet(string strSql,string tablename)
			{
				objConnection=open_connection();
				objDataSet=new DataSet();
				objAdapter= new OleDbDataAdapter(strSql,objConnection);
				objAdapter.Fill(objDataSet,tablename);
				//objDatareader.Close();
				objAdapter.Dispose();
				objConnection.Close();
				objConnection.Dispose();
				return objDataSet;
			}

		//
		protected void ddlRefno_SelectedIndexChanged(object sender, EventArgs e)
		{
		
		}
		
		//
		public void SetFocus(Control control)
		{
			StringBuilder sb = new StringBuilder();
 
			sb.Append("\r\n<script language='JavaScript'>\r\n");
			sb.Append("<!--\r\n"); 
			sb.Append("function SetFocus()\r\n"); 
			sb.Append("{\r\n"); 
			sb.Append("\tdocument.");
 
			Control p = control.Parent;
			while (!(p is System.Web.UI.HtmlControls.HtmlForm)) p = p.Parent; 
 
			sb.Append(p.ClientID);
			sb.Append("['"); 
			sb.Append(control.UniqueID); 
			sb.Append("'].focus();\r\n"); 
			sb.Append("}\r\n"); 
			sb.Append("window.onload = SetFocus;\r\n"); 
			sb.Append("// -->\r\n"); 
			sb.Append("</script>");
			
			control.Page.RegisterClientScriptBlock("SetFocus", sb.ToString());
		}

//			
			private bool validateUser(string id, string Pwd)

			{
				bool result=false;
				objConnection=open_connection();
				strSql="Select site_cd,cont_cd,login_id,password from cont_login where Login_id='"+id+"'";
				try
				{
					objCommand=new OleDbCommand(strSql,objConnection);
					objDatareader=objCommand.ExecuteReader();
					if(!objDatareader.Read())
					{
						result=false; // not found
					}
					else
					{
						if (objDatareader.IsDBNull(2))
							result=false;
						else
						{
							result = (String.Equals(Pwd,objDatareader.GetString(3)));
							
						}
					}
				}
				finally
				{
					objDatareader.Close();
					objCommand.Dispose();
					objConnection.Close();
					objConnection.Dispose();	
				}
				return result;
				
		}

//		private bool CheckUserLoc(string id)
//		{
//			bool result=false;
//			objConnection=open_connection();
//			strSql="Select site_cd,cont_cd,login_id,password from cont_login where Login_id='"+id+"'";
//			try
//			{
//				objCommand=new OleDbCommand(strSql,objConnection);
//				objDatareader=objCommand.ExecuteReader();
//				if(!objDatareader.Read())
//				{
//					result=false; // not found
//				}
//				else
//				{
//					if (objDatareader.IsDBNull(2))
//						result=false;
//					else
//					{
//						result = (String.Equals(Pwd,objDatareader.GetString(3)));
//							
//					}
//				}
//			}
//			finally
//			{
//				objDatareader.Close();
//				objCommand.Dispose();
//				objConnection.Close();
//				objConnection.Dispose();	
//			}
//			return result;
//				
//		}
		 
		 
		//
		public DropDownList PopList(DropDownList ddlistGen,string strSql,string strValue,string strText,string strExtraValue,string strExtraText)
		{
			objConnection=open_connection();
			if (open_con==true )
			{
				try
				{
					objCommand=new OleDbCommand(strSql,objConnection);
					objDatareader=objCommand.ExecuteReader();
					ddlistGen.DataSource=objDatareader;
					ddlistGen.DataTextField=strText;
					ddlistGen.DataValueField=strValue;
					ddlistGen.DataBind();
					objDatareader.Close();
					objConnection.Close();
					if (strExtraText != "" )
					{
						ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
						ddlistGen.SelectedIndex=0;
					}

					list_bind=true;
					return ddlistGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1104");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1107");
					}	
					//ErrorStr=objError.Message.Substring(59,30);
					//One or more errors occurred during processing of command. ORA-00904: invalid column name 
					//ErrorStr=ErrorMsg(objConnection,"MSG1201");
					list_bind=false;
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
		public DropDownList pop_list(DropDownList ddlistGen,string strSql,string strValue,string strText)
		{
			objConnection=open_connection();
			if (open_con==true )
			{
				try
				{
					objCommand=new OleDbCommand(strSql,objConnection);
					objDatareader=objCommand.ExecuteReader();
					ddlistGen.DataSource=objDatareader;
					ddlistGen.DataTextField=strText;
					ddlistGen.DataValueField=strValue;
					ddlistGen.DataBind();
					objDatareader.Close();
					objConnection.Close();
					//					if (strExtraText != "" )
					//					{
					//ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
					ddlistGen.SelectedIndex=0;
					//}

					list_bind=true;
					return ddlistGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1104");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1107");
					}	
					//ErrorStr=objError.Message.Substring(59,30);
					//One or more errors occurred during processing of command. ORA-00904: invalid column name 
					//ErrorStr=ErrorMsg(objConnection,"MSG1201");
					list_bind=false;
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

		//
		public DropDownList populate_list(DropDownList ddlistGen,string strSql,string strValue,string strText,string strExtraValue,string strExtraText)
		{

            //objConnection = new OleDbConnection(strDsn);
             

            
            objConnection=open_connection();
			if (open_con==true )
			{
				try
				{
					objCommand=new OleDbCommand(strSql,objConnection);
					objDatareader=objCommand.ExecuteReader();
					ddlistGen.DataSource=objDatareader;
					ddlistGen.DataTextField=strText;
					ddlistGen.DataValueField=strValue;
					ddlistGen.DataBind();
					objDatareader.Close();
					objConnection.Close();
					if (strExtraText != "" )
					{
						ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
						ddlistGen.SelectedIndex=0;
					}

					list_bind=true;
					return ddlistGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1104");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1107");
					}	
					//ErrorStr=objError.Message.Substring(59,30);
					//One or more errors occurred during processing of command. ORA-00904: invalid column name 
					//ErrorStr=ErrorMsg(objConnection,"MSG1201");
					list_bind=false;
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


		//
		public ListBox Fill_List(ListBox ddlistGen,string strSql,string strValue,string strText)
		{
			objConnection=open_connection();
			if (open_con==true )
			{
				try
				{
					objCommand=new OleDbCommand(strSql,objConnection);
					objDatareader=objCommand.ExecuteReader();
					ddlistGen.DataSource=objDatareader;
					ddlistGen.DataTextField=strText;
					ddlistGen.DataValueField=strValue;
					ddlistGen.DataBind();
					objDatareader.Close();
					objConnection.Close();
//					if (strExtraText != "" )
//					{
//						ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
//						ddlistGen.SelectedIndex=0;
//					}

					list_bind=true;
					return ddlistGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1104");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1107");
					}	
					//ErrorStr=objError.Message.Substring(59,30);
					//One or more errors occurred during processing of command. ORA-00904: invalid column name 
					//ErrorStr=ErrorMsg(objConnection,"MSG1201");
					list_bind=false;
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
	
			

		//
		public DataGrid BindGrid(DataGrid dgrdGen,string strSql)
		{			
			
			objConnection=open_connection();
			
			if (open_con==true )
			{
				try
				{
					objDataSet=new DataSet();
					objAdapter= new OleDbDataAdapter(strSql,objConnection);
					objAdapter.Fill(objDataSet,"site_dir");
					dgrdGen.DataSource=objDataSet.Tables["site_dir"].DefaultView;
					dgrdGen.DataBind();
					
					grid_bind=true;
					return dgrdGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1201");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1202");
					}	
					
					grid_bind=false;
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
			//return dgrdGen;
		}
		public DataGrid BindGridJob(DataGrid dgrdGen,string strSql)
		{			
			
			objConnection=open_connection();
			
			if (open_con==true )
			{
				try
				{
					objDataSet=new DataSet();
					objAdapter= new OleDbDataAdapter(strSql,objConnection);
					objAdapter.Fill(objDataSet,"Job_dir");
					dgrdGen.DataSource=objDataSet.Tables["Job_dir"].DefaultView;
					dgrdGen.DataBind();
					
					grid_bind=true;
					return dgrdGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1201");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1202");
					}	
					
					grid_bind=false;
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
			//return dgrdGen;
		}
//		public DataGrid BindGrid(DataGrid dgrdGen,string strSql)
//		{			
//			
//			objConnection=open_connection();
//			
//			if (open_con==true )
//			{
//				try
//				{
//					objDataSet=new DataSet();
//					objAdapter= new OleDbDataAdapter(strSql,objConnection);
//					objAdapter.Fill(objDataSet,"site_dir");
//					dgrdGen.DataSource=objDataSet.Tables["site_dir"].DefaultView;
//					dgrdGen.DataBind();
//					
//					grid_bind=true;
//					return dgrdGen;
//				}
//				catch(OleDbException objError)
//				{
//					if (objError.Message.Substring(0,21)=="Table does not exist.")
//					{
//						ErrorStr=ErrorMsg("MSG1201");
//					}	
//					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
//					{
//						ErrorStr=ErrorMsg("MSG1202");
//					}	
//					
//					grid_bind=false;
//					return null;
//				}
//				finally
//				{
//					objConnection.Close();
//					objConnection.Dispose();
//					objAdapter.Dispose();
//				}
//			}
//			else
//			{
//				return null;
//			}
//			//return dgrdGen;
//		}
		public DataGrid BindGridCont(DataGrid dgrdGen,string strSql)
		{			
			
			objConnection=open_connection();
			
			if (open_con==true )
			{
				try
				{
					objDataSet=new DataSet();
					objAdapter= new OleDbDataAdapter(strSql,objConnection);
					objAdapter.Fill(objDataSet,"CONT_CODE_DIR");
					dgrdGen.DataSource=objDataSet.Tables["CONT_CODE_DIR"].DefaultView;
					dgrdGen.DataBind();
					
					grid_bind=true;
					return dgrdGen;
				}
				catch(OleDbException objError)
				{
					if (objError.Message.Substring(0,21)=="Table does not exist.")
					{
						ErrorStr=ErrorMsg("MSG1201");
					}	
					if (objError.Message.Substring(59,30)=="ORA-00904: invalid column name")
					{
						ErrorStr=ErrorMsg("MSG1202");
					}	
					
					grid_bind=false;
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
			//return dgrdGen;
		}
		
		public void execute_sql(string strSql)
		{
			try
			{
				objConnection=open_connection();
				objCommand= new OleDbCommand(strSql,objConnection);
				objCommand.CommandType=CommandType.Text; 
				objCommand.ExecuteNonQuery();
				err_flag=false;
			}
			catch(OleDbException objError)
			{
				if (objError.Message.Substring(0,21)=="Table does not exist.")
				{
				//	ErrorStr=ErrorMsg(objConnection,"MSG1201");
					ErrorStr=ErrorMsg("MSG1104");
					err_flag=true;
				}	
				else if (objError.Message.Substring(0,28)=="ORA-00001: unique constraint")
				{
					//ErrorStr=ErrorMsg(objConnection,"MSG1103");
					ErrorStr=ErrorMsg("MSG1105");
					err_flag=true;
				}	
				else if (objError.Message.Substring(0,31)=="ORA-02292: integrity constraint")
				{
					//ErrorStr=ErrorMsg(objConnection,"MSG1107");
					ErrorStr=ErrorMsg("MSG1106");
					err_flag=true;
				}
				else if ((objError.Message.Length>=87) && (objError.Message.Substring(59,30)=="ORA-00904: invalid column name"))
				{
					//ErrorStr=ErrorMsg(objConnection,"MSG1202");
					ErrorStr=ErrorMsg("MSG1107");
					err_flag=true;
				}	
				else
				{
					ErrorStr=objError.Message;
					err_flag=true;
				}
			}
			catch(Exception objError)
			{
				ErrorStr=objError.Message;
				err_flag=true;
			}
			finally
			{
				objConnection.Close(); 			
			}
		}

		public OleDbConnection  open_connection()
		{
			try
			{
				objConnection=new OleDbConnection(strDsn);
				objConnection.Open();
				open_con = true;
				return objConnection;
				
			}
			catch(OleDbException objError)
			{
				if (objError.Message.Substring(0,9)=="ORA-12154")
				{
					ErrorStr="Connection Failed : System Could not resolve Service Name";
				}
				else if (objError.Message.Substring(0,9)=="ORA-01017")
				{
					ErrorStr="Invalid user Name/Password";
				}

				open_con=false;
				return null;
			}
			catch(Exception objError)
			{
				ErrorStr=objError.Message;
				open_con=false;
				return null;
			}
			
		}
	}

}
