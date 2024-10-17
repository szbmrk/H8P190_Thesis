import { db } from "../database/db.js";
import { hashPassword } from "../helper/passwordHash.js";
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
                player.rows[0].email,
                'PokerParty - Password Reset Request',
                getEmailText(resetUrl)
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

const getEmailText = (resetUrl) => {
    return `
    <div style="font-family: Arial, sans-serif; line-height: 1.6; color: #333;">
            <h2 style="color: #ff6600; text-align: center;">PokerParty</h2>
            <p>Hello,</p>
            <p>You are receiving this email because you (or someone else) have requested a password reset for your PokerParty account.</p>
            <p>To reset your password, please click the button below:</p>
            <p style="text-align: center;">
                <a href="${resetUrl}" style="background-color: #ff6600; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-size: 16px;">
                    Reset Password
                </a>
            </p>
            <p>If you did not request this, you can safely ignore this email.</p>
            <p>Thanks for being part of PokerParty!</p>
            <hr style="border: none; border-top: 1px solid #ddd; margin: 20px 0;" />
            <p style="text-align: center; font-size: 12px; color: #777;">&copy; 2024 PokerParty. All rights reserved.</p>
        </div>`
};

const generatePasswordResetToken = async () => {
    const passwordResetToken = crypto.randomBytes(20).toString('hex');
    const hashedToken = crypto.createHash('sha256').update(passwordResetToken).digest('hex');
    return hashedToken;
};

export const ChangePassword = async (req, res) => {
    const { newPassword, passwordResetToken } = req.body;

    const player = await db.query('SELECT * FROM players WHERE "passwordResetToken" = $1', [passwordResetToken]);

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
        res.status(500).json({ msg: 'Server error' });
    }
}