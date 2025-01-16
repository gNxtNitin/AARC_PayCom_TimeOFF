using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaycomTimeOffData.BusinessLogic
{
    public class ElementSelectors
    {
        //LOGIN PAGE
        public const string CLIENT_CODE_INPUT = "//div/input[@id='clientcode']";
        public const string CLIENT_USERNAME_INPUT = "//div/input[@id='username']";
        public const string CLIENT_PASSWORD_INPUT = "//div/input[@id='password']";
        public const string CLIENT_LOGIN_SUBMITBTN = "//div/input[@id='btnSubmit' and @name='login']";

        //SECURITY QUESTIONS PAGE
        public const string SECURITY_QUESTIONS_FORM = "//form[@id='securityQuestionLogin']";
        public const string SECURITY_QUESTIONS1_LABEL = "//div[@id='firstSecurityQuestion-row']/label";
        public const string SECURITY_QUESTIONS2_LABEL = "//div[@id='secondSecurityQuestion-row']/label";
        public const string SECURITY_QUESTIONS1_INPUT = "//input[@name='firstSecurityQuestion']";
        public const string SECURITY_QUESTIONS2_INPUT = "//input[@name='secondSecurityQuestion']";
        public const string SECURITY_QUESTIONS_SUBMITBTN = "//button[@type='submit' and @name='continue']";


        //HOME PAGE (AFTER LOGIN)
        public const string TIMEMANAGEMENT_MENUOPTION = "//li/a[@id='TimeManagement']";
        public const string TIMEOFFMANAGEMENT_CALENDAR_LINK = "//a[@id='Time-OffCalendar']";


        //TIMEOFF DETAILS
        public const string TIMEOFF_REQUEST_DIV = "//div[@id='requests']/div[@id='requestDetailsTabs']";


        //ANGLESHARP SELECTORS FOR EXTRACTION OF TEXT FROM HTML
        public const string TIMEOFF_REQUEST_TABLES = "div[class*='table-responsive']";
        public const string TIMEOFF_REQUEST_TABLE_ALLROWS = "table>tbody>tr:nth-of-type(2) table>tbody>tr";



        
        

    }
}
