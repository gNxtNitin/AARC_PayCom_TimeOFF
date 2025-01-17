using PaycomTimeOffData.BusinessLogic;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;

namespace PaycomTimeOffData
{
    public class GetTimeOffRecords
    {
        public static string FilePath = "";
        public static void ExtractTimeOffRecords(string url, IConfiguration config)
        {
            try
            {
                FilePath = config["LogInSecret:LogFilePath"];
                WebDriverHandler.CreateLog(FilePath, "");
                WebDriverHandler.CreateLog(FilePath, "Calling the Launch Browser at "+ DateTime.UtcNow);
                string userDataDir = config["UserDataDir:Chrome:Path"];
                string userProfile = config["UserDataDir:Chrome:Profile"];
                WebDriverHandler.LaunchBrowser(Browsers.ChromeBrowser, url, userDataDir, userProfile);

                LogIn(config["LogInSecret:ClientCode"], config["LogInSecret:UserName"], config["LogInSecret:Password"]);
                Dictionary<string, string> securityQuestion = config.GetSection("SecurityQuestions").Get<Dictionary<string, string>>();
                SolveSecurityQuestions(securityQuestion);

                IWebElement timeManagementOption = null;
                bool isLoggedIn = WebDriverHandler.CheckForElement(ElementSelectors.TIMEMANAGEMENT_MENUOPTION, 10, out timeManagementOption);

                if (isLoggedIn == true && timeManagementOption != null)
                {
                    WebDriverHandler.NavigateToUrl(config["PayComUrls:TimeOffCalenderUrl"]);
                    ExtractAndSaveInfo(config["CSVExportPath"], config["ConnectionStrings:ServerDb"]);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                WebDriverHandler.CreateLog(FilePath, ex.Message.ToString());

            }
            finally
            {
                WebDriverHandler.CloseBrowser();
            }
            
        }

        public static void LogIn(string clientCode, string userName, string password)
        {
            try
            {
                WebDriverHandler.CreateLog(FilePath, "Calling the LogIn Method at " + DateTime.UtcNow);
                WebDriverHandler.PopulateTextInput(ElementSelectors.CLIENT_CODE_INPUT, clientCode);
                WebDriverHandler.PopulateTextInput(ElementSelectors.CLIENT_USERNAME_INPUT, userName);
                WebDriverHandler.PopulateTextInput(ElementSelectors.CLIENT_PASSWORD_INPUT, password);
                WebDriverHandler.ClickOnElement(ElementSelectors.CLIENT_LOGIN_SUBMITBTN);
            }
            catch (Exception ex)
            {
                WebDriverHandler.CreateLog(FilePath, ex.Message.ToString());
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        private static void SolveSecurityQuestions(Dictionary<string, string> questAnsDict)
        {
            IWebElement sequrityQuestionPage = null;

            bool isSeqQuePage = WebDriverHandler.CheckForElement(ElementSelectors.SECURITY_QUESTIONS_FORM,5, out sequrityQuestionPage);
            if(isSeqQuePage == true && sequrityQuestionPage != null)
            {

                string question1 = WebDriverHandler.GetInnerText(ElementSelectors.SECURITY_QUESTIONS1_LABEL);
                string question2 = WebDriverHandler.GetInnerText(ElementSelectors.SECURITY_QUESTIONS2_LABEL);
                string ans1 = string.Empty;
                string ans2 = string.Empty;

                bool ans1Found = questAnsDict.TryGetValue(question1.ToLower(), out ans1);
                bool ans2Found = questAnsDict.TryGetValue(question2.ToLower(), out ans2);

                if (ans1Found == true && ans2Found == true) 
                {
                    WebDriverHandler.PopulateTextInput(ElementSelectors.SECURITY_QUESTIONS1_INPUT, ans1);
                    WebDriverHandler.PopulateTextInput(ElementSelectors.SECURITY_QUESTIONS2_INPUT, ans2);
                    WebDriverHandler.ClickOnElement(ElementSelectors.SECURITY_QUESTIONS_SUBMITBTN);
                }
            }

        }

        private static void ExtractAndSaveInfo(string path, string connectionString)
        {
           IWebElement timeOffRequestsDiv  = null;
            bool isTimeOffRequestsDivExists = WebDriverHandler.CheckForElement(ElementSelectors.TIMEOFF_REQUEST_DIV, 5, out timeOffRequestsDiv);
            if (isTimeOffRequestsDivExists == true && timeOffRequestsDiv != null)
            {
                try
                {
                    //CLEAN AND PARSE THE HTML
                    string htmlContent = timeOffRequestsDiv.GetAttribute("innerHTML");

                    HtmlToCSVConverter htmlToCSVConverter = new HtmlToCSVConverter();
                    htmlToCSVConverter.GetCSVFromHTMl(htmlContent, FilePath, connectionString);
                }
                catch (Exception ex)
                {
                    WebDriverHandler.CreateLog(FilePath, ex.Message.ToString());
                    throw ex;
                }
               
            }
        }
    }
}
