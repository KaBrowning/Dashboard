using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Dashboard
{
    public partial class CancelRunningScript : System.Web.UI.Page
    {
        protected string scriptName;
        private string machineName;
        private int scriptID;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.scriptName = "";
            this.machineName = "";
            this.scriptID = 0;

            if (!IsPostBack)
            {
                this.PopulateListBox();
            }
        }

        private void PopulateListBox()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("dbo.ScriptingDashboardGetActiveScriptNames", con) { CommandType = CommandType.StoredProcedure })
                {
                    var adpt = new SqlDataAdapter { SelectCommand = cmd };
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    this.lstBoxRunningScripts.DataSource = dt;
                    this.lstBoxRunningScripts.DataBind();
                    this.lstBoxRunningScripts.DataTextField = "ScriptName";
                    this.lstBoxRunningScripts.DataValueField = "ScriptName";
                    this.lstBoxRunningScripts.DataBind();
                }
            }
        }

        private void GetScriptInfo()
        {
            this.scriptName = this.lstBoxRunningScripts.SelectedValue;

            this.GetScriptID();
            this.GetMachineName();
        }

        private void GetScriptID()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var scriptIdCmd = new SqlCommand("dbo.ScriptingDashboardGetScriptID", con) { CommandType = CommandType.StoredProcedure })
                {
                    scriptIdCmd.Parameters.AddWithValue("@SelectedScriptName", this.scriptName);
                    this.scriptID = Convert.ToInt32(scriptIdCmd.ExecuteScalar());
                }
            }
        }

        private void GetMachineName()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var machineNameCmd = new SqlCommand("dbo.ScriptingDashboardGetMachineName", con) { CommandType = CommandType.StoredProcedure })
                {
                    machineNameCmd.Parameters.Add("@ScriptID", SqlDbType.SmallInt).Value = this.scriptID;
                    this.machineName = Convert.ToString(machineNameCmd.ExecuteScalar());
                }
            }
        }

        protected void btnStopScript_Click(object sender, EventArgs e)
        {
            this.GetScriptInfo();
            //this.btnStopScript.OnClientClick = "return confirm('Are you sure you want to stop this script?');";
            //if(true)
            //{
            //   
            //  Page.ClientScript.RegisterStartupScript(this.GetType(), "alert", "showMessage();", true);
            //}

            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("dbo.ScriptingDashboardUpdateScriptStatus", con) { CommandType = CommandType.StoredProcedure })
                {
                    cmd.Parameters.Add("@ScriptStatusID", SqlDbType.TinyInt).Value = 4;
                    cmd.Parameters.Add("@ScriptID", SqlDbType.SmallInt).Value = this.scriptID;
                    cmd.Parameters.Add("@MachineName", SqlDbType.VarChar, 100).Value = this.machineName;
                    cmd.ExecuteNonQuery();
                }
            }

            this.PopulateListBox();
        }
    }
}