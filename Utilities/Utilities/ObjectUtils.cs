using Newtonsoft.Json;

namespace Utilities.Utilities
{
    public static class ObjectUtils
    {
        public static T? DeepCopy<T>(T obj)
        {
            if (obj is null)
                return default;

            string json = JsonConvert.SerializeObject(obj);
            T? copy = JsonConvert.DeserializeObject<T>(json);

            return copy;
        }
    }
}
