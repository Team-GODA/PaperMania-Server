-- 부하 테스트용 최소 스키마 (Account/DataRepository SQL 기준, 로그인 + 플레이어 데이터 조회에 필요한 테이블만)
CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

CREATE SCHEMA paper_mania_account_data;
CREATE SCHEMA paper_mania_game_data;

CREATE TABLE paper_mania_account_data.player_account_data (
    id             SERIAL PRIMARY KEY,
    player_id      TEXT NOT NULL UNIQUE,
    email          TEXT NOT NULL UNIQUE,
    password       TEXT NOT NULL,
    is_new_account BOOLEAN NOT NULL DEFAULT TRUE,
    role           TEXT NOT NULL DEFAULT 'user',
    created_at     TIMESTAMP NOT NULL DEFAULT NOW()
);

CREATE TABLE paper_mania_game_data.player_game_data (
    user_id      INT PRIMARY KEY REFERENCES paper_mania_account_data.player_account_data (id),
    player_name  TEXT NOT NULL UNIQUE,
    player_exp   INT NOT NULL DEFAULT 0,
    player_level INT NOT NULL DEFAULT 1
);

-- 테스트 계정 (run.ps1의 LOADTEST_PLAYER_ID / LOADTEST_PASSWORD와 일치)
INSERT INTO paper_mania_account_data.player_account_data (player_id, email, password, is_new_account)
VALUES ('loadtest', 'loadtest@local.test', crypt('loadtest1234', gen_salt('bf', 10)), FALSE);

INSERT INTO paper_mania_game_data.player_game_data (user_id, player_name)
VALUES (1, 'LoadTester');
