<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="HistoricalScripts.aspx.cs" Inherits="Dashboard.HistoricalScripts" %>
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
        .table-header {
            background-color:#117C9A; 
            font-weight:bold; 
            color:white;
        }
        table, tr, td {
            border: 1px solid black;
            font-size: 14pt;          
        }
        td {
            text-align:center;
            padding: 3px 24px;
        }
    </style>

    <div class="jumbotron">  
        <h2>Historical Scripts</h2>
    </div>
    <br /> 
    
    <!--Use for icons: https://www.iconfinder.com/ -->
    <div class="status_key">
        <table id="statuses" runat="server">
            <tr>
                <td class="table-header">Symbol</td>                
                <td><img src="images/stopped-unknown.png" width="35" height="30" alt="Script stopped unexpectedly"/></td>
                <td><img src="images/checkmark.png" width="30" height="30" alt="Script has successfully completed"/></td>
                <td><img src="images/stopped-timeout.png" width="35" height="30" alt="Script has timed out"/></td>
                <td><img src="images/no-records.png" width="30" height="30"  alt="Script has no records to process"/></td>
            </tr>
            <tr>
                <td class="table-header">Meaning</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script has stopped due to unknown reasons">Stopped - Unhandled</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script has successfully completed">Completed</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script has stopped because it has timed out">Stopped - Timeout</td>
                <td data-toggle="tooltip" data-placement="bottom" title="Script has no records to process">No Records</td>
            </tr>
        </table>
    </div>

    <div class="data-table">
        <asp:Timer ID="timer" runat="server" Interval="5000" OnTick="Timer_Tick"></asp:Timer>
        <asp:GridView ID="gridView" runat="server" BackColor="White" BorderColor="#999999" BorderStyle="None" BorderWidth="1px" CellPadding="10" GridLines="Vertical" OnRowCreated="gridView_RowCreated" OnRowDataBound="gridView_RowDataBound" PageSize="15">
            <HeaderStyle BackColor="#117C9A" Font-Bold="True" ForeColor="White"/> 
        </asp:GridView>
    </div>
</asp:Content>
