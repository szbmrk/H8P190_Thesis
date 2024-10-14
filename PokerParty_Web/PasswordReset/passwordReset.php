<!DOCTYPE html>
<html lang="en">

<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Send Email</title>
</head>

<body>

    <?php
    if ($_SERVER["REQUEST_METHOD"] == "POST") {
        // Email details
        $to = "szmark16@gmail.com"; // Recipient's email address
        $subject = "Test Email from Poker Party";
        $message = "This is a test email sent from the Poker Party application!";
        $headers = "From: pokerparty@szobo.dev\r\n"; // Set the "From" address
    
        // Send email
        if (mail($to, $subject, $message, $headers)) {
            echo "<p>Email successfully sent to $to</p>";
        } else {
            echo "<p>Email sending failed.</p>";
        }
    }
    ?>

    <form method="post">
        <button type="submit">Send Test Email</button>
    </form>

</body>

</html>