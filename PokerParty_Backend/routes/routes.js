import express from "express";
import { login, register } from "../controllers/authentication.js";
import { addNewGame } from "../controllers/gamehistory.js";

const router = express.Router()

router.post('/register', register)
router.post('/login', login)
router.post('/games', addNewGame)

export default router