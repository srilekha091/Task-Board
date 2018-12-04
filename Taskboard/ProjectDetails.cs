using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Taskboard
{
    public class ProjectDetails
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public List<EmployeeTaskDetails> EmployeesWorkingOnProjectWithCorrespondingTasks { get; set; }
    }
}