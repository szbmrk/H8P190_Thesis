import express from "express";
import { login, register } from "../controllers/authentication.js";
import { createGame } from "../controllers/game.js";

const router = express.Router()

router.post('/register', register)
router.post('/login', login)
router.post('/game', createGame)

export default router