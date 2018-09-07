<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CancelRunningScript.aspx.cs" Inherits="Dashboard.CancelRunningScript" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
        <style>
        .content {
            margin-left: 1.5%;
        }
    </style>
  
    <div class="jumbotron">  
        <h2>Cancel Running Scripts</h2>
    </div>
    <br />

    <div class="content">
        <h4>All currently running scripts</h4>
        <asp:ListBox ID="lstBoxRunningScripts" runat="server" Width="300px"></asp:ListBox>
        <br /><br />
        <asp:Button runat="server" Text="Stop Script" CssClass="btn" OnClick="btnStopScript_Click"/>

<%--        <div class="alert alert-success alert-dismissible fade show" id="scriptStoppedAlert" role="alert">
            <strong>successfully stopped.</strong>
            <button type="button" class="close" data-dismiss="alert" aria-label="Close">
                <span aria-hidden="true">&times;</span> 
            </button>
        </div>--%>

    </div>
</asp:Content>
