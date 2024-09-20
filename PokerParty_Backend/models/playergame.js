import mongoose from "mongoose";

const PlayerGameSchema = new mongoose.Schema({
    playerId: { type: mongoose.Schema.Types.ObjectId, ref: 'Player' },
    gameId: { type: mongoose.Schema.Types.ObjectId, ref: 'Game' },
    ELOGained: { type: Number, default: 0 },
    XPGained: { type: Number, default: 0 },
    position: { type: Number, default: 0 },
    created_at: { type: Date, default: Date.now },
}, { versionKey: false });

export const PlayerGame = mongoose.model('PlayerGame', PlayerGameSchema);