import { db } from "../database/db.js";
import bcrypt from "bcrypt";
import { hashPassword } from "../helper/passwordHash.js";

export const register = async (req, res) => {
    const { playerName, email, password } = req.body;

    try {
        const existingPlayerNameQuery = 'SELECT * FROM players WHERE "playerName" = $1';
        const existingPlayer = await db.query(existingPlayerNameQuery, [playerName]);

        if (existingPlayer.rows.length > 0) {
            return res.status(400).json({ msg: `Player with ${playerName} playerName already exists` });
        }

        const existingPlayerEmailQuery = 'SELECT * FROM players WHERE email = $1';
        const existingEmail = await db.query(existingPlayerEmailQuery, [email]);

        if (existingEmail.rows.length > 0) {
            return res.status(400).json({ msg: `Player with ${email} email already exists` });
        }

        const hashedPassword = await hashPassword(password);

        const insertQuery = 'INSERT INTO players ("playerName", "email", "password") VALUES ($1, $2, $3)';
        await db.query(insertQuery, [playerName, email, hashedPassword]);

        res.status(201).json({ msg: 'Player registered successfully' });

    } catch (err) {
        console.error(err.message);
        res.status(500).json({ msg: 'Server error' });
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

        const safePlayer = {
            ...player,
            password: undefined
        };

        res.status(200).json({ msg: 'Player logged in successfully', safePlayer });

    } catch (err) {
        console.error(err.message);
        res.status(500).json({ msg: 'Server error' });
    }
};
