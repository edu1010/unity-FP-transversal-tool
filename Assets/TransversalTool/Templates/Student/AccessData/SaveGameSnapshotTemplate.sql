-- Crea la taula per emmagatzemar snapshots JSON de partida.
-- TODO: Completa claus i restriccions bàsiques.

CREATE TABLE IF NOT EXISTS savegame_snapshot (
  id INTEGER PRIMARY KEY,
  snapshot_json TEXT NOT NULL,
  created_at TEXT NOT NULL
);
