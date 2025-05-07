namespace FantasySports.Models
{
    // Models/TeamMatchup.cs
    public class TeamMatchup
    {
        public int Id { get; set; }
        public int MeetId { get; set; }
        public Meet Meet { get; set; }
        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; }
        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; }
        public int? HomeTeamScore { get; set; }
        public int? AwayTeamScore { get; set; }
        public bool IsCompleted => HomeTeamScore.HasValue && AwayTeamScore.HasValue;
    }
}
