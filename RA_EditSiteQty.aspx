<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RA_EditSiteQty.aspx.cs" MasterPageFile="~/RA_MenuPage.master"
    Inherits="RA_EditSiteQty" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">     

    <asp:Panel ID="pnlSelection" runat="server">
        <table width="100%" class="myTable">
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    Edit Site Quantity
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    Select Job Number
                </td>
                <td>
                    <asp:DropDownList ID="ddJobNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddJobNumber_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    Select Tender Number
                </td>
                <td>
                    <asp:DropDownList ID="ddTenderNo" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddTenderNo_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:GridView ID="gvSOR" runat="server" Font-Names="Arial" Font-Size="8pt" Width="95%"
                        AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" CellPadding="0"
                        BorderColor="#CCCCCC" BorderStyle="None" OnRowCommand="gvSOR_RowCommand" OnSelectedIndexChanged="gvSOR_SelectedIndexChanged"
                        OnRowDataBound="gvSOR_RowDataBound">
                        <Columns>
                            
                            <asp:TemplateField HeaderText="S.No " HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex+1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Sub Job Number" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                    <asp:HiddenField ID="hdTenderNo" runat="server" Value='<%#Eval("TENDER_NO") %>' />
                                    <asp:HiddenField ID="hdSorNumber" runat="server" Value='<%#Eval("SOR_NO") %>' />
                                    <asp:HiddenField ID="hdTenderSorId" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />                                    
                                    <asp:Label ID="lblSubJobNo" runat="server" Text='<%#Eval("SUB_JOB") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Unit" HeaderStyle-Width="6%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("UNIT_NO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Tender Number" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <%#Eval("TENDER_NO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Part Number" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <%#Eval("PART_NO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="SOR Number" >
                                <ItemTemplate>
                                    <%#Eval("SOR_NO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Contractor Code" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <%#Eval("C_CODE")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="LOI Number" HeaderStyle-Width="6%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("LOI_NO")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="LOI Date" HeaderStyle-Width="5%">
                                <ItemTemplate>
                                    <%#Eval("LOI_DATE")%>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CommandField ShowHeader="True" ShowSelectButton="True" HeaderStyle-Width="5%" />
                           
                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No SOR.</b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066" />
                        <AlternatingRowStyle BackColor="#e5eff8" BorderWidth="1px" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                    <asp:Panel ID="pnlSORItems" runat="server" Visible="false">
                        <asp:GridView ID="gvSORItems" runat="server" Font-Names="Arial" Font-Size="8pt" Width="95%"
                            AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" CellPadding="0"
                            BorderColor="#CCCCCC" BorderStyle="None" OnRowDataBound="gvSORItems_RowDataBound"
                            OnRowEditing="gvSORItems_RowEditing" OnRowCancelingEdit="gvSORItems_RowCancelingEdit"
                        OnRowUpdating="gvSORItems_RowUpdating" >
                            <Columns>
                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Sequence No">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                        <asp:HiddenField ID="hdSequenceNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                        <asp:HiddenField ID="hdSortNumber" runat="server" Value='<%#Eval("SORT_NO") %>' />
                                        <asp:HiddenField ID="hdItemRate" runat="server" Value='<%#Eval("ITEM_RATE") %>' />
                                        <asp:HiddenField ID="hdSORNumber" runat="server" Value='<%#Eval("sdesc") %>' />
                                        
                                        <asp:Label ID="lblSequenceNo" runat="server" Text='<%#Eval("SEQ_NO") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <%#Eval("ldesc")%>
                                    </ItemTemplate>
                                </asp:TemplateField>                     
                                <asp:TemplateField HeaderText="UOM" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Eval("UOM")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Rate" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Eval("ITEM_RATE")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Ho Quantity" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Eval("HO_QTY")%>
                                    </ItemTemplate>
                                </asp:TemplateField>                                

                                <asp:TemplateField HeaderText="Site Quantity" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("SITE_QTY")%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox runat="server" ID="txtSiteQty" CssClass="textEntry" Text='<%# Bind("SITE_QTY") %>' onkeypress='return event.charCode >= 48 && event.charCode <= 57'></asp:TextBox>
                                </EditItemTemplate>
                            </asp:TemplateField>                         

                               <asp:CommandField ShowEditButton="true" HeaderStyle-Width="5%" ItemStyle-HorizontalAlign="Center"  />  
                            </Columns>
                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                <b>No SOR Items.</b>
                            </EmptyDataTemplate>
                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                VerticalAlign="Middle" CssClass="myGridHeader" />
                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066" />
                            <AlternatingRowStyle BackColor="#e5eff8" BorderWidth="1px" />
                        </asp:GridView>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>
