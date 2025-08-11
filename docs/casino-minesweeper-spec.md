# Casino Minesweeper Game - Detailed Feature Specification

## 🎰 Game Overview

A hybrid **Minesweeper + Slot Machine** game. Players earn credits by revealing cells (points = adjacent mine count), then spend credits to spin a slot machine for positive or negative outcomes. Currency ($) is earned via slot spins and is the main leaderboard resource. No authentication; all data is local.

---

## 🗒️ Implementation TODO List

### 1. **Core Game Logic**
- [ ] **Cell Reveal**: Each cell awards points equal to its adjacent mine count.
- [ ] **Board Generation**: Randomized board size and mine count, fair distribution.
- [ ] **Level Progression**: After win, prompt for next randomized level.

### 2. **Slot Machine System**
- [ ] **Spin Cost**: 40 credits per spin.
- [ ] **Symbols**: Mine (💣), Flag (🚩), Number (1-5), Dollar ($).
- [ ] **Outcome Calculation**: 3 reels, outcome logic per symbol combination.
- [ ] **Positive Outcomes**:
    - [ ] Auto-flag 1-5 mines (not already flagged) [15%]
    - [ ] Auto-flag mines + bonus points [29%]
    - [ ] Win $ currency [10%]
    - [ ] Reveal 1-10 non-mine cells, award points [10%]
    - [ ] Reveal all cells, instant win [1%]
- [ ] **Negative Outcomes**:
    - [ ] Nothing happens, lose credits [30%]
    - [ ] Round restarts [4%]
    - [ ] Game over [1%]
- [ ] **Symbol Logic**:
    - 3 mines: Game over
    - 2 mines: Round restarts
    - 3 flags: Win round
    - 2 flags: Flag mines, award points
    - 1 flag: Flag mines, no points
    - Numbers: Dictate flags/cells revealed/points
    - 2 same numbers: Reveal sum of cells
    - $: Earn currency

### 3. **Currency System**
- [ ] **Credits**: Earned by cell reveals, spent on spins.
- [ ] **$ Currency**: Earned via slot, spent on spins, can be cashed out (1:0.9 loss).
- [ ] **Leaderboard**: Tracks $ currency, levels completed, credits earned, mines flagged, spins, positive/negative ratio.

### 4. **Leaderboard & User Management**
- [ ] **Local User Profiles**: Create/delete/select by name only.
- [ ] **Leaderboard Data**: Store locally (file/database), no authentication.
- [ ] **Leaderboard Stats**:
    - Levels completed
    - Credits earned
    - Mines flagged
    - Times spun
    - Positive/negative outcome ratio
    - $ currency (most important)

### 5. **UI/UX**
- [ ] **Minesweeper Board**: Classic look, no reset smiley, simple.
- [ ] **Slot Machine**: Flashy, Las Vegas style, 3 reels, animated.
- [ ] **Level Complete Popup**: Prompt for next level.
- [ ] **User Selection Screen**: Create/delete/select user by name.

---

## 🏗️ Classes & Fields Needed

### **Domain Entities**
- `Player`
    - `Name`, `Stats`, `Currency`, `Credits`
- `Game`
    - `Id`, `Player`, `Board`, `Status`, `Level`
- `GameBoard`
    - `Cells`, `Rows`, `Columns`, `MineCount`
- `Cell`
    - `Position`, `HasMine`, `IsRevealed`, `IsFlagged`, `AdjacentMineCount`
- `SlotMachine`
    - `Spin()`, `Reels`, `Symbols`, `OutcomeLogic`
- `SlotOutcome`
    - `Type`, `Description`, `Effect`, `Probability`
- `Leaderboard`
    - `Entries` (Player stats)

### **Supporting Types**
- `SymbolType` (Mine, Flag, Number, Dollar)
- `OutcomeType` (Positive, Negative)
- `PlayerStats` (Levels, Credits, MinesFlagged, Spins, OutcomeRatio, Currency)

### **UI Models**
- `BoardViewModel`
- `SlotMachineViewModel`
- `LeaderboardViewModel`
- `UserSelectionViewModel`

---

## ❓ What I Need From You

1. **Board Size Ranges**: What are the min/max rows/columns/mines for randomization?
2. **Slot Machine Visuals**: Any specific color scheme or assets?
3. **Currency/Credit Display**: How should credits and $ be shown in the UI?
4. **Leaderboard Storage**: File or local database? (No cloud)
5. **Any additional slot outcomes or features?**
6. **Should the slot machine be available at all times or only after certain progress?**
7. **Any restrictions on level progression or difficulty scaling?**

---

## 📝 Next Steps

- Confirm above details.
- Design domain models and slot logic.
- Implement board/slot/leaderboard systems.
- Build simple UI for board, flashy UI for slot.
- Add local user management and leaderboard storage.

---

**Please clarify the above questions to proceed with code and architecture design.**