using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Minesweeper.Domain.Entities
{
    /// <summary>
    /// Represents a leaderboard entry for a player.
    /// </summary>
    public class LeaderboardEntry
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

    /// <summary>
    /// Simple local leaderboard using JSON file storage.
    /// </summary>
    public class Leaderboard
    {
        private const string LeaderboardFile = "leaderboard.json";
        public List<LeaderboardEntry> Entries { get; set; } = new();

        public void Load()
        {
            if (File.Exists(LeaderboardFile))
            {
                var json = File.ReadAllText(LeaderboardFile);
                Entries = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json) ?? new List<LeaderboardEntry>();
            }
        }

        public void Save()
        {
            var json = JsonSerializer.Serialize(Entries, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(LeaderboardFile, json);
        }

        public void AddOrUpdateEntry(LeaderboardEntry entry)
        {
            var existing = Entries.Find(e => e.PlayerName == entry.PlayerName);
            if (existing != null)
            {
                // Update stats
                existing.LevelsCompleted = entry.LevelsCompleted;
                existing.CreditsEarned = entry.CreditsEarned;
                existing.MinesFlagged = entry.MinesFlagged;
                existing.TimesSpun = entry.TimesSpun;
                existing.PositiveOutcomeCount = entry.PositiveOutcomeCount;
                existing.NegativeOutcomeCount = entry.NegativeOutcomeCount;
                existing.DollarsAccumulated = entry.DollarsAccumulated;
            }
            else
            {
                Entries.Add(entry);
            }
            Save();
        }
    }
}
