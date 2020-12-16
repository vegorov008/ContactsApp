using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class Extensions
    {
        public static bool ParseBool(this string value)
        {
            if (value == "1")
            {
                return true;
            }
            else if (bool.TryParse(value, out var result))
            {
                return result;
            }
            else
            {
                return false;
            }
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNumber(this string value)
        {
            bool result = true;

            if (value.IsNullOrEmpty())
            {
                result = false;
            }
            else
            {
                for (int i = 0; i < value.Length; i++)
                {
                    if (!char.IsDigit(value[i]))
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }
    }
}

namespace System.Collections.Generic
{
    public static class Extensions
    {
        //public static void RemoveAll<T>(this ICollection<T> collection, T item)
        //{
        //    collection.Remove(item);
        //}

        //public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        //{
        //    foreach (var item in items)
        //    {
        //        collection.Remove(item);
        //    }
        //}

        public static void RemovenjnAll<T>(this IList<T> list, T item)
        {
            list.Remove(item);
        }

        public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }
    }
}
