namespace test.Models
{
    


    public class TeamMemberDto
    {
        public int IdTeamMember { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public List<TaskDto> TasksAssigned { get; set; }
        public List<TaskDto> TasksCreated { get; set; }
    }}