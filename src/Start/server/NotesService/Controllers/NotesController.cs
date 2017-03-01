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
            throw new NotImplementedException();
        }

        [Authorize]
        public async Task<Note> Post(Note note)
        {
            throw new NotImplementedException();
        }

        // POST api/values
        [HttpPatch]
        [HttpPut]
        [Authorize]
        public async Task<Note> Put(string id, Note note)
        {
            throw new NotImplementedException();
        }

        [Authorize]
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            throw new NotImplementedException();
        }
    }
}
