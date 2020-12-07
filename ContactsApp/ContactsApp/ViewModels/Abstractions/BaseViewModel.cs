using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;

namespace ContactsApp.ViewModels.Abstractions
{
    public abstract class BaseViewModel : BaseBindableObject<BaseViewModel>
    {
        public INavigation Navigation { get; set; }

        public virtual void OnAppearing()
        {

        }

        public virtual void OnDisappearing()
        {

        }
    }

    public abstract class BaseViewModel<TModel> : BaseViewModel where TModel : class
    {
        public virtual TModel Model
        {
            get => GetProperty<TModel>();
            set => SetProperty(value);
        }
    }
}
