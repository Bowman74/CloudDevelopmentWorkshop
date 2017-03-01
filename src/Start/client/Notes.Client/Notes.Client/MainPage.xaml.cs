using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//using Microsoft.WindowsAzure.MobileServices;
//using Notes.Client.Interfaces;
using Notes.Client.Services;
using Notes.Common;
using Xamarin.Forms;

namespace Notes.Client
{
    public partial class MainPage : ContentPage
    {
        private const string AuthenticaitonTokenKey = "authenticationToken";
        private const string UserIdKey = "userId";

        private bool LoggingIn = false;

        public MainPage()
        {
            InitializeComponent();

            Device.OnPlatform(
                Default: () =>
                {
                    ToolbarItems.Add(new ToolbarItem("Logout", "", () => { }, ToolbarItemOrder.Secondary, 2));
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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (!LoggingIn)
            {
                await GetNotesForUserAsync();
            }
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

        private async Task GetNotesForUserAsync()
        {
            await Task.Run(() => Notes = new List<Note>());
        }

        private async void ActionsClick()
        {
            const string logout = "Logout";
            var result = await DisplayActionSheet(null, null, null, null, logout);

            switch (result)
            {
                case logout:
                    break;
            }
        }
    }
}