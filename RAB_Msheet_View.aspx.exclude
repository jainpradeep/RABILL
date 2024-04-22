<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RAB_Msheet_View.aspx.cs"
    Inherits="RAB_Msheet_View" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RAB: Measurement Sheet</title>
    <link href="css/Site.css" rel="stylesheet" type="text/css" />
    <link href="css/EILDesign.css" rel="stylesheet" type="text/css" />
    <link href="css/EILDesign.css" type="text/css" rel="stylesheet" />
    <link href="css/ui-lightness/jquery-ui.css" rel="stylesheet" type="text/css" />
    <link href="css/bootstrap.min.css" rel="stylesheet" />
    <script src="js/jquery-1.9.1.js" type="text/javascript"></script>
    <script src="js/jquery-ui-1.10.3.custom.min.js" type="text/javascript"></script>
    <script src="js/jquery.numeric.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function refreshAndClose() {
            window.opener.location.reload(true);
            window.close();
        }


        function Done(jobNumber, tenderNumber, billType) {

            if (window.opener != null && !window.opener.closed) {
                window.opener.callbackBillEntry(true, jobNumber, tenderNumber, billType);
            }
            window.close();
        }

        function closeDialog() {
            window.opener.callbackBillEntry(false, jobNumber, tenderNumber, billType);
            window.close();
        }    

    </script>
    </script>
</head>
<!--<body  onunload="refreshAndClose();">-->
<body>
    <form id="form1" runat="server">
    <table cellpadding="5" cellspacing="5" border="0" width="100%" class="myTable">
        <tr>
            <td colspan="2" align="center" style="width: 100%">
                <u>
                    <h1>
                        Measurement Sheet</h1>
                </u>
            </td>
        </tr>
        <tr>
            <td class="myLabel" style="width: 30%">
                Sequence Number
            </td>
            <td>
                <asp:Label ID="lblSeqNumber" runat="server" Font-Bold="true"></asp:Label>
                <asp:HiddenField ID="hdreferenceId" runat="server" />
                <asp:HiddenField ID="hdactivityId" runat="server" />
                <asp:HiddenField ID="hdtenderSorId" runat="server" />
                <asp:HiddenField ID="hdrunningSrNo" runat="server" />
                <asp:HiddenField ID="hdJobNumber" runat="server" />
                <asp:HiddenField ID="hdTenderNo" runat="server" />
                <asp:HiddenField ID="hdFilePath" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center" style="padding-top: 10px;">
                <asp:GridView ID="gvMeasurementSheet" runat="server" ShowFooter="true" Font-Size="10pt"
                    AutoGenerateColumns="false" OnRowDataBound="gvMeasurementSheet_RowDataBound">
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
                        <asp:TemplateField HeaderText="Dia">
                            <ItemStyle HorizontalAlign="Right"></ItemStyle>
                            <ItemTemplate>
                                <%#Eval("unit4")%>
                            </ItemTemplate>
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
