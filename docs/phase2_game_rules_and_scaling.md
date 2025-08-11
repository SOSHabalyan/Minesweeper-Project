# Phase 2: Game Rules, Board Scaling, and Slot Machine Integration

## 🎮 Board Dimension & Mine Count Scaling

- **Minimum Board Size**: Use the original "Intermediate" (16x16, 40 mines) as the minimum.
- **Maximum Board Size**: Use the original "Expert" (16x30, 99 mines) as the maximum.
- **Mine Percentage Calculation**:
  - Calculate the mine percentage for the largest map:  
    - Expert: 16x30 = 480 cells, 99 mines → ~20.6%
    - Add 5%: 25.6% mine density for scaling.
- **Scaling Logic**:
  - For any new level, randomly select board dimensions between Intermediate and Expert.
  - Calculate mine count as:  
    `MineCount = (Rows * Columns) * 0.256` (rounded down)
  - Always ensure at least one safe cell.

## 🎰 Slot Machine Rules

- **Availability**: Slot machine is available from the start of every game.
- **Visuals**: No specifics required; just make it flashy and casino-like.
- **Credits & Dollars**:
  - "Credits" are earned by revealing cells (equal to adjacent mine count).
  - "Dollars" (currency) are earned via slot machine outcomes.
  - Display both credits and dollars below the slot machine screen in the UI.
- **Slot Machine Features**:
  - No additional features for now.
  - No claiming required; just spin to win.

## 🏆 Leaderboard

- **Storage**: Simple local JSON file.
- **Tracked Stats**:
  - Levels completed
  - Credits earned
  - Mines flagged
  - Times spun
  - Positive/negative outcome ratio
  - Dollars accumulated or present before loss (main ranking metric)

## 📝 Implementation Notes

- **Board Generation**: When a new level is created, randomly select dimensions between 16x16 and 16x30, then scale mine count as above.
- **Slot Machine**: Always enabled, can be spun at any time.
- **UI**: Credits and dollars shown below slot machine; board is classic Minesweeper style.
- **Leaderboard**: Read/write to local JSON file, no authentication.

## ✅ Next Steps

- Implement board scaling logic in game creation.
- Integrate slot machine UI and logic with credits/dollars display.
- Implement leaderboard persistence using local JSON.
- Ensure slot machine is available from game start.

---
**This document summarizes the updated game rules, scaling logic, and slot machine integration for Phase 2.**