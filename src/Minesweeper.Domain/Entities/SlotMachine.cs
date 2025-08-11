using System;
using System.Collections.Generic;

namespace Minesweeper.Domain.Entities
{
    /// <summary>
    /// Represents the slot machine for casino minesweeper.
    /// </summary>
    public class SlotMachine
    {
        public const int SpinCost = 40;
        private readonly Random _random;
        private static readonly List<SlotSymbol> Symbols = new()
        {
            SlotSymbol.Mine,
            SlotSymbol.Flag,
            SlotSymbol.Dollar,
            SlotSymbol.Number1,
            SlotSymbol.Number2,
            SlotSymbol.Number3,
            SlotSymbol.Number4,
            SlotSymbol.Number5
        };

        public SlotMachine(Random random)
        {
            _random = random;
        }

        public SlotOutcome Spin()
        {
            // Simulate 3 reels
            var reels = new List<SlotSymbol>
            {
                GetRandomSymbol(),
                GetRandomSymbol(),
                GetRandomSymbol()
            };
            return SlotOutcome.DetermineOutcome(reels);
        }

        private SlotSymbol GetRandomSymbol()
        {
            // TODO: [FULL_IMPLEMENTATION] Use weighted probabilities for each symbol
            // Current: Uniform random selection
            return Symbols[_random.Next(Symbols.Count)];
        }
    }

    public enum SlotSymbol
    {
        Mine,
        Flag,
        Dollar,
        Number1,
        Number2,
        Number3,
        Number4,
        Number5
    }

    public class SlotOutcome
    {
        public List<SlotSymbol> Symbols { get; set; } = new();
        public string Description { get; set; } = "";
        public bool IsPositive { get; set; }
        public int PointsAwarded { get; set; }
        public int FlagsPlaced { get; set; }
        public int CellsRevealed { get; set; }
        public int DollarsAwarded { get; set; }
        public bool GameWon { get; set; }
        public bool GameOver { get; set; }
        public bool RoundRestart { get; set; }

        public static SlotOutcome DetermineOutcome(List<SlotSymbol> symbols)
        {
            var outcome = new SlotOutcome { Symbols = symbols };
            // TODO: [FULL_IMPLEMENTATION] Implement full outcome logic based on casino-minesweeper spec
            // Example: 3 mines = game over
            if (symbols.TrueForAll(s => s == SlotSymbol.Mine))
            {
                outcome.Description = "Game Over! All reels are mines.";
                outcome.GameOver = true;
                outcome.IsPositive = false;
            }
            // TODO: Add all other outcome rules from spec
            return outcome;
        }
    }
}
