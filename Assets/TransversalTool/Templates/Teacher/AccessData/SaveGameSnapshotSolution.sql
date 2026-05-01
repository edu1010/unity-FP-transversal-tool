CREATE TABLE IF NOT EXISTS savegame_snapshot (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  snapshot_json TEXT NOT NULL,
  created_at TEXT NOT NULL
);
