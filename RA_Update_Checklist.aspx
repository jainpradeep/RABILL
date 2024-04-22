<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true"
    CodeFile="RA_Update_Checklist.aspx.cs" Inherits="RA_Update_Checklist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <table class="myTable">
        <tr>
            <td colspan="2" class="myGridHeader">
                Update Checklist
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">&nbsp;
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
            <td colspan="2" align="center">&nbsp;
            </td>
            </tr>
        <tr>
            <td colspan="2">
                <asp:GridView ID="gvChecklist" runat="server" Font-Size="10pt" AutoGenerateColumns="false" OnRowDataBound="gvChecklist_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#Container.DataItemIndex+1 %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Activities Completed">
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemTemplate>
                                <asp:Label ID="lblActivityDescription" runat="server" Text='<%#Eval("ITEMS_NAME") %>'></asp:Label>
                                <asp:HiddenField ID="hdJobNo" runat="server" Value='<%#Eval("JOB_NO") %>'/>
                                <asp:HiddenField ID="hdTenderNo" runat="server" Value='<%#Eval("TENDER_NO") %>'/>
                                <asp:HiddenField ID="hdPartNo" runat="server" Value='<%#Eval("PART_NO") %>'/>
                                <asp:HiddenField ID="hdCheckListId" runat="server" Value='<%#Eval("CHECKLIST_ID") %>'/>
                                <asp:HiddenField ID="hdVendor" runat="server" Value='<%#Eval("VENDER_CODE") %>'/>
                                <asp:HiddenField ID="hdBE" runat="server" Value='<%#Eval("CHECKED_BY_BE") %>'/>
                                <asp:HiddenField ID="hdAC" runat="server" Value='<%#Eval("CHECKED_BY_AC") %>'/>
                                <asp:HiddenField ID="hdRCM" runat="server" Value='<%#Eval("CHECKED_BY_RCM") %>'/>
                                <asp:HiddenField ID="hdIsFrozen" runat="server" Value='<%#Eval("IS_FREEZED") %>'/>
                                <asp:HiddenField ID="hdBECheck" runat="server" Value='<%#Eval("BE_CHECK") %>'/>
                                <asp:HiddenField ID="hdACCheck" runat="server" Value='<%#Eval("AC_CHECK") %>'/>
                                <asp:HiddenField ID="hdRCMCheck" runat="server" Value='<%#Eval("RCM_CHECK") %>'/>
                                <asp:HiddenField ID="hdVendorCheck" runat="server" Value='<%#Eval("VEND_CHECK") %>'/>
                                
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField  Visible="true" HeaderText="Vendor" >
                            <ItemTemplate>
                                <asp:CheckBox ID="chkVendor" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="5%"></HeaderStyle>
                        </asp:TemplateField>

                        <asp:TemplateField  Visible="true" HeaderText="BE" >
                            <ItemTemplate>
                                <asp:CheckBox ID="chkBE" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="5%"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="AC"  HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkAC" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="5%"></HeaderStyle>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="RCM"  HeaderStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkRCM" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="5%"></HeaderStyle>
                        </asp:TemplateField>
                    </Columns>
                    <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#000066" BackColor="AliceBlue"
                        Font-Size="9pt" />                    
                    <AlternatingRowStyle CssClass="myGridAlternatingItemStyle" BackColor="#F7F9FC" Font-Size="9pt" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
        <td colspan="2" align="center"><br />
        <asp:Button ID="btnSubmit" runat="server" Text="Submit" CssClass="myButton" OnClick="btnSubmit_Click" Font-Bold="true" Visible="false"/>
        <asp:Button ID="btnFreeze" runat="server" Text="Freeze Check List" CssClass="myButton" OnClick="btnFreeze_Click" Font-Bold="true" Visible="false"/>
        </td>
        </tr>
    </table>
</asp:Content>
