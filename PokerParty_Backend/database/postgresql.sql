CREATE TABLE PLAYERS (
    "_id" SERIAL PRIMARY KEY,
    "playerName" TEXT UNIQUE NOT NULL,
    "password" TEXT NOT NULL,
    "ELO" INTEGER DEFAULT 1000,
    "gamesPlayed" INTEGER DEFAULT 0,
    "gamesWon" INTEGER DEFAULT 0,
    "XP" INTEGER DEFAULT 0,
    "level" INTEGER DEFAULT 1,
    "created_at" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE GAMES (
    "_id" SERIAL PRIMARY KEY,
    "created_at" TIMESTAMP DEFAULT NOW()
);

CREATE TABLE PLAYER_GAMES (
    "_id" SERIAL PRIMARY KEY,
    "playerId" INTEGER REFERENCES PLAYERS("_id"),
    "gameId" INTEGER REFERENCES GAMES("_id"),
    "ELOGained" INTEGER,
    "XPGained" INTEGER,
    "position" INTEGER CHECK ("position" >= 1)
);

CREATE TABLE LOG (
    "_id" SERIAL PRIMARY KEY,
    "gameId" INTEGER REFERENCES GAMES("_id"),
    "log" TEXT,
    "created_at" TIMESTAMP DEFAULT NOW()
);