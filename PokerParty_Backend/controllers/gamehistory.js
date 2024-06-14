import { Game } from "../models/game.js";

export const addNewGame = async (req, res) => {
    const { playerName, ELOGained, XPGained, Position } = req.body;

    try {
        let game = new Game({
            playerName,
            ELOGained,
            XPGained,
            Position
        });

        await game.save();
        res.status(201).json({ msg: 'Game added successfully' });
    } catch (err) {
        res.status(500).json({ msg: err.message });
    }

}