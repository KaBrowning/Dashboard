using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace Dashboard
{
    //<summary>
    //      This class fills a grid view with historical script data,
    //      updates that data on each tick, and replaces the status value
    //      retrieved from the stored procedure with a corresponding image.
    //      Last Updated: July 10, 2018 by Kathryn Browning
    //</summary>
    public partial class HistoricalScripts : Page
    {
        private DataSet ds;
        private SqlDataAdapter adpt;

        //<summary>
        //      This method populates the drop down list and sets the 
        //      status table visibility to false on page load.
        //</summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.GetHistoricalScripts();
            }
        }

        //<summary>
        //      On each tick (5 seconds), the grid view is filled with
        //      refreshed history data.
        //</summary>
        protected void Timer_Tick(object sender, EventArgs e)
        {
            this.GetHistoricalScripts();
        }

        //<summary>
        //      This method adds coloration and a tooltip to each row that
        //      the mouse hovers over, and it adds the header tooltips
        //      on row creation.
        //</summary>
        protected void gridView_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes["onmouseover"] = "this.style.background='grey';";
                e.Row.Attributes["onmouseout"] = "this.style.textDecoration='none';this.style.background='white';";

                this.BuildHeaderTooltips();
            }
        }

        private void BuildHeaderTooltips()
        {
            this.gridView.HeaderRow.Cells[0].ToolTip = "The status symbol";
            this.gridView.HeaderRow.Cells[1].ToolTip = "The name of the script";
            this.gridView.HeaderRow.Cells[2].ToolTip = "The computer name that ran the script";
            this.gridView.HeaderRow.Cells[3].ToolTip = "The time the script started";
            this.gridView.HeaderRow.Cells[4].ToolTip = "The time the script stopped";
            this.gridView.HeaderRow.Cells[5].ToolTip = "The number of seconds it took to process each record";
            this.gridView.HeaderRow.Cells[6].ToolTip = "The total run time of the script";
            this.gridView.HeaderRow.Cells[7].ToolTip = "The total number of records the script processed";
        }

        //<summary>
        //      This method creates descriptive tooltips for each script and 
        //      replaces Script Status with an image.
        //</summary>
        protected void gridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            try
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    //hide the Description column and turn it into a tooltip for each script
                    e.Row.ToolTip = e.Row.Cells[8].Text;
                    e.Row.Cells[8].Visible = false;
                    gridView.HeaderRow.Cells[8].Visible = false;

                    if (e.Row.Cells[0] != null)
                    {
                        if (!String.IsNullOrEmpty(e.Row.Cells[0].Text))
                        {
                            string rowValue = e.Row.Cells[0].Text;
                            Image icon = new Image();

                            switch (rowValue)
                            {
                                case "4":
                                    icon.ImageUrl = "~/images/stopped-unknown.png";
                                    icon.ToolTip = "Script has stopped due to unknown reasons";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                                case "5":
                                    icon.ImageUrl = "~/images/checkmark.png";
                                    icon.ToolTip = "Script has completed";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                                case "6":
                                    icon.ImageUrl = "~/images/stopped-timeout.png";
                                    icon.ToolTip = "Script has stopped because it has timed out";
                                    e.Row.Cells[0].Controls.Add(icon);
                                    break;
                                case "7":
                                    icon.ImageUrl = "~/images/no-records.png";
                                    icon.ToolTip = "Script has no records to process";
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
        //      This method gets all historical scripts.
        //</summary>
        private void GetHistoricalScripts()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();
                using (var activeScriptsCmd = new SqlCommand("dbo.ScriptingDashboardGetHistoricalScriptInfo", con) { CommandType = CommandType.StoredProcedure })
                {
                    this.adpt = new SqlDataAdapter(activeScriptsCmd);
                    this.ds = new DataSet();
                    this.adpt.Fill(this.ds);

                    if (this.ds.Tables.Count > 0)
                    {
                        this.gridView.DataSource = this.ds.Tables[0];
                        this.gridView.DataBind();
                    }
                }
            }
        }
    }
}