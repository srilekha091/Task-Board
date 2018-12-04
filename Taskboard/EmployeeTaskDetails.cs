using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taskboard
{
    public class EmployeeTaskDetails
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public List<Task> EmployeeTasks { get; set; }
    }
}