using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Dashboard
{
    //<summary>
    //      This class allows data to be filled out and written to a spreadsheet.
    //</summary>
    public partial class Admin : Page
    {

        //<summary>
        //      On page load, the drop down list gets populated with currently running script values.
        //</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PopulateDropDownList();
            }
        }

        //<summary>
        //      This method inserts script metadata into the database.
        //</summary>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            ddlActiveScripts.SelectedIndex = 0;
            txtBoxScriptName.ReadOnly = false;
            txtBoxScriptName.BackColor = System.Drawing.Color.White;

            if (txtBoxScriptName.Text.Length > 0)
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
                {
                    con.Open();
                    using (var insertLog = new SqlCommand("dbo.ScriptingDashboardInsertMetadata", con) { CommandType = CommandType.StoredProcedure })
                    {
                        //Add is DEPRECATED, but parameterized outputs do not show correct results with AddWithValue
                        insertLog.Parameters.Add("@ScriptID", SqlDbType.Int).Direction = ParameterDirection.Output;
                        insertLog.Parameters.AddWithValue("@ScriptName", this.txtBoxScriptName.Text);
                        insertLog.Parameters.AddWithValue("@Author", this.txtBoxAuthor.Text);
                        insertLog.Parameters.AddWithValue("@Ext", this.txtBoxExtension.Text);
                        insertLog.Parameters.AddWithValue("@Description", this.txtBoxDescription.Text);
                        insertLog.Parameters.AddWithValue("@LastUpdated", this.txtBoxLastUpdated.Text);
                        insertLog.Parameters.AddWithValue("@ReportingInterval", this.txtBoxReportingInterval.Text);
                        insertLog.Parameters.AddWithValue("@RollingWindowInterval", this.txtBoxRollingWindow.Text);
                        insertLog.Parameters.AddWithValue("@HumanSecsPerRecord", this.txtBoxHumanSPR.Text);

                        int insertExecution = insertLog.ExecuteNonQuery();

                        var scriptName = Convert.ToString(insertLog.Parameters["@ScriptName"].Value);
                        var scriptID = Convert.ToInt32(insertLog.Parameters["@ScriptID"].Value);
                        this.lblConfirmation.Text = "The script ID for " + scriptName + " is: " + scriptID;
                    }
                }

                this.EmptyTextBoxes();
                this.ReFormatTableLabels();
            }
            else
            {
                Response.Write("<script language='javascript'>alert('Enter metadata information to submit a script.');</script>");
            }
        }

        //TODO: Find a way to make fields required but not required when trying to 
        //populate text boxes with already entered data.
        protected void btnSelectScript_Click(object sender, EventArgs e)
        {
            this.lblConfirmation.Text = "";
            this.GetProductionScriptInfo();
        }

        //<summary>
        //      Removes all text from the text boxes
        //</summary>
        private void EmptyTextBoxes()
        {
            txtBoxScriptName.Text = String.Empty;
            txtBoxAuthor.Text = String.Empty;
            txtBoxExtension.Text = String.Empty;
            txtBoxDescription.Text = String.Empty;
            txtBoxLastUpdated.Text = String.Empty;
            txtBoxReportingInterval.Text = String.Empty;
            txtBoxRollingWindow.Text = String.Empty;
            txtBoxHumanSPR.Text = String.Empty;
        }

        //<summary>
        //      Rebolds the labels to make the page look the same
        //      after data is submitted and page is reloaded.
        //</summary>
        private void ReFormatTableLabels()
        {
            this.lblScriptName.Font.Bold = true;
            this.lblAuthor.Font.Bold = true;
            this.lblExtension.Font.Bold = true;
            this.lblDescription.Font.Bold = true;
            this.lblLastUpdated.Font.Bold = true;
            this.lblReportingInterval.Font.Bold = true;
            this.lblRollingWindow.Font.Bold = true;
            this.lblHumanSPR.Font.Bold = true;
        }

        //<summary>
        //      This method populates the drop down list with Script Names of scripts that have
        //      a status of either 1, 2, or 3. 
        //</summary> 
        //TODO: find a way to include the ScriptName along with Script ID (1, Test) in ddl
        private void PopulateDropDownList()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("dbo.ScriptingDashboardGetProductionScripts", con) { CommandType = CommandType.StoredProcedure })
                {
                    var adpt = new SqlDataAdapter { SelectCommand = cmd };
                    var dt = new DataTable();
                    adpt.Fill(dt);
                    ddlActiveScripts.DataSource = dt;
                    ddlActiveScripts.DataBind();
                    ddlActiveScripts.DataTextField = "ScriptName";
                    ddlActiveScripts.DataValueField = "ScriptName";
                    ddlActiveScripts.DataBind();
                }
            }
            ddlActiveScripts.Items.Insert(0, new ListItem("Select Script", "0"));
        }

        private void GetProductionScriptInfo()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var scriptInfo = new SqlCommand("dbo.ScriptingDashboardGetProductionScriptInfo", con) { CommandType = CommandType.StoredProcedure })
                {
                    scriptInfo.Parameters.AddWithValue("@SelectedScriptName", ddlActiveScripts.SelectedItem.Value);

                    var sqlReader = scriptInfo.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        txtBoxScriptName.Text = sqlReader["ScriptName"].ToString();
                        txtBoxScriptName.ReadOnly = true;
                        txtBoxScriptName.BackColor = System.Drawing.Color.LightGray;
                        txtBoxAuthor.Text = sqlReader["Author"].ToString();
                        txtBoxExtension.Text = sqlReader["Ext"].ToString();
                        txtBoxDescription.Text = sqlReader["Description"].ToString();
                        txtBoxLastUpdated.Text = sqlReader["LastUpdated"].ToString();
                        txtBoxReportingInterval.Text = sqlReader["ReportingInterval"].ToString();
                        txtBoxRollingWindow.Text = sqlReader["RollingWindowInterval"].ToString();
                        txtBoxHumanSPR.Text = sqlReader["HumanSecsPerRecord"].ToString();
                    }
                }
            }
        }
    }
}