using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Mobile.Server.Config;
using Notes.Common;

namespace NotesService.Controllers
{
    [MobileAppController]
    public class NotesQueryController : ApiController
    {

        // GET api/values
        [Authorize]
        public async Task<Note[]> Post(Query query)
        {
            throw new NotImplementedException();
        }
    }
}