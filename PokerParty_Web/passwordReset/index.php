<?php
require 'passwordReset.php';
$token = isset($_GET['token']) ? $_GET['token'] : null;

if ($_SERVER['REQUEST_METHOD'] === 'POST' && $token) {
    $newPassword = $_POST['new_password'];
    $confirmPassword = $_POST['confirm_password'];

    $response = reset_password($token, $newPassword, $confirmPassword);
    if ($response['status'] === 'error') {
        $error = $response['message'];
    } else {
        $success = $response['message'];
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>PokerParty | Password Reset</title>
    <link rel="stylesheet" href="styles.css">
</head>

<body>
    <div class="container">
        <h2>Reset Your Password</h2>

        <?php if (isset($error)): ?>
            <div class="error"><?= $error ?></div>
        <?php elseif (isset($success)): ?>
            <div class="success"><?= $success ?></div>
        <?php endif; ?>

        <?php if ($token): ?>
            <form method="POST">
                <label for="new_password">New Password:</label>
                <input type="password" id="new_password" name="new_password" required>

                <label for="confirm_password">Confirm Password:</label>
                <input type="password" id="confirm_password" name="confirm_password" required>

                <input type="submit" value="Reset Password">
            </form>
        <?php else: ?>
            <div class="token-error">A valid token is required to reset your password. Please check the email you received.
            </div>
        <?php endif; ?>
    </div>
</body>

</html>
