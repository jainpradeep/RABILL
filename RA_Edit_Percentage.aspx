<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true" CodeFile="RA_Edit_Percentage.aspx.cs" Inherits="RA_Edit_Percentage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<table width="100%" class="myTable">
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    Define percentage as per contract
                </td>
            </tr>
            <tr>
            <td colspan="2" align="center">&nbsp;
            </td>
            </tr>
            <tr>
                <td class="myLabel" align="right">
                    Select Job Number
                </td>
                <td align="left">
                    <asp:DropDownList ID="ddJobNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddJobNumber_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
            <td colspan="2" align="center" style="color:red;"> <b> NOTE: For decrease enter negative value </b><br />
            </td>
            </tr>
            <tr>
            <td colspan="2" align="center">            
               
                        <asp:GridView ID="gvTenders" runat="server" Font-Names="Arial" Font-Size="9pt" Width="80%"
                            AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" CellPadding="0"
                            BorderColor="#CCCCCC" BorderStyle="None" OnRowDataBound="gvTenders_RowDataBound"
                            OnRowEditing="gvTenders_RowEditing" OnRowCancelingEdit="gvTenders_RowCancelingEdit"
                        OnRowUpdating="gvTenders_RowUpdating" >
                            <Columns>
                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                     <asp:HiddenField ID="hdID" runat="server" Value='<%#Eval("ID") %>' />
                                        <asp:HiddenField ID="hdFrozen" runat="server" Value='<%#Eval("is_frozen") %>' />                                      
                                        <asp:HiddenField ID="hdTenderNo" runat="server" Value='<%#Eval("TENDER_NO") %>' />
                                        <asp:HiddenField ID="hdPartNumber" runat="server" Value='<%#Eval("PART_NO") %>' />

                                        
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>                               
                                <asp:TemplateField HeaderText="Tender Description" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <%#Eval("description")%>
                                    </ItemTemplate>
                                </asp:TemplateField>     
                                
                                <asp:TemplateField HeaderText="Part Number" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <%#Eval("PART_NO")%>
                                    </ItemTemplate>
                                </asp:TemplateField>                             
                               
                                <asp:TemplateField HeaderText="Percentage" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("percentage_value")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtPercentage" CssClass="textEntry" Text='<%# Bind("percentage_value") %>' onkeypress='return ((event.charCode >= 48 && event.charCode <= 57) || event.charCode == 45 || event.charCode == 46)'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>                         

                               <asp:CommandField ShowEditButton="true" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center"  />  
                            </Columns>
                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                <b>No data.</b>
                            </EmptyDataTemplate>
                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                VerticalAlign="Middle" CssClass="myGridHeader" />
                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066" />
                            <AlternatingRowStyle BackColor="#e5eff8" BorderWidth="1px" />
                        </asp:GridView>

            </td>
            </tr>
            </table>


</asp:Content>

