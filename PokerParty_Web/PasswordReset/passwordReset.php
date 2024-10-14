<!DOCTYPE html>
<html>
<head>
    <title>Send Email</title>
</head>
<body>

<?php
if ($_SERVER["REQUEST_METHOD"] == "POST") {
    // Email details
    $to = "szmark16@gmail.com";
    $subject = "Test Email";
    $message = "This is a test email sent from PHP!";
    $headers = "From: example@yourdomain.com";

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