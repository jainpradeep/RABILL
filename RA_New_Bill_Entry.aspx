<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true" CodeFile="RA_New_Bill_Entry.aspx.cs" Inherits="RA_New_Bill_Entry" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
       <style type="text/css">
        .myRate1
        {
            display: none;
        }

        .cssPager td
        {
              padding-left: 4px;     
              padding-right: 4px;    
          }
    </style>
   

  <script language="javascript" type="text/javascript">


      window.callbackBillEntry = function (result, jobNumber, tenderNumber, billType) {
          
          //Code
          if (result == true) {
              //reload 

              // call this method to reload the added main item in SORNo dropdown
              document.forms[0].action = "RA_New_Bill_Entry.aspx";
              document.getElementById('<%=actionName.ClientID%>').value = "callbackBillEntry"
              document.getElementById('<%=jobNumber.ClientID%>').value = jobNumber
              document.getElementById('<%=tenderNumber.ClientID%>').value = tenderNumber
              document.getElementById('<%=hdBillType.ClientID%>').value = billType
              document.forms[0].method = "POST";
              document.forms[0].submit();
          }
          else {
              //don't reload - on cancel
          }
      }

</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <!-- Modal popup for updating the Bill -->

    <asp:Button ID="btnUpdateBill" runat="server" Text="Button" CssClass="hidden" />
    <asp:ModalPopupExtender ID="mpUpdateBill" runat="server" PopupControlID="pnl_updateBill_popup"
        TargetControlID="btnUpdateBill" CancelControlID="btnCancelUpdateBill"
        DropShadow="true" RepositionMode="RepositionOnWindowResizeAndScroll">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_updateBill_popup" Style="display: none; background: White; border: 3px solid; border-color: Gray; overflow: scroll; width: 900px; height: 700px;"
        runat="server">
        <div style="margin-right: 5px; margin-left: 5px; margin-bottom: 5px; margin-top: 5px;">
            <table cellpadding="5" cellspacing="5" border="0" width="100%">
                <tr>
                    <td colspan="2" align="center" class="myGridHeader">Update Bill                                              
                    </td>
                </tr>

                <tr id="tr3" runat="server" visible="true">
                    <td colspan="2">
                        <table cellpadding="5" cellspacing="5" border="0" width="100%">

                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <table cellpadding="5" cellspacing="5" border="0" width="80%" class="myTable">
                                        <tr>
                                            <td class="myLabel">Bill Number</td>
                                            <td>
                                                <asp:Label ID="lblUpdatedBillNo" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="myLabel">Bill Date</td>
                                            <td>
                                                <asp:Label ID="lblUpdatedBillDate" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="myLabel">Bill Period</td>
                                            <td>
                                                <asp:Label ID="lblUpdatedBillPeriod" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>

                            <tr id="trSelectSOR" runat="server" visible="false">
                                <td class="myLabel">
                                    <asp:Label ID="lblSelectSORNo" runat="server" Text="Select SOR Number"></asp:Label>

                                </td>
                                <td>
                                    <asp:DropDownList ID="ddSORNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddSORNumber_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr id="trSelectSequence" runat="server" visible="false">
                                <td class="myLabel">
                                    <asp:Label ID="lblSelectSeqNo" runat="server" Text="Select Sequence Number"></asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddSeqNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddSeqNumber_SelectedIndexChanged"
                                        AutoPostBack="true">
                                    </asp:DropDownList>
                                </td>
                            </tr>


                            <tr >
                                <td colspan="2">
                                    <asp:Label ID="lblSeqNote" runat="server" Text="NOTE: If splitted sequence is not visible to raise the bill, kindly check the approval of RCM for concerned item." ForeColor="Red" Font-Bold="true"></asp:Label>
                                </td>
                                
                            </tr>

                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>

                            <tr id="trUpdateItemsDetails" runat="server" visible="false">
                                <td colspan="2">
                                    <asp:Panel ID="pnlUpdateItems" runat="server">
                                        <asp:GridView ID="gvBillSeqItems" runat="server" Font-Names="Arial" Font-Size="8pt" Width="99%"
                                            AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                                            BorderStyle="None" OnRowDataBound="gvBillSeqItems_RowDataBound" AllowSorting="false" AllowPaging="false" PageSize="300">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>

                                                        <asp:Label ID="lblSortNo" runat="server" Text='<%#Eval("SORT_NO") %>' Font-Bold="true"></asp:Label>

                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="sdesc" HeaderText="SOR No" SortExpression="sdesc" Visible="false">
                                                    <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="myGridHeader"></HeaderStyle>
                                                </asp:BoundField>
                                                <asp:TemplateField HeaderText="Sequence No">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                                        <asp:HiddenField ID="hdSequenceNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                                        <asp:HiddenField ID="hdSortNumber" runat="server" Value='<%#Eval("SORT_NO") %>' />
                                                        <asp:HiddenField ID="hdItemRate" runat="server" Value='<%#Eval("ITEM_RATE") %>' />
                                                        <asp:HiddenField ID="hdItemQuantity" runat="server" Value='<%#Eval("HO_QTY") %>' />
                                                        <asp:HiddenField ID="hdSORQty" runat="server" Value='<%#Eval("SITE_QTY") %>' />
                                                        <asp:HiddenField ID="hdSORTenderId" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                        <asp:Label ID="lblSequenceNo" runat="server" Text='<%#Eval("SEQ_NO") %>' Font-Bold="true"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <%#Eval("ldesc")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="UOM" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Bold="true">
                                                    <ItemTemplate>
                                                        <%#Eval("UOM")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="HO Qty" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" Visible="false">
                                                    <ItemTemplate>
                                                        <%#Eval("HO_QTY")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SOR Qty" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true">
                                                    <ItemTemplate>
                                                        <%#Eval("SITE_QTY")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rate" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true">
                                                    <ItemTemplate>
                                                        <%#Eval("ITEM_RATE")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="" ItemStyle-Font-Bold="true">
                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotalAmount" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Qty upto previous Bill" ItemStyle-Font-Bold="true"
                                                    Visible="false">
                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAllTotalQty" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount upto previous Bill" ItemStyle-Font-Bold="true"
                                                    Visible="false">
                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAllTotalAmount" runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Details">
                                                    <HeaderStyle CssClass="myGridHeader"></HeaderStyle>
                                                    <ItemTemplate>
                                                        <div style="width: auto;">
                                                            <asp:GridView ID="gvSORSplits" runat="server" Font-Size="10pt" AutoGenerateColumns="false"
                                                                OnRowDataBound="gvSORSplits_RowDataBound">
                                                                <Columns>
                                                                    <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                        <ItemTemplate>
                                                                            <%#Container.DataItemIndex+1 %>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Activity">
                                                                        <ItemStyle HorizontalAlign="Left" />
                                                                        <ItemTemplate>
                                                                            <asp:HiddenField ID="hdChildReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                                                            <asp:HiddenField ID="hdChildSequenceNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                                                            <asp:HiddenField ID="hdActivityPercent" runat="server" Value='<%#Eval("ACTIVITY_PERCENT") %>' />
                                                                            <asp:HiddenField ID="hdIsBreakable" runat="server" Value='<%#Eval("IS_BREAKABLE") %>' />
                                                                            <asp:HiddenField ID="hdActivityId" runat="server" Value='<%#Eval("ACTIVITY_ID") %>' />
                                                                            <asp:HiddenField ID="hdContractorFrozen" runat="server" Value='<%#Eval("CONT_IS_FROZEN") %>' />
                                                                            <asp:HiddenField ID="hdVendorQty" runat="server" Value='<%#Eval("CONT_QTY") %>' />
                                                                            <asp:HiddenField ID="hdBEFrozen" runat="server" Value='<%#Eval("BENGG_IS_FROZEN") %>' />
                                                                            <asp:HiddenField ID="hdACFrozen" runat="server" Value='<%#Eval("AC_IS_FROZEN") %>' />
                                                                            <asp:HiddenField ID="hdRCMFrozen" runat="server" Value='<%#Eval("RCM_IS_FROZEN") %>' />
                                                                            <asp:HiddenField ID="hdBEQty" runat="server" Value='<%#Eval("BENGG_QTY") %>' />
                                                                            <asp:HiddenField ID="hdACQty" runat="server" Value='<%#Eval("AC_QTY") %>' />
                                                                            <asp:HiddenField ID="hdRCMQty" runat="server" Value='<%#Eval("RCM_QTY") %>' />
                                                                            <asp:HiddenField ID="hdRunSrNo" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                                                            <asp:HiddenField ID="hdTenderSORId" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                                            <asp:Label ID="lblActivityDescription" runat="server" Text='<%#Eval("ACTIVITY_DESC") %>'></asp:Label>
                                                                            <asp:HiddenField ID="hdInitialItemRate" runat="server" Value='<%#Eval("ITEM_RATE") %>' />


                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Percentage">
                                                                        <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblinitPercentage" runat="server" CssClass="myNumber5" Font-Bold="true"
                                                                                Text='<%#Eval("ACTIVITY_PERCENT")%>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Qty (Upto Prev. bill)">
                                                                        <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblQuantity" runat="server" CssClass="myNumber1" Font-Bold="true"
                                                                                Text='<%#Eval("previousQty")%>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Amount (Upto Previous Bill)">
                                                                        <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblAmount" runat="server" Font-Bold="true" Text='<%#Eval("activityAmt")%>'></asp:Label>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Contr. Current Qty" ItemStyle-HorizontalAlign="Right">
                                                                        <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                        <ItemStyle HorizontalAlign="Center" />
                                                                        <ItemTemplate>
                                                                            <asp:TextBox ID="txtVenQuantity" runat="server" MaxLength="15" Width="50" CssClass="myInput numeric myNumber2"
                                                                                ReadOnly="true" Text='<%#Eval("msheetQty")%>' Font-Bold="true" Visible="false"></asp:TextBox>
                                                                            <asp:Label ID="lblVendQuantity" runat="server" Text='<%#Eval("msheetQty")%>'></asp:Label>
                                                                            <asp:Label ID="lblMyInitAmount" runat="server" CssClass="myRate1" Visible="false"
                                                                                Text='<%#Eval("ITEM_RATE")%>'></asp:Label><br />

                                                                            <asp:HyperLink ID="hlUploadMSheet" runat="server" Target="_blank" Font-Bold="true"></asp:HyperLink>

                                                                            <asp:UpdatePanel runat="server" ID="test">
                                                                                <ContentTemplate>
                                                                                    <asp:Button ID="btnMeasurementSheet1" runat="server" Text="M. Sheet" Visible="false"
                                                                                        OnClick="btnMeasurementSheet1_Click" UseSubmitBehavior="false"
                                                                                        Font-Bold="true" Key='<%# Eval("REF_ID") + "$" + Eval("SEQ_NO") + "$" + Eval("ACTIVITY_ID") + "$" + Eval("BENGG_IS_FROZEN")+ "$" + Eval("ACTIVITY_DESC")+ "$" + Eval("AC_IS_FROZEN")+ "$" + Eval("RCM_IS_FROZEN")+ "$" + Eval("RUN_SL_NO")+ "$" + Eval("CONT_IS_FROZEN")+ "$" + Eval("TEND_SOR_ID")+ "$" + Eval("UOM")%>' />

                                                                                </ContentTemplate>
                                                                                <Triggers>

                                                                                    <asp:AsyncPostBackTrigger ControlID="btnMeasurementSheet1" EventName="Click" />
                                                                                </Triggers>
                                                                            </asp:UpdatePanel>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Total Quantity">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblMyTotal" runat="server" CssClass="myNumber3"></asp:Label><br />
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Total Amount">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblMyAmount" runat="server" CssClass="myNumber4"></asp:Label>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="BE">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblBEReject" runat="server"></asp:Label>
                                                                            <asp:CheckBox ID="chkBEReject" runat="server" />
                                                                            <asp:Label ID="lblBEQuantity" runat="server" Text='<%#Eval("BENGG_QTY") %>'></asp:Label>
                                                                            <asp:TextBox ID="txtBEQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="AC">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblACFrozen" runat="server"></asp:Label>
                                                                            <asp:CheckBox ID="chkACReject" runat="server" />
                                                                            <asp:Label ID="lblACQuantity" runat="server" Text='<%#Eval("AC_QTY") %>'></asp:Label>
                                                                            <asp:TextBox ID="txtACQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="RCM">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblRCMFrozen" runat="server"></asp:Label>
                                                                            <asp:CheckBox ID="chkRCMReject" runat="server" />
                                                                            <asp:Label ID="lblRCMQuantity" runat="server" Text='<%#Eval("RCM_QTY") %>'></asp:Label>
                                                                            <asp:TextBox ID="txtRCMQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                        </ItemTemplate>
                                                                        <HeaderStyle Width="10%"></HeaderStyle>
                                                                    </asp:TemplateField>

                                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="Status<br/>MSheet">
                                                                        <ItemTemplate>
                                                                            <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("STATUS") %>'></asp:Label>
                                                                            <br />
                                                                            <asp:HyperLink ID="hlMSheet" runat="server" Target="_blank" Font-Bold="true"></asp:HyperLink>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>

                                                                </Columns>
                                                                <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#000066" BackColor="AliceBlue"
                                                                    Font-Size="9pt" />
                                                                <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                                                            </asp:GridView>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                                            <EmptyDataTemplate>
                                                <b>No SOR Items.</b>
                                            </EmptyDataTemplate>
                                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                VerticalAlign="Middle" CssClass="myGridHeader" />
                                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                                Font-Size="9pt" />
                                            <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" CssClass="cssPager" Font-Bold="true" />
                                        </asp:GridView>
                                    </asp:Panel>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2">
                                    <asp:Panel ID="Panel1" runat="server">
                                        <div style="width: 100%; overflow: scroll; height: 200px;">
                                            <asp:GridView ID="gvAddedSequences" runat="server" Font-Names="Arial" Width="90%" AutoGenerateColumns="False"
                                                BorderWidth="1px" CellPadding="0" BorderColor="#CCCCCC" BorderStyle="None" Caption="Added Bill Items"
                                                Font-Size="Medium" OnRowDataBound="gvAddedSequences_RowDataBound">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="S.No " HeaderStyle-Width="5%">
                                                        <ItemTemplate>
                                                            <%#Container.DataItemIndex+1 %>

                                                            <asp:HiddenField ID="hdbillID" runat="server" Value='<%#Eval("bill_id") %>' />
                                                            <asp:HiddenField ID="hdBillRefID" runat="server" Value='<%#Eval("ref_id") %>' />

                                                            <asp:HiddenField ID="hdTenderSorRefID" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                            <asp:HiddenField ID="hdBillRunningSRNo" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                                            <asp:HiddenField ID="hdRAOverallBillNumber" runat="server" Value='<%#Eval("RA_BLL_NO") %>' />
                                                            <asp:HiddenField ID="hdBillStatus" runat="server" Value='<%#Eval("status") %>' />
                                                            <asp:HiddenField ID="hdBillSeqNo" runat="server" Value='<%#Eval("seq_no") %>' />
                                                            <asp:HiddenField ID="hdContQty" runat="server" Value='<%#Eval("cont_qty") %>' />
                                                            <asp:HiddenField ID="hdActSeq" runat="server" Value='<%#Eval("act_seq") %>' />
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Sequence No">
                                                        <ItemTemplate>
                                                            <%#Eval("seq_no")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Description">
                                                        <ItemTemplate>
                                                            <%#Eval("SDESC")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="UOM" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%#Eval("UOM")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="SOR Qty." ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%#Eval("SITE_QTY")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Item Rate" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <%#Eval("item_rate")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Percentage" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <%#Eval("ACTivity_PERCENT")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Cont. Qty" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <%#Eval("cont_qty")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right">
                                                        <ItemTemplate>
                                                            <%#Eval("qty_amount")%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>

                                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="M. Sheet" HeaderStyle-Width="10%">
                                                        <ItemTemplate>
                                                            <asp:HyperLink ID="hlActivity" runat="server" Target="_blank" Font-Bold="true"></asp:HyperLink>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>


                                                </Columns>
                                                <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                                                <EmptyDataTemplate>
                                                    <b>Bill not found.</b>
                                                </EmptyDataTemplate>
                                                <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" CssClass="myGridHeader" />
                                                <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                                    Font-Size="9pt" />
                                                <SelectedRowStyle CssClass="myGridSelectedItemStyle" />

                                            </asp:GridView>
                                        </div>
                                    </asp:Panel>
                                </td>
                            </tr>


                            <tr id="trRemarks" runat="server">
                                <td class="myLabel" width="20%" align="right">Remarks <font color="red">*&nbsp;&nbsp;</font>
                                </td>
                                <td style="width: 70%" align="left">
                                    <asp:TextBox ID="txtRemarks" runat="server" MaxLength="100" Width="450" ></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Label runat="server" ID="lblUpdateBillNote" ForeColor="Red" Font-Bold="true" Text="NOTE for (BE, AC and RCM): Please mention reason for rejection or else mention accept all in remark field"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr id="trButtons" runat="server">
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnSubmitCont" runat="server" Visible="false" Text="Submit to Billing Engineer" Font-Bold="true"
                                        OnClick="btnSubmitCont_Click" OnClientClick="return confirm('Are you sure, you want to Submit,Please re-check before submitting?');" CssClass="btn btn-danger" />
                                    <!-- BE Engg Buttons-->
                                    <asp:Button ID="btnBERejectAll" runat="server" Visible="false" Text="Reject All" Font-Bold="true"
                                        OnClick="btnBERejectAll_Click" OnClientClick="return confirm('Are you sure, you want to Reject all?');" CssClass="btn btn-danger" />
                                    <asp:Button ID="btnBERejectPartial" runat="server" Visible="false" Text="Reject Partial"
                                        Font-Bold="true" OnClick="btnBERejectPartial_Click" OnClientClick="return confirm('Are you sure, you want to Reject partial?');" />
                                    <asp:Button ID="btnBESubmit" runat="server" Visible="false" Text="Accept All (Send for Approval to AC)"
                                        Font-Bold="true" OnClick="btnBESubmit_Click" OnClientClick="return confirm('Are you sure, you want to send it to AC?');" CssClass="btn btn-success" />
                                    <!-- BE Engg Buttons-->
                                    <!-- AC Buttons-->
                                    <asp:Button ID="btnACSubmit" runat="server" Visible="false" Text="Accept All" Font-Bold="true"
                                        OnClick="btnACSubmit_Click" OnClientClick="return confirm('Are you sure, you want to Accept all?');" CssClass="btn btn-success" />
                                    <asp:Button ID="btnACReject" runat="server" Visible="false" Text="Reject All" Font-Bold="true"
                                        OnClick="btnACReject_Click" OnClientClick="return confirm('Are you sure, you want to Reject all?');" CssClass="btn btn-danger" />
                                    <!-- AC Buttons-->
                                    <!-- RCM Buttons-->
                                    <asp:Button ID="btnRCMRejectAll" runat="server" Visible="false" Text="Reject All"
                                        Font-Bold="true" OnClick="btnRCMRejectAll_Click" OnClientClick="return confirm('Are you sure, you want to Reject all?');" CssClass="btn btn-danger" />
                                    <asp:Button ID="btnRCMSubmit" runat="server" Visible="false" Text="Approve All" Font-Bold="true"
                                        OnClick="btnRCMSubmit_Click" OnClientClick="return confirm('Are you sure, you want to Approve all?');" CssClass="btn btn-success" />

                                    <!-- RCM Buttons-->
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>

                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button ID="btnUpdateNewBill" runat="server" Text="Update New Bill" Font-Bold="true"
                                        OnClick="btnUpdateNewBill_Click" CssClass="btn btn-primary" Visible="false" />
                                    &nbsp;&nbsp; 
                                    <asp:Button ID="btnCancelUpdateBill" runat="server" Text="Close" Font-Bold="true" OnClick="btnCancelUpdateBill_Click" CssClass="btn btn-primary" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" colspan="2">
                        <asp:Label ID="lblUpdateNewBill" runat="server" CssClass="myInput" Font-Bold="true" ForeColor="Red"></asp:Label>
                    </td>
                </tr>

            </table>
        </div>
    </asp:Panel>



    <!-- Modal popup for Creating New Bill entry -->

    <asp:Button ID="btnAddBill" runat="server" Text="Button" CssClass="hidden" />
    <asp:ModalPopupExtender ID="mpCreateBillEntry" runat="server" PopupControlID="pnl_addBill_popup"
        TargetControlID="btnAddBill" CancelControlID="btnCancelNewBill"
        DropShadow="true" RepositionMode="RepositionOnWindowResizeAndScroll">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_addBill_popup" Style="display: none; background: White; border: 3px solid; border-color: Gray; overflow: scroll; width: 500px; height: 400px;"
        runat="server">
        <div style="margin-right: 5px; margin-left: 5px; margin-bottom: 5px; margin-top: 5px;">
            <table cellpadding="5" cellspacing="5"  border="0" width="100%">
                <tr>
                    <td colspan="2" align="center" class="myGridHeader">Create New Bill
                        
                    </td>
                </tr>

                <tr id="tr2" runat="server" visible="true">
                    <td colspan="2">
                        <table cellpadding="5" cellspacing="5" border="0" width="100%">

                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Bill Number
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewBillNumber" runat="server" MaxLength="100" Width="200" CssClass="myInput"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">Bill Date
                                </td>
                                <td>
                                    <asp:TextBox ID="txtNewBillDate" runat="server" MaxLength="13" Width="100" onkeypress="return inputLimiter(event,'Date')"></asp:TextBox>

                                    <asp:ImageButton ID="imgCalBillDate" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif" />
                                    <asp:CalendarExtender ID="CalendarExtender3" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                                        TargetControlID="txtNewBillDate" PopupButtonID="imgCalBillDate" FirstDayOfWeek="Monday">
                                    </asp:CalendarExtender>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">Billing Period From (Date)
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPeriodFrom" runat="server" MaxLength="13" Width="100" onkeypress="return inputLimiter(event,'Date')"></asp:TextBox>

                                    <asp:ImageButton ID="imgCalFromDate" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif" />
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                                        TargetControlID="txtPeriodFrom" PopupButtonID="imgCalFromDate" FirstDayOfWeek="Monday">
                                    </asp:CalendarExtender>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">Billing Period To (Date)
                                </td>
                                <td>
                                    <asp:TextBox ID="txtPeriodTo" runat="server" MaxLength="13" Width="100" onkeypress="return inputLimiter(event,'Date')"></asp:TextBox>

                                    <asp:ImageButton ID="imgCalToDate" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif" />
                                    <asp:CalendarExtender ID="CalendarExtender4" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                                        TargetControlID="txtPeriodTo" PopupButtonID="imgCalToDate" FirstDayOfWeek="Monday">
                                    </asp:CalendarExtender>
                                </td>
                            </tr>


                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button ID="btnAddNewBill" runat="server" Text="Create New Bill" Font-Bold="true"
                                        OnClick="btnAddNewBill_Click" CssClass="btn btn-primary" />
                                    &nbsp;&nbsp;
                                    <asp:Button ID="btnCancelNewBill" runat="server" Text="Close" Font-Bold="true" OnClick="btnCanceNewBill_Click" CssClass="btn btn-primary" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" colspan="2">
                        <asp:Label ID="lblErrorNewBill" runat="server" CssClass="myInput" Font-Bold="true" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="Label5" runat="server" Font-Bold="true" CssClass="myInput"></asp:Label>
                        <asp:HiddenField ID="hdJobNoNewBill" runat="server" />
                        <asp:HiddenField ID="hdSubJobNoNewBill" runat="server" />
                        <asp:HiddenField ID="hdTenderNoNewBill" runat="server" />
                        <asp:HiddenField ID="hdPartNoNewBill" runat="server" />
                        <asp:HiddenField ID="hdSORIDNewBill" runat="server" />
                    </td>
                </tr>


            </table>
        </div>
    </asp:Panel>


    <asp:Panel ID="pnlSelection" runat="server">
        <table width="100%" class="myTable">
            <tr>
                <td colspan="2" align="center" class="myGridHeader">Add/Update Billing

                    <asp:HiddenField ID="actionName" runat="server" />
                    <asp:HiddenField ID="jobNumber" runat="server" />
                    <asp:HiddenField ID="tenderNumber" runat="server" />
                    <asp:HiddenField ID="hdBillType" runat="server" />

                </td>
            </tr>

            <tr>
                <td colspan="2" align="center">
                    <center>
                    <table width="90%">
                        <tr>
                            <td class="myLabel">Select Job Number
                            </td>
                            <td>
                                <asp:DropDownList ID="ddJobNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddJobNumber_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="myLabel">Select Tender Number
                            </td>
                            <td>
                                <asp:DropDownList ID="ddTenderNo" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddTenderNo_SelectedIndexChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </td>
                        </tr>

                        <tr>
                            <td></td>
                            <td>
                                <br />
                                <asp:Button ID="btnCreateNewBill" runat="server" Text="Create New Bill" CssClass="btn btn-primary" Visible="false" Font-Bold="true" OnClick="btnCreateNewBill_Click" />
                            </td>
                        </tr>

                        <tr id="trAllBills" runat="server" visible="true">
                            <td colspan="2" align="center" style="padding: 5px;width:95%;">
                                <asp:Panel ID="pnlBillsAdded" runat="server">
                                    <div style="width: 100%; overflow: scroll; height: 250px;">
                                        <asp:GridView ID="gvAllBills" runat="server" Font-Names="Arial" Width="90%" AutoGenerateColumns="False"
                                            BorderWidth="1px" CellPadding="0" BorderColor="#CCCCCC" BorderStyle="None" Caption="RA Bills"
                                            Font-Size="Medium" OnSelectedIndexChanged="gvAllBills_SelectedIndexChanged"
                                            OnRowDataBound="gvAllBills_RowDataBound">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No " HeaderStyle-Width="5%">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>

                                                        <asp:HiddenField ID="hdbillID" runat="server" Value='<%#Eval("ID") %>' />
                                                        <asp:HiddenField ID="hdTenderSorRefID" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                        <asp:HiddenField ID="hdBillRunningSRNo" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                                        <asp:HiddenField ID="hdRAOverallBillNumber" runat="server" Value='<%#Eval("RA_BLL_NO") %>' />
                                                        <asp:HiddenField ID="hdRABillNumber" runat="server" Value='<%#Eval("BILL_NUMBER") %>' />
                                                        <asp:HiddenField ID="hdBillStatus" runat="server" Value='<%#Eval("BILL_STATUS") %>' />
                                                        <asp:HiddenField ID="hdBillingDate" runat="server" Value='<%#Eval("BILL_DATE") %>' />

                                                        <asp:HiddenField ID="hdBillPeriodFrom" runat="server" Value='<%#Eval("PERIOD_FROM") %>' />
                                                        <asp:HiddenField ID="hdBillPeriodTo" runat="server" Value='<%#Eval("PERIOD_TO") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Date" HeaderStyle-Width="10%">
                                                    <ItemTemplate>
                                                        <%#Eval("BILL_DATE")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Number" HeaderStyle-Width="15%">
                                                    <ItemTemplate>
                                                        <%#Eval("BILL_NUMBER")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Period" HeaderStyle-Width="20%" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%#Eval("BILL_PERIOD")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Bill Status" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%#Eval("BILL_STATUS")%>
                                                    </ItemTemplate>
                                                </asp:TemplateField>


                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="Activity History"  ItemStyle-Height="45">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="hlActivity" runat="server" Target="_blank" Font-Bold="true" CssClass="btn btn-success"></asp:HyperLink>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:CommandField ShowHeader="True" ShowSelectButton="True" 
                                                    ItemStyle-ForeColor="White" ItemStyle-Font-Bold="true" SelectText="View Bill" ItemStyle-CssClass="btn btn-primary" ItemStyle-BorderColor="Black"/>
                                            </Columns>
                                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                                            <EmptyDataTemplate>
                                                <b>Bill not found.</b>
                                            </EmptyDataTemplate>
                                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                VerticalAlign="Middle" CssClass="myGridHeader" />
                                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                                Font-Size="9pt" />
                                            <SelectedRowStyle CssClass="myGridSelectedItemStyle" />
                                            <%-- <AlternatingRowStyle CssClass="myGridAlternatingItemStyle" />--%>
                                        </asp:GridView>
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>


                        <tr id="trItemsDetails" runat="server" visible="false">
                            <td colspan="2">
                                <asp:Panel ID="pnlSORItems" runat="server">
                                    <asp:GridView ID="gvSORItems" runat="server" Font-Names="Arial" Font-Size="8pt" Width="99%"
                                        AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                                        BorderStyle="None" OnRowDataBound="gvSORItems_RowDataBound" AllowSorting="false" AllowPaging="true" PageSize="300" OnPageIndexChanging="gvSORItems_PageIndexChanging">
                                        <Columns>
                                            <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>

                                                    <asp:Label ID="lblSortNo" runat="server" Text='<%#Eval("SORT_NO") %>' Font-Bold="true"></asp:Label>

                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="sdesc" HeaderText="SOR No" SortExpression="sdesc">
                                                <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="myGridHeader"></HeaderStyle>
                                            </asp:BoundField>
                                            <asp:TemplateField HeaderText="Sequence No">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                                    <asp:HiddenField ID="hdSequenceNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                                    <asp:HiddenField ID="hdSortNumber" runat="server" Value='<%#Eval("SORT_NO") %>' />
                                                    <asp:HiddenField ID="hdItemRate" runat="server" Value='<%#Eval("ITEM_RATE") %>' />
                                                    <asp:HiddenField ID="hdItemQuantity" runat="server" Value='<%#Eval("HO_QTY") %>' />
                                                    <asp:HiddenField ID="hdSORQty" runat="server" Value='<%#Eval("SITE_QTY") %>' />
                                                    <asp:HiddenField ID="hdSORTenderId" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                    <asp:Label ID="lblSequenceNo" runat="server" Text='<%#Eval("SEQ_NO") %>' Font-Bold="true"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Description" ItemStyle-HorizontalAlign="Left">
                                                <ItemTemplate>
                                                    <%#Eval("ldesc")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="UOM" ItemStyle-HorizontalAlign="Center" ItemStyle-Font-Bold="true">
                                                <ItemTemplate>
                                                    <%#Eval("UOM")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="HO Qty" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true" Visible="false">
                                                <ItemTemplate>
                                                    <%#Eval("HO_QTY")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="SOR Qty" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true">
                                                <ItemTemplate>
                                                    <%#Eval("SITE_QTY")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Rate" ItemStyle-HorizontalAlign="Right" ItemStyle-Font-Bold="true">
                                                <ItemTemplate>
                                                    <%#Eval("ITEM_RATE")%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="" ItemStyle-Font-Bold="true">
                                                <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTotalAmount" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Qty upto previous Bill" ItemStyle-Font-Bold="true"
                                                Visible="false">
                                                <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAllTotalQty" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount upto previous Bill" ItemStyle-Font-Bold="true"
                                                Visible="false">
                                                <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <asp:Label ID="lblAllTotalAmount" runat="server"></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Details">
                                                <HeaderStyle CssClass="myGridHeader"></HeaderStyle>
                                                <ItemTemplate>
                                                    <div style="width: auto;">
                                                        <asp:GridView ID="gvSORSplits" runat="server" Font-Size="10pt" AutoGenerateColumns="false"
                                                            OnRowDataBound="gvSORSplits_RowDataBound">
                                                            <Columns>
                                                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemTemplate>
                                                                        <%#Container.DataItemIndex+1 %>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Activity">
                                                                    <ItemStyle HorizontalAlign="Left" />
                                                                    <ItemTemplate>
                                                                        <asp:HiddenField ID="hdChildReferenceID" runat="server" Value='<%#Eval("REF_ID") %>' />
                                                                        <asp:HiddenField ID="hdChildSequenceNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                                                        <asp:HiddenField ID="hdActivityPercent" runat="server" Value='<%#Eval("ACTIVITY_PERCENT") %>' />
                                                                        <asp:HiddenField ID="hdIsBreakable" runat="server" Value='<%#Eval("IS_BREAKABLE") %>' />
                                                                        <asp:HiddenField ID="hdActivityId" runat="server" Value='<%#Eval("ACTIVITY_ID") %>' />
                                                                        <asp:HiddenField ID="hdContractorFrozen" runat="server" Value='<%#Eval("CONT_IS_FROZEN") %>' />
                                                                        <asp:HiddenField ID="hdVendorQty" runat="server" Value='<%#Eval("CONT_QTY") %>' />
                                                                        <asp:HiddenField ID="hdBEFrozen" runat="server" Value='<%#Eval("BENGG_IS_FROZEN") %>' />
                                                                        <asp:HiddenField ID="hdACFrozen" runat="server" Value='<%#Eval("AC_IS_FROZEN") %>' />
                                                                        <asp:HiddenField ID="hdRCMFrozen" runat="server" Value='<%#Eval("RCM_IS_FROZEN") %>' />
                                                                        <asp:HiddenField ID="hdBEQty" runat="server" Value='<%#Eval("BENGG_QTY") %>' />
                                                                        <asp:HiddenField ID="hdACQty" runat="server" Value='<%#Eval("AC_QTY") %>' />
                                                                        <asp:HiddenField ID="hdRCMQty" runat="server" Value='<%#Eval("RCM_QTY") %>' />
                                                                        <asp:HiddenField ID="hdRunSrNo" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                                                        <asp:HiddenField ID="hdTenderSORId" runat="server" Value='<%#Eval("TEND_SOR_ID") %>' />
                                                                        <asp:Label ID="lblActivityDescription" runat="server" Text='<%#Eval("ACTIVITY_DESC") %>'></asp:Label>
                                                                        <asp:HiddenField ID="hdInitialItemRate" runat="server" Value='<%#Eval("ITEM_RATE") %>' />
                                                                        <asp:HiddenField ID="HiddenField1" runat="server" Value='<%#Eval("ITEM_RATE") %>' />

                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Percentage">
                                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblinitPercentage" runat="server" CssClass="myNumber5" Font-Bold="true"
                                                                            Text='<%#Eval("ACTIVITY_PERCENT")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Qty (Upto Prev. bill)">
                                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblQuantity" runat="server" CssClass="myNumber1" Font-Bold="true"
                                                                            Text='<%#Eval("previousQty")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Amount (Upto Previous Bill)">
                                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblAmount" runat="server" Font-Bold="true" Text='<%#Eval("activityAmt")%>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField HeaderText="Contr. Current Qty" ItemStyle-HorizontalAlign="Right">
                                                                    <HeaderStyle CssClass="myTableHeader"></HeaderStyle>
                                                                    <ItemStyle HorizontalAlign="Center" />
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtVenQuantity" runat="server" MaxLength="15" Width="50" CssClass="myInput numeric myNumber2"
                                                                            ReadOnly="true" Text='<%#Eval("msheetQty")%>' Font-Bold="true"></asp:TextBox>
                                                                        <asp:Label ID="lblVendQuantity" runat="server" Text='<%#Eval("msheetQty")%>'></asp:Label> 
                                                                        <asp:Label ID="lblMyInitAmount" runat="server" CssClass="myRate1" Visible="false"
                                                                            Text='<%#Eval("ITEM_RATE")%>' ></asp:Label><br />

                                                                        <asp:HyperLink ID="hlUploadMSheet" runat="server" Target="_blank" Font-Bold="true"></asp:HyperLink>

                                                                        <asp:UpdatePanel runat="server" ID="test">
                                                                            <ContentTemplate>
                                                                                <asp:Button ID="btnMeasurementSheet" runat="server" Text="M. Sheet" Visible="false"
                                                                                    OnClick="btnMeasurementSheet_Click" UseSubmitBehavior="false"
                                                                                    Font-Bold="true" Key='<%# Eval("REF_ID") + "$" + Eval("SEQ_NO") + "$" + Eval("ACTIVITY_ID") + "$" + Eval("BENGG_IS_FROZEN")+ "$" + Eval("ACTIVITY_DESC")+ "$" + Eval("AC_IS_FROZEN")+ "$" + Eval("RCM_IS_FROZEN")+ "$" + Eval("RUN_SL_NO")+ "$" + Eval("CONT_IS_FROZEN")+ "$" + Eval("TEND_SOR_ID")+ "$" + Eval("UOM")%>' />

                                                                            </ContentTemplate>
                                                                            <Triggers>

                                                                                <asp:AsyncPostBackTrigger ControlID="btnMeasurementSheet" EventName="Click" />
                                                                            </Triggers>
                                                                        </asp:UpdatePanel>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Total Quantity">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMyTotal" runat="server" CssClass="myNumber3"></asp:Label><br />
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderText="Total Amount">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblMyAmount" runat="server" CssClass="myNumber4"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="BE">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblBEReject" runat="server"></asp:Label>
                                                                        <asp:CheckBox ID="chkBEReject" runat="server" />
                                                                        <asp:Label ID="lblBEQuantity" runat="server" Text='<%#Eval("BENGG_QTY") %>'></asp:Label>
                                                                        <asp:TextBox ID="txtBEQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="AC">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblACFrozen" runat="server"></asp:Label>
                                                                        <asp:CheckBox ID="chkACReject" runat="server" />
                                                                        <asp:Label ID="lblACQuantity" runat="server" Text='<%#Eval("AC_QTY") %>'></asp:Label>
                                                                        <asp:TextBox ID="txtACQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                                </asp:TemplateField>
                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="RCM">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblRCMFrozen" runat="server"></asp:Label>
                                                                        <asp:CheckBox ID="chkRCMReject" runat="server" />
                                                                        <asp:Label ID="lblRCMQuantity" runat="server" Text='<%#Eval("RCM_QTY") %>'></asp:Label>
                                                                        <asp:TextBox ID="txtRCMQuantity" runat="server" MaxLength="15" Width="40" class="myInput numeric" Visible="true"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <HeaderStyle Width="10%"></HeaderStyle>
                                                                </asp:TemplateField>

                                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" Visible="true" HeaderText="Status<br/>MSheet">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblStatus" runat="server" Text='<%#Eval("STATUS") %>'></asp:Label>
                                                                        <br />
                                                                        <asp:HyperLink ID="hlMSheet" runat="server" Target="_blank" Font-Bold="true"></asp:HyperLink>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                            </Columns>
                                                            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#000066" BackColor="AliceBlue"
                                                                Font-Size="9pt" />
                                                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                                                        </asp:GridView>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                                        <EmptyDataTemplate>
                                            <b>No SOR Items.</b>
                                        </EmptyDataTemplate>
                                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                            VerticalAlign="Middle" CssClass="myGridHeader" />
                                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                            Font-Size="9pt" />
                                        <PagerStyle BackColor="White" ForeColor="#000066" HorizontalAlign="Left" CssClass="cssPager" Font-Bold="true" />
                                    </asp:GridView>
                                </asp:Panel>
                            </td>
                        </tr>




                        <%--<tr id="trMultiRole" runat="server" visible="false">
                <td class="myLabel">
                    Select Role (Multiple Role found)
                </td>
                <td>
                    <asp:DropDownList ID="ddRole" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddRole_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>--%>
                    </table>
                        </center>
                </td>
            </tr>
        </table>
    </asp:Panel>


    <!-- Modal popup for Piping related items -->

    <asp:Button ID="btn_DummyMeasurementSheetP_Control" runat="server" Text="Button" CssClass="hidden" />
    <asp:ModalPopupExtender ID="ModalPopupExtenderForMSheetP" runat="server" PopupControlID="pnl_measurementSheetP_popup"
        TargetControlID="btn_DummyMeasurementSheetP_Control" CancelControlID="btnCancelP"
        DropShadow="true" RepositionMode="RepositionOnWindowResizeAndScroll">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_measurementSheetP_popup" Style="display: none; background: White; border: 3px solid; border-color: Gray; overflow: scroll; width: 800px;"
        runat="server">
        <div style="margin-right: 15px; margin-left: 15px; margin-bottom: 15px; margin-top: 5px;">
            <table cellpadding="5" cellspacing="5" border="0">
                <tr>
                    <td colspan="2" align="center" style="width: 100%">
                        <u>
                            <h1>Measurement Sheet </h1>
                        </u>
                    </td>
                </tr>

                <tr>
                    <td class="myLabel" style="width: 30%">Item Number
                    </td>
                    <td align="left" style="width: 70%">
                        <asp:Label ID="lblSequenceNumberP" runat="server" CssClass="myInput" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" style="width: 30%">UOM
                    </td>
                    <td align="left" style="width: 70%">
                        <asp:Label ID="lblUOM" runat="server" Font-Bold="true" CssClass="myInput"></asp:Label>
                    </td>
                </tr>
                <tr id="tr1" runat="server" visible="true">
                    <td colspan="2" style="width: 100%">
                        <table cellpadding="5" cellspacing="5" border="0">

                            <tr>
                                <td class="myLabel" style="width: 30%">Line Number
                                </td>
                                <td align="left" style="width: 70%">
                                    <asp:DropDownList ID="ddLineNumber" runat="server" AutoPostBack="true" CssClass="ddlInput" OnSelectedIndexChanged="ddLineNumber_SelectedIndexChanged"></asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">Joint Number
                                </td>
                                <td>
                                    <asp:TextBox ID="txtJointNo" runat="server" MaxLength="20" Width="100" CssClass="myInput"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">ID
                                </td>
                                <td>
                                    <asp:TextBox ID="txtQuantityP" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">ReportNo
                                </td>
                                <td>
                                    <asp:TextBox ID="txtReportNo" runat="server" MaxLength="50" Width="100" CssClass="myInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Inspection Date
                                </td>
                                <td>
                                    <asp:TextBox ID="txtInspectionDate" runat="server" MaxLength="13" Width="100" onkeypress="return inputLimiter(event,'Date')"></asp:TextBox>

                                    <asp:ImageButton ID="btnCal1" runat="server" Width="15px" Height="15px" ImageUrl="~/Images/cal.gif" />
                                    <asp:CalendarExtender ID="CalendarExtender1" runat="server" Format="dd-MM-yyyy" EnabledOnClient="true"
                                        TargetControlID="txtInspectionDate" PopupButtonID="btnCal1" FirstDayOfWeek="Monday">
                                    </asp:CalendarExtender>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">WPS No
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWPSNo" runat="server" MaxLength="50" Width="100" CssClass="myInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Welder No
                                </td>
                                <td>
                                    <asp:TextBox ID="txtWelderNo" runat="server" MaxLength="50" Width="100" class="myInput"></asp:TextBox>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">COM1
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlCom1" runat="server" CssClass="myInput"></asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">COM2
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlCom2" runat="server" CssClass="myInput"></asp:DropDownList>
                                </td>
                            </tr>

                            <tr>
                                <td class="myLabel">Remarks
                                </td>
                                <td>
                                    <asp:TextBox ID="txtItemRemarksP" runat="server" MaxLength="500" Width="350" class="myInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button ID="btnAddItemP" runat="server" Text="Add Item" CssClass="myButton" Font-Bold="true"
                                        OnClick="btnAddItemP_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" colspan="2">
                        <asp:Label ID="Label3" runat="server" CssClass="myInput" Font-Bold="true" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblErrorP" runat="server" Font-Bold="true" CssClass="myInput"></asp:Label>
                        <asp:HiddenField ID="hdActivityIdP" runat="server" />
                        <asp:HiddenField ID="hdReferenceIdP" runat="server" />
                        <asp:HiddenField ID="hdSequenceNumberP" runat="server" />
                        <asp:HiddenField ID="hdRunningSerailNoP" runat="server" />
                        <asp:HiddenField ID="hdDraftNoP" runat="server" />
                        <asp:HiddenField ID="hdBE_FrozenP" runat="server" />
                        <asp:HiddenField ID="hdCont_FrozenP" runat="server" />
                        <asp:HiddenField ID="hd_tenderSorIdP" runat="server" />
                        <asp:HiddenField ID="hd_UOMP" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:GridView ID="gvMeasurementSheetP" runat="server" ShowFooter="true" Font-Size="10pt"
                            AutoGenerateColumns="false" OnRowDataBound="gvMeasurementSheetP_RowDataBound" OnRowDeleting="gvMeasurementSheetP_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                        <asp:HiddenField ID="hdMSheetRSerialNumP" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                        <asp:HiddenField ID="hdMSheetIDP" runat="server" Value='<%#Eval("ID") %>' />
                                        <asp:HiddenField ID="hdMSheetRefIdP" runat="server" Value='<%#Eval("REF_ID") %>' />
                                        <asp:HiddenField ID="hdMSheetSeqNoP" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                        <asp:HiddenField ID="hdMSheetTSorIdP" runat="server" Value='<%#Eval("TENDER_SOR_ID") %>' />
                                        <asp:HiddenField ID="hdMSheetActSeqP" runat="server" Value='<%#Eval("ACT_SEQ") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Line No">
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("ACTIVTY_DESC")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unit">
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("UNIT")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Joint No">
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("JointNo")%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalQtyTextP" runat="server" Text="Total" Font-Bold="true" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Id">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <%#Eval("CALCULATED_QTY")%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalQtyP" runat="server" Font-Bold="true" ForeColor="Blue" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="W. V .Inspection ReportNo">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("ReportNo")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="W.V.Inspection Date">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("InspectionDate")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="WPS No">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("wpsNo")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Welder No">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("welderNo")%>
                                    </ItemTemplate>

                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="COM1">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <%#Eval("com1")%>
                                    </ItemTemplate>

                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="COM2">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <%#Eval("com2")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowHeader="True" ShowDeleteButton="True" HeaderStyle-Width="5%" ControlStyle-ForeColor="Chocolate" />
                            </Columns>
                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                <b>Measurement sheet entry not found.</b>
                            </EmptyDataTemplate>
                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                VerticalAlign="Middle" CssClass="myGridHeader" />
                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                Font-Size="9pt" Font-Bold="true" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                            <AlternatingRowStyle CssClass="myGridAlternatingItemStyle" BackColor="#F7F9FC" Font-Size="8pt" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="myInput" align="center" colspan="2">
                        <table>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnCancelP" runat="server" Text="Close" Font-Bold="true" OnClick="btnCancelP_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Button ID="btn_DummyMeasurementSheet_Control" runat="server" Text="Button" CssClass="hidden" />
    <asp:ModalPopupExtender ID="ModalPopupExtenderForMSheet" runat="server" PopupControlID="pnl_measurementSheet_popup"
        TargetControlID="btn_DummyMeasurementSheet_Control" CancelControlID="btnCancel"
        DropShadow="true" RepositionMode="RepositionOnWindowResizeAndScroll">
    </asp:ModalPopupExtender>
    <asp:Panel ID="pnl_measurementSheet_popup" Style="display: none; background: White; border: 3px solid; border-color: Gray; overflow: scroll; width: 800px;"
        runat="server">
        <div style="margin-right: 15px; margin-left: 15px; margin-bottom: 15px; margin-top: 5px;">
            <table cellpadding="5" cellspacing="5" border="0">
                <tr>
                    <td colspan="2" align="center" style="width: 100%">
                        <u>
                            <h1>Measurement Sheet</h1>
                        </u>
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" style="width: 30%">Activity description
                    </td>
                    <td align="left" style="width: 70">
                        <asp:Label ID="lblActivityDesc" runat="server" CssClass="myInput" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" style="width: 30%">Item Number
                    </td>
                    <td align="left" style="width: 70%">
                        <asp:Label ID="lblSequenceNumber" runat="server" CssClass="myInput" Font-Bold="true"></asp:Label>
                    </td>
                </tr>
                <tr id="trMSheetEntry" runat="server" visible="true">
                    <td colspan="2" style="width: 100%">
                        <table cellpadding="5" cellspacing="5" border="0">
                            <tr>
                                <td class="myLabel" style="width: 30%">Activity Description
                                </td>
                                <td align="left" style="width: 70%">
                                    <asp:TextBox ID="txtActDesc" runat="server" MaxLength="500" Width="350"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Unit
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddUnit" runat="server" CssClass="myInput">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Quantity
                                </td>
                                <td>
                                    <asp:TextBox ID="txtQuantity" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Length
                                </td>
                                <td>
                                    <asp:TextBox ID="txtLength" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Breadth
                                </td>
                                <td>
                                    <asp:TextBox ID="txtBreadth" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Height
                                </td>
                                <td>
                                    <asp:TextBox ID="txtHeight" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Unit Weight
                                </td>
                                <td>
                                    <asp:TextBox ID="txtUnitWeight" runat="server" MaxLength="13" Width="100" class="myInput numeric"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="myLabel">Remarks
                                </td>
                                <td>
                                    <asp:TextBox ID="txtItemRemarks" runat="server" MaxLength="500" Width="350" class="myInput"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">&nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" align="center">
                                    <asp:Button ID="btnAddItem" runat="server" Text="Add Item" CssClass="myButton" Font-Bold="true"
                                        OnClick="btnAddItem_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="myLabel" colspan="2">
                        <asp:Label ID="lblNote" runat="server" CssClass="myInput" Font-Bold="true" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblError" runat="server" Font-Bold="true" CssClass="myInput"></asp:Label>
                        <asp:HiddenField ID="hdActivityId" runat="server" />
                        <asp:HiddenField ID="hdReferenceId" runat="server" />
                        <asp:HiddenField ID="hdSequenceNumber" runat="server" />
                        <asp:HiddenField ID="hdRunningSerailNo" runat="server" />
                        <asp:HiddenField ID="hdDraftNo" runat="server" />
                        <asp:HiddenField ID="hdBE_Frozen" runat="server" />
                        <asp:HiddenField ID="hdCont_Frozen" runat="server" />
                        <asp:HiddenField ID="hd_tenderSorId" runat="server" />
                        <asp:HiddenField ID="hd_UOM" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="center">
                        <asp:GridView ID="gvMeasurementSheet" runat="server" ShowFooter="true" Font-Size="10pt"
                            AutoGenerateColumns="false" OnRowDataBound="gvMeasurementSheet_RowDataBound" OnRowDeleting="gvMeasurementSheet_RowDeleting">
                            <Columns>
                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1 %>
                                        <asp:HiddenField ID="hdMSheetRSerialNum" runat="server" Value='<%#Eval("RUN_SL_NO") %>' />
                                        <asp:HiddenField ID="hdMSheetID" runat="server" Value='<%#Eval("ID") %>' />
                                        <asp:HiddenField ID="hdMSheetRefId" runat="server" Value='<%#Eval("REF_ID") %>' />
                                        <asp:HiddenField ID="hdMSheetSeqNo" runat="server" Value='<%#Eval("SEQ_NO") %>' />
                                        <asp:HiddenField ID="hdMSheetTSorId" runat="server" Value='<%#Eval("TENDER_SOR_ID") %>' />
                                        <asp:HiddenField ID="hdMSheetActSeq" runat="server" Value='<%#Eval("ACT_SEQ") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Description">
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("ACTIVTY_DESC")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Unit">
                                    <ItemStyle HorizontalAlign="Left"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("UNIT")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("QUANTITY")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Length">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("LENGTH")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Breadth">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("BREADTH")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Height">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("HEIGHT")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Height">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <ItemTemplate>
                                        <%#Eval("unit_Weight")%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalQtyText" runat="server" Text="Total Quantity" Font-Bold="true" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quantity">
                                    <ItemStyle HorizontalAlign="Right"></ItemStyle>
                                    <FooterStyle HorizontalAlign="Right" />
                                    <ItemTemplate>
                                        <%#Eval("CALCULATED_QTY")%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalQty" runat="server" Font-Bold="true" ForeColor="Blue" />
                                    </FooterTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Remarks">
                                    <ItemStyle HorizontalAlign="Left" />
                                    <ItemTemplate>
                                        <%#Eval("REMARKS")%>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowHeader="True" ShowDeleteButton="True" HeaderStyle-Width="5%" ControlStyle-ForeColor="Chocolate" />
                            </Columns>
                            <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                            <EmptyDataTemplate>
                                <b>Measurement sheet entry not found.</b>
                            </EmptyDataTemplate>
                            <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                VerticalAlign="Middle" CssClass="myGridHeader" />
                            <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                                Font-Size="9pt" Font-Bold="true" />
                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                            <AlternatingRowStyle CssClass="myGridAlternatingItemStyle" BackColor="#F7F9FC" Font-Size="8pt" />
                        </asp:GridView>
                    </td>
                </tr>
                <tr>
                    <td class="myInput" align="center" colspan="2">
                        <table>
                            <tr>
                                <td align="center" colspan="2">
                                    <asp:Button ID="btnCancel" runat="server" Text="Close" Font-Bold="true" OnClick="btnCancel_Click" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

</asp:Content>

