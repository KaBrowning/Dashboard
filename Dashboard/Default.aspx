<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Dashboard._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

       <div class="jumbotron">
        <h1>Scripting Dashboard</h1><br />
        <h2>Welcome!</h2>
    </div>

    <div class="row">
        <div class="col-md-4" style="margin-left: 5%;">
            <h2>Admin</h2>
            Script administrators have their own view that allows them to enter script metadata information into the database. 
            <br /><br />
            <a class="btn btn-default" href="Admin.aspx">View Admin Page &raquo;</a>
        </div>
        <div class="col-md-4">
            <h2>Dashboard</h2>
            The dashboard view allows you to see summary information about scripts, including a rolling window that can help pinpoint memory leaks.
            <br /><br />
            <a class="btn btn-default" href="Dashboard.aspx">View Dashboard &raquo;</a>
        </div>
    </div>

</asp:Content>
