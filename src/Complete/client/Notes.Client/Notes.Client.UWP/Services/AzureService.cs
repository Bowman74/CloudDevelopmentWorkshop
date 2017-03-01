using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.WindowsAzure.MobileServices;
using Notes.Client.Interfaces;
using Notes.Client.UWP.Services;

[assembly: Xamarin.Forms.Dependency(typeof(AzureService))]
namespace Notes.Client.UWP.Services
{
    public class AzureService : IAzureService
    {
        private static IMobileServiceClient _client;

        public IMobileServiceClient GetMobileServicesClinet()
        {
            if (_client == null)
            {
                _client = new MobileServiceClient(Helpers.Constants.ClientAddress);
            }
            return _client;
        }

        public async Task<MobileServiceUser> LoginAsync()
        {
            var client = GetMobileServicesClinet();

            return await client.LoginAsync(MobileServiceAuthenticationProvider.WindowsAzureActiveDirectory);
        }
    }
}
