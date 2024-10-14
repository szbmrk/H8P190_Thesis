export const validateRequest = (requiredFields, allowEither = false) => {
    return (req, res, next) => {
        const errors = [];

        if (allowEither) {
            const hasAtLeastOneField = requiredFields.some(field => req.body[field]);
            if (!hasAtLeastOneField) {
                return res.status(400).json({
                    msg: `At least one of the following fields is required: ${requiredFields.join(', ')}`
                });
            }
        } else {
            requiredFields.forEach(field => {
                if (!req.body[field]) {
                    errors.push(`${field} is required`);
                }
            });

            if (errors.length > 0) {
                return res.status(400).json({
                    msg: "Incorrect request body!",
                    errors: errors
                });
            }
        }

        next();
    };
};
