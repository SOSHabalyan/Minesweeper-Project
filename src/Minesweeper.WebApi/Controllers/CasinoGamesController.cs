using Microsoft.AspNetCore.Mvc;
using Minesweeper.Domain.Aggregates;
using Minesweeper.Domain.Entities;
using Minesweeper.Domain.ValueObjects;
using System.Collections.Concurrent;
using System.Linq;

namespace Minesweeper.WebApi.Controllers;

[ApiController]
[Route("api/games")]
[Produces("application/json")]
public class CasinoGamesController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, CasinoGame> _games = new();

    [HttpPost]
    public ActionResult<GameDto> StartGame([FromBody] StartGameRequest request)
    {
        var gameId = Guid.NewGuid().ToString();
        var playerId = PlayerId.New();
        var difficulty = Minesweeper.Domain.ValueObjects.GameDifficulty.Intermediate;
        var game = new CasinoGame(Minesweeper.Domain.ValueObjects.GameId.From(Guid.Parse(gameId)), playerId, difficulty, new Random());
        _games[gameId] = game;
        return Ok(MapToDto(game, gameId));
    }

    [HttpGet("{id}")]
    public ActionResult<GameDto> GetGame(string id)
    {
        if (_games.TryGetValue(id, out var game))
            return Ok(MapToDto(game, id));
        return NotFound();
    }

    [HttpPost("{id}/reveal")]
    public ActionResult<GameDto> RevealCell(string id, [FromBody] RevealCellRequest request)
    {
        if (_games.TryGetValue(id, out var game))
        {
            game.Board.RevealCell(Minesweeper.Domain.ValueObjects.CellPosition.Of(request.Row, request.Col));
            return Ok(MapToDto(game, id));
        }
        return NotFound();
    }

    [HttpPost("{id}/spin")]
    public ActionResult<SlotOutcomeDto> SpinSlot(string id)
    {
        if (_games.TryGetValue(id, out var game))
        {
            var outcome = game.SpinSlot();
            return Ok(new SlotOutcomeDto
            {
                Description = outcome.Description,
                IsPositive = outcome.IsPositive,
                PointsAwarded = outcome.PointsAwarded,
                FlagsPlaced = outcome.FlagsPlaced,
                CellsRevealed = outcome.CellsRevealed,
                DollarsAwarded = outcome.DollarsAwarded,
                GameWon = outcome.GameWon,
                GameOver = outcome.GameOver,
                RoundRestart = outcome.RoundRestart
            });
        }
        return NotFound();
    }

    private GameDto MapToDto(CasinoGame game, string id)
    {
        return new GameDto
        {
            Id = id,
            PlayerName = "Player", // TODO: [FULL_IMPLEMENTATION] Use actual player name
            Credits = game.Credits,
            Dollars = game.Dollars,
            Level = game.Level,
            Board = new BoardDto
            {
                Rows = game.Board.Difficulty.Rows,
                Columns = game.Board.Difficulty.Columns,
                Cells = MapCells(game.Board)
            }
        };
    }
    private CellDto[,] MapCells(GameBoard board)
    {
        var cells = new CellDto[board.Difficulty.Rows, board.Difficulty.Columns];
        for (int r = 0; r < board.Difficulty.Rows; r++)
        {
            for (int c = 0; c < board.Difficulty.Columns; c++)
            {
                var cell = board.GetCell(Minesweeper.Domain.ValueObjects.CellPosition.Of(r, c));
                cells[r, c] = new CellDto
                {
                    Row = r,
                    Column = c,
                    IsRevealed = cell.IsRevealed,
                    IsFlagged = cell.IsFlagged,
                    HasMine = cell.HasMine,
                    AdjacentMineCount = cell.AdjacentMineCount
                };
            }
        }
        return cells;
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
        public CellDto[,] Cells { get; set; } = new CellDto[0, 0];
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
    public class StartGameRequest
    {
        public string PlayerName { get; set; } = "";
    }
    public class RevealCellRequest
    {
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
