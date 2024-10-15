import { db } from "../database/db.js";


export const createGame = async (req, res) => {
    try {
        db.query('INSERT INTO games DEFAULT VALUES');
        res.status(201).json({ msg: 'Game created successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).json({ msg: 'Server error' });
    }
};