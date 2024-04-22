<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true" CodeFile="RA_Edit_MOP.aspx.cs" Inherits="RA_Edit_MOP" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

<table width="100%" class="myTable">
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    Update MOP
                    <asp:HiddenField ID="hd_MOPID" runat="server" />
                </td>
            </tr>
            <tr>
            <td colspan="2" align="center">&nbsp;
            </td>
            </tr>
           
          
            <tr>
            <td colspan="2" align="center">            
               
                        <asp:GridView ID="gvTenders" runat="server" Font-Names="Arial" Font-Size="9pt" Width="80%"
                            AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" CellPadding="0"
                            BorderColor="#CCCCCC" BorderStyle="None" OnRowEditing="gvTenders_RowEditing" OnRowCancelingEdit="gvTenders_RowCancelingEdit"
                        OnRowUpdating="gvTenders_RowUpdating" >
                            <Columns>
                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                     <asp:HiddenField ID="hdMOPID" runat="server" Value='<%#Eval("mopid") %>' />
                                        <asp:HiddenField ID="hdHeadingID" runat="server" Value='<%#Eval("headingid") %>' />                                     
                                        <asp:HiddenField ID="hdSubHeadingID" runat="server" Value='<%#Eval("subheadingid") %>' />
                                        
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>                               
                                <asp:TemplateField HeaderText="Heading Description" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <%#Eval("HEADING_DESC")%>
                                    </ItemTemplate>
                                </asp:TemplateField>     
                                
                                <asp:TemplateField HeaderText="Sub-Heading Description" ItemStyle-HorizontalAlign="Left" >
                                    <ItemTemplate>
                                        <%#Eval("SUB_HEADING_DESC")%>
                                    </ItemTemplate>
                                </asp:TemplateField>                             
                               
                                <asp:TemplateField HeaderText="Upto Previous Bill"  ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <%#Eval("UPTO_PREV_BILL_AMT")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtUptoPrevBill" CssClass="textEntry" Text='<%# Bind("UPTO_PREV_BILL_AMT") %>' onkeypress='return ((event.charCode >= 48 && event.charCode <= 57) || event.charCode == 45 || event.charCode == 46)'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField> 
                                
                                  <asp:TemplateField HeaderText="Since Previous Bill"  ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblSincePrevBill" Text='<%# Bind("SINCE_PREV_BILL_AMT") %>' ></asp:Label>
                                </ItemTemplate>
                                 <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtSincePrevBill" CssClass="textEntry" Text='<%# Bind("SINCE_PREV_BILL_AMT") %>' onkeypress='return ((event.charCode >= 48 && event.charCode <= 57) || event.charCode == 45 || event.charCode == 46)'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField> 
                                
                               <asp:TemplateField HeaderText="Total Upto Date"  ItemStyle-HorizontalAlign="Right">
                                <ItemTemplate>                                    
                                    <asp:Label runat="server" ID="lblTotalUptoDate" Text='<%# Bind("TOTAL_UPTO_DATE") %>' ></asp:Label>
                                </ItemTemplate>                                
                            </asp:TemplateField>                         

                               <asp:CommandField ShowEditButton="true" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center"  />  
                            </Columns>
                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                <b>No MOP data</b>
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

