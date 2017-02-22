
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Notes.Client.Droid.Services;
using Notes.Client.Interfaces;
using Xamarin.Forms.Platform.Android;

[assembly: Xamarin.Forms.Dependency(typeof(AzureService))]
namespace Notes.Client.Droid.Services
{
    public class AzureService : IAzureService
    {
        public static FormsAppCompatActivity Context { get; set; }
        private static IMobileServiceClient _client;

        public async Task<MobileServiceUser> LoginAsync()
        {
            var client = GetMobileServicesClinet();

            return await client.LoginAsync(Context, MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
        }

        public IMobileServiceClient GetMobileServicesClinet()
        {
            if (_client == null)
            {
                _client = new MobileServiceClient(Helpers.Constants.ClientAddress);
            }
            return _client;
        }
    }
}