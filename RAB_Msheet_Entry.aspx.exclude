<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RAB_Msheet_Entry.aspx.cs" Inherits="RAB_Msheet_Entry" %>

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
     

        function Done(jobNumber,tenderNumber,billType) {
           
            if (window.opener != null && !window.opener.closed) {
                window.opener.callbackBillEntry(true, jobNumber,tenderNumber,billType);
            }
            window.close();
        }

        function closeDialog() {
            window.opener.callbackBillEntry(false, jobNumber,tenderNumber,billType);
            window.close();
        }    

    </script>

</head>
<%--<body  onunload="refreshAndClose();">--%>
    <body >
    <form id="form1" runat="server">
       <div style="text-align:left;padding-left:15px;padding-top:20px;padding-bottom:15px;">
       <font style="color:Red">
       <a target="_blank" href="ExcelTemplates/MSheetTemp.xlsx"><b>Click here to download the sample Measurement Sheet template</b></a><br /><br />
       <b>Instructions</b><br />
       1. Only .xlsx file may be uploaded.<br />
       2. Do not add any Formula in the .xlsx file.<br />
       3. Do not add/change the cell positions of the text, otherwise data will not be uploaded.<br />
       4. Insert the new row for the measurements and enter <b>valid SOR sequence No and Unit</b><br />
       5. Unit is Mandatory field, else that row will not be considered<br />
       
       </font>
       </div>
      
            <table cellpadding="5" cellspacing="5" border="0" width="100%" class="myTable">
                <tr>
                    <td colspan="2" align="center" style="width: 100%">
                        <u>
                            <h1>Measurement Sheet</h1>
                        </u>
                    </td>
                </tr>

                <tr>
                <td class="myLabel" style="width: 30%">
                Sequence Number
                </td>
                <td>
                            <asp:Label ID="lblSeqNumber" runat=server Font-Bold=true></asp:Label>                           
                             <asp:HiddenField ID="hdreferenceId" runat="server"  />                                                                          
                             <asp:HiddenField ID="hdactivityId" runat="server"  />                                        
                             <asp:HiddenField ID="hdtenderSorId" runat="server"  />       
                             <asp:HiddenField ID="hdrunningSrNo" runat="server"  /> 

                             <asp:HiddenField ID="hdJobNumber" runat="server"  /> 
                             <asp:HiddenField ID="hdTenderNo" runat="server"  /> 
                             <asp:HiddenField ID="hdFilePath" runat="server" />
                </td>
                </tr>
                <tr>
                <td colspan="2" align="center">&nbsp;
                </td>
                </tr>
                <tr>
                <td class="myLabel" style="width: 30%">
                Browse Measurement Sheet
                </td>
                <td>
                <asp:FileUpload ID="fuMSheet" runat="server" />
                </td>
                </tr>

                <tr id="trSelectWorksheet" runat="server" >
                <td colspan="2" align="center" style="padding-top:10px;">
                <asp:Button ID="btnWorksheet" runat="server" OnClick="getWorkSheetNames" Text="Get WorkSheet" Font-Bold=true/>
                </td>
                </tr>

                <tr id="trWorkSheet" runat="server" visible="true">
                
                <td class="myLabel" style="width: 30%">Select Worksheet </td>
                <td>
                <asp:DropDownList ID="ddWorkSheet" CssClass="myInput" runat="server"></asp:DropDownList>
                </td>
                </tr>

                <tr id="trGv" runat="server" visible="true">
                <td colspan="2">
                <asp:GridView id="gvTemp" runat="server" Visible=true></asp:GridView>
                </td>
                </tr>

                <tr id="trUpload" runat="server" >
                <td colspan="2" align="center" style="padding-top:10px;">
                <asp:Button ID="btnUpload" runat="server" OnClick="ImportExcel" Text="Upload Measurement Sheet" Font-Bold=true Visible="false"/>
                </td>
                </tr>

                              
              
                <tr>
                    <td colspan="2" align="center"  style="padding-top:10px;">
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
                                <asp:CommandField ShowHeader="True" ShowDeleteButton="True" HeaderStyle-Width="5%" ControlStyle-ForeColor="Chocolate" Visible="false"/>
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
                                    <asp:Button ID="btnCancel" runat="server" Text="Close and View" Font-Bold="true" OnClick="btnCancel_Click" />
                                    <asp:Button ID="btnSimpleClose" runat="server" Text="Close" Font-Bold="true" OnClick="btnSimpleClose_Click" Visible="false"/>
                                   <br /><br /> <asp:Button ID="btnDeleteMSheet" runat="server" Text="Delete Measurement Sheet" Font-Bold="true" OnClick="btnDeleteMSheet_Click" OnClientClick="return confirm('Are you sure, you want to delete this measurement sheet?');" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        
    </form>
</body>
</html>
