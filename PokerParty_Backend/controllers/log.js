import { sql } from '@vercel/postgres';

export const createLog = async (req, res) => {
    try {
        const gameId = req.params.gameId;
        const { log } = req.body;
        sql`INSERT INTO logs ("gameId", "log") VALUES (${gameId}, ${log})`;
        res.status(201).json({ msg: 'Game created successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};