import { db } from "../database/db.js";

export const createLog = async (req, res) => {
    try {
        const gameId = req.params.gameId;
        const { log } = req.body;
        const logQuery = 'INSERT INTO logs ("gameId", "log") VALUES ($1, $2)';
        db.query(logQuery, [gameId, log]);
        res.status(201).json({ msg: 'Game created successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};