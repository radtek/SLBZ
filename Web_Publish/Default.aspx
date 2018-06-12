<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" MasterPageFile="~/MasterPage.master"
 Inherits="PublishProcess"
 %>

 <asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
<asp:ScriptManager ID="ScriptManager1" Runat="Server"></asp:ScriptManager>
 <asp:Timer ID="Timer1" Interval="600000" runat="server" 
        ontick="Timer1_Tick">
        </asp:Timer>
  
    <%          
        Println("最后一次刷新时间: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));        
    %>
      <br />

     <asp:CheckBox ID="CheckBoxGdh" runat="server" Text="显示稿袋号" />
    <asp:Button ID="ButtonSubmit" runat="server" 
        Text="查 询"   onclick="ButtonSubmit_Click" />

    <br />
    <asp:GridView ID="GridViewError" runat="server" AutoGenerateColumns="False" 
        BorderColor="Red" Caption="错误新息">
        <Columns>
            <asp:BoundField DataField="文件名" HeaderText="文件名" HtmlEncode="False">
            <ControlStyle BorderColor="Red" />
            <HeaderStyle BorderColor="Red" Font-Bold="True" ForeColor="Red" />
            <ItemStyle BorderColor="Red" />
            </asp:BoundField>
            <asp:BoundField DataField="错误" HeaderText="错误" HtmlEncode="False">
            <ControlStyle BorderColor="Red" />
            <HeaderStyle BorderColor="Red" Font-Bold="True" ForeColor="Red" />
            <ItemStyle BorderColor="Red" />
            </asp:BoundField>
        </Columns>
        <RowStyle BorderColor="Red" />
    </asp:GridView>
    <br />

    <asp:GridView ID="GridViewHistorical" runat="server" 
        AutoGenerateColumns="False" CaptionAlign="Left" Caption="2个小时以内的完成出版的记录" 
        >
        <Columns>
            <asp:BoundField DataField="文件名" HeaderText="文件名" HtmlEncode="False" />
            <asp:BoundField DataField="板材咬口" HeaderText="板材咬口" HtmlEncode="False" />
            <asp:BoundField DataField="颜色" HeaderText="颜色" HtmlEncode="False" />
            <asp:BoundField DataField="加网信息" HeaderText="加网信息" HtmlEncode="False" />
        </Columns>
    </asp:GridView>
    <br />
    <asp:GridView ID="GridViewDynamic" runat="server" AutoGenerateColumns="False" 
        CaptionAlign="Left" Caption="当前出版记录">
    <Columns>
    <asp:BoundField DataField="文件名" HeaderText="文件名" HtmlEncode="False" />
            <asp:BoundField DataField="板材咬口" HeaderText="板材咬口" HtmlEncode="False" />
            <asp:BoundField DataField="颜色" HeaderText="颜色" HtmlEncode="False" />
            <asp:BoundField DataField="加网信息" HeaderText="加网信息" HtmlEncode="False" />
    </Columns>
    </asp:GridView>
       
    <br />
    <asp:Label ID="LableError" runat="server" BorderColor="Red" ForeColor="Red" 
        Text="***有错误，请回页首查看" Visible="False"></asp:Label>
    <br />
       
    </asp:Content>






   

