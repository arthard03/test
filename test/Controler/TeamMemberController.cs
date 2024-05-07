using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using test.Models;

namespace test.Controler
{
    namespace YourNamespace.Controllers
    {
        [ApiController]
        [Route("api/team-members")]
        public class TeamMemberController : ControllerBase
        {
            private readonly IConfiguration _configuration; 

            public TeamMemberController(IConfiguration configuration)
            {
              _configuration = configuration;
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<TeamMemberDto>> GetTeamMemberWithTasks(int id)
            {
                var teamMember = await GetTeamMemberByIdAsync(id);

                if (teamMember == null)
                {
                    return NotFound(); 
                }

                var tasksAssigned = await GetTasksByTeamMemberIdAsync(id, true);
                var tasksCreated = await GetTasksByTeamMemberIdAsync(id, false);

                var teamMemberDto = new TeamMemberDto
                {
                    FirstName = teamMember.FirstName,
                    LastName = teamMember.LastName,
                    Email = teamMember.Email,
                    TasksAssigned = tasksAssigned,
                    TasksCreated = tasksCreated
                };

                return Ok(teamMemberDto); 
            }

            private async Task<TeamMemberDto> GetTeamMemberByIdAsync(int id)
            {
                using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
                {
                    await connection.OpenAsync();

                    var query =
                        "SELECT IdTeamMember, FirstName, LastName, Email FROM dbo.TeamMember WHERE IdTeamMember = @Id";
                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new TeamMemberDto
                                {
                                    IdTeamMember = reader.GetInt32(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    Email = reader.GetString(3)
                                };
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                }
            }

            private async Task<List<TaskDto>> GetTasksByTeamMemberIdAsync(int id, bool assignedTo)
            {
                var tasks = new List<TaskDto>();

                using (var connection = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]))
                {
                    await connection.OpenAsync();

                    var query = assignedTo
                        ? "SELECT IdTask, Name, Description, Deadline, p.Name AS ProjectName, tt.Name AS TaskTypeName FROM dbo.Task t JOIN dbo.Project p ON t.IdProject = p.IdProject JOIN dbo.TaskType tt ON t.IdTaskType = tt.IdTaskType WHERE IdAssignedTo = @Id"
                        : "SELECT IdTask, Name, Description, Deadline, p.Name AS ProjectName, tt.Name AS TaskTypeName FROM dbo.Task t JOIN dbo.Project p ON t.IdProject = p.IdProject JOIN dbo.TaskType tt ON t.IdTaskType = tt.IdTaskType WHERE IdCreator = @Id";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                tasks.Add(new TaskDto
                                {
                                    IdTask = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Deadline = reader.GetDateTime(3),
                                    ProjectName = reader.GetString(4),
                                    TaskTypeName = reader.GetString(5)
                                });
                            }
                        }
                    }
                }

                return tasks;
            }
        }
    }
}