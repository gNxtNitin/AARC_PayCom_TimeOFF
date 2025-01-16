using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaycomTimeOffData.BusinessLogic
{
    public class RequestRecords
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string LeaveType { get; set; }
        public string LeaveHours { get; set; }
        public string OfficeStartTime { get; set; }
        public string FinalMessage { get; set; }
    }
}
