using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Dashboard
{
    //<summary>
    //      This class allows the user to select a script from the dropdown list to see its current status.
    //      It displays a status table that shows what each ScriptStatusID number means. The user can manually 
    //      stop the script by pressing the "Stop Script" button.
    //</summary>
    public partial class RunningScripts : Page
    {
        //<summary>
        //      This method populates the drop down list and sets the 
        //      status table visibility to false on page load.
        //</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.GetActiveScripts();
            }
        }

        //Don't surround this with !IsPostBack, because the data has to be
        //refreshed, and it won't refreshed if inside a !IsPostBack
        protected void Timer_Tick(object sender, EventArgs e)
        {
            this.GetActiveScripts();
        }

        //<summary>
        //      This method adds coloration and a tooltip to each row that
        //      the mouse hovers over.
        //</summary>
        protected void gridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.cursor='pointer'; this.style.background='grey';";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.background='white';";
                e.Row.ToolTip = "Click to select row";
            }
        }

        //<summary>
        //      This method enables the Stop button when a row is clicked.
        //      It also replaces the status IDs with their respective images.
        //</summary>
        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = "this.style.background='#5F9AA8';";

                    if (e.Row.Cells[0] != null)
                    {
                        if (!String.IsNullOrEmpty(e.Row.Cells[0].Text))
                        {
                            string rowValue = e.Row.Cells[0].Text;
                            Image icon = new Image();

                            switch (rowValue)
                            {
                                case "1":
                                    icon.ImageUrl = "~/images/start.png";
                                    icon.ToolTip = "Script has started";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                                case "2":
                                    icon.ImageUrl = "~/images/gear.png";
                                    icon.ToolTip = "Script is now processing records";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                                case "3":
                                    icon.ImageUrl = "~/images/gear-struggling.png";
                                    icon.ToolTip = "Script is having trouble processing records";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                //can log to db or send out an email to dev
            }
        }

        //<summary>
        //      This method gets all active scripts.
        //</summary>
        private void GetActiveScripts()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var activeScriptsCmd = new SqlCommand("dbo.ScriptingDashboardGetScriptStatusInfo", con) { CommandType = CommandType.StoredProcedure })
                {
                    SqlDataReader reader;
                    reader = activeScriptsCmd.ExecuteReader();
                    this.gridView.DataSource = reader;
                    this.gridView.DataBind();
                }
            }
        }
    }
}