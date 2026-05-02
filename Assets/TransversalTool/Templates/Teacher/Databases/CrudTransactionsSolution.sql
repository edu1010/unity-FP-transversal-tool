BEGIN TRANSACTION;

INSERT INTO matches(player_id, level_id, score, played_at)
VALUES ('p001', 3, 1250, datetime('now'));

UPDATE matches
SET score = 1320
WHERE player_id = 'p001' AND level_id = 3;

DELETE FROM matches
WHERE player_id = 'p001' AND score < 0;

COMMIT;
