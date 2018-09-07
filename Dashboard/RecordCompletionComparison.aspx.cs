using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Dashboard
{
    //<summary>
    //      This class shows a comparison between the Human Seconds
    //      Per Record and the Script Seconds Per Record of a script
    //      selected by the user.
    //</summary>
    public partial class RecordCompletionComparison : Page
    {
        #region Properties
        //must be public so that they can be accessed by the aspx class
        public int HumanSecsPerRecord { get; set; }
        public int ScriptSecsPerRecord { get; set; }
        #endregion

        //<summary>
        //      On page load, the drop down list gets populated with currently running script values.
        //</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PopulateDropDownList();
                if (this.ddlScriptsRunning.Items.Count > 1)
                {
                    this.btnViewComparison.Enabled = true;
                    this.lblNoRunningScripts.Text = "";
                }
                else
                {
                    this.lblNoRunningScripts.Text = "No scripts currently running";
                    this.btnViewComparison.Enabled = false;
                }
            }
        }

        //<summary>
        //      This method shows a comparison between human seconds per record and script
        //      seconds per record for the script specified by user selection of the script
        //      ID from the drop down list.
        //</summary>
        protected void btnViewComparison_Click(object sender, EventArgs e)
        {
            this.GetHumanSecsPerRecord();
            this.GetScriptSecsPerRecord();
        }

        //<summary>
        //      This method populates the drop down list with Script Names of scripts that have
        //      a status of either 1, 2, or 3. 
        //</summary> 
        private void PopulateDropDownList()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("dbo.ScriptingDashboardGetActiveScriptNames", con) { CommandType = CommandType.StoredProcedure })
                {
                    var adpt = new SqlDataAdapter { SelectCommand = cmd };
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    this.ddlScriptsRunning.DataSource = dt;
                    this.ddlScriptsRunning.DataBind();
                    this.ddlScriptsRunning.DataTextField = "ScriptName";
                    this.ddlScriptsRunning.DataValueField = "ScriptName";
                    this.ddlScriptsRunning.DataBind();
                }
            }
            this.ddlScriptsRunning.Items.Insert(0, new ListItem("Select Script", "0"));
        }

        //<summary>
        //      This method gets the human seconds per record from the database
        //      for the specified script.
        //</summary>
        private void GetHumanSecsPerRecord()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var humanRecordsRPS = new SqlCommand("dbo.ScriptingDashboardGetHumanSecs", con) { CommandType = CommandType.StoredProcedure })
                {
                    humanRecordsRPS.Parameters.Add("@SelectedScriptName", SqlDbType.VarChar, 50).Value = ddlScriptsRunning.SelectedItem.Value;
                    var humanRecordsRPSInt = Convert.ToInt32(humanRecordsRPS.ExecuteScalar());
                    this.HumanSecsPerRecord = humanRecordsRPSInt;
                }
                // Call the JavaScript method to draw a chart populated with data from the database
                Page.ClientScript.RegisterStartupScript(this.GetType(), "myFunction", "google.charts.setOnLoadCallback(drawChartWithHumanSecsPerRecord);", true);
            }
        }

        //<summary>
        //      This method gets the script seconds per record from the database
        //      for the specified script.
        //</summary>
        //<exception cref="InvalidCastException">
        //      Thrown if there is a NULL database value for ScriptsSecsPerRecord, which
        //      would hapeen if the script has just started running or if it encounters a
        //      problem.
        //</exception>
        private void GetScriptSecsPerRecord()
        {
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
                {
                    con.Open();
                    var scriptRecordsRPS = new SqlCommand("dbo.ScriptingDashboardGetScriptSecs", con) { CommandType = CommandType.StoredProcedure };
                    scriptRecordsRPS.Parameters.Add("@SelectedScriptName", SqlDbType.VarChar, 50).Value = ddlScriptsRunning.SelectedItem.Value;
                    var scriptRecordsRPSInt = Convert.ToInt32(scriptRecordsRPS.ExecuteScalar());
                    this.ScriptSecsPerRecord = scriptRecordsRPSInt;

                    // Call the JavaScript method to draw a chart populated with data from the database
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "myFunction2", "google.charts.setOnLoadCallback(drawChartWithScriptSecsPerRecord);", true);
                }
            }
            catch (InvalidCastException)
            {
                this.ScriptSecsPerRecord = 0;
            }
        }
    }
}