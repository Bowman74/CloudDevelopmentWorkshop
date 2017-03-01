using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Mobile.Server.Config;
using Notes.Common;

namespace NotesService.Controllers
{ 
    [MobileAppController]
    public class NotesController : ApiController
    {
        // GET api/values
        [Authorize]
        public async Task<Note[]> Get()
        {
            var returnValue = new Collection<Note>();
            var documentSerivce = new NoteDocumentService();

            var client = await documentSerivce.GetClientAsync();
            var queryOptions = new FeedOptions { MaxItemCount = -1 };

            IDocumentQuery<Note> noteQuery = client.CreateDocumentQuery<Note>(
                    UriFactory.CreateDocumentCollectionUri(NoteDocumentService.DocumentDatabaseName, NoteDocumentService.DocumentCollectionName), queryOptions)
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

        [Authorize]
        public async Task<Note> Post(Note note)
        {
            try
            {
                var documentSerivce = new NoteDocumentService();
                var client = await documentSerivce.GetClientAsync();
                await
                    client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(NoteDocumentService.DocumentDatabaseName, NoteDocumentService.DocumentCollectionName), note);

            }
            catch (Exception ex)
            {
                Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
            return note;
        }

        // POST api/values
        [HttpPatch]
        [HttpPut]
        [Authorize]
        public async Task<Note> Put(string id, Note note)
        {
            try
            {
                var documentSerivce = new NoteDocumentService();
                var client = await documentSerivce.GetClientAsync();
                await
                    client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(NoteDocumentService.DocumentDatabaseName, NoteDocumentService.DocumentCollectionName, id), note);
            }
            catch (Exception ex)
            {
                Request.CreateResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
            return note;
        }

        [Authorize]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            try
            {
                var documentSerivce = new NoteDocumentService();
                var client = await documentSerivce.GetClientAsync();
                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(NoteDocumentService.DocumentDatabaseName, NoteDocumentService.DocumentCollectionName, id));

                return Request.CreateResponse(HttpStatusCode.OK);
                
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}
