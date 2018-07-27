
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GaoDaiHao.aspx.cs" MasterPageFile="~/MasterPage.master"
    Inherits="GaoDaiHao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <title>稿袋号</title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div>
        <asp:TextBox ID="TextBoxSearch" runat="server" Width="199px"></asp:TextBox>
        <asp:Button ID="ButtonSearch" runat="server" onclick="ButtonSearch_Click" 
            Text="搜索" />
        <br />
        <br />
        <asp:GridView ID="DgvGdh" runat="server">
        </asp:GridView>
    </div>
</asp:Content>
