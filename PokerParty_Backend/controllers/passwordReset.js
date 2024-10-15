import { db } from "../database/db.js";
import { sendEmail } from "../helper/sendEmail.js";
import crypto from "crypto";

export const sendPasswordResetEmail = async (req, res) => {
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

        const playerId = player.rows[0]._id;
        const toEmail = player.rows[0].email;

        const passwordResetToken = await generatePasswordResetToken();

        const updatePasswrdResetTokenQuery = 'UPDATE players SET "passwordResetToken" = $1 WHERE _id = $2';
        await db.query(updatePasswrdResetTokenQuery, [passwordResetToken, playerId]);

        const resetUrl = `https://pokerparty.szobo.dev/passwordReset?token=${passwordResetToken}`;

        try {
            await sendEmail(
                toEmail,
                'Password Reset',
                `You are receiving this email because you (or someone else) have requested the reset of the password for your account.\n\nPlease click on the following link, or paste this into your browser to complete the process:\n\n${resetUrl}\n\nIf you did not request this, please ignore this email and your password will remain unchanged.\n`
            );
            return res.status(200).json({ msg: `Email sent successfully to ${toEmail}` });
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

const generatePasswordResetToken = async () => {
    const passwordResetToken = crypto.randomBytes(20).toString('hex');
    const hashedToken = crypto.createHash('sha256').update(passwordResetToken).digest('hex');
    return hashedToken;
};

export const ChangePassword = async (req, res) => {
    const { newPassword, passwordResetToken } = req.body;

    const hashedToken = crypto.createHash('sha256').update(passwordResetToken).digest('hex');
    const player = await db.query('SELECT * FROM players WHERE "passwordResetToken" = $1', [hashedToken]);

    if (player.rows.length === 0) {
        return res.status(400).json({ msg: 'Invalid password reset token' });
    }

    const playerId = player.rows[0]._id;
    const hashedPassword = await hashPassword(newPassword);

    try {
        const updatePasswordQuery = 'UPDATE players SET password = $1, "passwordResetToken" = $2 WHERE _id = $3';
        await db.query(updatePasswordQuery, [hashedPassword, null, playerId]);
        return res.status(200).json({ msg: 'Password updated successfully' });
    } catch (err) {
        console.error(err.message);
        res.status(500).send('Server error');
    }
}