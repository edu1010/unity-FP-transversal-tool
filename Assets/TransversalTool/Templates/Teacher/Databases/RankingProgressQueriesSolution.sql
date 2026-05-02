SELECT player_id, MAX(score) AS best_score
FROM matches
GROUP BY player_id
ORDER BY best_score DESC
LIMIT 10;

SELECT level_id, AVG(score) AS avg_score
FROM matches
GROUP BY level_id
ORDER BY level_id ASC;
