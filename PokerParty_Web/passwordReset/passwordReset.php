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
    $result = file_get_contents($url, false, $context);

    if ($result === FALSE) {
        return array('status' => 'error', 'message' => 'An error occurred while processing your request. Please try again.');
    }

    $response = json_decode($result, true);
    return array('status' => 'success', 'message' => $response['msg']);
}
