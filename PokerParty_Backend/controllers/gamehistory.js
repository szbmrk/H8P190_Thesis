import { Game } from "../models/game.js";

export const addNewGame = async (req, res) => {
    const { player, ELOGained, XPGained, Position } = req.body;

    try {
        let game = new Game({
            player,
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