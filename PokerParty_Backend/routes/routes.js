import express from "express";
import { login, register } from "../controllers/authentication.js";
import { createGame } from "../controllers/game.js";
import { createLog } from "../controllers/log.js";
import { validateRequest } from "../middleware/validateRequest.js";

const router = express.Router()

router.post('/register', validateRequest([
    { field: 'playerName', type: 'string', required: true },
    { field: 'password', type: 'string', required: true }
]), register)
router.post('/login', validateRequest([
    { field: 'playerName', type: 'string', required: true },
    { field: 'password', type: 'string', required: true }
]), login)
router.post('/game', createGame)
router.post('/log/:gameId', validateRequest([
    { field: 'log', type: 'string', required: true }
]), createLog)

export default router