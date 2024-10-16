import { db } from "../database/db.js";
import { sendEmail } from "../helper/sendEmail.js";

export const sendForgotPlayerNameEmail = async (req, res) => {
    const { email } = req.body;

    try {
        const player = await db.query('SELECT * FROM players WHERE email = $1', [email]);

        if (player.rows.length === 0) {
            return res.status(400).json({ msg: 'Invalid email' });
        }

        try {
            await sendEmail(
                player.rows[0].email,
                'PokerParty - Player Name Reminder',
                getEmailText(player.rows[0].playerName)
            );
            return res.status(200).json({ msg: `Email sent successfully to ${player.rows[0].email}` });
        }
        catch (err) {
            console.error(err.message);
            return res.status(500).json({ msg: 'Error sending email, try again!' });
        }


    } catch (err) {
        console.error(err.message);
        res.status(500).json({ msg: 'Server error' });
    }
};

const getEmailText = (playerName) => {
    return `
    <div style="font-family: Arial, sans-serif; line-height: 1.6; color: #333;">
        <h2 style="color: #ff6600; text-align: center;">PokerParty</h2>
        <p>Hi there,</p>
        <p>You are receiving this email because you (or someone else) requested your player name to log in to your PokerParty account.</p>
        <p style="font-size: 18px; font-weight: bold;">Your player name is: 
            <span style="color: #ff6600;">${player.rows[0].playerName}</span>
        </p>
        <p>If you didn’t request this information, please ignore this email. No further action is required on your part.</p>
        <p>Thank you for playing with us!</p>
        <hr style="border: none; border-top: 1px solid #ddd; margin: 20px 0;" />
        <p style="text-align: center; font-size: 12px; color: #777;">&copy; 2024 PokerParty. All rights reserved.</p>
    </div>`
};