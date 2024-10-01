import { sql } from '@vercel/postgres';

export const createGame = async (req, res) => {
    try {
        sql`INSERT INTO games DEFAULT VALUES`;
        res.status(201).json({ msg: 'Game created successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};