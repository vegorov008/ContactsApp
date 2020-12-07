using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ContactsApp
{
    public static class ExceptionHandler
    {
        public static void HandleException(Exception ex)
        {
            try
            {
                try
                {
                    Debug.WriteLine(ex);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
            catch
            {

            }
        }
    }
}
