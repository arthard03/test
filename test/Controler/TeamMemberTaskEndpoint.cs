using System.Data.SqlClient;
using test.Models;

namespace test.Controler;


    public class TeamMemberTaskEndpoint
    {
        private string connectionString = "Server=localhost;Database=APBD;User Id=sa;Password=asdP929klks";

        public List<TaskData> GetTeamMemberTasks(int teamMemberId)
        {
            List<TaskData> tasks = new List<TaskData>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sqlQuery = @"SELECT Task.Name AS TaskName, Task.Description AS TaskDescription, 
                                        Task.Deadline AS TaskDeadline, Project.Name AS ProjectName, 
                                        TaskType.Name AS TaskType
                                        FROM Task
                                        INNER JOIN Project ON Task.IdProject = Project.IdProject
                                        INNER JOIN TaskType ON Task.IdTaskType = TaskType.IdTaskType
                                        WHERE Task.IdAssignedTo = @TeamMemberId OR Task.IdCreator = @TeamMemberId
                                        ORDER BY Task.Deadline DESC";

                    SqlCommand command = new SqlCommand(sqlQuery, connection);
                    command.Parameters.AddWithValue("@TeamMemberId", teamMemberId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            TaskData task = new TaskData
                            {
                                TaskName = reader["TaskName"].ToString(),
                                TaskDescription = reader["TaskDescription"].ToString(),
                                TaskDeadline = Convert.ToDateTime(reader["TaskDeadline"]),
                                ProjectName = reader["ProjectName"].ToString(),
                                TaskType = reader["TaskType"].ToString()
                            };
                            tasks.Add(task);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            return tasks;
        }
    }
