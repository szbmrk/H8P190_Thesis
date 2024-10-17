import AWS from 'aws-sdk';

AWS.config.update({
    accessKeyId: process.env.AWS_ACCESS_KEY_ID,
    secretAccessKey: process.env.AWS_SECRET_ACCESS_KEY,
    region: process.env.AWS_REGION
});

const ses = new AWS.SES();

export const sendEmail = async (to, subject, html) => {
    const params = {
        Source: process.env.SENDER_EMAIL,
        Destination: {
            ToAddresses: [to],
        },
        Message: {
            Body: {
                Html: {
                    Charset: "UTF-8",
                    Data: html,
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