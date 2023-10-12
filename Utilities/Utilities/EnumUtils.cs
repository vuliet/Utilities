namespace Utilities.Utilities
{
    public static class EnumUtils
    {
        public static bool EnumContainsKey<T>(string value) where T : Enum
        {
            var props = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(x => x.ToString())
                .ToList();

            return props.Contains(value);
        }
    }
}
