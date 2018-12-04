using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taskboard
{
    public class Task
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public string TaskDescription { get; set; }
        public string TaskStatus { get; set; }
        public int EmployeeId { get; set; }
        public int ProjectId { get; set; }
    }
}