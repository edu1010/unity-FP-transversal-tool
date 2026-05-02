CREATE TABLE players (
  id TEXT PRIMARY KEY,
  player_name TEXT NOT NULL
);

CREATE TABLE levels (
  id INTEGER PRIMARY KEY,
  level_name TEXT NOT NULL
);

CREATE TABLE matches (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  player_id TEXT NOT NULL,
  level_id INTEGER NOT NULL,
  score INTEGER NOT NULL,
  played_at TEXT NOT NULL,
  FOREIGN KEY(player_id) REFERENCES players(id),
  FOREIGN KEY(level_id) REFERENCES levels(id)
);
