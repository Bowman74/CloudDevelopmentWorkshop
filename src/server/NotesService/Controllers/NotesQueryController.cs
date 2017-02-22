using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Mobile.Server.Config;
using Notes.Common;
using Database = Microsoft.Azure.Documents.Database;

namespace NotesService.Controllers
{
    [MobileAppController]
    public class NotesQueryController : ApiController
    {
        private static readonly string EndpointUri = ConfigurationManager.AppSettings["EndPointUri"];
        private static readonly string PrimaryKey = ConfigurationManager.AppSettings["PrimaryKey"];

        private DocumentClient _client;

        private string documentDatabaseName = "notes";
        private string documentCollectionName = "notes_collection";

        // GET api/values
        [Authorize]
        public async Task<Note[]> Get(Query query)
        {
            var returnValue = new Collection<Note>();

            var client = await GetClientAsync();
            var queryOptions = new FeedOptions { MaxItemCount = -1 };

            IDocumentQuery<Note> noteQuery = client.CreateDocumentQuery<Note>(
                    UriFactory.CreateDocumentCollectionUri(documentDatabaseName, documentCollectionName), queryOptions)
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

        private async Task<IDocumentClient> GetClientAsync()
        {
            if (_client == null)
            {
                _client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);

                await CreateDatabaseIfNotExists(documentDatabaseName, _client);

                await CreateDocumentCollectionIfNotExists(documentDatabaseName, documentCollectionName, _client);
            }

            return _client;
        }

        private async Task CreateDatabaseIfNotExists(string databaseName, IDocumentClient documentClient)
        {
            try
            {
                await documentClient.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await documentClient.CreateDatabaseAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName, IDocumentClient documentClient)
        {
            try
            {
                await documentClient.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                // If the document collection does not exist, create a new collection
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    DocumentCollection collectionInfo = new DocumentCollection();
                    collectionInfo.Id = collectionName;

                    // Optionally, you can configure the indexing policy of a collection. Here we configure collections for maximum query flexibility 
                    // including string range queries. 
                    collectionInfo.IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 });

                    // DocumentDB collections can be reserved with throughput specified in request units/second. 1 RU is a normalized request equivalent to the read
                    // of a 1KB document.  Here we create a collection with 400 RU/s. 
                    await documentClient.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}