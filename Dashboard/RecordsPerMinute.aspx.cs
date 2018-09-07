using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Dashboard
{
    //<summary>
    //      This class calculates how many records are run per a specified time interval
    //      per a specified script and displays that data on a javascript real-time bar graph.
    //</summary>
    public partial class RecordsPerMinute : Page
    {
        #region Properties
        public int ReportingInterval { get; set; }
        public int ReportingIntervalMilliseconds { get; set; }
        public int RecordsProcessed { get { return this.RecordsProcessedDuringReportingInterval(); } set { RecordsProcessed = value; } }
        #endregion

        //<summary>
        //      This method populates the drop down list on page load.
        //</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PopulateDropDownList();
                if (this.ddlScriptsRunning.Items.Count > 1)
                {
                    this.btnMonitor.Enabled = true;
                    this.lblNoRunningScripts.Text = "";
                }
                else
                {
                    this.lblNoRunningScripts.Text = "No scripts currently running";
                    this.btnMonitor.Enabled = false;
                }
            }
        }

        //<summary>
        //      This method populates the bar chart with real time data of how many records have been processed
        //</summary>
        protected void btnMonitor_Click(object sender, EventArgs e)
        {
            this.GetRollingWindowInterval();
            this.GetReportingInterval();

            // Calls the JavaScript method to start the clock
            Page.ClientScript.RegisterStartupScript(this.GetType(), "clockFunction", "startTime();", true);
        }

        //<summary>
        //      This method populates the drop down list with currently running scripts.
        //</summary>
        private void PopulateDropDownList()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                var cmd = new SqlCommand("dbo.ScriptingDashboardGetActiveScriptNames", con) { CommandType = CommandType.StoredProcedure };
                var adpt = new SqlDataAdapter { SelectCommand = cmd };
                var dt = new DataTable();
                adpt.Fill(dt);
                ddlScriptsRunning.DataSource = dt;
                ddlScriptsRunning.DataBind();
                ddlScriptsRunning.DataTextField = "ScriptName";
                ddlScriptsRunning.DataValueField = "ScriptName";
                ddlScriptsRunning.DataBind();
            }

            ddlScriptsRunning.Items.Insert(0, new ListItem("Select Script", "0"));
        }


        //<summary>
        //      This method gets the rolling window interval which determines how long the bar graph will continue to update.
        //</summary
        private void GetRollingWindowInterval()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                var rollingWindowInterval = new SqlCommand("dbo.ScriptingDashboardGetRollingWindowInterval", con) { CommandType = CommandType.StoredProcedure };
                rollingWindowInterval.Parameters.Add("@SelectedScriptName", SqlDbType.VarChar, 50).Value = ddlScriptsRunning.SelectedItem.Value;

                var rollingWindowIntervalInt = Convert.ToInt32(rollingWindowInterval.ExecuteScalar());
                this.lblRollingWindowInterval.Text = "Rolling Window Interval: " + rollingWindowIntervalInt.ToString() + " hours";
            }
        }

        //<summary>
        //      This method gets the reporting interval which will tell the bar graph how frequently to update.
        //</summary>
        private void GetReportingInterval()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                var reportingInterval = new SqlCommand("dbo.ScriptingDashboardGetReportingInterval", con) { CommandType = CommandType.StoredProcedure };
                reportingInterval.Parameters.Add("@SelectedScriptName", SqlDbType.VarChar, 50).Value = ddlScriptsRunning.SelectedItem.Value;

                var reportingIntervalInt = Convert.ToInt32(reportingInterval.ExecuteScalar());
                this.lblreportingInterval.Text = "Reporting Interval: " + reportingIntervalInt.ToString() + " minutes";
                this.ReportingInterval = reportingIntervalInt;
                this.ReportingIntervalMilliseconds = reportingIntervalInt * 60 * 1000;

                // Call the JavaSCript method to draw the chart populated with database data
                Page.ClientScript.RegisterStartupScript(this.GetType(), "myChart", "drawSecondChart();", true);
            }
        }

        //<summary>
        //      This method calculates how many records have been processed in the reporting interval timespan, so that
        //      when the graph updates every x minutes, the number of records that have been processed during that time appears.
        //</summary>
        private int RecordsProcessedDuringReportingInterval()
        {
            var currentTime = DateTime.Now;
            var recordProcessingTime = currentTime.Add(new TimeSpan(0, -this.ReportingInterval, 0)); //hours, minutes, seconds

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                var scriptRecords = new SqlCommand("dbo.ScriptingDashboardGetScriptSecondsDuringInterval", con) { CommandType = CommandType.StoredProcedure };
                scriptRecords.Parameters.Add("@RecordProcessingTime", SqlDbType.SmallDateTime).Value = recordProcessingTime;
                scriptRecords.Parameters.Add("@CurrentTime", SqlDbType.SmallDateTime).Value = currentTime;

                var recordsProcessedDuringInterval = Convert.ToInt32(scriptRecords.ExecuteScalar());
                return recordsProcessedDuringInterval;
            }
        }
    }
}