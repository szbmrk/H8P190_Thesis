import { db } from "../database/db";

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
                'PokerParty - Forgot Player Name',
                `You are receiving this email because you (or someone else) have requested your player name for your account.\n\nYour player name is: ${player.rows[0].playerName}\n\nIf you did not request this, please ignore this email.\n`
            );
            return res.status(200).json({ msg: `Email sent successfully to ${toEmail}` });
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
