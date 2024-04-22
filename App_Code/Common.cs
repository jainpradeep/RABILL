using System;
using System.Collections.Generic;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Collections;
using AppCode;
using System.Data;
using System.IO;


public static class Common
{
    static string EncryptionKey = "PSUTECH2019";
    static byte[] bytes = ASCIIEncoding.ASCII.GetBytes("ZeroConn");
    public static void Show(string error)
    {
        Page page = HttpContext.Current.Handler as Page;
        if (page != null)
        {
            error = error.Replace("'", "\'");
            ScriptManager.RegisterStartupScript(page, page.GetType(), "err_msg", "alert('" + error + "');", true);
        }
    }

    //Generate Email with multiple CC
    public static void SendRABMail(string from, ArrayList toRecepient, string subject, string message, ArrayList ccRecepient)
    {
        if (toRecepient.Count < 1)
            return;
        System.Net.Mail.MailMessage msgTest = new System.Net.Mail.MailMessage();
        msgTest.From = new System.Net.Mail.MailAddress(from);
        for (int i = 0; i < toRecepient.Count; i++)
        {
            msgTest.To.Add(new System.Net.Mail.MailAddress(toRecepient[i].ToString()));
        }
        if (ccRecepient != null && ccRecepient.Count > 0)
        {
            for (int i = 0; i < ccRecepient.Count; i++)
            {
                msgTest.CC.Add(new System.Net.Mail.MailAddress(ccRecepient[i].ToString()));
            }
        }
       
        //msgTest.Bcc.Add("rajesh.gupta@eil.co.in");        

        msgTest.Subject = subject;
        msgTest.Body = message;
        msgTest.IsBodyHtml = true;
        msgTest.Priority = System.Net.Mail.MailPriority.High;
        try
        {
            // System.Net.Mail.SmtpClient c = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SMTPServerName"]);
            System.Net.Mail.SmtpClient c = new System.Net.Mail.SmtpClient("appsmtp.eil.co.in");
           
            c.Send(msgTest);
        }
        catch (HttpException ex)
        {
            //Response.Write("HTTP Error: " + ex.ToString());
        }
        catch (Exception ex)
        {
            // Response.Write("Error: " + ex.ToString());
        }
    }

    public static void SendTestMail(string from, string toRecepient, string subject, string message, string ccRecepient)
    {
        if (toRecepient.Length < 1)
            return;
        System.Net.Mail.MailMessage msgTest = new System.Net.Mail.MailMessage();
        msgTest.From = new System.Net.Mail.MailAddress(from);

        msgTest.To.Add(new System.Net.Mail.MailAddress(toRecepient));

        msgTest.CC.Add(new System.Net.Mail.MailAddress(ccRecepient));


        //msgTest.Bcc.Add("rajesh.gupta@eil.co.in");


        msgTest.Subject = subject;
        msgTest.Body = message;
        msgTest.IsBodyHtml = true;
        msgTest.Priority = System.Net.Mail.MailPriority.High;
        try
        {
            // System.Net.Mail.SmtpClient c = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["SMTPServerName"]);
            //System.Net.Mail.SmtpClient c = new System.Net.Mail.SmtpClient("10.10.0.247");
            System.Net.Mail.SmtpClient c = new System.Net.Mail.SmtpClient();
            c.Send(msgTest);
        }
        catch (HttpException ex)
        {
            //Response.Write("HTTP Error: " + ex.ToString());
        }
        catch (Exception ex)
        {
            // Response.Write("Error: " + ex.ToString());
        }
    }

    public static string encrypt(string encryptString)
    {

        byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {  
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76  
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(clearBytes, 0, clearBytes.Length);
                    cs.Close();
                }
                encryptString = Convert.ToBase64String(ms.ToArray());
            }
        }
        return encryptString;
    }
    public static string Decrypt(string cipherText)
    {

        cipherText = cipherText.Replace(" ", "+");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);
        using (Aes encryptor = Aes.Create())
        {
            Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {  
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76  
        });
            encryptor.Key = pdb.GetBytes(32);
            encryptor.IV = pdb.GetBytes(16);
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.Close();
                }
                cipherText = Encoding.Unicode.GetString(ms.ToArray());
            }
        }
        return cipherText;
    }  
}