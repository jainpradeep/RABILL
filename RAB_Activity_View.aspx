<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RAB_Activity_View.aspx.cs"
    Inherits="RAB_Activity_View" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RAB: Measurement Sheet</title>
    <link href="css/Site.css" rel="stylesheet" type="text/css" />
    <link href="css/EILDesign.css" rel="stylesheet" type="text/css" />
    <link href="css/EILDesign.css" type="text/css" rel="stylesheet" />
    <link href="css/ui-lightness/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />    
    
    
</head>
<!--<body  onunload="refreshAndClose();">-->
<body>
    <form id="form1" runat="server">
    <table cellpadding="5" cellspacing="5" border="0" width="100%" class="myTable">
        <tr>
            <td colspan="2" align="center" style="width: 100%">
                <u>
                    <h1>
                        Bill Activities</h1>
                </u>
            </td>
        </tr>
       
        <tr>
            <td colspan="2" align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" style="padding-top: 10px;">
                <asp:GridView ID="gvActivities" runat="server" ShowFooter="false" Font-Size="10pt"
                    AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        
                         <asp:TemplateField HeaderText="Activity">
                            <ItemStyle HorizontalAlign="Left"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("ACTIVITY_DESC")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Remarks">
                            <ItemStyle HorizontalAlign="Left" Width="25%"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("REMARKS")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date" >
                            <ItemStyle HorizontalAlign="Left" Width="25%"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("REMARKS_ON")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User ID">
                            <ItemStyle HorizontalAlign="Left" Width="10%"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("REMARKS_BY")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Role">
                            <ItemStyle HorizontalAlign="Left" Width="10%"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("ROLE")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                    <EmptyDataTemplate>
                        <b>Activity not found.</b>
                    </EmptyDataTemplate>
                    <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                        VerticalAlign="Middle" CssClass="myGridHeader" />
                    <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="30px" ForeColor="#000066"
                        Font-Size="10pt" Font-Bold="false" />
                    <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="10pt" />
                    <AlternatingRowStyle CssClass="myGridAlternatingItemStyle" BackColor="#F7F9FC" Font-Size="10pt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td class="myInput" align="center" colspan="2">
                <table>
                    <tr>
                        <td align="center" colspan="2">
                            <asp:Button ID="btnSimpleClose" runat="server" Text="Close" Font-Bold="true" OnClick="btnSimpleClose_Click" CssClass="btn btn-primary"/>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
