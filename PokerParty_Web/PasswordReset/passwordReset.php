<?php
$token = isset($_GET['token']) ? $_GET['token'] : null;

if (!$token) {
    $error = "This page only works if a valid token is provided.";
}

if ($_SERVER['REQUEST_METHOD'] === 'POST' && $token) {
    $newPassword = $_POST['new_password'];
    $confirmPassword = $_POST['confirm_password'];

    if ($newPassword !== $confirmPassword) {
        $error = "Passwords do not match!";
    } else {
        $data = array(
            'token' => $token,
            'newPassword' => $newPassword
        );

        $ch = curl_init("http://localhost:5000/password-reset");

        curl_setopt($ch, CURLOPT_CUSTOMREQUEST, "PUT");
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json'));
        curl_setopt($ch, CURLOPT_POSTFIELDS, json_encode($data));

        $response = curl_exec($ch);

        if (curl_errno($ch)) {
            $error = "API Request Error: " . curl_error($ch);
        } else {
            $statusCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);

            if ($statusCode == 200) {
                $success = "Password updated successfully!";
            } else {
                $error = "Error: Unable to update password (HTTP Status $statusCode)";
            }
        }

        curl_close($ch);
    }
}
?>

<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Password Reset</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f0f2f5;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
        }

        .container {
            background-color: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 400px;
        }

        h2 {
            text-align: center;
            color: #333;
        }

        input[type="password"],
        input[type="submit"] {
            width: 100%;
            padding: 10px;
            margin: 10px 0;
            border-radius: 5px;
            border: 1px solid #ccc;
        }

        input[type="submit"] {
            background-color: #4CAF50;
            color: white;
            border: none;
            cursor: pointer;
        }

        input[type="submit"]:hover {
            background-color: #45a049;
        }

        .error,
        .success {
            text-align: center;
            color: white;
            padding: 10px;
            margin-top: 10px;
            border-radius: 5px;
        }

        .error {
            background-color: #f44336;
        }

        .success {
            background-color: #4CAF50;
        }
    </style>
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
            <div class="error">A token is required to reset your password. Please check the link you received.</div>
        <?php endif; ?>
    </div>

</body>

</html>