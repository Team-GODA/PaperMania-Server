-- 레벨업 비교용 시드: 레벨 정의 60,000개(모두 max_exp 100) + 계정 loadtest1..50(요청자마다 별도 계정, 행 잠금 경쟁 방지)
CREATE TABLE IF NOT EXISTS paper_mania_game_data.level_definition (
    level            INT PRIMARY KEY,
    max_exp          INT NOT NULL,
    max_action_point INT NOT NULL
);
CREATE TABLE IF NOT EXISTS paper_mania_game_data.player_currency_data (
    user_id                   INT PRIMARY KEY REFERENCES paper_mania_account_data.player_account_data (id),
    action_point              INT NOT NULL DEFAULT 100,
    action_point_max          INT NOT NULL DEFAULT 100,
    gold                      INT NOT NULL DEFAULT 0,
    paper_piece               INT NOT NULL DEFAULT 0,
    last_action_point_updated TIMESTAMP NOT NULL DEFAULT NOW()
);

TRUNCATE paper_mania_game_data.level_definition;
INSERT INTO paper_mania_game_data.level_definition
SELECT g, 100, 100 FROM generate_series(1, 60000) g;

INSERT INTO paper_mania_account_data.player_account_data (player_id, email, password, is_new_account)
SELECT 'loadtest' || g, 'loadtest' || g || '@local.test', crypt('loadtest1234', gen_salt('bf', 10)), FALSE
FROM generate_series(1, 50) g
ON CONFLICT (player_id) DO NOTHING;

INSERT INTO paper_mania_game_data.player_game_data (user_id, player_name)
SELECT id, 'LoadTester' || substr(player_id, 9) FROM paper_mania_account_data.player_account_data
WHERE player_id ~ '^loadtest[0-9]+$'
ON CONFLICT (user_id) DO NOTHING;

INSERT INTO paper_mania_game_data.player_currency_data (user_id)
SELECT user_id FROM paper_mania_game_data.player_game_data
ON CONFLICT (user_id) DO NOTHING;
