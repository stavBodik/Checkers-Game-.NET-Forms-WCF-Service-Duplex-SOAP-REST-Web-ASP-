<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="WebApplication.WebSite.About" %>

<asp:Content ContentPlaceHolderID="title" runat="server">
       <title>About</title>
</asp:Content>


<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
        
    <div class="row">
        <h2>About</h2>
        <hr />
        Student Name : Stanislav Bodik
        <br>
        ID : 306667478
    </div>



</asp:Content>