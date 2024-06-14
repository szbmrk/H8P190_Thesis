import { Player } from "../models/player.js";
import bcrypt from "bcrypt";

export const register = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        let player = await Player.findOne({ playerName });
        if (user) {
            return res.status(400).json({ msg: 'Player already exists' });
        }

        player = new Player({
            playerName,
            password
        });

        await player.save();
        res.status(201).json({ msg: 'Player registered successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
}

export const login = async (req, res) => {
    const { playerName, password } = req.body;

    try {
        let player = await User.findOne({ playerName });
        if (!player) {
            return res.status(400).json({ msg: 'Invalid playerName', player: null });
        }

        const isMatch = await bcrypt.compare(password, player.password);
        if (!isMatch) {
            return res.status(400).json({ msg: 'Invalid credentials', player: null });
        }

        res.status(200).json({ msg: 'Player logged in successfully', player: player });

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};