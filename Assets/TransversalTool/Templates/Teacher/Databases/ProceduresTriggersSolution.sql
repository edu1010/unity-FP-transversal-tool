CREATE TABLE IF NOT EXISTS match_audit (
  id INTEGER PRIMARY KEY AUTOINCREMENT,
  match_id INTEGER,
  old_score INTEGER,
  new_score INTEGER,
  changed_at TEXT
);

CREATE TRIGGER IF NOT EXISTS trg_matches_score_audit
AFTER UPDATE OF score ON matches
FOR EACH ROW
BEGIN
  INSERT INTO match_audit(match_id, old_score, new_score, changed_at)
  VALUES (OLD.id, OLD.score, NEW.score, datetime('now'));
END;
