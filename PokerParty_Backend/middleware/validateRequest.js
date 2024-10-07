export const validateRequest = (validations) => {
    return (req, res, next) => {
        const errors = [];

        validations.forEach(validation => {
            const { field, type, required } = validation;

            if (required && !req.body[field]) {
                errors.push(`${field} is required`);
            } else if (type && typeof req.body[field] !== type) {
                errors.push(`${field} must be a ${type}`);
            }
        });

        if (errors.length > 0) {
            return res.status(400).json({ msg: "Invalid request body!", errors: errors });
        }

        next();
    };
};
