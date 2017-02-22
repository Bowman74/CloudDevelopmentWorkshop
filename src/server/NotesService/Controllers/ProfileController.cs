using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Azure.Mobile.Server.Config;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.Mobile.Server.Authentication;
using System.Data.Services.Client;
using System.Net;
using Microsoft.IdentityModel.Clients.ActiveDirectory;


namespace NotesService.Controllers
{
    [MobileAppController]
    public class ProfileController : ApiController
    {
        [Authorize]
        public async Task<Notes.Common.User> Get()
        {
            var serviceIdentity =
                await
                    ((ClaimsPrincipal) User).GetAppServiceIdentityAsync<AzureActiveDirectoryCredentials>(
                        this.Request);

            var authority = "https://graph.windows.net/bowman74hotmail.onmicrosoft.com";

            var client =
                new ActiveDirectoryClient(new Uri(authority), GetAuthenticationTokenAsync);

            var currentUserResult = await client.Users.Where(f => f.ObjectId == serviceIdentity.ObjectId).ExecuteAsync();

            var currentUser = currentUserResult.CurrentPage.Single();

            var returnValue = new Notes.Common.User
            {
                FullName = currentUser.DisplayName,
                FirstName = currentUser.GivenName,
                LastName = currentUser.Surname,
                UserId = currentUser.UserPrincipalName
            };
            return returnValue;
        }
        
        private async Task<string> GetAuthenticationTokenAsync()
        {
            var secret = CloudConfigurationManager.GetSetting(Constants.ConfigurationKeys.ClientSecret) ?? string.Empty;
            ClientCredential clientCred = new ClientCredential(CloudConfigurationManager.GetSetting(Constants.ConfigurationKeys.ClientId), secret);
            AuthenticationContext authenticationContext = new AuthenticationContext(string.Format(CloudConfigurationManager.GetSetting(Constants.ConfigurationKeys.Authority), CloudConfigurationManager.GetSetting(Constants.ConfigurationKeys.AdSite)), false);
            return (await authenticationContext.AcquireTokenAsync(Constants.AdIntegration.Graph, clientCred)).AccessToken;
        }
    }
}