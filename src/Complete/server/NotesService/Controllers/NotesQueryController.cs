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
            var returnValue = new Collection<Note>();
            var documentSerivce = new NoteDocumentService();

            var client = await documentSerivce.GetClientAsync();
            var queryOptions = new FeedOptions { MaxItemCount = -1 };

            IDocumentQuery<Note> noteQuery = client.CreateDocumentQuery<Note>(
                    UriFactory.CreateDocumentCollectionUri(NoteDocumentService.DocumentDatabaseName, NoteDocumentService.DocumentCollectionName), queryOptions)
                    .Where(t => t.Content.Contains(query.QueryString))
                    .AsDocumentQuery();

            while (noteQuery.HasMoreResults)
            {
                foreach (var note in await noteQuery.ExecuteNextAsync<Note>().ConfigureAwait(false))
                {
                    returnValue.Add(note);
                }
            }
            return returnValue.ToArray();
        }
    }
}