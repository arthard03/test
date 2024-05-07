using Microsoft.AspNetCore.Mvc;
using test.Models;

namespace test.Controler
{

namespace YourNamespace.Controllers
{
    public class TeamMemberTasksController : ControllerBase
    {
        private readonly TeamMemberTaskEndpoint teamMemberTaskEndpoint;

        public TeamMemberTasksController()
        {
            teamMemberTaskEndpoint = new TeamMemberTaskEndpoint();
        }

        [HttpGet]
        [Route("api/teammember/{id}/tasks")]
        public IActionResult GetTeamMemberTasks(int id)
        {
            List<TaskData> tasks = teamMemberTaskEndpoint.GetTeamMemberTasks(id);

            if (tasks == null || tasks.Count == 0)
            {
                return NotFound();
            }

            return Ok(tasks);
        }
    }
}
}