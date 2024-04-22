<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>RA Billing Management System - Login Page</title>
    <link href="css/EILDesign.css" type="text/css" rel="stylesheet" />
    <link href="css/Site.css" type="text/css" rel="stylesheet" />
    <script type="text/javascript" src="js/aes.js"></script>
    <script type="text/javascript">

        function RefreshCaptcha() {

            var img = document.getElementById("imgCaptcha");

            img.src = "Captcha.ashx?query=" + Math.random();
        }


        function SubmitsEncryold() {

            //if (!Page_ClientValidate()) {
            //    return false;

            //}
          
            //var txtpassword = document.getElementById("<%=txtPassword.ClientID %>").value.trim();
            //var txtUserName = document.getElementById("<%=txtUserName.ClientID %>").value.trim();
            //var txtCaptcha = document.getElementById("<%=txtCaptcha.ClientID %>").value.trim();

            var txtpassword = document.getElementById("<%=txtPassword.ClientID %>").value;
            var txtUserName = document.getElementById("<%=txtUserName.ClientID %>").value;
            var txtCaptcha = document.getElementById("<%=txtCaptcha.ClientID %>").value;

            if (txtpassword == "" || txtUserName=="" || txtCaptcha=="" )
            {
                alert('Error: Kindly enter Valid USername, Password and Captcha!');
                return false;
            }

            //if (txtpassword == "") {

            //    return false;
            //}

            else {

                //var aesValue = document.getElementById("<%=aesKeyField.ClientID %>").value.trim();
                var aesValue = document.getElementById("<%=aesKeyField.ClientID %>").value;
              
                var key = CryptoJS.enc.Utf8.parse(aesValue);

                var iv = CryptoJS.enc.Utf8.parse(aesValue);

                 var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

                {
                    keySize: 128 / 8,

                    iv: iv,

                    mode: CryptoJS.mode.CBC,

                    padding: CryptoJS.pad.Pkcs7

                });

                document.getElementById("<%=txtPassword.ClientID %>").value = encryptedpassword;

               document.getElementById("<%=aesKeyField.ClientID %>").value = null;

            }
        }


        function SubmitsEncry() {

            //if (!Page_ClientValidate()) {
            //    return false;

            //}

            //var txtpassword = document.getElementById("<%=txtPassword.ClientID %>").value.trim();
            //var txtUserName = document.getElementById("<%=txtUserName.ClientID %>").value.trim();
            //var txtCaptcha = document.getElementById("<%=txtCaptcha.ClientID %>").value.trim();

            var txtpassword = document.getElementById("<%=txtPassword.ClientID %>").value;
            var txtUserName = document.getElementById("<%=txtUserName.ClientID %>").value;
            var txtCaptcha = document.getElementById("<%=txtCaptcha.ClientID %>").value;

            if (txtpassword == "" || txtUserName == "" || txtCaptcha == "") {
                alert('Error: Kindly enter Valid Username, Password and Captcha!');
                return false;
            }

                //if (txtpassword == "") {

                //    return false;
                //}

            else {

                //var aesValue = document.getElementById("<%=aesKeyField.ClientID %>").value.trim();
                var aesValue = document.getElementById("<%=aesKeyField.ClientID %>").value;

                var key = CryptoJS.enc.Utf8.parse(aesValue);

                var iv = CryptoJS.enc.Utf8.parse(aesValue);

                var encryptedpassword = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(txtpassword), key,

               {
                   keySize: 128 / 8,

                   iv: iv,

                   mode: CryptoJS.mode.CBC,

                   padding: CryptoJS.pad.Pkcs7

               });

                document.getElementById("<%=txtPassword.ClientID %>").value = encryptedpassword;

                document.getElementById("<%=aesKeyField.ClientID %>").value = null;

            }
        }

    </script>


</head>
<body class="myLoginBody">
    <form id="form1" method="post" runat="server">
        <table id="Table1" cellspacing="0" cellpadding="0" width="100%" border="0">
            <tr>
                <td>
                    <table id="myTableMain" cellspacing="0" cellpadding="0" border="0">
                        <tr valign="top">
                            <td width="100%">
                                <table id="Table4" cellspacing="0" cellpadding="0">
                                    <tr valign="top">
                                        <td width="100%">
                                            <table id="myTopBrandBannerTable" cellspacing="0" cellpadding="0">
                                                <tr valign="middle" height="25">
                                                    <td id="myTopBrandBanner" nowrap align="right">
                                                        <table cellpadding="0" cellspacing="0" border="0">
                                                            <tr>
                                                    </td>
                                                    <td nowrap align="center">
                                                        <a class="myHeaderLink" target="_doc" href="https://m.facebook.com">Other GoI Websites</a></td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="Td3">&nbsp;</td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr valign="top">
                <td width="100%">
                    <table id="myMiddleBannerTable" cellspacing="0" cellpadding="0">
                        <tr valign="top">
                            <td width="100%">
                                <table id="myMiddleBrandBannerTable" cellspacing="0" cellpadding="0">
                                    <tr valign="middle" height="25">
                                        <td id="myMiddleBrandBanner" nowrap align="left">
                                            <span class="myLoginTopFrame" id="lblHeader">RA Billing Management System</span></td>
                                        <td id="myMiddleBanner" align="right">
                                            <span class="myLoginTopFrame" id="Span1"></span>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td id="myMiddleBrandBannerBlank">&nbsp;</td>
                        </tr>
                    </table>
                    <table id="myBottomBannerTable" cellspacing="0" cellpadding="0">
                        <tr>
                            <td>
                                <table id="myBottomBrandBannerTable" cellspacing="0" cellpadding="0">
                                    <tr valign="middle" height="25">
                                        <td id="myBottomBanner" align="right">&nbsp;</td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table id="Table5" cellspacing="0" cellpadding="0" width="100%">
            <tr>
                <td valign="middle" align="left" colspan="2">
                    <img height="2" src="" border="0"></td>
            </tr>
            <tr>
                <td align="left" colspan="2">
                    <img src="images/ref.jpg" width="100%" border="0"></td>
            </tr>
            <tr>
                <td align="left" colspan="2">
                    <img height="3" src="" border="0"></td>
            </tr>
            <tr>
                <td valign="top" width="80%">
                    <p class="myLoginBodyText" align="left">
                        <br />
                        <br />
                        <b>RA Billing System</b>  handles Tender SOR items for RA billing including measurement sheet and final executed quantities at project site. 
                                    <br />
                        This software is integrated with the CBA, SORPS.
                        <br />
                        <br />
                        <br />
                        <font color='Red'>
                            <b>NOTE 1: Payment terms is to be finalized before raising any bill. <u>After the bill is raised into the system the facility for splitting of items is not possible.</u>
                            </b>
                        </font>
                          <br />
                        <br />
                    <%--<asp:Label ID="lblNoteMergeBill" runat="server" Text="NOTE 2: Kindly arrange to merge the bills first (If any) before adding the new bills in the system." Font-Bold="true" ForeColor="Red"></asp:Label>--%>
                        <asp:Label ID="lblNoteMergeBill" runat="server" Text="NOTE 2: Kindly check the user manual for the system usage." Font-Bold="true" ForeColor="Red"></asp:Label>
                </td>
                <td valign="bottom" align="left">
                    <table class="myLoginText" id="TableBodyRight1" style="height: 100%" cellspacing="0"
                        cellpadding="0" width="95%">
                        <tr style="height: 20px">
                            <td class="myLoginTopLabel" valign="middle" align="center">Sign In
                            </td>
                        </tr>
                        <tr style="width: 100%; height: 60%">
                            <td valign="top">
                                <table id="TableBodyRight" style="height: 100%" cellspacing="0" cellpadding="0" width="100%">

                                    <tr>
                                        <td style="height: 41.43%" valign="top">
                                            <asp:Panel ID="Panel1" runat="server">
                                                <table id="TableLogin" style="height: 100%" cellspacing="0" cellpadding="0" width="100%">
                                                    <tr>
                                                        <td class="myLoginLabel" style="height: 31px">&nbsp;User ID
                                                        </td>
                                                        <td class="myLoginLabel" style="height: 31px">
                                                            <asp:TextBox ID="txtUserName" runat="server" MaxLength="8" TabIndex="1" CssClass="myLoginText"
                                                                Style="height: 14px; width: 100px;"></asp:TextBox>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="myLoginLabel" style="height: 31px">&nbsp;Password
                                                        </td>
                                                        <td class="myLoginLabel" style="height: 31px">
                                                            <asp:TextBox ID="txtPassword" TabIndex="2" runat="server" TextMode="Password" CssClass="myLoginText"
                                                                Style="height: 14px; width: 100px;"></asp:TextBox>
                                                        </td>
                                                    </tr>

                                                    <tr>
                                                        <td  align="center" style="padding-left:10px;">
                                                            <img src="Captcha.ashx" id="imgCaptcha" />
</td><td>
                                                       <a href="#" onclick="javascript:RefreshCaptcha();"><b>Refresh</b></a>
                                                        </td>
                                                    </tr>

                                                    <tr>

                                                        <td class="myLoginLabel" style="height: 31px">Captcha </td>

                                                        <td>
                                                            <asp:HiddenField ID="aesKeyField" runat="server" Value="" />
                                                            <asp:TextBox ID="txtCaptcha" runat="server" CssClass="myInput"
                                                                MaxLength="10" Width="95%" ></asp:TextBox>

                                                        </td>

                                                    </tr>

                                                    <tr>
                                                        <td class="myLoginMessage" colspan="2" align="center">&nbsp;
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="myLoginMessage" colspan="2" align="center"></td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr style="height: 5%; padding-top: 8px;">
                                        <td style="height: 11.52%" class="myLoginLabel">
                                            <div id="DIV1" align="center">
                                                <asp:Button ID="btnLogin" TabIndex="4" runat="server" Font-Bold="true" CssClass="myLoginButton" Text="Login"
                                                    OnClick="btnLogin_Click" OnClientClick="return SubmitsEncry();"></asp:Button>
                                            </div>
                                        </td>
                                    </tr>

                                    <tr style="height: 5%">
                                        <td class="myLoginLabel" style="height: 10%" colspan="2"></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <tr>
                        <td colspan="2">&nbsp;</td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <span class="myError" id="Span2" style="width: 248px">
                                <asp:Label ID="lblError" runat="server" Width="248px" Font-Bold="true"></asp:Label>
                            </span>
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2"><b>Total Hits - </b>
                            <asp:Label ID="lblTotalHits" runat="server" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <hr style="width: 100%; height: 1px" width="100%" size="1">
                        </td>
                    </tr>
                    <tr>
                        <td align="left" colspan="2">
                            <table border="0">
                                <tr align="left">
                                    <td align="left">
                                        <img height="35" src="images\eil_logo_01.gif" border="0" alt="Logo" />
                                    </td>
                                    <td align="left">
                                        <div class="myLoginInfo">
                                            Developed by : Information Technology Services,&nbsp;Engineers India Limited, New
                                                Delhi.
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
        </table>
        </td>
            </tr>
        </table>






      
  
    </table>  
    </form>
</body>
</html>
