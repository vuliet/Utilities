namespace Utilities
{
    public enum AppError
    {
        UNKNOWN = 0,
        OPERATION_FAIL = 1,
        UNAUTHORIZED = 2,
        INVALID_PARAMETERS = 3,
        INVALID_OPERATION = 4,
        PERMISSION_DENIED = 5,
        SYSTEM_BUSY = 6,
        TOO_MANY_REQUESTS = 7,
        RECORD_NOTFOUND = 8,
        TOKEN_INVALID = 9,
        TOKEN_WRONG = 10,
        TOKEN_EXPIRED = 11,
        OTP_INVALID = 12,
        OTP_WRONG = 13,
        OTP_EXPIRED = 14,
        PASSWORD_WRONG = 15,
        GACODE_REQUIRED = 16,
        GACODE_WRONG = 17,
        WRONG_CREDENTIALS = 18,
        USER_INACTIVE = 19,
        USER_BANED = 20,
        INSUFFICIENT_FUNDS = 21,
        EMAIL_EXIST = 22,
        SOLD_OUT = 23,
        OWNER_ERROR = 24,
        GOOGLE_LOGIN_FAIL = 25
    }
}
