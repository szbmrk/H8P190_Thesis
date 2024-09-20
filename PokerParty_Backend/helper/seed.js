import { Player } from "../models/player.js";
import { Game } from "../models/game.js";
import { Log } from "../models/log.js";
import { PlayerGame } from "../models/playergame.js";

export async function seedData() {
    const players = await Player.create([
        { playerName: 'Alice', password: 'password123', ELO: 1200, gamesPlayed: 10, gamesWon: 6, XP: 500, level: 3 },
        { playerName: 'Bob', password: 'securepass', ELO: 1100, gamesPlayed: 8, gamesWon: 3, XP: 400, level: 2 },
        { playerName: 'Charlie', password: 'chessMaster', ELO: 1300, gamesPlayed: 15, gamesWon: 10, XP: 750, level: 4 }
    ]);

    const games = await Game.create([
        {},
        {},
        {}
    ]);

    await Log.create([
        { gameId: games[0]._id, log: 'Game started' },
        { gameId: games[1]._id, log: 'Game started' },
        { gameId: games[2]._id, log: 'Game started' }
    ]);

    await PlayerGame.create([
        { playerId: players[0]._id, gameId: games[0]._id, ELOGained: 15, XPGained: 50, position: 1 },
        { playerId: players[1]._id, gameId: games[0]._id, ELOGained: -15, XPGained: 30, position: 2 },
        { playerId: players[2]._id, gameId: games[1]._id, ELOGained: 10, XPGained: 40, position: 1 },
        { playerId: players[0]._id, gameId: games[1]._id, ELOGained: -10, XPGained: 25, position: 2 },
        { playerId: players[1]._id, gameId: games[2]._id, ELOGained: 5, XPGained: 35, position: 1 },
        { playerId: players[2]._id, gameId: games[2]._id, ELOGained: -5, XPGained: 20, position: 2 }
    ]);

    console.log('Seed data inserted successfully');
}