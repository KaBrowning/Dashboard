<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RecordsPerMinute.aspx.cs" Inherits="Dashboard.RecordsPerMinute" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
      <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.bundle.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/Chart.js/2.7.2/Chart.min.js"></script>
    <style>
        .dropdownlist {
            margin: 2% auto 0.5%;
            font-size: larger;
            display: block;
        }
        .heading {
            padding: 30px;
            margin-left: 300px;
            margin-bottom: 30px;
            font-size: 21px;
            font-weight: 200;
            color: inherit;
            background-color: #eeeeee;
        }
        .sub-heading {
            color:black; 
            margin-left: 1.5%;
        }
        #clock {
            font-size: large;
        }
    </style>
    <script>
        function startTime() {
            var today = new Date();
            var h = today.getHours();
            var m = today.getMinutes();
            var s = today.getSeconds();
            m = checkTime(m);
            s = checkTime(s);
            document.getElementById('clock').innerHTML =
                h + ":" + m + ":" + s;
            var t = setTimeout(startTime, 500);
        }

        function checkTime(i) {
            if (i < 10) { i = "0" + i };  // add zero in front of numbers < 10
            return i;
        }
    </script>
      

    <div class="jumbotron">
        <h2>Records Processed Per Minute</h2>
    </div>

    <div class="sub-heading">
        <h3>Rolling Window Monitor</h3>
        <p>The rolling window below shows the number of records that are processed per reporting interval. This view can be used to catch any memory leak behavior
            or anything that may be impacting system latency.
        </p>

        <div class="dropdownlist">
            <asp:Label runat="server" ID="lblNoRunningScripts"></asp:Label><br />
            <asp:DropDownList ID="ddlScriptsRunning" runat="server" CssClass="ddl">
            </asp:DropDownList>       
            <asp:Button ID="btnMonitor" runat="server" Text="Monitor" CssClass="btn" OnClick="btnMonitor_Click"/>        
        </div>
        <asp:Label ID="lblRollingWindowInterval" runat="server"></asp:Label><br />
        <asp:Label ID="lblreportingInterval" runat="server"></asp:Label>
        <br /><br />
        <div id="clock"></div>
    </div>
    
    <br />

    <div style="width:95%; height:auto; margin: 0 auto;">
       <canvas id="myChart" height="600" width="2000" style="display:inline-block;"></canvas>
        <script>
            //Chart.js version 2.7
            var ctx = document.getElementById("myChart").getContext('2d');       
            var myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: [],
                    datasets: [{
                        label: '# of Records',
                        data: [], //y axis
                        //backgroundColor: [
                        //    'rgba(40, 119, 79, 0.2)',
                        //],
                        //borderColor: [
                        //    'rgba(40, 119, 79, 1)',
                        //],
                        borderWidth: 1
                    }]
                },
                options: {
                    scales: {
                        yAxes: [{
                            display: true,
                            position: 'right',
                            ticks: {
                                beginAtZero: true,
                                suggestedMax: 200,
                                stepSize: 10
                            }
                        }, {
                            display: true,
                            position: 'left',
                            ticks: {
                                beginAtZero: true,
                                suggestedMax: 200,
                                stepSize: 10
                                }
                            }],
                        xAxes: [{
                            scaleLabel: {
                                display: true,
                                labelString: 'Minutes'
                            }
                        }]
                    }
                }
            });


            function drawSecondChart() {
                var ctx = document.getElementById("myChart").getContext('2d');
                var myChart = new Chart(ctx, {
                    type: 'bar',
                    data: {
                        labels: [],
                        datasets: [{
                            label: '# of Records',
                            data: [], //y axis
                            //backgroundColor: [
                            //    'rgba(40, 119, 79, 0.2)',
                            //],
                            //borderColor: [
                            //    'rgba(40, 119, 79, 1)',
                            //],
                            borderWidth: 1
                        }]
                    },
                    options: {
                        scales: {
                            yAxes: [{
                                display: true,
                                position: 'right',
                                ticks: {
                                    beginAtZero: true,
                                    suggestedMax: 200,
                                    stepSize: 10
                                }
                            }, {
                                display: true,
                                position: 'left',
                                ticks: {
                                    beginAtZero: true,
                                    suggestedMax: 200,
                                    stepSize: 10
                                }
                                }],
                            xAxes: [{
                                scaleLabel: {
                                    display: true,
                                    labelString: 'Minutes'
                                }
                            }]
                        }
                    }
                });

                //setInterval is a Windows method used to set a time interval, which in 
                //this case calls the addData() method every x milliseconds
                setInterval(function () {
                    addData();
                }, <%=ReportingIntervalMilliseconds%>); //may need to add a few milliseconds here if working with long running scripts
            }

            //addData() controls what data is loaded into the bar graph. It creates a label with 
            //the specified timespans (reporting interval), then adds the rpm data to the graph
            var reportingInterval = <%=ReportingInterval%>;
            function addData() {
                var rpm = <%=RecordsProcessed%>
                myChart.data.datasets.borderWidth = 1;
                myChart.data.labels.push(reportingInterval);
                myChart.data.datasets[0].data.push(rpm);
                myChart.update();
                reportingInterval += <%=ReportingInterval%>; //this forces the label value to increment by the reporting interval, so they accurately represent the elapsed time
            }
        </script>
    </div>
</asp:Content>
