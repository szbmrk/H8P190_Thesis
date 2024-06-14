import mongoose from "mongoose";
import { User } from "./user";

const GameSchema = new mongoose.Schema({
    player: { type: User, required: true },
    ELOGained: { type: Number, required: true },
    XPGained: { type: Number, required: true },
    Position: { type: Number, required: true },
    created_at: { type: Date, default: Date.now },
}, { versionKey: false });

export const Game = mongoose.model('Game', GameSchema);