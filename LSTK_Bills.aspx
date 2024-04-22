<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true" CodeFile="LSTK_Bills.aspx.cs" Inherits="LSTK_Bills" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

    <table width="100%" class="myTable" id="Table2" runat="server">
      <tr>
            <td colspan="2" align="center" class="myGridHeader">
                View/Upload LSTK Contracts
            </td>
        </tr>
        <tr >
                <td colspan="2" class="myLabel" align="center">
                    Upload New Bill&nbsp;<asp:RadioButton ID="rbNewBill" runat="server" GroupName="rbBill"
                        OnCheckedChanged="rbNewBill_CheckedChanged" AutoPostBack="true" />&nbsp;&nbsp;
                    Search Bill &nbsp;<asp:RadioButton ID="rbSearchBill" runat="server" GroupName="rbBill"
                        AutoPostBack="true" OnCheckedChanged="rbSearchBill_CheckedChanged" />
                </td>
        </tr>
        <tr>
             <td colspan="2">&nbsp;</td>
         </tr>
    </table>
      <table width="70%" class="myTable" id="tblSearch" runat="server" visible="false">
        <tr>
            <td colspan="2" align="center" class="myGridHeader">
                View LSTK Bills
            </td>
        </tr>
        <tr>
            <td class="myLabel">
                Select Job Number
            </td>
            <td>
                <asp:DropDownList ID="ddJobNumberSearch" runat="server" CssClass="myInput" 
                    AutoPostBack="false">
                </asp:DropDownList>
            </td>
        </tr>

          <tr>
                         <td class="myLabel">Job Description</td>
                        <td><asp:TextBox ID="txtJobDescription" runat="server" MaxLength="100" CssClass="myInput" Width="300"></asp:TextBox> </td>
                    </tr>
                    <tr>
                         <td class="myLabel">Contractor Name</td>
                        <td> 
                            <asp:TextBox ID="txtContractorName" runat="server" MaxLength="100" CssClass="myInput" Width="300"></asp:TextBox>
                        </td>
                    </tr>  
          <tr>                        
                        <td colspan="2">&nbsp;</td>
              </tr>
          <tr>                        
                        <td colspan="2" align="center">

                            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                     <asp:Button ID="btnSearchBill" runat="server" Text="Search" OnClick="btnSearchBill_Click" Font-Bold="true" CssClass="myButton" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btnSearchBill" />
                                </Triggers>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
       
          <tr>
              <td colspan="2"><hr /></td>
          </tr>

            <tr>
                        <td colspan="2">
                            <asp:GridView ID="gvSearch" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered dataTable"
                                HeaderStyle-ForeColor="Black" DataKeyNames="ID" OnRowDataBound="gvSearch_RowDataBound" >
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No." ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="50">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                            <asp:HiddenField ID="hdIDSearch" runat="server" Value='<%# Bind("ID")%>' />
                                            <asp:HiddenField ID="hdAttachmentNameSearch" runat="server" Value='<%# Bind("FILE_NAME")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                                  
                                     <asp:TemplateField HeaderText="Job No." ItemStyle-Wrap="true" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <%#Eval("JOB_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Tender No." ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("TENDER_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Part No." ItemStyle-Wrap="true" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <%#Eval("PART_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Contractor" ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("CONTRACTOR_NAME")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Job Desc" ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("JOB_DESC")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                  

                                    <asp:TemplateField HeaderText="Bill Type" ItemStyle-Wrap="true" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <%#Eval("ATTACH_TYPE")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Upload Date" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <%#Eval("UPLOADED_ON")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Wrap="true">
                                        <ItemTemplate>
                                            <%#Eval("REMARKS")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" ItemStyle-Wrap="true" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlAttachmentSearch" runat="server" CssClass="btn btn-primary" Text="View"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                   
                                </Columns>
                                <EmptyDataTemplate>
                                    <b>Bills not found</b>
                                </EmptyDataTemplate>
                                <RowStyle Wrap="true" />
                            </asp:GridView>
                        </td>
                    </tr>
         </table>
    
    <table width="100%" class="myTable" id="tblUploadBill" runat="server" visible="false">
        <tr>
            <td colspan="2" align="center" class="myGridHeader">
                Add / View LSTK Bills
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
        <tr  runat="server" >
            <td class="myLabel">
                Select Part Number
            </td>
            <td>
                <asp:DropDownList ID="ddPartNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddPartNumber_SelectedIndexChanged"
                    AutoPostBack="true">
                </asp:DropDownList>
            </td>
        </tr>

         <tr  runat="server" >
            <td class="myLabel">
                Job Name
            </td>
             <td>
                 <asp:Label ID="lblJobName" runat="server" CssClass="myInput" Font-Bold="true"></asp:Label>
             </td>
             </tr>

         <tr  runat="server" >
            <td class="myLabel">
                Contractor
            </td>
             <td>
                 <asp:Label ID="lblContractor" runat="server" CssClass="myInput" Font-Bold="true"></asp:Label>
             </td>
             </tr>
        <tr>
             <td colspan="2"><br /></td>
         </tr>
        <tr id="trUploadBills" runat="server" visible="false">
            <td colspan="2" align="center">
                <table width="50%" cellpadding="5" cellspacing="5">
                    <tr><td colspan="2" class="myGridHeader">Upload New Bill </td></tr>
                    <tr>
                        <td class="myInput">Select Bill Type</td>
                        <td>
                            <asp:HiddenField ID="hdifJobRelatedToRCM" runat="server"/>
                            <asp:DropDownList ID="ddBillType" runat="server" CssClass="myInput">
                                <asp:ListItem Text="--Select--" Value=""> </asp:ListItem>
                                <asp:ListItem Text="Final Bill" Value="Final Bill"> </asp:ListItem>
                                <asp:ListItem Text="Measurement Sheet" Value="Measurement Sheet"> </asp:ListItem>
                                 <asp:ListItem Text="Others" Value="Others"> </asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                         <td class="myInput">Remarks</td>
                        <td><asp:TextBox ID="txtRemarks" runat="server" MaxLength="100" CssClass="myInput"></asp:TextBox> </td>
                    </tr>
                    <tr>
                         <td class="myInput">Browse File (Excel, Word or PDF files)</td>
                        <td> 

                        </td>
                    </tr>                   

                    <tr>                        
                        <td colspan="2" align="center">

                            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:FileUpload ID="fuAttachment" runat="server" />&nbsp;
                                 <asp:Button ID="btFileUpload" runat="server" Text="Upload" OnClick="btFileUpload_Click" Font-Bold="true" CssClass="myButton" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:PostBackTrigger ControlID="btFileUpload" />
                                </Triggers>
                            </asp:UpdatePanel>

                        </td>
                    </tr>
                     <tr>                        
                        <td colspan="2"><asp:Label ID="lblError" runat="server" Font-Bold="true" ForeColor="Red"/> </td>
                    </tr>
                </table>
            </td>
        </tr>

         <tr>
             <td colspan="2"><br /></td>
         </tr>

        <tr id="viewBills" runat="server" >
            <td colspan="2" align="center">
                <table width="50%">
                    <tr><td colspan="2" class="myGridHeader">Uploaded LSTK Bills</td></tr>
                    <tr>
                        <td colspan="2">
                            <asp:GridView ID="gvAttachments" runat="server" AutoGenerateColumns="false" CssClass="table table-striped table-bordered dataTable"
                                HeaderStyle-ForeColor="Black" DataKeyNames="ID" OnRowDataBound="gvAttachments_RowDataBound" OnRowDeleting="gvAttachments_RowDeleting">
                                <Columns>
                                    <asp:TemplateField HeaderText="S.No." ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="50">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1 %>
                                            <asp:HiddenField ID="hdID" runat="server" Value='<%# Bind("ID")%>' />
                                            <asp:HiddenField ID="hdAttachmentName" runat="server" Value='<%# Bind("FILE_NAME")%>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>                                  
                                     <asp:TemplateField HeaderText="Job No." ItemStyle-Wrap="true" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <%#Eval("JOB_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Tender No." ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("TENDER_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Part No." ItemStyle-Wrap="true" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <%#Eval("PART_NUMBER")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Contractor" ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("CONTRACTOR_NAME")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                      <asp:TemplateField HeaderText="Job Desc" ItemStyle-Wrap="true" >
                                        <ItemTemplate>
                                            <%#Eval("JOB_DESC")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>                                   

                                    <asp:TemplateField HeaderText="Bill Type" ItemStyle-Wrap="true" ItemStyle-Width="150">
                                        <ItemTemplate>
                                            <%#Eval("ATTACH_TYPE")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Upload Date" ItemStyle-Width="100">
                                        <ItemTemplate>
                                            <%#Eval("UPLOADED_ON")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                     <asp:TemplateField HeaderText="Uploaded By" >
                                        <ItemTemplate>
                                            <%#Eval("UPLOADED_BY")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remarks" ItemStyle-Wrap="true">
                                        <ItemTemplate>
                                            <%#Eval("REMARKS")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="" ItemStyle-Wrap="true" ItemStyle-Width="50">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="hlAttachment" runat="server" CssClass="btn btn-primary" Text="View"></asp:HyperLink>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ShowDeleteButton="true" ItemStyle-HorizontalAlign="Left" ControlStyle-CssClass="btn btn-danger" ItemStyle-Width="70" />
                                </Columns>
                                <EmptyDataTemplate>
                                    <b>Bills not found</b>
                                </EmptyDataTemplate>
                                <RowStyle Wrap="true" />
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
       
    </table>

</asp:Content>

