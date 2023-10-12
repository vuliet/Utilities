namespace Utilities
{
    public class Constants
    {
        public const int KeyEncodeSize = 256;

        public static readonly string[] SpecialCharacters = new[]
        {
            "`", "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "-", "_", "=", "+",
            "{", "}", "[", "]", ";", ":", "'", "\"", "<", ",", ">", ".", "?", "/", "\\", "|"
        };

        public const string PoolRandomString = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public const string PoolRandomStringNumber = "0123456789";

        public const string DateFormat = "yyyy-MM-dd";

        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    }
}
