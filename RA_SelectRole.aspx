<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RA_SelectRole.aspx.cs" Inherits="RA_SelectRole" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>EIL: RAB- Select Role</title>
    <link href="css/EILDesign.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
    <center>
    <table  cellspacing="0" cellpadding="0" border="0" style="padding-top:100px;width:350px;">
        <tr style="height: 20px">
            <td class="myLoginTopLabel" valign="middle" align="center" colspan="2">
                Switch Role <br /> (You have been assigned multiple roles in the system)
            </td>
        </tr>
        <tr style="height: 40px" align="left">
            <td class="myLabel" style="width:40%">
                Select Role
            </td>
            <td align="left"  style="width:60%">
                <asp:DropDownList ID="ddRole" runat="server" AutoPostBack="true" CssClass="myInput"
                    OnSelectedIndexChanged="ddRole_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
        </tr>
    </table>
    </center>
    </form>
</body>
</html>
