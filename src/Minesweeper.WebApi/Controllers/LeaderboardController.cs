using Microsoft.AspNetCore.Mvc;
using Minesweeper.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.WebApi.Controllers;

[ApiController]
[Route("api/leaderboard")]
[Produces("application/json")]
public class LeaderboardController : ControllerBase
{
    [HttpGet]
    public ActionResult<List<LeaderboardEntryDto>> GetLeaderboard()
    {
        var leaderboard = new Leaderboard();
        leaderboard.Load();
        var dtos = leaderboard.Entries.Select(e => new LeaderboardEntryDto
        {
            PlayerName = e.PlayerName,
            LevelsCompleted = e.LevelsCompleted,
            CreditsEarned = e.CreditsEarned,
            MinesFlagged = e.MinesFlagged,
            TimesSpun = e.TimesSpun,
            PositiveOutcomeCount = e.PositiveOutcomeCount,
            NegativeOutcomeCount = e.NegativeOutcomeCount,
            DollarsAccumulated = e.DollarsAccumulated
        }).ToList();
        return Ok(dtos);
    }

    public class LeaderboardEntryDto
    {
        public string PlayerName { get; set; } = "";
        public int LevelsCompleted { get; set; }
        public int CreditsEarned { get; set; }
        public int MinesFlagged { get; set; }
        public int TimesSpun { get; set; }
        public int PositiveOutcomeCount { get; set; }
        public int NegativeOutcomeCount { get; set; }
        public int DollarsAccumulated { get; set; }
    }
}
