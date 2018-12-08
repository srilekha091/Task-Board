using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;

namespace Taskboard
{
    /// <summary>
    /// Summary description for TaskManagerService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class TaskManagerService : System.Web.Services.WebService
    {
        [WebMethod]
        public void GetAllProjects()
        {
            List<Project> listOfProjects = new List<Project>();
            JavaScriptSerializer js = new JavaScriptSerializer();

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select * from dbo.Project", con);
                con.Open();

                SqlDataReader rdr;

                try
                {
                    rdr = cmd.ExecuteReader();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Table not found");
                    con.Close();
                    return;
                }

                while (rdr.Read())
                {
                    Project project = new Project();
                    project.ProjectId = Convert.ToInt32(rdr["ProjectId"]);
                    project.ProjectName = rdr["ProjectName"].ToString();
                    project.ProjectDescription = rdr["ProjectDescription"].ToString();
                    project.EmployeesWorkingOnProject = new List<Employee>();
                    listOfProjects.Add(project);
                }

            }

            using (SqlConnection connection = new SqlConnection(cs))
            {
                foreach (Project project in listOfProjects)
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("select dbo.Employee.EmployeeId, dbo.Employee.EmployeeName " +
                                     "from dbo.Employee " +
                                     "INNER JOIN dbo.EmployeeProjects On dbo.EmployeeProjects.EmployeeId = dbo.Employee.EmployeeId " +
                                     "where ProjectId = " + project.ProjectId, connection);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        Employee employee = new Employee();
                        employee.EmployeeId = Convert.ToInt32(reader["EmployeeId"]);
                        employee.EmployeeName = reader["EmployeeName"].ToString();
                        project.EmployeesWorkingOnProject.Add(employee);
                    }
                    connection.Close();
                }
            }

            Context.Response.Write(js.Serialize(listOfProjects));
        }

        [WebMethod]
        public void GetProject(int projectId)
        {
            ProjectDetails projectDetails = new ProjectDetails();
            JavaScriptSerializer js = new JavaScriptSerializer();

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select * from Project where ProjectId = '" + projectId + "'", con);
                con.Open();

                SqlDataReader rdr;
                rdr = cmd.ExecuteReader();

                while(rdr.Read())
                {
                    projectDetails.ProjectId = Convert.ToInt32(rdr["ProjectId"]);
                    projectDetails.ProjectName = rdr["ProjectName"].ToString();
                    projectDetails.ProjectDescription = rdr["ProjectDescription"].ToString();
                    projectDetails.EmployeesWorkingOnProjectWithCorrespondingTasks = new List<EmployeeTaskDetails>();
                }
            }

            using (SqlConnection connection = new SqlConnection(cs))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("select dbo.Employee.EmployeeId, dbo.Employee.EmployeeName " +
                                    "from dbo.Employee " +
                                    "INNER JOIN dbo.EmployeeProjects On dbo.EmployeeProjects.EmployeeId = dbo.Employee.EmployeeId " +
                                    "where ProjectId = " + projectDetails.ProjectId, connection);

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    EmployeeTaskDetails employeeTaskDetails = new EmployeeTaskDetails();
                    employeeTaskDetails.EmployeeId = Convert.ToInt32(reader["EmployeeId"]);
                    employeeTaskDetails.EmployeeName = reader["EmployeeName"].ToString();
                    employeeTaskDetails.EmployeeTasks = new List<Task>();
                    projectDetails.EmployeesWorkingOnProjectWithCorrespondingTasks.Add(employeeTaskDetails);
                }

                // Adding a new EmployeeTaskDetail object at the end of the list so we can use
                // ng-repeat and add an extra column at the end for adding new employee.
                projectDetails.EmployeesWorkingOnProjectWithCorrespondingTasks.Add(new EmployeeTaskDetails());
            }

            foreach (EmployeeTaskDetails employeeDetails in projectDetails.EmployeesWorkingOnProjectWithCorrespondingTasks)
            {

                List<Task> listOfTasks = new List<Task>();

                using (SqlConnection con = new SqlConnection(cs))
                {
                    SqlCommand cmd = new SqlCommand("select * from Tasks where " +
                                                    "ProjectId = " + projectId + " AND " +
                                                    "EmployeeId = " + employeeDetails.EmployeeId, con);
                    con.Open();

                    SqlDataReader rdr;
                    rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Task task = new Task();
                        task.Id = Convert.ToInt32(rdr["Id"]);
                        task.TaskTitle = rdr["TaskTitle"].ToString();
                        task.TaskDescription = rdr["TaskDescription"].ToString();
                        task.TaskStatus = rdr["TaskStatus"].ToString();
                        task.EmployeeId = Convert.ToInt32(rdr["EmployeeId"]);
                        task.ProjectId = Convert.ToInt32(rdr["ProjectId"]);
                        listOfTasks.Add(task);
                    }

                    // Adding a new Task object at the end of the list so we can use
                    // ng-repeat and add an extra row at the end for adding new Task.
                    listOfTasks.Add(new Task());

                    employeeDetails.EmployeeTasks = listOfTasks;
                }
            }

            Context.Response.Write(js.Serialize(projectDetails));
        }

        [WebMethod]
        public void GetEmployee(int employeeId)
        {
            Employee employee = new Employee();
            JavaScriptSerializer js = new JavaScriptSerializer();

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select * from Employee where EmployeeId = " + employeeId, con);
                con.Open();

                SqlDataReader rdr;
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    employee.EmployeeId = Convert.ToInt32(rdr["EmployeeId"]);
                    employee.EmployeeName = rdr["EmployeeName"].ToString();
                }
            }

            Context.Response.Write(js.Serialize(employee));
        }

        [WebMethod]
        public void GetTaskList(int projectId, int employeeId)
        {
            List<Task> listOfTasks = new List<Task>();
            JavaScriptSerializer js = new JavaScriptSerializer();

            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("select * from Tasks where " +
                                                "ProjectId = " + projectId + " AND " +
                                                "EmployeeId = " + employeeId, con);
                con.Open();

                SqlDataReader rdr;
                rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    Task task = new Task();
                    task.Id = Convert.ToInt32(rdr["Id"]);
                    task.TaskTitle = rdr["TaskTitle"].ToString();
                    task.TaskDescription = rdr["TaskDescription"].ToString();
                    task.TaskStatus = rdr["TaskStatus"].ToString();
                    task.EmployeeId = Convert.ToInt32(rdr["EmployeeId"]);
                    task.ProjectId = Convert.ToInt32(rdr["ProjectId"]);
                    listOfTasks.Add(task);
                }
            }

            Context.Response.Write(js.Serialize(listOfTasks));
        }

        [WebMethod]
        public void AddNewProject(Project project)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand insertNewProjectCmd = new SqlCommand("INSERT INTO Project (ProjectName, ProjectDescription) VALUES ('" +
                                                    project.ProjectName + "', '" + project.ProjectDescription + "')"
                                                    , con);
                con.Open();

                try
                {
                    insertNewProjectCmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Table not found. Creating the necessary tables.");
                    con.Close();
                    con.Open();

                    // Creating Project table
                    SqlCommand createProjectTable = new SqlCommand("Create Table Project (" +
                                            "ProjectId int IDENTITY(1, 1) NOT NULL PRIMARY KEY, " +
                                            "ProjectName varchar(255) NOT NULL, " +
                                            "ProjectDescription varchar(255), )", con);

                    // Creating Employee table
                    SqlCommand createEmployeeTable = new SqlCommand("Create Table Employee (" +
                                            "EmployeeId int IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                            "EmployeeName varchar(255) NOT NULL)", con);

                    // Creating EmployeeProject table
                    SqlCommand createEmpProjTable = new SqlCommand("Create Table EmployeeProjects (" +
                                            "Id int IDENTITY(1,1) NOT NULL PRIMARY KEY," +
                                            "EmployeeId int NOT NULL FOREIGN KEY REFERENCES Employee(EmployeeId)," +
                                            "ProjectId int NOT NULL FOREIGN KEY REFERENCES Project(ProjectId))", con);

                    createProjectTable.ExecuteNonQuery();
                    createEmployeeTable.ExecuteNonQuery();
                    createEmpProjTable.ExecuteNonQuery();
                    insertNewProjectCmd.ExecuteNonQuery();

                    // Insert Employees into Employee table 
                    foreach (Employee employee in project.EmployeesWorkingOnProject)
                    {
                        SqlCommand insertIntoEmpTable = new SqlCommand("INSERT INTO Employee (EmployeeName) Values " +
                                                                        "('" + employee.EmployeeName + "')", con);

                        insertIntoEmpTable.ExecuteNonQuery();
                    }

                    con.Close();
                }

                con.Close();
                con.Open();

                if (project.EmployeesWorkingOnProject != null)
                {
                    foreach (Employee employee in project.EmployeesWorkingOnProject)
                    {
                        SqlCommand insertIntoEmpProjCmd = new SqlCommand("INSERT INTO EmployeeProjects (EmployeeId, ProjectId) " +
                                                                         "SELECT Employee.EmployeeId, Project.ProjectId " +
                                                                         "FROM Employee, Project " +
                                                                         "WHERE Employee.EmployeeName = " + "'" + employee.EmployeeName + "'" +
                                                                         "AND Project.ProjectName = " + "'" + project.ProjectName + "'", con);

                        insertIntoEmpProjCmd.ExecuteNonQuery();
                    }
                }
            }

        }

        [WebMethod]
        public void AddNewEmployee(Employee employee)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand insertNewEmployeeCmd = new SqlCommand("INSERT INTO Employee (EmployeeName) VALUES ('" +
                                                    employee.EmployeeName + "')", con);
                con.Open();

                insertNewEmployeeCmd.ExecuteNonQuery();
            }
        }

        [WebMethod]
        public void AddNewTask(Task task)
        {
            string cs = ConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;
            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand insertNewTask = new SqlCommand("INSERT INTO Tasks " +
                    "(TaskTitle, TaskDescription, TaskStatus, EmployeeId, ProjectId) VALUES ('" +
                    task.TaskTitle + ", " + task.TaskDescription + ", " + task.TaskStatus + ", " + 
                    task.EmployeeId + ", " + task.ProjectId + "')", con);

                con.Open();

                try
                {
                    insertNewTask.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    Console.WriteLine("Task table not found. Creating the Task table.");
                    con.Close();
                    con.Open();

                    // Creating Task table
                    SqlCommand createTaskTable = new SqlCommand("Create Table Task (" +
                                            "Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                                            "TaskTitle varchar(255) NOT NULL, " +
                                            "TaskDescription varchar(255), " +
                                            "TaskStatus varchar(25) NOT NULL CHECK " +
                                            "(TaskStatus IN('Done', 'On Hold', 'In Process', 'Sent', 'Schedule'))," +
                                            "EmployeeId int NOT NULL FOREIGN KEY REFERENCES Employee(EmployeeId)," +
                                            "ProjectId int NOT NULL FOREIGN KEY REFERENCES Project(ProjectId))",
                                            con);

                    createTaskTable.ExecuteNonQuery();
                    insertNewTask.ExecuteNonQuery();
                }
            }
        }
    }
}
