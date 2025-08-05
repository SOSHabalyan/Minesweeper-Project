using Microsoft.AspNetCore.Mvc;
using Minesweeper.Domain.Aggregates;
using Minesweeper.Domain.ValueObjects;
using System.Collections.Concurrent;

namespace Minesweeper.WebApi.Controllers;

/// <summary>
/// Game management endpoints for Minesweeper
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GamesController : ControllerBase
{
    private readonly ILogger<GamesController> _logger;

    // In-memory storage for demo purposes
    private static readonly ConcurrentDictionary<Guid, Game> _games = new();

    public GamesController(ILogger<GamesController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Creates a new minesweeper game
    /// </summary>
    /// <param name="request">Game creation parameters</param>
    /// <returns>Created game information</returns>
    [HttpPost]
    [ProducesResponseType<GameResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<GameResponse> CreateGame([FromBody] CreateGameRequest request)
    {
        try
        {
            var gameId = GameId.New();
            var playerId = PlayerId.New(); // In real app, get from authenticated user
            var game = new Game(gameId, playerId, request.Difficulty, new Random());

            _games[gameId.Value] = game;

            var response = MapToResponse(game);
            return CreatedAtAction(nameof(GetGame), new { id = gameId.Value }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating game");
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets a specific game by ID
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <returns>Game information</returns>
    [HttpGet("{id}")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameResponse> GetGame(Guid id)
    {
        try
        {
            if (!_games.TryGetValue(id, out var game))
            {
                return NotFound(new { error = "Game not found" });
            }

            var response = MapToResponse(game);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting game {GameId}", id);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    /// <summary>
    /// Reveals a cell in the game
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <param name="request">Cell reveal parameters</param>
    /// <returns>Updated game state</returns>
    [HttpPost("{id}/reveal")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameResponse> RevealCell(Guid id, [FromBody] RevealCellRequest request)
    {
        try
        {
            if (!_games.TryGetValue(id, out var game))
            {
                return NotFound(new { error = "Game not found" });
            }

            var position = CellPosition.Of(request.Row, request.Column);
            var result = game.RevealCell(position);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            var response = MapToResponse(game);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revealing cell in game {GameId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Toggles a flag on a cell
    /// </summary>
    /// <param name="id">Game ID</param>
    /// <param name="request">Cell flag parameters</param>
    /// <returns>Updated game state</returns>
    [HttpPost("{id}/flag")]
    [ProducesResponseType<GameResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<GameResponse> ToggleFlag(Guid id, [FromBody] FlagCellRequest request)
    {
        try
        {
            if (!_games.TryGetValue(id, out var game))
            {
                return NotFound(new { error = "Game not found" });
            }

            var position = CellPosition.Of(request.Row, request.Column);
            var result = game.ToggleFlag(position);

            if (result.IsFailure)
            {
                return BadRequest(new { error = result.Error });
            }

            var response = MapToResponse(game);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error flagging cell in game {GameId}", id);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Gets available game difficulties
    /// </summary>
    /// <returns>List of available difficulties</returns>
    [HttpGet("difficulties")]
    [ProducesResponseType<IEnumerable<object>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<object>> GetDifficulties()
    {
        var difficulties = GameDifficulty.GetPredefinedDifficulties()
            .Select(d => new
            {
                d.Name,
                d.Rows,
                d.Columns,
                d.MineCount,
                d.TotalCells,
                d.SafeCells,
                MineDensity = Math.Round(d.MineDensity, 1)
            });

        return Ok(difficulties);
    }

    /// <summary>
    /// Gets all active games (for demo purposes)
    /// </summary>
    /// <returns>List of all games</returns>
    [HttpGet]
    [ProducesResponseType<IEnumerable<GameSummary>>(StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<GameSummary>> GetAllGames()
    {
        var games = _games.Values
            .Select(g => new GameSummary
            {
                Id = g.Id.Value,
                Status = g.Status.ToString(),
                Difficulty = g.Board.Difficulty.Name,
                RemainingMines = g.GetRemainingMineCount(),
                IsActive = g.IsActive
            });

        return Ok(games);
    }

    private static GameResponse MapToResponse(Game game)
    {
        var cells = new List<CellResponse>();

        for (int row = 0; row < game.Board.Difficulty.Rows; row++)
        {
            for (int col = 0; col < game.Board.Difficulty.Columns; col++)
            {
                var position = CellPosition.Of(row, col);
                var cell = game.Board.GetCell(position);

                cells.Add(new CellResponse
                {
                    Row = row,
                    Column = col,
                    IsRevealed = cell.IsRevealed,
                    IsFlagged = cell.IsFlagged,
                    HasMine = cell.IsRevealed && cell.HasMine, // Only show mine if revealed
                    AdjacentMineCount = cell.IsRevealed ? cell.AdjacentMineCount : null
                });
            }
        }

        return new GameResponse
        {
            Id = game.Id.Value,
            Status = game.Status.ToString(),
            Difficulty = new DifficultyResponse
            {
                Name = game.Board.Difficulty.Name,
                Rows = game.Board.Difficulty.Rows,
                Columns = game.Board.Difficulty.Columns,
                MineCount = game.Board.Difficulty.MineCount
            },
            RemainingMines = game.GetRemainingMineCount(),
            IsActive = game.IsActive,
            Cells = cells
        };
    }
}

// Request/Response Models

/// <summary>
/// Request model for creating a new game
/// </summary>
public record CreateGameRequest(GameDifficulty Difficulty);

/// <summary>
/// Request model for revealing a cell
/// </summary>
public record RevealCellRequest(int Row, int Column);

/// <summary>
/// Request model for flagging a cell
/// </summary>
public record FlagCellRequest(int Row, int Column);

/// <summary>
/// Response model for game information
/// </summary>
public record GameResponse
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public DifficultyResponse Difficulty { get; init; } = new();
    public int RemainingMines { get; init; }
    public bool IsActive { get; init; }
    public List<CellResponse> Cells { get; init; } = new();
}

/// <summary>
/// Response model for game difficulty
/// </summary>
public record DifficultyResponse
{
    public string Name { get; init; } = string.Empty;
    public int Rows { get; init; }
    public int Columns { get; init; }
    public int MineCount { get; init; }
}

/// <summary>
/// Response model for individual cells
/// </summary>
public record CellResponse
{
    public int Row { get; init; }
    public int Column { get; init; }
    public bool IsRevealed { get; init; }
    public bool IsFlagged { get; init; }
    public bool HasMine { get; init; }
    public int? AdjacentMineCount { get; init; }
}

/// <summary>
/// Summary model for game list
/// </summary>
public record GameSummary
{
    public Guid Id { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Difficulty { get; init; } = string.Empty;
    public int RemainingMines { get; init; }
    public bool IsActive { get; init; }
}
