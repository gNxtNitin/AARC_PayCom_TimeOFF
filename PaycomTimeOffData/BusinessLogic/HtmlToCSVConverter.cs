using AngleSharp.Html;
using AngleSharp;
using System.Net.Http;
using AngleSharp.Dom;
using System.Text.RegularExpressions;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Data.SqlClient;
using System.Linq;

namespace PaycomTimeOffData.BusinessLogic
{
    public class HtmlToCSVConverter
    {
        private readonly IBrowsingContext context;

        public HtmlToCSVConverter()
        {
            var config = Configuration.Default.WithDefaultLoader();
            context = BrowsingContext.New(config);
        }

        public async void GetCSVFromHTMl(string html, string path, string connectionString)
        {
            try
            {
                IDocument document = await context.OpenAsync(req => req.Content(html));
                IHtmlCollection<IElement> allTables = document.QuerySelectorAll(ElementSelectors.TIMEOFF_REQUEST_TABLES);

                if (allTables != null && allTables.Count() > 0)
                {
                    List<RequestRecords> records = GetRequestRecordsFromTable(allTables);

                    if (records != null && records.Count() > 0)
                    {
                        //SaveAsCsv(path, records);
                        EmptyTable(connectionString, path);
                        SaveToDb(connectionString, path, records);

                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WebDriverHandler.CreateLog(path, ex.Message.ToString());
            }

        }

        private List<RequestRecords> GetRequestRecordsFromTable(IHtmlCollection<IElement> tables)
        {
            List<RequestRecords> allRecords = new List<RequestRecords>();
            int TimeDuration = 0;
            string OfficeStartHours = "";


            foreach (var table in tables)
            {
                RequestRecords recordsTemp = new RequestRecords();

                var rows = table.QuerySelectorAll(ElementSelectors.TIMEOFF_REQUEST_TABLE_ALLROWS);

                try
                {
                    TimeDuration = 0;
                    string text = System.Web.HttpUtility.HtmlDecode(rows[0].TextContent.Trim());
                    recordsTemp.DisplayName = text;
                    Regex regex1 = new Regex(@"^(?<LastName>[a-zA-Z]+),\s*(?<FirstName>[a-zA-Z]+)\s*\((?<ID>\d+)\)$");
                    var matches = regex1.Match(text);
                    if (matches.Success)
                    {
                        recordsTemp.FirstName = matches.Groups["FirstName"].Value.Trim();
                        recordsTemp.LastName = matches.Groups["LastName"].Value.Trim();
                        recordsTemp.Id = Convert.ToInt32(matches.Groups["ID"].Value.Trim());
                    }

                    var td3 = rows[2].QuerySelectorAll("td");
                    string text2 = System.Web.HttpUtility.HtmlDecode(td3[1].TextContent.Trim());
                    string[] parts = text2.Split('.');
                    recordsTemp.LeaveHours = text2;
                    recordsTemp.LeaveType = System.Web.HttpUtility.HtmlDecode(td3[0].TextContent.Trim());
                    TimeDuration = Convert.ToInt32(parts[0]);

                    for (int i = 1; i < rows.Count(); i++)
                    {
                        var tr1Td = rows[i].QuerySelectorAll("td");
                        string td1Text = System.Web.HttpUtility.HtmlDecode(tr1Td[0].TextContent.Trim());
                        string td2Text = System.Web.HttpUtility.HtmlDecode(tr1Td[1].TextContent.Trim());

                        if (td1Text.Contains("Starting Time"))
                        {
                            string Start_Time = td2Text;
                            string[] parts2 = Start_Time.Split(' ');
                            recordsTemp.OfficeStartTime = td2Text;
                            OfficeStartHours = parts2[1];
                            if (TimeDuration < 8)
                            {
                                //if (OfficeStartHours.ToUpper() == "PM")
                                //{
                                //    recordsTemp.FinalMessage = "Leave for the first half of the day";
                                //}
                                //if (OfficeStartHours.ToUpper() == "AM")
                                //{
                                //    recordsTemp.FinalMessage = "Leave for the second half of the day";
                                //}
                                recordsTemp.FinalMessage = "Leave for "+ TimeDuration + " hours starting from "+ td2Text + ".";
                            }
                            else
                            {
                                recordsTemp.FinalMessage = "Leave for the full day";
                            }
                        }
                    }

                    allRecords.Add(recordsTemp);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return allRecords;
        }

        //private void SaveAsCsv(string path, List<RequestRecords> requestRecords)
        //{
        //    string fileName = DateTime.Now.ToString("dd_MM_yyyy") + "_TimeOffRequests.csv";
        //    string completePath = Path.Combine(path, fileName);
        //    using (var writer = new StreamWriter(completePath))
        //    using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
        //    {
        //        csv.WriteRecords(requestRecords);
        //    }
        //}

        public void SaveToDb(string connectionString,string FilePath, List<RequestRecords> requestRecords)
        {

            if (!string.IsNullOrEmpty(connectionString))
            {
                WebDriverHandler.CreateLog(FilePath, "Calling the Save Query to insert data into Table");
                string insertQuery = "INSERT INTO [dbo].[aarc_Daily_Notifications](Name,DisplayName, Userid, LeaveType,LeaveHours,OfficeStartTime,Message, FilterDate) VALUES (@Name,@DisplayName, @UserId, @LeaveType,@LeaveHours,@OfficeStartTime,@Message, GETUTCDATE())";
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    foreach (var record in requestRecords)
                    {
                        using (SqlCommand cmd = new SqlCommand(insertQuery, sqlConnection))
                        {
                            cmd.Parameters.AddWithValue("@Name", record.FirstName + " " + record.LastName);
                            cmd.Parameters.AddWithValue("@DisplayName", record.DisplayName);
                            cmd.Parameters.AddWithValue("@Userid", record.Id);
                            cmd.Parameters.AddWithValue("@LeaveType", record.LeaveType);
                            cmd.Parameters.AddWithValue("@LeaveHours", record.LeaveHours);
                            cmd.Parameters.AddWithValue("@OfficeStartTime", record.OfficeStartTime);
                            cmd.Parameters.AddWithValue("@Message", record.FinalMessage);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public void EmptyTable(string connectionString,string FilePath)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                WebDriverHandler.CreateLog(FilePath, "Calling the Truncate Table");
                string insertQuery = "TRUNCATE TABLE [dbo].[aarc_Daily_Notifications]";
                using (var sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    using (SqlCommand cmd = new SqlCommand(insertQuery, sqlConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
