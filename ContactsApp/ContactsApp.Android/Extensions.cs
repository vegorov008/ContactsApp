using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ContactsApp.Droid
{
    public static class Extensions
    {
        public static Task<TResult> RunOnUiTrhreadAsync<TResult>(this Activity activity, Func<TResult> action)
        {
            TaskCompletionSource<TResult> taskCompletionSource = new TaskCompletionSource<TResult>();
            activity.RunOnUiThread(() =>
            {
                try
                {
                    TResult result = default;
                    try
                    {
                        result = action();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.HandleException(ex);
                    }
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            });

            return taskCompletionSource.Task;
        }

        public static Task RunOnUiTrhreadAsync(this Activity activity, Action action)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            activity.RunOnUiThread(() =>
            {
                try
                {
                    try
                    {
                        action();
                        taskCompletionSource.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        taskCompletionSource.SetResult(false);
                        ExceptionHandler.HandleException(ex);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            });

            return taskCompletionSource.Task;
        }
    }
}