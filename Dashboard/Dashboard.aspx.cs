using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace Dashboard
{
    // <summary>
    //      This class calculates the daily FTE value, the monthly FTE value, and the number
    //      of records that have been processed in the past hour.
    //      Last Updated: July 10, 2017 by Kathryn Browning
    // </summary>
    public partial class Dashboard : Page
    {
        private DateTime date;
        private DateTime tmrwDate;
        private int dateMonth;
        private int dateYear;

        private static int WORK_DAY_HOURS = 8;
        private static int SECONDS = 60;

        public Dashboard()
        {
            this.date = DateTime.Today;
            this.tmrwDate = DateTime.Today.AddDays(1);
            this.dateMonth = 0;
            this.dateYear = 0;
        }

        //<summary>
        //      This method fills the summary bubbles with information
        //      pulled from the database.
        //</summary>
        //<exception cref="InvalidCastException">
        //      Thrown only if all scripts in the database are still running
        //      which means they would have a NULL stop time.
        //</exception
        protected void Page_Load(object sender, EventArgs e)
        {
            this.CalculateDailyFTE();
            this.CalculateMonthlyFTE();
            this.CalculateRecordsProcessedInPastHour();
        }

        //<summary>
        //      Refreshes the summary information every five seconds.
        //</summary>
        protected void Timer_Tick(object sender, EventArgs e)
        {
            this.CalculateDailyFTE();
            this.CalculateMonthlyFTE();
            this.CalculateRecordsProcessedInPastHour();
        }

        #region Calculate Daily FTE
        //<summary>
        //      This method gets the record count, human seconds per record, and script seconds
        //      per record for the current day, then performs the FTE calculation based on those
        //      values.
        //</summary>
        private void CalculateDailyFTE()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();

                System.Diagnostics.Debug.WriteLine(this.date.ToString());
                System.Diagnostics.Debug.WriteLine(this.tmrwDate.ToString());

                var totalRecordsProcessed = this.GetRecordCountToday(con);
                var humanRecordsRPSInt = this.GetHumanSecondsToday(con);
                var actualScriptRPSInt = this.GetScriptSecondsToday(con);

                this.PerformDailyFTECalculation(humanRecordsRPSInt, actualScriptRPSInt, totalRecordsProcessed);
            }
        }

        //<summary>
        //      This method gets the total number of records processed today.
        //</summmary>
        private int GetRecordCountToday(SqlConnection con)
        {
            var totalRecordsProcessed = 0;
            try
            {
                using (var cmdRecordCount = new SqlCommand("dbo.ScriptingDashboardGetRecordCountToday", con) { CommandType = CommandType.StoredProcedure })
                {
                    cmdRecordCount.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;
                    cmdRecordCount.Parameters.Add("@TomorrowDate", SqlDbType.SmallDateTime).Value = this.tmrwDate;
                    totalRecordsProcessed = Convert.ToInt32(cmdRecordCount.ExecuteScalar());
                }
            }
            catch (InvalidCastException)
            {
                totalRecordsProcessed = 0;
            }
            return totalRecordsProcessed;
        }

        //<summary>
        //      This method gets the total number of human seconds it would have taken
        //      to process each record for the current day.
        //</summary>
        private int GetHumanSecondsToday(SqlConnection con)
        {
            using (var humanRecordsRPS = new SqlCommand("dbo.ScriptingDashboardGetHumanSecsToday", con) { CommandType = CommandType.StoredProcedure })
            {
                humanRecordsRPS.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;
                humanRecordsRPS.Parameters.Add("@TomorrowDate", SqlDbType.SmallDateTime).Value = this.tmrwDate;
                var humanRecordsRPSInt = Convert.ToInt32(humanRecordsRPS.ExecuteScalar());

                return humanRecordsRPSInt;
            }
        }

        //<summary>
        //      This method gets the total number of seconds it takes the script to
        //      process each record for the current day.
        //</summary>
        private int GetScriptSecondsToday(SqlConnection con)
        {
            using (var actualScriptRPS = new SqlCommand("dbo.ScriptingDashboardGetScriptSecsToday", con) { CommandType = CommandType.StoredProcedure })
            {
                actualScriptRPS.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;
                actualScriptRPS.Parameters.Add("@TomorrowDate", SqlDbType.SmallDateTime).Value = this.tmrwDate;
                var actualScriptRPSInt = Convert.ToInt32(actualScriptRPS.ExecuteScalar());

                return actualScriptRPSInt;
            }
        }

        //<summary>
        //      This method calculates the daily FTE by counting how many records have been processed since the
        //      start of the current day and by comparing the estimated amount of time it takes a human to 
        //      process a record with the actual time it takes the script to process a record. If no records have
        //      been processed during the current day, the site displays a message saying so.
        //</summary>
        //<exception cref="DivideByZeroException">
        //      Thrown if no scripts have been run during the current day.
        //</exception>
        private void PerformDailyFTECalculation(double humanRecordsRPSInt, double actualScriptRPSInt, double totalRecordsProcessed)
        {
            try
            {
                var FTEEquivalentPerRecord = (actualScriptRPSInt / humanRecordsRPSInt) / SECONDS;
                var FTEMinutes = FTEEquivalentPerRecord * totalRecordsProcessed;
                var FTEToday = (FTEMinutes / SECONDS) / WORK_DAY_HOURS;
                FTEToday = Math.Round(FTEToday, 2);

                this.lblDailyFTE.Text = FTEToday.ToString();
            }
            catch (DivideByZeroException)
            {
                this.lblDailyFTE.Text = "No scripts run today.";
            }
        }
        #endregion

        #region Calculate Monthly FTE
        //<summary>
        //      This method gets the record count, human seconds per record, and script seconds
        //      per record since the start of the month, then performs the FTE calculation based 
        //      on those values.
        //</summary>
        private void CalculateMonthlyFTE()
        {
            using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
            {
                con.Open();

                this.dateMonth = this.date.Month;
                this.dateYear = this.date.Year;

                var totalRecordsProcessed = this.GetRecordCountCurrentMonth(con);
                var humanRecordsRPSInt = this.GetHumanSecondsCurrentMonth(con);
                var actualScriptRPSInt = this.GetScriptSecondsCurrentMonth(con);
                var weekdaysElapsed = this.CalculateWeekdaysElapsed();

                this.PerformMonthlyFTECalculation(humanRecordsRPSInt, actualScriptRPSInt, totalRecordsProcessed, weekdaysElapsed);
            }
        }

        //<summary>
        //      This method gets the total number of records processed
        //      since the start of the current month.
        //</summary>

        private int GetRecordCountCurrentMonth(SqlConnection con)
        {
            var totalRecordsProcessed = 0;
            try
            {
                using (var cmdRecordCount = new SqlCommand("dbo.ScriptingDashboardGetRecordCountCurrentMonth", con) { CommandType = CommandType.StoredProcedure })
                {
                    cmdRecordCount.Parameters.Add("@DateYear", SqlDbType.VarChar, 4).Value = this.dateYear;
                    cmdRecordCount.Parameters.Add("@DateMonth", SqlDbType.VarChar, 12).Value = this.dateMonth; ;
                    cmdRecordCount.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;

                    totalRecordsProcessed = Convert.ToInt32(cmdRecordCount.ExecuteScalar());
                }
            }
            catch (InvalidCastException)
            {
                totalRecordsProcessed = 0;
            }
            return totalRecordsProcessed;
        }

        //<summary>
        //      This method gets the total number of human seconds
        //      it takes to process each record since the start of 
        //      the current month.
        //</summary>
        private int GetHumanSecondsCurrentMonth(SqlConnection con)
        {
            using (var humanRecordsRPS = new SqlCommand("dbo.ScriptingDashboardGetHumanSecsCurrentMonth", con) { CommandType = CommandType.StoredProcedure })
            {
                humanRecordsRPS.Parameters.Add("@DateYear", SqlDbType.VarChar, 4).Value = this.dateYear;
                humanRecordsRPS.Parameters.Add("@DateMonth", SqlDbType.VarChar, 12).Value = this.dateMonth;
                humanRecordsRPS.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;
                var humanRecordsRPSInt = Convert.ToInt32(humanRecordsRPS.ExecuteScalar());

                return humanRecordsRPSInt;
            }
        }

        //<summary>
        //      This method gets the total number of script seconds
        //      it takes to process each record since the start of 
        //      the current month.
        //</summary>
        private int GetScriptSecondsCurrentMonth(SqlConnection con)
        {
            using (var actualScriptRPS = new SqlCommand("dbo.ScriptingDashboardGetScriptSecsCurrentMonth", con) { CommandType = CommandType.StoredProcedure })
            {
                actualScriptRPS.Parameters.Add("@DateYear", SqlDbType.VarChar, 4).Value = this.dateYear;
                actualScriptRPS.Parameters.Add("@DateMonth", SqlDbType.VarChar, 12).Value = this.dateMonth;
                actualScriptRPS.Parameters.Add("@Date", SqlDbType.SmallDateTime).Value = this.date;
                var actualScriptRPSInt = Convert.ToInt32(actualScriptRPS.ExecuteScalar());

                return actualScriptRPSInt;
            }
        }

        //<summary>
        //      This method calculates the monthly FTE by determining how many records have been processed since the 
        //      start of the current month and by comparing the estimated amount of time it takes a human to process
        //      a record with the actual time it takes the script to process a record. If no records have been
        //      processed during the current month, the site displays a message saying so.
        //</summary>
        //<exception cref="DivideByZeroException">
        //      Thrown if no scripts have been run during the current month.
        //</exception>
        private void PerformMonthlyFTECalculation(double humanRecordsRPSInt, double actualScriptRPSInt, double totalRecordsProcessed, double weekdaysElapsed)
        {
            // (FTE / 60 / (total elapsed weekdays * 8) + hours passed in current day) --> calculation
            try
            {
                var FTEEquivalentPerRecord = (actualScriptRPSInt / humanRecordsRPSInt) / SECONDS;
                var FTEMinutes = FTEEquivalentPerRecord * totalRecordsProcessed;
                var FTEMonth = (FTEMinutes / SECONDS) / (weekdaysElapsed * WORK_DAY_HOURS);
                FTEMonth = Math.Round(FTEMonth, 2);

                this.lblMonthlyFTE.Text = FTEMonth.ToString();
                this.lblWeekdaysElapsed.Text = weekdaysElapsed.ToString() + " weekdays elapsed since " + this.dateMonth + "-01-" + this.dateYear;
            }
            catch (DivideByZeroException)
            {
                this.lblMonthlyFTE.Text = "No scripts run this month.";
            }
        }
        #endregion

        //<summary>
        //      This method calculates how many weekdays have elapsed 
        //      since the start of the month. This is used in calculating
        //      monthly FTE.
        //</summary>
        private int CalculateWeekdaysElapsed()
        {
            var endDate = DateTime.Today;
            var startMonth = endDate.Month;
            var currentYear = endDate.Year;

            var startDate = new DateTime(currentYear, startMonth, 1); //(year, month, day)

            TimeSpan span = endDate - startDate;
            var businessDays = span.Days + 1; //+1 to include current day
            var daysofWeek = businessDays / 7;

            if (businessDays > daysofWeek * 7)
            {
                var firstDayOfWeek = startDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)startDate.DayOfWeek;
                var lastDayOfWeek = endDate.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)endDate.DayOfWeek;

                //determines whether 1 or 2 days is left in the weekend
                if (lastDayOfWeek < firstDayOfWeek)
                {
                    lastDayOfWeek += 7;
                }

                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7) //both sat and sun remain
                    {
                        businessDays -= 2;
                    }
                    else if (lastDayOfWeek >= 6) //only sat remains
                    {
                        businessDays -= 1;
                    }
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7) //only sun remains
                {
                    businessDays -= 1;
                }
            }
            businessDays -= daysofWeek + daysofWeek;
            return businessDays;
        }



        //<summary>
        //      This method calculates the number of records processed in the past hour
        //      and sets the label text.
        //</summary>
        private void CalculateRecordsProcessedInPastHour()
        {
            try
            {
                using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["ScriptingDashboard"].ConnectionString))
                {
                    con.Open();

                    var currentDateTime = DateTime.Now;
                    var pastHourDate = currentDateTime.AddHours(-1);
                    System.Diagnostics.Debug.WriteLine(pastHourDate);

                    using (var cmdRecordCount = new SqlCommand("dbo.ScriptingDashboardGetRecordCountPastHour", con) { CommandType = CommandType.StoredProcedure })
                    {
                        cmdRecordCount.Parameters.Add("@PastHourDate", SqlDbType.SmallDateTime).Value = pastHourDate;
                        cmdRecordCount.Parameters.Add("@CurrentDateTime", SqlDbType.SmallDateTime).Value = currentDateTime;
                        var totalRecordsProcessed = Convert.ToInt32(cmdRecordCount.ExecuteScalar());

                        this.lblRecordsPerMinute.Text = totalRecordsProcessed + " records";
                    }
                }
            }
            catch (Exception)
            {
                this.lblRecordsPerMinute.Text = "No records processed in past hour.";
            }
        }
    }
}