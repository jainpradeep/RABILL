<%@ Page Title="" Language="C#" MasterPageFile="~/RA_MenuPage.master" AutoEventWireup="true"
    CodeFile="RA_MOP_Update.aspx.cs" Inherits="RA_MOP_Update" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

 <table width="100%" class="myTable">
    
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    Add/Update MOP Fields
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

            <tr id="trBillNu" runat="server" visible="true" >
                <td class="myLabel">
                    Select Bill Number
                </td>
                <td>
                    <asp:DropDownList ID="ddBillNumber" runat="server" CssClass="myInput" OnSelectedIndexChanged="ddBillNumber_SelectedIndexChanged"
                        AutoPostBack="true">
                    </asp:DropDownList>
                </td>
            </tr>

            <tr>
            <td colspan="2">&nbsp;</td>
            </tr>
</table>
        <asp:Panel ID="pnlJobMOP" runat="server" Visible="false">
        <table width="100%" class="myTable">
         <tr>
                <td colspan="2" align="center" class="myGridHeader">
                    MOP Fields
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:GridView ID="gvJobMop" runat="server" Font-Names="Arial" Font-Size="8pt"
                        Width="70%" AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None" OnRowDataBound="gvJobMop_RowDataBound" AllowSorting="true">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex+1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Heading" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate >
                                    <asp:HiddenField ID="hdJobID" runat="server" Value='<%#Eval("ID") %>' />
                                    <asp:HiddenField ID="hdJobOrder" runat="server" Value='<%#Eval("HEADING_ORDER") %>' />
                                    <asp:HiddenField ID="hdJobNumber" runat="server" Value='<%#Eval("JOB_NO") %>' />
                                    <asp:HiddenField ID="hdTenderNumber" runat="server" Value='<%#Eval("TENDER_NO") %>' />                                    
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Eval("HEADING_DESC") %>' Font-Bold="true" Width="300"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <HeaderStyle CssClass="myGridHeader"></HeaderStyle>
                                <ItemTemplate>
                                    <div style="width: auto;">
                                        <asp:GridView ID="gvJobMopSubHeadings" runat="server" Font-Size="10pt" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sub Headings">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdJobSubHeadingID" runat="server" Value='<%#Eval("ID") %>' />
                                                        <asp:Label ID="lblJobSubHeading" runat="server" Text='<%#Eval("SUB_HEADING_DESC") %>' Width="300"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#000066" BackColor="AliceBlue"
                                                Font-Size="9pt" />
                                                 <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" CssClass="myGridHeader" />
                                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                                        </asp:GridView>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No MOP </b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
                </tr>
                <tr id="trJobMOP" runat="server" visible="false">
                <td colspan="2" align="center">    <br />            
                <asp:Button ID="btnAddNewFields" runat="server" Text="Add more MOP fields" Font-Bold=true OnClick="btnAddNewFields_Click"/>
                </td>
                
                </tr>
            </table>
     </asp:Panel>

        <asp:Panel ID="pnlMasterMop" runat="server" Visible=false>
        <table width="100%" class="myTable">
            <tr>
            <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                   Master fields for MOP
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:GridView ID="gvMopHeadings" runat="server" Font-Names="Arial" Font-Size="8pt"
                        Width="99%" AutoGenerateColumns="False" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None" OnRowDataBound="gvMopHeadings_RowDataBound" AllowSorting="true">
                        <Columns>
                            <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate>
                                    <%#Container.DataItemIndex+1 %>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Heading" ItemStyle-VerticalAlign="Top">
                                <ItemTemplate >
                                    <asp:HiddenField ID="hdID" runat="server" Value='<%#Eval("ID") %>' />
                                    <asp:HiddenField ID="hdOrder" runat="server" Value='<%#Eval("HEADING_ORDER") %>' />
                                    <asp:Label ID="lblHeading" runat="server" Text='<%#Eval("HEADING_DESC") %>' Font-Bold="true" Width="300"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="">
                                <HeaderStyle CssClass="myGridHeader"></HeaderStyle>
                                <ItemTemplate>
                                    <div style="width: auto;">
                                        <asp:GridView ID="gvMopSubHeadings" runat="server" Font-Size="10pt" AutoGenerateColumns="false">
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No " ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Sub Headings">
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdSubHeadingID" runat="server" Value='<%#Eval("ID") %>' />
                                                        <asp:Label ID="lblSubHeading" runat="server" Text='<%#Eval("SUB_HEADING_DESC") %>' Width="300"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <RowStyle HorizontalAlign="Center" VerticalAlign="Middle" ForeColor="#000066" BackColor="AliceBlue"
                                                Font-Size="9pt" />
                                                 <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                                                    VerticalAlign="Middle" CssClass="myGridHeader" />
                                            <SelectedRowStyle BackColor="#669999" Font-Bold="True" ForeColor="White" Font-Size="8pt" />
                                        </asp:GridView>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No MOP </b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
                </tr>
                <tr id="trPopulateMop" runat="server" visible="false">
                <td colspan="2" align="center">
                <asp:Label ID="lblMopError" runat="server" Font-Bold=true></asp:Label>
                <asp:Button ID="btnPopulateMopData" runat="server" Text="Populate MOP fields" Font-Bold=true OnClick="btnPopulateMopData_Click"/>
                </td>
                
                </tr>
               
        </table>
    </asp:Panel>


    <asp:Panel ID="pnlAddExtraMOPFields" runat="server" Visible=false>
        <table width="100%" class="myTable">
            <tr>
            <td colspan="2">&nbsp;</td>
            </tr>
            <tr>
                <td colspan="2" align="center" class="myGridHeader">
                   Add new fields for MOP
                </td>
            </tr>

            <tr>
                <td  align="center" class="myLabel">
                   Select Heading
                </td>
                <td>
                <asp:DropDownList ID="ddHeadings" class="myInput" runat="server" OnSelectedIndexChanged="ddHeadings_SelectedIndexChanged"
                        AutoPostBack="true"></asp:DropDownList>
                </td>
            </tr>
            

            <tr>
                <td  align="center" class="myLabel">
                   Enter New Sub Heading Title
                </td>
                <td>
                <asp:TextBox ID="txtSubHeading" runat="server" CssClass="myInput" MaxLength="100" Width="300"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td colspan="2"  align="center" ><br />
                   <asp:Button ID="btnSaveField" Text="Save New Field" runat=server Font-Bold="true" OnClick="btnSaveField_Click" />

                   <asp:Button ID="btnCheckMOP" Text="Check MOP" runat=server Font-Bold="true" OnClick="btnCheckMOP_Click"  />

                </td>
            </tr>

            <tr>
                <td colspan="2" align="center">
                    <asp:GridView ID="gvSubHeading" runat="server" Font-Names="Arial" Font-Size="8pt"
                        Width="99%" AutoGenerateColumns="true" BackColor="White" BorderWidth="1px" BorderColor="#CCCCCC"
                        BorderStyle="None"  AllowSorting="true">
                        
                        <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" HorizontalAlign="Center" />
                        <EmptyDataTemplate>
                            <b>No data found </b>
                        </EmptyDataTemplate>
                        <HeaderStyle BorderWidth="1px" Font-Bold="True" ForeColor="White" HorizontalAlign="Center"
                            VerticalAlign="Middle" CssClass="myGridHeader" />
                        <RowStyle HorizontalAlign="Left" VerticalAlign="Middle" Height="20px" ForeColor="#000066"
                            Font-Size="9pt" />
                    </asp:GridView>
                </td>
                </tr>
               
               
        </table>
    </asp:Panel>



</asp:Content>
