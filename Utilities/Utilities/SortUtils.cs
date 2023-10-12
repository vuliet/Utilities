namespace Utilities.Utilities
{
    public static class SortUtils
    {
        public static SortedDictionary<string, object> SortObjDataByAlphabet(object obj)
        {
            var properties = obj.GetType().GetProperties();
            var sortedObject = new SortedDictionary<string, object>();

            foreach (var property in properties)
                sortedObject[property.Name] = property.GetValue(obj) ?? "";

            return sortedObject;
        }
    }
}
