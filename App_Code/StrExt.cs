using System;
using System.Collections.Generic;
using System.Web;


public static class StrExt
{

    public static string ParseSQL(this string theString)
    {
         return !String.IsNullOrEmpty(theString)? theString.Replace("--", "")
           // .Replace("--", "")
            //.Replace("!", "")
            //.Replace("%", "")            
            .Replace("'", "''")
            .Replace("^", "") : null;             
        
    }

}