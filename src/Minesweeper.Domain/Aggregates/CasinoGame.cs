using Minesweeper.Domain.Common;
using Minesweeper.Domain.Entities;
using Minesweeper.Domain.ValueObjects;
using System;

namespace Minesweeper.Domain.Aggregates
{
    /// <summary>
    /// Casino Minesweeper game aggregate with slot machine and currency logic
    /// </summary>
    public class CasinoGame : Entity<GameId>
    {
        public PlayerId PlayerId { get; private set; }
        public GameBoard Board { get; private set; }
        public SlotMachine SlotMachine { get; private set; }
        public int Credits { get; private set; }
        public int Dollars { get; private set; }
        public int Level { get; private set; }
        public int MinesFlagged { get; private set; }
        public int TimesSpun { get; private set; }
        public int PositiveOutcomeCount { get; private set; }
        public int NegativeOutcomeCount { get; private set; }
        public int LevelsCompleted { get; private set; }
        public CasinoGame(GameId gameId, PlayerId playerId, GameDifficulty difficulty, Random? random = null)
        {
            Id = gameId;
            PlayerId = playerId;
            Board = new GameBoard(difficulty, random);
            SlotMachine = new SlotMachine(random ?? new Random());
            Credits = 0;
            Dollars = 0;
            Level = 1;
        }
        public void AwardCredits(int points)
        {
            Credits += points;
        }
        public bool CanSpin() => Credits >= SlotMachine.SpinCost;
        public SlotOutcome SpinSlot()
        {
            if (!CanSpin()) throw new InvalidOperationException("Not enough credits to spin.");
            Credits -= SlotMachine.SpinCost;
            TimesSpun++;
            var outcome = SlotMachine.Spin();
            if (outcome.IsPositive) PositiveOutcomeCount++;
            else NegativeOutcomeCount++;
            // TODO: Apply outcome effects to game state (flag mines, award points/dollars, reveal cells, win/lose, etc.)
            if (outcome.DollarsAwarded > 0) Dollars += outcome.DollarsAwarded;
            if (outcome.FlagsPlaced > 0) MinesFlagged += outcome.FlagsPlaced;
            if (outcome.GameWon) LevelsCompleted++;
            return outcome;
        }
        // TODO: Implement level progression, board scaling, and leaderboard integration
    }
}
