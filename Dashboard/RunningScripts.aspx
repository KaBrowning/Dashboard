<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RunningScripts.aspx.cs" Inherits="Dashboard.RunningScripts" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
     <style>
        .data-table {
            margin-left: 1.5%;
            font-size:larger; 
            width: auto;
            display: inline;
            float: left;
        }
        .status_key {          
            margin-left: 1.5%;
            margin-bottom: 2%;
        }
        .stop-btn {
            margin-left: 1.5%;
        }
        .table-header {
            background-color:#117C9A; 
            font-weight:bold; 
            color:white;
        }
        table, tr, td {
            border:1px solid black;
            font-size:14pt;          
        }
        td {
            text-align:center;
            padding: 3px 15px;
        }
    </style>

    <div class="jumbotron">  
        <h2>Currently Running Scripts</h2>
    </div>
    <br /> 
    
    <!--Use for icons: https://www.iconfinder.com/ -->
    <div class="status_key">
        <table id="statuses" runat="server">
            <tr>
                <td class="table-header">Symbol</td>                
                <td><img src="images/start.png" width="30" height="30" alt="Script started"/></td>
                <td><img src="images/gear.png" width="30" height="30" alt="Script in progress"/></td>
                <td><img src="images/gear-struggling.png" width="30" height="30" alt="Script struggling to run"/></td>
            </tr>
            <tr>
                <td class="table-header">Meaning</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script has started">Started</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script is now processing records">Running</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script is having trouble processing records">Struggling</td>
            </tr>
        </table>
    </div>

    <div class="data-table">
        <asp:Timer ID="timer" runat="server" Interval="5000" OnTick="Timer_Tick"></asp:Timer>
        <asp:GridView ID="gridView" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="4" GridLines="Vertical" OnRowCreated="gridView_RowCreated" OnRowDataBound="gridView_RowDataBound">
            <HeaderStyle BackColor="#117C9A" Font-Bold="True" ForeColor="White"/> 
        </asp:GridView>
    </div>
</asp:Content>
