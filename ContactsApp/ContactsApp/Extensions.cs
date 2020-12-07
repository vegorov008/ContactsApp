using System;
using System.Collections.Generic;
using System.Text;

namespace ContactsApp
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
