import mongoose from "mongoose";

const GameSchema = new mongoose.Schema({
    playerName: { type: String, required: true },
    ELOGained: { type: Number, required: true },
    XPGained: { type: Number, required: true },
    position: { type: Number, required: true },
    created_at: { type: Date, default: Date.now },
}, { versionKey: false });

export const Game = mongoose.model('Game', GameSchema);