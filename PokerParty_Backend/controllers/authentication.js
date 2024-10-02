import { db } from "../database/db.js";
import bcrypt from "bcrypt";

export const register = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        const existingPlayerQuery = 'SELECT * FROM players WHERE "playerName" = $1';
        const existingPlayer = await db.query(existingPlayerQuery, [playerName]);

        if (existingPlayer.rows.length > 0) {
            return res.status(400).json({ msg: 'Player already exists' });
        }

        const saltRounds = 10;
        const hashedPassword = await bcrypt.hash(password, saltRounds);

        const insertQuery = 'INSERT INTO players ("playerName", "password") VALUES ($1, $2)';
        await db.query(insertQuery, [playerName, hashedPassword]);

        res.status(201).json({ msg: 'Player registered successfully' });

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};

export const login = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        const loginQuery = 'SELECT * FROM players WHERE "playerName" = $1';
        const result = await db.query(loginQuery, [playerName]);

        if (result.rows.length === 0) {
            return res.status(400).json({ msg: 'Invalid playerName', player: null });
        }

        const player = result.rows[0];

        const isMatch = await bcrypt.compare(password, player.password);

        if (!isMatch) {
            return res.status(400).json({ msg: 'Invalid credentials', player: null });
        }

        res.status(200).json({ msg: 'Player logged in successfully', player });

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};
