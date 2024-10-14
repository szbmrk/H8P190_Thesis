import AWS from 'aws-sdk';

AWS.config.update({ region: process.env.AWS_REGION });

const ses = new AWS.SES();

export const sendEmail = async (email, subject, message) => {
    const params = {
        Source: process.env.SENDER_EMAIL,
        Destination: {
            ToAddresses: [email],
        },
        Message: {
            Body: {
                Text: {
                    Charset: "UTF-8",
                    Data: message,
                },
            },
            Subject: {
                Charset: 'UTF-8',
                Data: subject,
            },
        },
    };

    await ses.sendEmail(params).promise();
};