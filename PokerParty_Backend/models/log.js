import mongoose from "mongoose";

const LogSchema = new mongoose.Schema({
    gameId: { type: mongoose.Schema.Types.ObjectId, ref: 'Game' },
    log: { type: String, required: true },
    created_at: { type: Date, default: Date.now },
}, { versionKey: false });

export const Log = mongoose.model('Log', LogSchema);