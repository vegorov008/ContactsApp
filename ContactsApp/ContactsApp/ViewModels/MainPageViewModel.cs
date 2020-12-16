using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ContactsApp.Models;
using ContactsApp.Services;
using ContactsApp.ViewModels.Abstractions;
using Xamarin.Forms;

namespace ContactsApp.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        readonly object _uiUpdateSyncObject = new object();

        readonly Dictionary<RefreshStatus, string> _statusStrings = new Dictionary<RefreshStatus, string>()
        {
            { RefreshStatus.RefreshNeeded, "Требуется синхронизация" },
            { RefreshStatus.Refreshing, "Cинхронизация выполняется..." },
            { RefreshStatus.RefreshSucceed, "Синхронизация выполнена успешно" },
            { RefreshStatus.RefreshFailed, "Ошибка синхронизации" }
        };

        enum RefreshStatus
        {
            RefreshNeeded,
            Refreshing,
            RefreshSucceed,
            RefreshFailed
        }

        readonly IContactsService _contactsService;
        readonly IDataStore<Contact> _contactsDataStore;
        readonly List<ContactViewModel> _viewModels;

        volatile bool _isBusy;

        RefreshStatus _status;
        RefreshStatus Status
        {
            get => _status;
            set
            {
                try
                {
                    _status = value;
                    StatusText = _statusStrings[_status];
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            }
        }

        public ObservableCollection<ContactViewModel> Items { get; set; } = new ObservableCollection<ContactViewModel>();

        public string StatusText
        {
            get => GetProperty<string>();
            set => SetProperty(value);
        }

        public ICommand RefreshCommand { get; set; }

        public MainPageViewModel()
        {
            try
            {
                _contactsService = DependencyService.Resolve<IContactsService>();
                _contactsDataStore = DependencyService.Resolve<IDataStore<Contact>>();
                _viewModels = new List<ContactViewModel>();

                RefreshCommand = new Command(Refresh);
                Status = RefreshStatus.RefreshNeeded;
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        async void Refresh()
        {
            try
            {
                if (!_isBusy)
                {
                    _isBusy = true;

                    Status = RefreshStatus.Refreshing;
                    await Task.Run(async () =>
                    {
                        try
                        {
                            var contacts = await _contactsService.GetContactsAsync();
                            if (contacts != null)
                            {
                                await _contactsDataStore.RemoveAllAsync();
                                await _contactsDataStore.AddOrUpdateAsync(contacts);

                                Update();
                                Status = RefreshStatus.RefreshSucceed;
                            }
                            else
                            {
                                Status = RefreshStatus.RefreshFailed;
                            }
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.HandleException(ex);
                            Status = RefreshStatus.RefreshFailed;
                        }
                    });

                    _isBusy = false;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        async Task Load()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var contacts = await _contactsDataStore.GetItemsAsync();
                    if (contacts?.Count > 0)
                    {
                        lock (_viewModels)
                        {
                            for (int i = 0; i < contacts.Count; i++)
                            {
                                _viewModels.Add(new ContactViewModel(contacts[i]));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex);
                }
            });
        }

        void UpdateItems()
        {
            try
            {
                lock (_uiUpdateSyncObject)
                {
                    Items.Clear();
                    lock (_viewModels)
                    {
                        if (_viewModels?.Count > 0)
                        {
                            for (int i = 0; i < _viewModels.Count; i++)
                            {
                                Items.Add(_viewModels[i]);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        async void Update()
        {
            try
            {
                await Load();
                UpdateItems();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }

        public override void OnAppearing()
        {
            try
            {
                base.OnAppearing();
                Update();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex);
            }
        }
    }
}
