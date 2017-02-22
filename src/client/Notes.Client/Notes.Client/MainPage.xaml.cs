using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Notes.Client.Interfaces;
using Notes.Client.Services;
using Notes.Common;
using Xamarin.Forms;

namespace Notes.Client
{
    public partial class MainPage : ContentPage
    {
        private const string AuthenticaitonTokenKey = "authenticationToken";
        private const string UserIdKey = "userId";

        public MainPage()
        {
            InitializeComponent();

            Device.OnPlatform(
                Default: () =>
                {
                    ToolbarItems.Add(new ToolbarItem("Logout", "", async () => { await LogougAsync(); }, ToolbarItemOrder.Secondary, 2));
                },
                iOS: () => ToolbarItems.Add(new ToolbarItem("More", "", ActionsClick, ToolbarItemOrder.Primary, 1))
            );

            BindingContext = this;
            NoteList.ItemTapped += NoteList_ItemTapped;

            AddNote.Clicked += AddNote_OnClicked;
        }

        private async void NoteList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var note = e.Item as Note;
            await Navigation.PushAsync(new EditNote(note));
        }

        private async void AddNote_OnClicked(object sender, EventArgs eventArgs)
        {
            await Navigation.PushAsync(new EditNote(null));
        }

        protected override async  void OnAppearing()
        {
            base.OnAppearing();
            await AuthenticateAsync();
            await GetNotesForUserAsync();

        }

        public static readonly BindableProperty NotesProperty = BindableProperty.Create(nameof(Notes), typeof(IList<Note>), typeof(MainPage), new List<Note>());
        public IList<Note> Notes
        {
            get { return (IList<Note>)GetValue(NotesProperty); }
            set { SetValue(NotesProperty, value); }
        }

        private string _Filter = string.Empty;
        public string Filter
        {
            get { return _Filter;  }
            set
            {
                _Filter = value;
                GetNotesForUserAsync();
            }
        }

        private async Task AuthenticateAsync()
        {
            var appSettingsService = new AppSettingsService();
            var azureService = DependencyService.Get<IAzureService>();
            if (azureService.GetMobileServicesClinet().CurrentUser == null)
            {
                string userId;
                string token;

                if (appSettingsService.TryGetValue(AuthenticaitonTokenKey, out token) &&
                    appSettingsService.TryGetValue(UserIdKey, out userId))
                {
                    var user = new MobileServiceUser(userId);
                    user.MobileServiceAuthenticationToken = token;
                    azureService.GetMobileServicesClinet().CurrentUser = user;

                    if (await GetCurrentUserInfo(azureService))
                    {
                        return;
                    }
                }
                await LoginAsync(appSettingsService, azureService);
            }
        }

        private async Task LoginAsync(AppSettingsService appSettingsService, IAzureService azureService)
        {
            do
            {
                try
                {
                    var user = await azureService.LoginAsync();
                    appSettingsService.Add(AuthenticaitonTokenKey, user.MobileServiceAuthenticationToken);
                    appSettingsService.Add(UserIdKey, user.UserId);

                    await GetCurrentUserInfo(azureService);
                }
                catch (Exception) { }
            } while (UserInformation.User == null);
        }

        private async Task<bool> GetCurrentUserInfo(IAzureService azureService)
        {
            bool returnValue;
            try
            {
                UserInformation.User = await azureService.GetMobileServicesClinet().InvokeApiAsync<User>("Profile", HttpMethod.Get, null);
                Title = $"Tasks for: {UserInformation.User.FullName}";
                returnValue = true;
            }
            catch (MobileServiceInvalidOperationException)
            {
                returnValue = false;
            }
            return returnValue;
        }

        private async Task GetNotesForUserAsync()
        {
            try
            {
                IsBusy = true;
                var azureService = DependencyService.Get<IAzureService>();
                Note[] notes = null;
                if (_Filter != string.Empty)
                {
                    notes = await azureService.GetMobileServicesClinet()
                                .InvokeApiAsync<Query, Note[]>("NotesQuery", new Query {QueryString = _Filter}, HttpMethod.Get, null);
                }
                else
                {
                    notes =await azureService.GetMobileServicesClinet().InvokeApiAsync<Note[]>("Notes", HttpMethod.Get, null);
                }

                Notes = notes.ToList();
            }
            catch (UnauthorizedAccessException) { }
            finally
            {
                IsBusy = false;
            }
        }

        private async void ActionsClick()
        {
            const string logout = "Logout";
            var result = await DisplayActionSheet(null, null, null, null, logout);

            switch (result)
            {
                case logout:
                    await LogougAsync();
                    break;
            }
        }

        private async Task LogougAsync()
        {
            UserInformation.User = null;
            var appSettingsService = new AppSettingsService();
            var azureService = DependencyService.Get<IAzureService>();
            appSettingsService.Remove(AuthenticaitonTokenKey);
            appSettingsService.Remove(UserIdKey);
            await azureService.GetMobileServicesClinet().LogoutAsync();

            await LoginAsync(appSettingsService, azureService);
        }
    }
}