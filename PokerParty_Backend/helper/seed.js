import { db } from "../database/db.js";
import dotenv from "dotenv";
import bcrypt from 'bcrypt';
dotenv.config();

const hashPassword = async (password) => {
    const saltRounds = 10;
    return await bcrypt.hash(password, saltRounds);
};

const checkIfAlreadySeeded = async () => {
    const playerQuery = 'SELECT * FROM players WHERE "playerName" = $1';
    const player = await db.query(playerQuery, ['Player1']);
    return player.rows.length > 0;
};

const seedPlayers = async () => {
    const hashedPassword1 = await hashPassword('password123');
    const hashedPassword2 = await hashPassword('password456');

    const createPlayersQuery = 'INSERT INTO players ("_id", "playerName", email, password, "ELO", "gamesPlayed", "gamesWon", "XP", level) VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9)';
    await db.query(createPlayersQuery, [1, 'Player1', 'player1@gmail.com', hashedPassword1, 1000, 10, 5, 100, 1]);
    await db.query(createPlayersQuery, [2, 'Player2', 'player1@gmail.com', hashedPassword2, 900, 8, 3, 80, 1]);
};

const seedGames = async () => {
    await db.query('INSERT INTO games ("_id", created_at) VALUES (1, NOW())');
    await db.query('INSERT INTO games ("_id", created_at) VALUES (2, NOW())');
};

const seedPlayerGames = async () => {
    const createPlayerGamesQuery = 'INSERT INTO player_games ("playerId", "gameId", "ELOGained", "XPGained", "position") VALUES ($1, $2, $3, $4, $5)';
    await db.query(createPlayerGamesQuery, [1, 1, 10, 10, 1]);
    await db.query(createPlayerGamesQuery, [2, 1, -10, 5, 2]);
    await db.query(createPlayerGamesQuery, [1, 2, 10, 10, 2]);
    await db.query(createPlayerGamesQuery, [2, 2, -10, 5, 1]);
};

const seedLogs = async () => {
    const logQuery = 'INSERT INTO logs ("gameId", log, created_at) VALUES ($1, $2, NOW())';
    await db.query(logQuery, [1, 'Player1 wins!']);
    await db.query(logQuery, [2, 'Player2 wins!']);
};

export const seedDatabase = async () => {
    try {
        const alreadySeeded = await checkIfAlreadySeeded();
        if (alreadySeeded) {
            console.log('Database already seeded');
            return;
        }

        await seedPlayers();
        await seedGames();
        await seedPlayerGames();
        await seedLogs();
        console.log('Database seeded successfully!');
    } catch (err) {
        console.error('Error seeding the database', err);
    }
};