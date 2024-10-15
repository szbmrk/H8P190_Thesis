import express from "express";
import { login, register } from "../controllers/authentication.js";
import { createGame } from "../controllers/game.js";
import { createLog } from "../controllers/log.js";
import { validateRequest } from "../middleware/validateRequest.js";
import { ChangePassword, sendPasswordResetEmail } from "../controllers/passwordReset.js";

const router = express.Router()

router.post('/register', validateRequest(['playerName', 'email', 'password']), register)
router.post('/login', validateRequest(['playerName', 'password']), login)
router.post('/game', createGame)
router.post('/log/:gameId', validateRequest(['log']), createLog)
router.post('/reset-password', validateRequest(['email', 'playerName'], true), sendPasswordResetEmail)
router.put('/change-password', validateRequest(['newPassword', 'passwordResetToken']), ChangePassword)

export default router