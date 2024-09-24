import { sql } from '@vercel/postgres';
import dotenv from "dotenv";
import bcrypt from 'bcrypt';
dotenv.config();

const hashPassword = async (password) => {
    const saltRounds = 10;
    return await bcrypt.hash(password, saltRounds);
};

const clearDatabase = async () => {
    await sql`TRUNCATE TABLE player_games, log, games, players RESTART IDENTITY CASCADE`;
};

const seedPlayers = async () => {
    const hashedPassword1 = await hashPassword('password123');
    const hashedPassword2 = await hashPassword('password456');

    await sql`
    INSERT INTO players ("playerName", password, "ELO", "gamesPlayed", "gamesWon", "XP", level)
    VALUES
      (${'Player1'}, ${hashedPassword1}, 1200, 5, 3, 150, 2),
      (${'Player2'}, ${hashedPassword2}, 1100, 7, 2, 200, 3);
  `;
};

const seedGames = async () => {
    await sql`
    INSERT INTO games (created_at)
    VALUES
      (NOW()),
      (NOW());
  `;
};

const seedPlayerGames = async () => {
    await sql`
    INSERT INTO player_games ("playerId", "gameId", "ELOGained", "XPGained", position)
    VALUES
      (1, 1, 10, 50, 1),
      (2, 1, -5, 20, 2);
  `;
};

const seedLogs = async () => {
    await sql`
    INSERT INTO log ("gameId", log, created_at)
    VALUES
      (1, 'Player1 won the match with a full house.', NOW()),
      (2, 'Player2 folded early in the game.', NOW());
  `;
};

const seedDatabase = async () => {
    try {
        await clearDatabase();
        await seedPlayers();
        await seedGames();
        await seedPlayerGames();
        await seedLogs();
        console.log('Database seeded successfully!');
    } catch (err) {
        console.error('Error seeding the database', err);
    }
};

seedDatabase();