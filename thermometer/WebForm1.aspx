<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="thermometer.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
    <meta http-equiv='cache-control' content='no-cache' />
    <meta http-equiv='expires' content='0' />
    <meta http-equiv='pragma' content='no-cache' />
    <meta charset="utf-8" />

    <title>Thermometer</title>
    <script src="js/jquery/jquery-1.11.1.js"></script>

    <script type="text/javascript" src="https://www.google.com/jsapi"></script>
    <script src="js/googleVisualization/googleLoad.js"></script>       

    <script src="js/thermometer/thermometer.js"></script>
    <link href="css/StyleSheet1.css" rel="stylesheet" />

    <script type="text/javascript">
    function deleteConfirm(pubid) {
        var result = confirm('Do you want to delete ' + pubid + ' ?');
        if (result) {
            return true;
        }
        else {
            return false;
        }
    }
    function giveNotification(threshold) {
        var result = confirm('Threshold level ' + threshold + ' has been reached!');
        if (result) {
            return true;
        }
        else {
            return false;
        }
    }
    </script>

</head>
<body>
  <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" />
      <div>
      <asp:Timer ID="Timer1" OnTick="Timer1_Tick" runat="server" Interval="8500"></asp:Timer><%--five minute timer to refresh thermostate value--%>
    </div>   
    <asp:UpdatePanel ID="UpdatePanel1" UpdateMode="Conditional" runat="server" ChildrenAsTriggers="true">
      <Triggers>
        <asp:AsyncPostBackTrigger ControlID="Timer1" EventName="Tick" />
        <asp:AsyncPostBackTrigger ControlID="Button1" EventName="Click"/>
        <asp:AsyncPostBackTrigger ControlID="Button2" EventName="Click" /> 
      </Triggers>
      <ContentTemplate>        
        <div style="border:solid;border-width:0px;border-color:black; height:400px;width:1240px;padding:5px;">   
          <div id="chart_div" style="border:solid;border-width:0px;border-color:black; width: 400px; height:400px; float:left;margin-right:5px"></div>               
          <div style="border:solid;border-width:0px;border-color:blue; width: 800px; height: 500px; float:left;">
            <table id="tblTemperature" runat="server" border="0" cellpadding="0" cellspacing="0">
              <tr>
                <th>Vancouver Temperature Information</th>
              </tr>          
              <tr>
                <td>Temperature: <asp:Label ID="lblTemp" runat="server" /></td>
              </tr>
              <tr>
                <td><asp:Button runat="server" ID="Button1" Text="°C" OnClick="Button1_Click" CommandArgument="updateCelcius" style="background-color:#73d5ef;" /></td>
              </tr>
              <tr>
                <td><asp:Button runat="server" ID="Button2" Text="°F" OnClick="Button2_Click" CommandArgument="updateFah" style="background-color:#ccc;" /></td>
              </tr>           
              <tr>
                <td>Enter Threshold Levels</td>
              </tr>
              <tr>
                <td>
                  <div>  
                    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" ShowFooter="True" DataKeyNames="id"   

                        onrowcancelingedit="GridView1_RowCancelingEdit"   
                        onrowdeleting="GridView1_RowDeleting" onrowediting="GridView1_RowEditing"   
                        onrowupdating="GridView1_RowUpdating" BackColor="White"  
                        onrowcommand="GridView1_RowCommand"
                        OnRowDataBound="GridView1_RowDataBound"

                        BorderColor="White" BorderStyle="Ridge" BorderWidth="2px" CellPadding="3"   
                        CellSpacing="1" GridLines="None">  
                        <Columns>
                        <asp:TemplateField HeaderText="id">
                            <ItemTemplate>
                                <asp:Label ID="txt_id" runat="server" Text='<%#Eval("id") %>'/>
                            </ItemTemplate>                           
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Threshold °C">  
                            <ItemTemplate>  
                                <asp:Label ID="Label2" runat="server" Text='<%# Bind("threshold_C") %>'></asp:Label>  
                            </ItemTemplate>
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("threshold_C") %>'  Enabled="true"></asp:TextBox>  
                            </EditItemTemplate>                              
                            <FooterTemplate>
                                <asp:TextBox ID="i_C" width="40px" runat="server" Enabled="True"/>
                                <%--<asp:RequiredFieldValidator ID="v_C" runat="server" ControlToValidate="i_C" Text="?" ValidationGroup="validaiton"/>--%>
                            </FooterTemplate>  
                        </asp:TemplateField>
                              
                        <asp:TemplateField HeaderText="Threshold °F">  
                            <ItemTemplate>  
                                <asp:Label ID="Label3" runat="server" Text='<%# Bind("threshold_F") %>'></asp:Label>  
                            </ItemTemplate>
                            <EditItemTemplate>  
                                <asp:TextBox ID="TextBox3" runat="server" Text='<%# Bind("threshold_F") %>'  Enabled="false"></asp:TextBox>  
                            </EditItemTemplate>  
                            <FooterTemplate>
                                <asp:TextBox ID="i_F" width="40px" runat="server" Enabled="false"/>
                                <%--<asp:RequiredFieldValidator ID="v_F" runat="server" ControlToValidate="i_F" Text="?" ValidationGroup="validaiton"/>--%>
                            </FooterTemplate>  
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Threshold Direction">  
                            <ItemTemplate>  
                                <asp:Label ID="Label4" runat="server" Text='<%# Bind("threshold_D") %>'></asp:Label>  
                            </ItemTemplate>  
                            <EditItemTemplate>  
                                <asp:DropDownList ID="DropDownList2" runat="server" SelectedValue='<%# Bind("threshold_D") %>'>  
                                    <asp:ListItem>UP/DOWN</asp:ListItem>  
                                    <asp:ListItem>UP</asp:ListItem>  
                                    <asp:ListItem>DOWN</asp:ListItem>  
                                </asp:DropDownList>  
                            </EditItemTemplate>  
                            <FooterTemplate>
                                <asp:DropDownList ID="i_D" runat="server">  
                                    <asp:ListItem>UP/DOWN</asp:ListItem>  
                                    <asp:ListItem>UP</asp:ListItem>  
                                    <asp:ListItem>DOWN</asp:ListItem>  
                                </asp:DropDownList>  
                            </FooterTemplate>  
                        </asp:TemplateField>
                             
                        <asp:TemplateField HeaderText="Threshold Notification Level">  
                            <ItemTemplate>  
                                <asp:Label ID="Label5" runat="server" Text='<%# Bind("threshold_NL") %>'></asp:Label>  
                            </ItemTemplate>
                            <EditItemTemplate>  
                                <asp:DropDownList ID="DropDownList3" runat="server" SelectedValue='<%# Bind("threshold_NL") %>'>  
                                    <asp:ListItem>ALL NOTIFICATIONS</asp:ListItem>  
                                    <asp:ListItem>SINGLE</asp:ListItem>  
                                </asp:DropDownList>  
                            </EditItemTemplate>                              
                            <FooterTemplate>
                                <asp:DropDownList ID="i_NL" runat="server">  
                                    <asp:ListItem>ALL NOTIFICATIONS</asp:ListItem>  
                                    <asp:ListItem>SINGLE</asp:ListItem>    
                                </asp:DropDownList>  
                            </FooterTemplate>    
                        </asp:TemplateField>
                             
                        <asp:TemplateField HeaderText="Threshold Alarm">  
                            <ItemTemplate>  
                                <asp:Label ID="Label6" runat="server" Text='<%# Bind("threshold_A") %>'></asp:Label>  
                            </ItemTemplate>  
                            <EditItemTemplate>  
                                <asp:DropDownList ID="DropDownList4" runat="server" SelectedValue='<%# Bind("threshold_A") %>'>  
                                    <asp:ListItem>ACTIVE</asp:ListItem>  
                                    <asp:ListItem>SOUNDED</asp:ListItem>  
                                </asp:DropDownList>  
                            </EditItemTemplate>  
                            <FooterTemplate>
                                <asp:DropDownList ID="i_A" runat="server">  
                                    <asp:ListItem>ACTIVE</asp:ListItem>  
                                    <asp:ListItem>SOUNDED</asp:ListItem>  
                                </asp:DropDownList>  
                            </FooterTemplate>    
                        </asp:TemplateField>

                        <asp:TemplateField>
                            <EditItemTemplate>
                                <asp:Button ID="ButtonUpdate" runat="server" CommandName="Update"  Text="Update"  />
                                <asp:Button ID="ButtonCancel" runat="server" CommandName="Cancel"  Text="Cancel" />
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Button ID="ButtonEdit" runat="server" CommandName="Edit"  Text="Edit"  />
                                <asp:Button ID="ButtonDelete" runat="server" CommandName="Delete"  Text="Delete"  />
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Button ID="ButtonAdd" runat="server" CommandName="AddNew"  Text="Add New Row" ValidationGroup="validaiton" />
                            </FooterTemplate>
                        </asp:TemplateField> 

                    </Columns>  
                    <FooterStyle BackColor="#C6C3C6" ForeColor="Black" />  
                    <HeaderStyle BackColor="#4A3C8C" Font-Bold="True" ForeColor="#E7E7FF" />  
                    <PagerStyle BackColor="#C6C3C6" ForeColor="Black" HorizontalAlign="Right" />  
                    <RowStyle BackColor="#DEDFDE" ForeColor="Black" />  
                    <SelectedRowStyle BackColor="#9471DE" Font-Bold="True" ForeColor="White" />  
                    <SortedAscendingCellStyle BackColor="#F1F1F1" />  
                    <SortedAscendingHeaderStyle BackColor="#594B9C" />  
                    <SortedDescendingCellStyle BackColor="#CAC9C9" />  
                    <SortedDescendingHeaderStyle BackColor="#33276A" />  
                  </asp:GridView>        
                </div>
                <div >
                  <br />&nbsp;&nbsp;&nbsp;&nbsp;
                  <asp:Label ID="lblmsg" runat="server"></asp:Label>
                </div>
              </td>
            </tr>
          </table>
        </div>
        <asp:HiddenField ID="hfTemp" runat="server" />
        <asp:HiddenField ID="hfDegrees" runat="server" />

<%--    <asp:HiddenField ID="testTemperature" runat="server" />
        <asp:Button ID="test1" runat="server" Text="test" OnClick="test1_Click" />--%>

      </ContentTemplate>
    </asp:UpdatePanel>
  </form>
  <!-- Google Viz Kit Activation -->
  <script type="text/javascript">
    google.load('visualization', '1', { packages: ['gauge'] });
  </script>
</body>
</html>
