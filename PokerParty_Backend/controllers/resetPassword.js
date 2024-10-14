import { sendEmail } from "../helper/sendEmail.js";

export const sendResetPasswordEmail = async (req, res) => {
    const { email, playerName } = req.body;

    try {
        let player = null;

        if (email) {
            const playerEmailQuery = 'SELECT * FROM players WHERE email = $1';
            player = await db.query(playerEmailQuery, [email]);
        }

        if (playerName) {
            const playerPlayerNameQuery = 'SELECT * FROM players WHERE "playerName" = $1';
            player = await db.query(playerPlayerNameQuery, [playerName]);
        }

        if (player.rows.length === 0) {
            return res.status(400).json({ msg: 'Invalid email or playerName' });
        }

        const email = player.rows[0].email;

        try {
            await sendEmail(email, 'Reset Password', 'Click on the link to reset your password');
            return res.status(200).json({ msg: 'Email sent successfully' });
        }
        catch (err) {
            console.error(err.message);
            return res.status(500).json({ msg: 'Error sending email, try again!' });
        }

    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
};