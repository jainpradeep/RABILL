<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true"
    CodeFile="RA_MOP_Fill.aspx.cs" Inherits="RA_MOP_Fill" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <table width="100%" class="myTable">
        <tr>
            <td colspan="2" align="center" class="myGridHeader">
                MOP 
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
        <tr id="trBillNu" runat="server" visible="true">
            <td class="myLabel">
                Select Bill Number
            </td>
            <td>
                <asp:DropDownList ID="ddBillNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddBillNumber_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:DropDownList>
            </td>
        </tr>
       
    </table>
    <asp:Panel ID="pnlJobMOP" runat="server" Visible="false">
        <table width="100%" class="myTable">
            <tr>
            <td colspan="2" align="center" class="myGridHeader">
                MEMORANDUM OF PAYMENT
            </td>
        </tr>
         <tr id="trShowMOPEdit" runat="server" visible="false">
                <td class="myLabel" colspan="2" align="right">
                    <asp:HyperLink ID="lnlMOPEdit" runat="server" Font-Bold=true ForeColor="Brown"></asp:HyperLink>
                </td>
            </tr>

            <tr>
                <td class="myLabel">
                    Bill No
                </td>
                <td>
                    <asp:Label ID="lblBillNumber" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    Bill Date
                </td>
                <td>
                    <asp:Label ID="lblBillDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    Period of Measurement
                </td>
                <td>
                    <asp:Label ID="lblPeriod" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    1. Name of Work
                </td>
                <td>
                    <asp:Label ID="lblWork" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    2. Name of Contractor
                </td>
                <td>
                    <asp:Label ID="lblContractor" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    3. W.O./L.O.I. No.
                </td>
                <td>
                    <asp:Label ID="lblFOI" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    4. Date of Award of Contract
                </td>
                <td>
                    <asp:Label ID="lblDateAwarded" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    5. Contractual Completion duration
                </td>
                <td>
                    <asp:Label ID="lblCompletionDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="myLabel">
                    6. Contract Value
                </td>
                <td>
                    <asp:Label ID="lblContractValue" runat="server"></asp:Label>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                   <hr />
                </td>
            </tr>

            <tr>
                <td class="myLabel">
                    7. Enhanced Contract Value, if any
                </td>
                <td>
                    <asp:Label ID="lblEnhancedValue" runat="server"></asp:Label>
                    <asp:TextBox ID="txtEnhancedValue" runat="server" MaxLength="12" Width="300" ></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>

            <tr>
                <td class="myLabel">
                    8. Actual Completion Date/Remarks
                </td>
                <td>                    
                    <asp:Label ID="lblActualCompletionDate" runat="server"></asp:Label>
                    <asp:TextBox ID="txtCompletionDate" runat="server" MaxLength="100" Width="300" ></asp:TextBox>                                    
                                  
                  <%--  <asp:TextBox ID="txtCompletionDate" runat="server" MaxLength="13" Width="100" onkeypress="return inputLimiter(event,'Date')"></asp:TextBox>
                                    
                                    <asp:ImageButton ID="btnCal1" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif" />
                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                        TargetControlID="txtCompletionDate" PopupButtonID="btnCal1" FirstDayOfWeek="Monday">
                    </asp:CalendarExtender>--%>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>

            <tr>
                <td class="myLabel">
                    9. Extension of Time Period, If any
                </td>
                <td>
                    <asp:Label ID="lblTimeExtension" runat="server"></asp:Label>
                    <asp:TextBox ID="txtTimeExtension" runat="server" MaxLength="100" Width="300" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>

            <tr>
                <td class="myLabel">
                    10. % Progress
                </td>
                <td>
                <asp:Label ID="lblPercentProgress" runat="server"></asp:Label>
                    <asp:TextBox ID="txtPercentProgress" runat="server" MaxLength="100" Width="300" ></asp:TextBox>

                    <asp:HiddenField ID="hdMOPId" runat="server"/>
                    <asp:HiddenField ID="hdMOPVendStatus" runat="server"/>
                    <asp:HiddenField ID="hdMOPBEStatus" runat="server"/>
                    <asp:HiddenField ID="hdMOPACStatus" runat="server"/>
                    <asp:HiddenField ID="hdMOPRCMStatus" runat="server"/>

                </td>
            </tr>
            <tr>
                <td colspan="2">
                    &nbsp;
                </td>
            </tr>

            <tr id="trHeaderRow" runat="server" visible="false">
                <td colspan="2" align="center" class="myGridHeader">
                    <asp:GridView ID="gvMOPHeading" runat="server" Font-Names="Arial" Font-Size="8pt"
                         AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None" OnRowDataBound="gvMOPHeading_RowDataBound" AllowSorting="true" ShowHeader="false" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Left" ItemStyle-VerticalAlign="Top"  ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="lblHeadingOrder" runat="server" Text='<%#Eval("HEADING_ORDER") %>'
                                        Font-Bold="true" width="15%"></asp:Label>
                                        
                                </ItemTemplate>
                                
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Heading" ItemStyle-VerticalAlign="Top"  ItemStyle-Width="35%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdHeadingID" runat="server" Value='<%#Eval("heading_id") %>' />
                                    <asp:HiddenField ID="hdSubHeadingID" runat="server" Value='<%#Eval("sub_heading") %>' />
                                    <asp:HiddenField ID="hdValueExists" runat="server" Value='<%#Eval("VALUE_EXISTS") %>' />
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Eval("description") %>' Font-Bold="true"
                                       ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblValue" runat="server" Font-Bold="true" Text='<%#Eval("HEADING_VALUE") %>'></asp:Label>
                                    <asp:TextBox ID="txtValue" runat="server" Text='<%#Eval("HEADING_VALUE") %>' CssClass="myInput numeric"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>

                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No MOP </b>
                        </EmptyDataTemplate>
                        
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%" border="1">
                        <tr>
                            <td style="width:5%" Height="20px">
                                <b>10.</b>
                            </td>
                            <td style="width:35%" class="myLabel">
                                Value of Work Done (X)
                            </td>
                            <td align="center" style="width:20%">
                               <b> Upto<br /> Previous Bill</b>
                            </td>
                            <td align="center" style="width:20%">
                                <b>Since<br /> Previous Bill</b>
                            </td>
                            <td align="center" style="width:20%">
                                <b>Total<br /> Upto Date</b>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:5%">
                            </td>
                            <td style="width:35%">
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblUptoPreviousBill" runat="server"></asp:Label>
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblSincePreviousBill" runat="server"></asp:Label>
                                <asp:TextBox ID="txtSincePreviousBill" runat="server" Visible="false"
                                ></asp:TextBox>
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblTotalUptodate" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

             <tr>
                <td colspan="2">
                    <table width="100%" style="font:Bold;">
                        <tr>
                            <td style="width:5%" Height="20px">
                               <b> 11.</b>
                            </td>
                            <td align="left" class="myLabel" style="width:35%">
                                RECOVERIES (*)
                            </td>
                            <td style="width:20%">
                               
                            </td>
                            <td style="width:20%">
                                
                            </td>
                            <td style="width:20%" align="right">
                                <asp:Button ID="btnEditRecoveries" runat="server" Text="Edit MOP values" CssClass="myButton" OnClick="btnEditRecoveries_Click"
            OnClientClick="return confirm('Are you sure, you want to edit values of MOP?');" Visible="false" Font-Bold="true"/>
                            </td>
                        </tr>
                     
                    </table>
                </td>
            </tr>


            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    <asp:GridView ID="gvMopRecoveriesValues" runat="server" Font-Names="Arial" Font-Size="8pt"
                         AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None" OnRowDataBound="gvMopRecoveriesValues_RowDataBound" AllowSorting="true" ShowHeader=false  Width="100%">
                        <Columns>
                           <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="left" ItemStyle-VerticalAlign="Top" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubHeadingOrder" runat="server" Text='<%#Eval("SUB_HEADING_ORDER") %>' Font-Bold="true"
                                       ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Heading" ItemStyle-VerticalAlign="Top" ItemStyle-Width="35%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdHeadingID" runat="server" Value='<%#Eval("heading_id") %>' />
                                    <asp:HiddenField ID="hdSubHeadingID" runat="server" Value='<%#Eval("sub_heding_id") %>' />
                                    <asp:HiddenField ID="hdValueExists" runat="server" Value='<%#Eval("VALUE_EXISTS") %>' />
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Eval("description") %>' Font-Bold="true"
                                        Width="300"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%-- <asp:TextBox ID="txtUptoPrevBill" runat="server" Text='<%#Eval("uptoPrevBill") %>' CssClass="myInput numeric"></asp:TextBox>--%>
                                    <asp:Label ID="lbluptoPrevBill" runat="server" Text='<%#Eval("uptoPrevBill") %>' Font-Bold="true"
                                        ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblSincePrevBill" runat="server" Font-Bold="true" Text='<%#Eval("sincePrevBill") %>'></asp:Label>
                                    <asp:TextBox ID="txtSincePrevBill" runat="server" Text='<%#Eval("sincePrevBill") %>' CssClass="myInput numeric"></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lbltotalBill" runat="server" Text='<%#Eval("total") %>' Font-Bold="true"
                                        ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No values </b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <table width="100%" border="1">
                        <tr>
                            <td style="width:5%">
                                <b>12.</b>
                            </td>
                            <td align="left"  class="myLabel" style="width:35%">
                                Total Recoveries (Y)
                            </td>
                           
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblTotalRecovPB" runat="server" Font-Bold=true></asp:Label>
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblTotalRecovSPB" runat="server" Font-Bold=true></asp:Label>
                                
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblTotalRecovTotal" runat="server" Font-Bold=true></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>

            <tr>
                <td colspan="2">
                    <table width="100%">
                        <tr>
                            <td style="width:5%">
                                <b>13.</b>
                            </td>
                            <td align="left"  class="myLabel" style="width:35%">
                                PAYMENTS RECOMMENDED :
                            </td>
                            <td style="width:20%">
                               
                            </td>
                            <td style="width:20%">
                                
                            </td>
                            <td style="width:20%">
                                                  
                            </td>
                        </tr>
                      </table> 
                </td>
            </tr>

            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    <asp:GridView ID="gvMopPaymentRecommendations" runat="server" Font-Names="Arial" Font-Size="8pt"
                         AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None" OnRowDataBound="gvMopPaymentRecommendations_RowDataBound" AllowSorting="true" ShowHeader=false  Width="100%">
                        <Columns>
                           <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="left" ItemStyle-VerticalAlign="Top" ItemStyle-Width="5%">
                                <ItemTemplate>
                                    <asp:Label ID="lblSubHeadingOrder" runat="server" Text='<%#Eval("SUB_HEADING_ORDER") %>' Font-Bold="true"
                                       ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Heading" ItemStyle-VerticalAlign="Top" ItemStyle-Width="35%">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdHeadingID" runat="server" Value='<%#Eval("heading_id") %>' />
                                    <asp:HiddenField ID="hdSubHeadingID" runat="server" Value='<%#Eval("sub_heding_id") %>' />
                                    <asp:HiddenField ID="hdValueExists" runat="server" Value='<%#Eval("VALUE_EXISTS") %>' />
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Eval("description") %>' Font-Bold="true"
                                        Width="300"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    
                                    <asp:Label ID="lbluptoPrevBill" runat="server" Text='<%#Eval("uptoPrevBill") %>' Font-Bold="true"
                                        ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblSincePrevBill" runat="server" Font-Bold="true" Text='<%#Eval("sincePrevBill") %>'></asp:Label>
                                    <asp:TextBox ID="txtSincePrevBill" runat="server" Text='<%#Eval("sincePrevBill") %>'></asp:TextBox>
                                </ItemTemplate>
                            </asp:TemplateField>
                             <asp:TemplateField HeaderText="" ItemStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lbltotalBill" runat="server" Text='<%#Eval("total") %>' Font-Bold="true"
                                        ></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No values </b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%" border="1">
                        <tr>
                            <td style="width:5%">
                               <b> 14.</b>
                            </td>
                            <td align="left"  class="myLabel" style="width:35%">
                                Total Payments Recommended (Z)
                            </td>
                           
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblPaymentRecUP" runat="server" Font-Bold=true></asp:Label>
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblPaymentRecSP" runat="server" Font-Bold=true></asp:Label>
                                
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblPaymentRecTU" runat="server" Font-Bold=true></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <table width="100%"  border="1">
                        <tr>
                            <td style="width:5%">
                                <b>15.</b>
                            </td>
                            <td align="left"  class="myLabel" style="width:35%">
                                Net Amount Payable (X-Y+Z)
                            </td>
                            <td style="width:20%">
                               
                            </td>
                            <td style="width:20%">
                                
                            </td>
                            <td style="width:20%">
                                
                            </td>
                        </tr>
                        <tr>
                            <td style="width:5%">
                            </td>
                            <td style="width:35%">
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblNetAmountUP" runat="server" Font-Bold=true></asp:Label>
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblNetAmountSP" runat="server" Font-Bold=true></asp:Label>
                                
                            </td>
                            <td style="width:20%" align="center">
                                <asp:Label ID="lblNetAmountTU" runat="server" Font-Bold=true></asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>


            <tr>
            <td colspan="2">
            <asp:Label ID="lblMOPStatus" runat="server" Font-Bold=true ForeColor="Brown"></asp:Label>
            </td>
            </tr>

            <tr id="trActionButtons" runat="server" visible="false">
            <td colspan="2" align="center" style="font-weight:bold">

            <asp:Button ID="btnAddMOPVend" Text="Submit MOP (Send to BE for approval)" runat="server" 
            OnClick="btnAddMOPVend_Click" OnClientClick="return confirm('Are you sure, you want to Send MOP to BE?');"/>

            <asp:Button ID="btnAddMopData" Text="Submit MOP (Send to AC for approval)" runat="server" OnClick="btnAddMopData_Click"
            OnClientClick="return confirm('Are you sure, you want to Send MOP to AC?');"/>

            <asp:Button ID="btnRejectMOPBE" Text="Reject MOP (Send to Vendor for Correction)" runat="server" OnClick="btnRejectMOPBE_Click"
            OnClientClick="return confirm('Are you sure, you want to reject and Send MOP to Vendor?');"/>

            <asp:Button ID="btnApproveAC" Text="Submit MOP to RCM" runat="server" OnClick="btnApproveAC_Click"
            OnClientClick="return confirm('Are you sure, you want to send MOP to RCM?');"/>
            <asp:Button ID="btnRejectAC" Text="Reject MOP (Send back to BE)" runat="server" OnClick="btnRejectAC_Click"
            OnClientClick="return confirm('Are you sure, you want to Reject ?');"/>

            <asp:Button ID="btnApproveRCM" Text="Approve MOP" runat="server" OnClick="btnApproveRCM_Click"
            OnClientClick="return confirm('Are you sure, you want to Approve MOP?');"/>
            <asp:Button ID="btnRejectRCM" Text="Reject MOP (Send back to AC)" runat="server" OnClick="btnRejectRCM_Click"
            OnClientClick="return confirm('Are you sure, you want to Reject this MOP?');"/>

            </td>
            </tr>

            <tr>
            <td colspan="2">&nbsp;</td>
            </tr>

        </table>
    </asp:Panel>
</asp:Content>
