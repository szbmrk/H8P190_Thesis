import express from "express";
import { login, register } from "../controllers/authentication.js";
import { createGame } from "../controllers/game.js";
import { createLog } from "../controllers/log.js";

const router = express.Router()

router.post('/register', register)
router.post('/login', login)
router.post('/game', createGame)
router.post('/log/:gameId', createLog)

export default router