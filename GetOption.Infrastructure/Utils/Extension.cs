using GetOption.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GetOption.Infrastructure.Utils
{
    internal static class Extension
    {

        public static  GetOptionToDocumentQuery<T> ToDocumentQuery<T>(this GetOption<T> option)
        {
            GetOptionToDocumentQuery<T> getOptionToDocumentQuery = new GetOptionToDocumentQuery<T>(option);
            return getOptionToDocumentQuery;
        }


        public static string ToUTCFormat(this DateTime dateTime)
        {
            if (dateTime != default(DateTime))
            {

                return dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");
            }
            return null;
        }


        public static string RemoveHyphen(this string value)
        {

            return value?.Trim().Replace("-", "");
        }

        public static string ToCamelCase(this string value)
        {

            return value?.Trim().Remove(0, 1).Insert(0, value.First().ToString().ToLower());
        }


        public static Dictionary<Tkey,List<TValue>> ToDictionary<Tkey,TValue>(this IEnumerable<IGrouping<Tkey,TValue>> groupingList)
        {

            return groupingList.ToDictionary(grouping => grouping.Key, grouping => grouping.ToList());

        }


     
    }
}
