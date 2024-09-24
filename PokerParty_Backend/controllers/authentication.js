import { sql } from '@vercel/postgres';
import bcrypt from "bcrypt";

export const register = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        const existingPlayer = await sql`
            SELECT * FROM players WHERE "playerName" = ${playerName}
        `;

        if (existingPlayer.rows.length > 0) {
            return res.status(400).json({ msg: 'Player already exists' });
        }

        const saltRounds = 10;
        const hashedPassword = await bcrypt.hash(password, saltRounds);

        await sql`
            INSERT INTO players ("playerName", "password")
            VALUES (${playerName}, ${hashedPassword})
        `;

        res.status(201).json({ msg: 'Player registered successfully' });

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};

export const login = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        const result = await sql`
            SELECT * FROM players WHERE "playerName" = ${playerName}
        `;

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
