
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Notes.Client.Droid.Services;
using Notes.Client.Interfaces;

[assembly: Xamarin.Forms.Dependency(typeof(AzureService))]
namespace Notes.Client.Droid.Services
{
    public class AzureService : IAzureService
    {
        public Task<MobileServiceUser> LoginAsync()
        {
            throw new System.NotImplementedException();
        }

        public IMobileServiceClient GetMobileServicesClinet()
        {
            throw new System.NotImplementedException();
        }
    }
}