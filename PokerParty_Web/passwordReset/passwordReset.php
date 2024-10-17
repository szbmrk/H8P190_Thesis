<?php

function reset_password($token, $newPassword, $confirmPassword)
{
    if ($newPassword !== $confirmPassword) {
        return array('status' => 'error', 'message' => 'Passwords do not match!');
    }

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
    $result = @file_get_contents($url, false, $context);  // @ suppresses error, but we want to manually handle it

    // Check if the request was successful
    if ($result === FALSE) {
        $error = error_get_last();
        var_dump($error); // Debugging line to inspect error

        return array('status' => 'error', 'message' => 'An error occurred while processing your request. Please try again.');
    }

    // Decode the response and check if it's valid JSON
    $response = json_decode($result, true);
    if (json_last_error() !== JSON_ERROR_NONE) {
        var_dump($result);  // Debugging line to inspect the raw result
        return array('status' => 'error', 'message' => 'Invalid response from API.');
    }

    return array('status' => 'success', 'message' => $response['msg']);
}
