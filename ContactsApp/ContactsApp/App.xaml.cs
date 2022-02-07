using System;
using ContactsApp.Models;
using ContactsApp.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ContactsApp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                SqLiteDataProvider sqLiteDataProvider = new SqLiteDataProvider();
                DependencyService.RegisterSingleton(sqLiteDataProvider);
                DependencyService.RegisterSingleton<IDataStore<Contact>>(new SqLiteDataStore<Contact>(sqLiteDataProvider));

                MainPage = new MainPage();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
