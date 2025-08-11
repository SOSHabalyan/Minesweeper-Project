using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Minesweeper.Blazor.Services
{
    public class MinesweeperApiService
    {
        private readonly HttpClient _http;
        public MinesweeperApiService(HttpClient http)
        {
            _http = http;
        }

        // Leaderboard
        public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync()
        {
            return await _http.GetFromJsonAsync<List<LeaderboardEntryDto>>("api/leaderboard") ?? new();
        }

        // Users
        public async Task<List<string>> GetUsersAsync()
        {
            return await _http.GetFromJsonAsync<List<string>>("api/users") ?? new();
        }
        public async Task CreateUserAsync(string name)
        {
            await _http.PostAsJsonAsync("api/users", new { name });
        }

        // Game
        public async Task<GameDto?> StartGameAsync(string playerName)
        {
            var response = await _http.PostAsJsonAsync("api/games", new { playerName });
            return await response.Content.ReadFromJsonAsync<GameDto>();
        }
        public async Task<GameDto?> GetGameAsync(string gameId)
        {
            return await _http.GetFromJsonAsync<GameDto>($"api/games/{gameId}");
        }
        public async Task<GameDto?> RevealCellAsync(string gameId, int row, int col)
        {
            var response = await _http.PostAsJsonAsync($"api/games/{gameId}/reveal", new { row, col });
            return await response.Content.ReadFromJsonAsync<GameDto>();
        }

        // Slot Machine
        public async Task<SlotOutcomeDto?> SpinSlotAsync(string gameId)
        {
            var response = await _http.PostAsync($"api/games/{gameId}/spin", null);
            return await response.Content.ReadFromJsonAsync<SlotOutcomeDto>();
        }
    }

    // DTOs
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
    public class GameDto
    {
        public string Id { get; set; } = "";
        public string PlayerName { get; set; } = "";
        public int Credits { get; set; }
        public int Dollars { get; set; }
        public int Level { get; set; }
        public BoardDto Board { get; set; } = new();
    }
    public class BoardDto
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public CellDto[,] Cells { get; set; } = new CellDto[0,0];
    }
    public class CellDto
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public bool IsRevealed { get; set; }
        public bool IsFlagged { get; set; }
        public bool HasMine { get; set; }
        public int AdjacentMineCount { get; set; }
    }
    public class SlotOutcomeDto
    {
        public string Description { get; set; } = "";
        public bool IsPositive { get; set; }
        public int PointsAwarded { get; set; }
        public int FlagsPlaced { get; set; }
        public int CellsRevealed { get; set; }
        public int DollarsAwarded { get; set; }
        public bool GameWon { get; set; }
        public bool GameOver { get; set; }
        public bool RoundRestart { get; set; }
    }
}
