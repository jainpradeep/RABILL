<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true" CodeFile="RA_Edit_Tender.aspx.cs" Inherits="RA_Edit_Tender" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<table width="100%" class="myTable">
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    Update Tender data
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
            <td colspan="2" align="left" > <b> 
            

            
             </b><br />
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
                                                                                                                
                                        <asp:HiddenField ID="hdTenderNo" runat="server" Value='<%#Eval("TENDER_NO") %>' />
                                        <asp:HiddenField ID="hdPartNumber" runat="server" Value='<%#Eval("PART_NO") %>' />

                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>                                  
                              
                                <asp:TemplateField HeaderText="Tender No" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <%#Eval("TENDER_NO")%>
                                    </ItemTemplate>
                                </asp:TemplateField> 

                                <asp:TemplateField HeaderText="Part Number" ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="5%">
                                    <ItemTemplate>
                                        <%#Eval("PART_NO")%>
                                    </ItemTemplate>
                                </asp:TemplateField>  
                                                            
                                <asp:TemplateField HeaderText="Tender Description" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <%#Eval("description")%>
                                    </ItemTemplate>
                                </asp:TemplateField>    
                                
                                          <asp:TemplateField HeaderText="Contract Value" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("amount")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtAmount" CssClass="textEntry" Text='<%# Bind("amount") %>' onkeypress='return ((event.charCode >= 48 && event.charCode <= 57) || event.charCode == 45 || event.charCode == 46)'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>  
                         

                             <asp:TemplateField HeaderText="LOI No" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("LOI_No")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtLOI" CssClass="textEntry" Text='<%# Bind("LOI_No") %>' width="100"></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>
                            
                             <asp:TemplateField HeaderText="Awarded Date" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("Date_AWARDED")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                     <asp:TextBox runat="server" ID="txtAwardedDate" CssClass="textEntry" Width="100" ></asp:TextBox>
                    <asp:ImageButton ID="btnCalTo" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif"
                        Visible="true" />
                    <asp:CalendarExtender ID="calTo" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                        Enabled="true" TargetControlID="txtAwardedDate" PopupButtonID="btnCalTo" FirstDayOfWeek="Monday">
                    </asp:CalendarExtender>    
                                </EditItemTemplate>
                            </asp:TemplateField>


                               <asp:TemplateField HeaderText="Completion Date" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("Completion_Date")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                     <asp:TextBox runat="server" ID="txtCompletionDate" CssClass="textEntry" Width="100" ></asp:TextBox>
                    <asp:ImageButton ID="btnCalCompletion" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif"
                        Visible="true" />
                    <asp:CalendarExtender ID="calCompletionTo" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                        Enabled="true" TargetControlID="txtCompletionDate" PopupButtonID="btnCalCompletion" FirstDayOfWeek="Monday">
                    </asp:CalendarExtender>    
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

