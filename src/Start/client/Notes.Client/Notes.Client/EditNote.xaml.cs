using Notes.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
//using Notes.Client.Interfaces;
using Notes.Client.Services;
using Xamarin.Forms;
//using Microsoft.Azure.Mobile.Analytics;

namespace Notes.Client
{
    public partial class EditNote : ContentPage
    {

        private bool IsNew = false;

        public EditNote(Note note)
        {
            InitializeComponent();
            InitializeNote(note);
            BindingContext = this;

            Device.OnPlatform(
                Default: () =>
                {
                    ToolbarItems.Add(new ToolbarItem("Delete", "", async () => { await DeleteNoteAsync(); }, ToolbarItemOrder.Secondary, 2));
                },
                iOS: () => ToolbarItems.Add(new ToolbarItem("More", "", ActionsClick, ToolbarItemOrder.Primary, 1))
            );

            SaveNote.Clicked += SaveNote_Clicked;
        }

        private async void SaveNote_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync(true);
        }

        private void InitializeNote(Note note)
        {
            if (note == null)
            {
                Note = new Note
                {
                    CreateDate = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString(),
                    EnteredBy = UserInformation.User != null ? UserInformation.User.UserId: String.Empty,
                    Content = string.Empty
                };
                IsNew = true;
            }
            else
            {
                Note = note;
            }
        }

        public static readonly BindableProperty NoteProperty = BindableProperty.Create(nameof(Note), typeof(Note), typeof(EditNote), new Note());
        public Note Note
        {
            get { return (Note)GetValue(NoteProperty); }
            set { SetValue(NoteProperty, value); }
        }

        private async void ActionsClick()
        {
            const string delete = "Delete";
            var result = await DisplayActionSheet(null, null, null, null, delete);

            switch (result)
            {
                case delete:
                    await DeleteNoteAsync();
                    break;
            }
        }

        private async Task DeleteNoteAsync()
        {
            var answer = await DisplayAlert("Delete", "Would you like to delete this note?", "Yes", "No");
            if (answer)
            {
                await Navigation.PopAsync(true);
            }
        }
    }
}
