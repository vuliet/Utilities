using System.ComponentModel;

namespace Utilities
{
    public enum AppError
    {
        [Description("Unknow")]
        UNKNOWN,
        [Description("Operation fail")]
        OPERATION_FAIL,
        [Description("Unauthorized")]
        UNAUTHORIZED,
        [Description("Invalid parameters")]
        INVALID_PARAMETERS,
        [Description("Invalid operation")]
        INVALID_OPERATION,
        [Description("Permission denied")]
        PERMISSION_DENIED,
        [Description("System so busy")]
        SYSTEM_BUSY,
        [Description("Too many requests")]
        TOO_MANY_REQUESTS,
        [Description("Not found")]
        RECORD_NOTFOUND,
        [Description("Token invalid")]
        TOKEN_INVALID,
        [Description("Token wrong")]
        TOKEN_WRONG,
        [Description("Token expired")]
        TOKEN_EXPIRED,
        [Description("OTP invalid")]
        OTP_INVALID,
        [Description("OTP wrong")]
        OTP_WRONG,
        [Description("OTP expired")]
        OTP_EXPIRED,
        [Description("Password wrong")]
        PASSWORD_WRONG,
        [Description("Google authenticator code is required")]
        GACODE_REQUIRED,
        [Description("Google authenticator code wrong")]
        GACODE_WRONG,
        [Description("Credentials no match")]
        WRONG_CREDENTIALS,
        [Description("User is inactive")]
        USER_INACTIVE,
        [Description("User is baned")]
        USER_BANED,
        [Description("Insufficient funds")]
        INSUFFICIENT_FUNDS,
        [Description("Email exist")]
        EMAIL_EXIST,
        [Description("Sold out")]
        SOLD_OUT,
        [Description("Owner error")]
        OWNER_ERROR,
        [Description("Google authenticator error")]
        GOOGLE_LOGIN_FAIL
    }
}
