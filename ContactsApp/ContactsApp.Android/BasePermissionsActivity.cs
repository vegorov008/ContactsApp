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
    public class BasePermissionsActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly Dictionary<int, TaskCompletionSource<bool>> _permissionRequests = new Dictionary<int, TaskCompletionSource<bool>>();

        volatile int _requestCode;

        public Task<bool> TryGrantPermissions(string permission)
        {
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();
            try
            {
                var permissionStatus = CheckSelfPermission(permission);
                if (permissionStatus != Android.Content.PM.Permission.Granted)
                {
                    int requestCode = ++_requestCode;

                    lock (_permissionRequests)
                    {
                        _permissionRequests.Add(requestCode, taskCompletionSource);
                    }

                    RequestPermissions(new string[] { permission }, requestCode);
                }
                else
                {
                    taskCompletionSource.SetResult(true);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
            return taskCompletionSource.Task;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                bool granted = true;
                for (int i = 0; i < grantResults.Length; i++)
                {
                    if (grantResults[i] == Android.Content.PM.Permission.Denied)
                    {
                        granted = false;
                        break;
                    }
                }

                lock (_permissionRequests)
                {
                    if (_permissionRequests.TryGetValue(requestCode, out var taskCompletionSource))
                    {
                        _permissionRequests.Remove(requestCode);
                        taskCompletionSource.TrySetResult(granted);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }
    }
}