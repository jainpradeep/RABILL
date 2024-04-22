using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;
using System.Collections;

namespace AppCode
{
    public partial class dbFunction
    {
        DbProviderFactory factory = null;
        DbTransaction objTransaction;
        public Int32 ctobj;

        public dbFunction()
        {
            factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings["raConnection"].ProviderName);
        }

        public int executeNonQuery(string strSQL, Dictionary<string, string> param)
        {
            int result = 0;
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                result = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return result;
        }

        public string executeNonQueryWithReturning(string strSQL, Dictionary<string, string> param)
        {
            string id = "";
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                DbParameter out_id = cmd.CreateParameter();
                out_id.ParameterName = "out_id";
                out_id.Size = 100;
                out_id.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(out_id);
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                cmd.ExecuteNonQuery();
                id = cmd.Parameters["out_id"].Value.ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return id;
        }

        public string executeScalar(string strSQL, Dictionary<string, string> param)
        {
            string str = "";
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                if (param != null)
                {
                    foreach (KeyValuePair<string, string> entry in param)
                    {
                        DbParameter param_name = cmd.CreateParameter();
                        param_name.ParameterName = entry.Key;
                        param_name.Value = entry.Value;
                        cmd.Parameters.Add(param_name);
                    }
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                str = cmd.ExecuteScalar().ToString();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return str;
        }

        public void executeProcedure(string procedureName, Dictionary<string, string> param)
        {            
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = procedureName;
            try
            {
                cmd.Parameters.Clear();
                cmd.CommandType = CommandType.StoredProcedure;
                if (param != null)
                {
                    foreach (KeyValuePair<string, string> entry in param)
                    {
                        DbParameter param_name = cmd.CreateParameter();
                        param_name.ParameterName = entry.Key;
                        param_name.Value = entry.Value;
                        cmd.Parameters.Add(param_name);
                    }
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }            
        }

        public bool doesRecordExists(string strSQL, Dictionary<string, string> param)
        {
            bool exist = false;
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                IDataReader myReader = null;
                myReader = cmd.ExecuteReader();
                if (myReader.Read())
                {
                    exist = true;
                }
                myReader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return exist;
        }

        public int executeTransaction(string[] strSql, Dictionary<string, string>[] param)
        
        {
            int result = 0;
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd=connection.CreateCommand();            
            try
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                
                objTransaction = connection.BeginTransaction();
                cmd.Transaction = objTransaction;

                for (int i = 0; i < strSql.Length; i++)
                {
                    cmd.CommandText = strSql[i];
                    cmd.Parameters.Clear();                
                    foreach (KeyValuePair<string, string> entry in param[i])
                    {
                        DbParameter param_name = cmd.CreateParameter();
                        param_name.ParameterName = entry.Key;
                        param_name.Value = entry.Value;
                        cmd.Parameters.Add(param_name);
                    }
                    cmd.ExecuteNonQuery();
                }

                result = 1;
                objTransaction.Commit();
            }
            catch (Exception ex)
            {
                objTransaction.Rollback();
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                connection.Dispose();
            }
            return result;
        }

        public DataTable bindDataTable(string strSQL, Dictionary<string, string> param)
        {
            DataTable tblGen = new DataTable();
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    tblGen.Load(reader);                    
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return tblGen;
        }

        public GridView bindGridView(GridView grdVGenIn, string strSQL, Dictionary<string, string> param)
        {
            DataTable tblGen = new DataTable();
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (IDataReader reader = cmd.ExecuteReader())
                {                    
                    tblGen.Load(reader);                       
                }                
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                grdVGenIn.DataSource = tblGen;
                grdVGenIn.DataBind();
            }
            return grdVGenIn;
        }

        public FormView bindFormView(FormView fvGenIn, string strSQL, Dictionary<string, string> param)
        {
            DataTable tblGen = new DataTable();
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    tblGen.Load(reader);
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                if (tblGen.Rows.Count > 0)
                {
                    fvGenIn.DataSource = tblGen;
                    fvGenIn.DataBind();
                    fvGenIn.Visible = true;
                }
                else
                {
                    fvGenIn.Visible = false;
                }
            }
            return fvGenIn;
        }


        public DropDownList bindDropDownList(DropDownList ddlistGen, string strSQL, Dictionary<string, string> param, string strValue, string strText, string strExtraValue, string strExtraText)
        {
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                foreach (KeyValuePair<string, string> entry in param)
                {
                    DbParameter param_name = cmd.CreateParameter();
                    param_name.ParameterName = entry.Key;
                    param_name.Value = entry.Value;
                    cmd.Parameters.Add(param_name);
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    ddlistGen.DataSource = reader;
                    ddlistGen.DataTextField = strText;
                    ddlistGen.DataValueField = strValue;
                    ddlistGen.DataBind();
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
                if (!strExtraText.Equals(string.Empty))
                {
                    ddlistGen.Items.Insert(0, new ListItem(strExtraText, strExtraValue));
                    ddlistGen.SelectedIndex = 0;
                }
            }
            return ddlistGen;
        }

        public string generateSupportID()
        {
            DataTable tblGen = new DataTable();
            string qry = "select SP_SUPPORT_ID.NEXTVAL from dual";
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = qry;
            
            try
            {
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            using (IDataReader reader = cmd.ExecuteReader())
            {
                tblGen.Load(reader);
            }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();               
            }
            return tblGen.Rows[0].ToString();
        }

        // Updating Invoice table for the Comments received by User
        public int updateInvoiceTable(string contractId, string invoiceId)
        {
            int commentResult = 0;
            
            StringBuilder sbQueryTotalComment = new StringBuilder();
            StringBuilder sbQueryTotalYComment = new StringBuilder();
            Dictionary<string, string> paramListComment = new Dictionary<string, string>();
            paramListComment.Add("CONTRACT_ID", contractId);
            paramListComment.Add("INV_ID", invoiceId);
            sbQueryTotalComment.Append("select nvl(count(*),0) countTotal from SP_USERSUPPORT where CONTRACT_ID=:CONTRACT_ID and INV_ID=:INV_ID ");
            sbQueryTotalYComment.Append(" select nvl(count(*),0) countY from SP_USERSUPPORT where CONTRACT_ID=:CONTRACT_ID and INV_ID=:INV_ID and CLOSED='Y' ");
            string totalComment = executeScalar(sbQueryTotalComment.ToString(), paramListComment);
            string totalYComment = executeScalar(sbQueryTotalYComment.ToString(), paramListComment);
            if (totalComment.Length < 1)
            {
                totalComment = "0";
            }            if (totalYComment.Length < 1)
            {
                totalYComment = "0";
            }
            StringBuilder sbQueryUpdate = new StringBuilder();
            if ((int.Parse(totalComment) - int.Parse(totalYComment)) < (int.Parse(totalComment)) && ((int.Parse(totalComment) - int.Parse(totalYComment)) > 0))
            {
                sbQueryUpdate.Append("update SP_INVOICES set STATUS_ID = 1 where CONTRACT_ID=:CONTRACT_ID and INV_ID=:INV_ID");
            }
            else if ((int.Parse(totalComment) - int.Parse(totalYComment)) == 0)
            {
                sbQueryUpdate.Append("update SP_INVOICES set STATUS_ID = 2 where CONTRACT_ID=:CONTRACT_ID and INV_ID=:INV_ID");
            }
            
            if (sbQueryUpdate.Length > 0)
            {
                commentResult = executeNonQuery(sbQueryUpdate.ToString(), paramListComment);
            }
            return commentResult;
        }

        public string getHoDEmail(string employeeEmpNo)
        {
            string hodEmail = "";
            StringBuilder sbquery = new StringBuilder();
            // SELECT a.EMPNO,    a.EMPNAME,     a.EMAIL1,     a.prst_level
            sbquery.Append(" SELECT  nvl(a.EMAIL1,'NA') hodEmail ")
        .Append(" FROM vw_employee@pdbview_link a, VW_DDL_HOD@pdbview_link b ,SP_INVOICE_EXCEPTION_EMP c ")
        .Append(" WHERE a.empno = b.hod_empno ")
        .Append(" and C.EMPNO!=b.hod_empno ")
        .Append(" AND (b.div,b.dept,b.loc_code ) ")
           .Append("  in (select prst_divn,prst_sectn,prst_locn from vw_employee@pdbview_link aa where upper(aa.empno)=:empno) ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("empno", employeeEmpNo.ToString());
            hodEmail = executeScalar(sbquery.ToString(), paramList);
            return hodEmail;
        }

        public string getEmpEmail(string employeeEmpNo)
        {
            string empEmail = "";
            StringBuilder sbquery = new StringBuilder();
            // SELECT a.EMPNO,    a.EMPNAME,     a.EMAIL1,     a.prst_level
            sbquery.Append(" SELECT   nvl(a.EMAIL1,'NA') empEmail ")
        .Append(" FROM vw_employee@pdbview_link a ")
        .Append(" WHERE a.empno =:empno ")
        .Append(" and sep_type=0 ");
             Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("empno", employeeEmpNo.ToString());
            empEmail = executeScalar(sbquery.ToString(), paramList);
            return empEmail;
        }

        public string isUserHoD(string EmpNo)
        {
            string hodEmpNo = "";
            StringBuilder sbquery = new StringBuilder();
            // SELECT a.EMPNO,    a.EMPNAME,     a.EMAIL1,     a.prst_level
            sbquery.Append(" SELECT  a.empno ")
        .Append(" FROM vw_employee@pdbview_link a, VW_DDL_HOD@pdbview_link b ")
        .Append(" WHERE a.empno = b.hod_empno ")
        .Append(" AND (b.div,b.dept,b.loc_code ) ")
           .Append("  in (select prst_divn,prst_sectn,prst_locn from vw_employee@pdbview_link aa where upper(aa.empno)=:empno) ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("empno", EmpNo.ToString());
            hodEmpNo = executeScalar(sbquery.ToString(), paramList);
            return hodEmpNo;
        }

        public DataTable getEmpDetail(string employeeEmpNo)
        {
            DataTable dtTable = new DataTable();
            StringBuilder sbquery = new StringBuilder();
            sbquery.Append(" SELECT empname,designation,department,nvl(a.EMAIL1,'NA') empEmail ")
            .Append(" FROM vw_employee@pdbview_link a ")
            .Append(" WHERE a.empno =:empno ")
            .Append(" and sep_type=0 ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("empno", employeeEmpNo.ToString());
            dtTable = bindDataTable(sbquery.ToString(), paramList);
            return dtTable;
        }

        public Int32 ExecuteStatementCount(string strSQL, Dictionary<string, string> param)
        {
            DbConnection connection = factory.CreateConnection();
            DbCommand cmd = connection.CreateCommand();
            cmd.CommandText = strSQL;
            try
            {
                cmd.Parameters.Clear();
                if (param != null)
                {
                    foreach (KeyValuePair<string, string> entry in param)
                    {
                        DbParameter param_name = cmd.CreateParameter();
                        param_name.ParameterName = entry.Key;
                        param_name.Value = entry.Value;
                        cmd.Parameters.Add(param_name);
                    }
                }
                connection.ConnectionString = ConfigurationManager.ConnectionStrings["raConnection"].ConnectionString;
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }
                object obj = cmd.ExecuteScalar().ToString();
                ctobj = Convert.ToInt32(obj);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                cmd.Dispose();
                connection.Close();
            }
            return ctobj;
        }

        public Hashtable getJobEmails(string jobNumber, string tenderNo,string partNo)
        {
            Hashtable htEmails = new Hashtable();
            StringBuilder sbQuery= new StringBuilder();
            string[] RCMEmail;
            string[] ACEmail;
            string[] BEEmail;
            ArrayList lstRCMEmail;
            ArrayList lstACEmail;
            ArrayList lstBEEmail;

            sbQuery.Append(@"select role,to_char(wm_concat(distinct email1)) emailIds from RAB_TENDER_USERS  a, vw_employee@pdbview b
                where job_no=:job_no and tender_no=:tender_no and part_no =:part_no
                and A.EMPNO=b.empno and b.sep_type=0 and email1 is not null 
                group by role
                    union
                select distinct 'RCM' ROLE, to_char(wm_concat(email1)) emailIds from site_dir a, vw_employee@pdbview b
                where site_cd in (select site_cd from JOB_DIR a 
                where A.JOB_NO=:rcm_job_no) and A.empno_rcm=b.empno 
                and b.sep_type=0 and email1 is not null ");
            Dictionary<string, string> paramList = new Dictionary<string, string>();
            paramList.Add("job_no", jobNumber);
            paramList.Add("tender_no", tenderNo);
            paramList.Add("part_no", partNo);
            paramList.Add("rcm_job_no", jobNumber);
            DataTable dtTemp = new DataTable();
            dtTemp = bindDataTable(sbQuery.ToString(), paramList);
            int countRows = dtTemp.Rows.Count;
            if (countRows > 0)
            {
                for (int i=0;i< countRows;i++)
                {                    
                   if("RCM".Equals(dtTemp.Rows[i]["ROLE"].ToString()))
                   {
                       RCMEmail = dtTemp.Rows[i]["emailIds"].ToString().ToString().Split(',');
                       lstRCMEmail = new ArrayList(RCMEmail.Length);
                       lstRCMEmail.AddRange(RCMEmail);
                       htEmails.Add("RCM", lstRCMEmail);
                   }
                   if ("BE".Equals(dtTemp.Rows[i]["ROLE"].ToString()))
                   {                                          
                       BEEmail = dtTemp.Rows[i]["emailIds"].ToString().ToString().Split(',');
                       lstBEEmail = new ArrayList(BEEmail.Length);
                       lstBEEmail.AddRange(BEEmail);
                       htEmails.Add("BE", lstBEEmail);
                   }
                   if ("AC".Equals(dtTemp.Rows[i]["ROLE"].ToString()))
                   {                       
                       ACEmail = dtTemp.Rows[i]["emailIds"].ToString().ToString().Split(',');
                       lstACEmail = new ArrayList(ACEmail.Length);
                       lstACEmail.AddRange(ACEmail);
                       htEmails.Add("AC", lstACEmail);
                   }
                }
            }
            return htEmails;        
        }

        public class HashSalt
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
        }

        public HashSalt GenerateSHA256Hash(String input, string salt)
        {
            HashSalt hashSalt;

            if (salt == "")
            {
                hashSalt = new HashSalt { Hash = "", Salt = "" };
                return hashSalt;
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input + salt);
            System.Security.Cryptography.SHA256Managed sha256hashstring =
                new System.Security.Cryptography.SHA256Managed();
            byte[] hash = sha256hashstring.ComputeHash(bytes);

            var hashPassword = Convert.ToBase64String(hash);
            hashSalt = new HashSalt { Hash = hashPassword, Salt = salt };
            return hashSalt;

        }

    }   
}