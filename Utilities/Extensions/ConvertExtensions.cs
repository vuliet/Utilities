﻿namespace Utilities.Extensions
{
    public static class ConvertExtensions
    {
        public static List<int> ConvertStringToListInt(this string strIds)
        {
            if (string.IsNullOrEmpty(strIds))
                return new List<int>();

            var lstResult = new List<int>();
            var arrId = strIds.Split(',');

            foreach (var item in arrId)
            {
                if (int.TryParse(item, out int rsid))
                    lstResult.Add(rsid);
            }

            return lstResult;
        }

        public static Guid ToGuid(this string id)
        {
            return Guid.Parse(id);
        }
    }
}
