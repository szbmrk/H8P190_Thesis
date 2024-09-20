import mongoose from "mongoose";

const GameSchema = new mongoose.Schema({
    created_at: { type: Date, default: Date.now },
}, { versionKey: false });

export const Game = mongoose.model('Game', GameSchema);