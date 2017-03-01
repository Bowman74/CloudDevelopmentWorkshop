using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Notes.Client.Interfaces
{
    public interface IAzureService
    {
        Task<MobileServiceUser> LoginAsync();
        IMobileServiceClient GetMobileServicesClinet();
    }
}