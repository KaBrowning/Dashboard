<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="Dashboard.Dashboard" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        h3 {
            text-align:center;
        }
        img {
            display:block;
            margin-top:3%;
            margin-left:auto;
            margin-right:auto;
            width:290px;
            height:287px;
        }
        .image_text {
            position: absolute;
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
            font-size: 22pt;
        }
        .small_label {
            font-size: 50%;
        }
    </style> 
    <div class="jumbotron">  
        <h2>Dashboard Summary</h2>
    </div>
        <div class="row">
            <div class="col" style="text-align: center;"><h3>Daily FTE Equivalent</h3></div>
            <div class="col" style="text-align: center;"><h3>Monthly FTE Equivalent</h3></div>
            <div class="col" style="text-align: center;"><h3>Records Per Hour</h3></div>
         </div>
         <div class="row">
            <div class="col" style="text-align: center;">The full-time employee equivalent for scripts run today.</div>
            <div class="col" style="text-align: center;">The full-time employee equivalent for scripts run since the 1st of the month.</div>
            <div class="col" style="text-align: center;">Total records processed in the past 60 minutes.</div>
        </div>
        <div class="row">
            <div class="col" style="text-align: center;">
                <img src="images/bluecircle3.png" alt="blue circle">
                <div class="image_text">
                    <asp:Label ID="lblDailyFTE" runat="server" />
                </div>
            </div>
            <div class="col" style="text-align: center;">
                <img src="images/bluecircle4.png" alt="blue circle">
                <div class="image_text">                
                    <asp:Label ID="lblMonthlyFTE" runat="server" /><br />
                    <div class="small_label">
                        <asp:Label ID="lblWeekdaysElapsed" runat="server" />
                    </div>
                </div>
            </div>
            <div class="col" style="text-align: center;">
                <img src="images/bluecircle5.png" alt="blue circle">
                <div class="image_text">
                    <asp:Label ID="lblRecordsPerMinute" runat="server" />
                </div>
            </div>
        </div>
        <asp:Timer ID="timer" runat="server" Interval="5000" OnTick="Timer_Tick"></asp:Timer>
</asp:Content>
