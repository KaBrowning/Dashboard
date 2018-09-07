<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RecordCompletionComparison.aspx.cs" Inherits="Dashboard.RecordCompletionComparison" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <meta http-equiv="content-language" content="IE-Edge"/>
    <script type="text/javascript" src="https://www.gstatic.com/charts/loader.js"></script>
    <script type="text/javascript">
        google.charts.load('current', { 'packages': ['gauge'] });
        google.charts.setOnLoadCallback(drawChartWithZeroValue1); //have to have 2 different methods in order to show 2 charts
        google.charts.setOnLoadCallback(drawChartWithZeroValue2);

        function drawChartWithZeroValue1() {
            var data = google.visualization.arrayToDataTable([
                ['Label', 'Value'],
                ['Seconds', 0],
            ]);

            var options = {
                width: 400, height: 300,
                redFrom: 60, redTo: 100,
                yellowFrom: 30, yellowTo: 60,
                greenFrom: 0, greenTo: 30,
                minorTicks: 5
            };

            var chart = new google.visualization.Gauge(document.getElementById('chart_div'));

            chart.draw(data, options);       
        }

        function drawChartWithZeroValue2() {
            var data = google.visualization.arrayToDataTable([
                ['Label', 'Value'],
                ['Seconds', 0],
            ]);

            var options = {
                width: 400, height: 300,
                redFrom: 60, redTo: 100,
                yellowFrom: 30, yellowTo: 60,
                greenFrom: 0, greenTo: 30,
                minorTicks: 5
            };

            var chart = new google.visualization.Gauge(document.getElementById('chart_div2'));

            chart.draw(data, options);
        }
               
        function drawChartWithHumanSecsPerRecord() {
            var data = google.visualization.arrayToDataTable([
                ['Label', 'Value'],
                ['Seconds', <%=HumanSecsPerRecord%>],
            ]);

            var options = {
                width: 400, height: 300,
                redFrom: 30, redTo: 100,
                yellowFrom: 10, yellowTo: 30,
                greenFrom: 0, greenTo: 10,
                minorTicks: 5
            };

            var chart = new google.visualization.Gauge(document.getElementById('chart_div'));

            chart.draw(data, options);
        }

        function drawChartWithScriptSecsPerRecord() {
            var data = google.visualization.arrayToDataTable([
                ['Label', 'Value'],
                ['Seconds', <%=ScriptSecsPerRecord%>],
            ]);

            var options = {
                width: 400, height: 300,
                redFrom: 30, redTo: 100,
                yellowFrom: 10, yellowTo: 30,
                greenFrom: 0, greenTo: 10,
                minorTicks: 5
            };

            var chart = new google.visualization.Gauge(document.getElementById('chart_div2'));

            chart.draw(data, options);
        }
    </script>

    <style>
        .dropdownlist {
            margin: 2% auto;
            margin-bottom:2%;
            font-size:larger;
            display: block;
        }
        .heading {
            padding:30px;
            margin-left:300px;
            margin-bottom:30px;
            font-size:21px;
            font-weight:200;
            line-height:2.1428571435;
            color:inherit;
            background-color:#eeeeee;
        }
        .sub-heading {
            display:inline-block; 
            margin-left: 1.5%;
        }
        .speedometer {
            margin-left:350px;
            margin-top:5%;
        }

    </style>
        
    <div class="jumbotron">
        <h2>Record Completion Comparison</h2>
    </div>

    <div class="sub-heading"> 
        <h3>Seconds Per Record</h3>
        <p>Even though scripts follow the same process as a person to process records, the processing time is typically faster. 
            The green sections represent the standard time established for each.
        </p>        

        <div class="dropdownlist">
            <asp:Label runat="server" ID="lblNoRunningScripts"></asp:Label><br />
            <asp:DropDownList ID="ddlScriptsRunning" runat="server" CssClass="ddl">
            </asp:DropDownList>       
            <asp:Button ID="btnViewComparison" runat="server" Text="View Comparison" CssClass="btn" OnClick="btnViewComparison_Click"/>        
        </div>    

        <div style="float:left; margin-left:15%; text-align: center;">
            <h3>Human Seconds per Record</h3>
            <div id="chart_div" style="width: 400px; height: 300px;"></div>
        </div>

        <div style="float:right; text-align: center;">
            <h3>Script Seconds per Record</h3>
            <div id="chart_div2" style="width: 400px; height: 300px;"></div>
        </div>
    </div>
</asp:Content>
