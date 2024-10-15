<?php
$token = isset($_GET['token']) ? $_GET['token'] : null;

if ($_SERVER['REQUEST_METHOD'] === 'POST' && $token) {
    $newPassword = $_POST['new_password'];
    $confirmPassword = $_POST['confirm_password'];

    if ($newPassword !== $confirmPassword) {
        $error = "Passwords do not match!";
    } else {

        $url = "http://localhost:5000/api/change-password";
        $data = array('passwordResetToken' => $token, 'newPassword' => $newPassword);

        $options = array(
            'http' => array(
                'header' => "Content-type: application/json\r\n",
                'method' => 'PUT',
                'content' => json_encode($data)
            )
        );

        $context = stream_context_create($options);
        $result = file_get_contents($url, false, $context);

        if ($result === FALSE) {
            $error = "An error occurred while processing your request. Please try again.";
        } else {
            $response = json_decode($result, true);
            $success = $response['msg'];
        }

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
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: #f0f2f5;
            display: flex;
            justify-content: center;
            align-items: center;
            height: 100vh;
            margin: 0;
        }

        .container {
            background-color: #ffffff;
            padding: 40px;
            border-radius: 12px;
            box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
            width: 100%;
            max-width: 400px;
        }

        h2 {
            text-align: center;
            color: #333333;
            margin-bottom: 25px;
            font-size: 1.8rem;
            font-weight: 600;
        }

        label {
            font-size: 15px;
            color: #666666;
            margin-bottom: 5px;
            display: block;
        }

        input[type="password"],
        input[type="submit"] {
            width: 100%;
            padding: 15px;
            margin: 12px 0 25px;
            border-radius: 8px;
            border: 1px solid #dddddd;
            box-sizing: border-box;
            font-size: 1rem;
            outline: none;
            transition: all 0.3s ease;
        }

        input[type="password"]:focus {
            border-color: #007bff;
            box-shadow: 0 0 6px rgba(0, 123, 255, 0.3);
        }

        input[type="submit"] {
            background-color: #007bff;
            color: #ffffff;
            border: none;
            cursor: pointer;
            font-size: 1rem;
            font-weight: 500;
            transition: background-color 0.3s ease, transform 0.2s ease;
        }

        input[type="submit"]:hover {
            background-color: #0056b3;
            transform: translateY(-2px);
        }

        input[type="submit"]:active {
            transform: translateY(0);
        }

        .error,
        .success,
        .token-error {
            text-align: center;
            padding: 12px;
            margin-bottom: 20px;
            border-radius: 8px;
            font-size: 15px;
            font-weight: 500;
        }

        .error {
            background-color: #f8d7da;
            color: #721c24;
        }

        .success {
            background-color: #d4edda;
            color: #155724;
        }

        .token-error {
            background-color: #fff3cd;
            color: #856404;
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
            <div class="error">A valid token is required to reset your password. Please check the link you received.
            </div>
        <?php endif; ?>
    </div>

</body>

</html>