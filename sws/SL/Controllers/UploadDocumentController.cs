using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sws.BLL;
using sws.DAL;
using sws.DAL.Entities;
using sws.DAL.Repositories;
using sws.SL.DTOs;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ.Client.Events;

namespace sws.SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDocumentController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IDocumentLogic _documentLogic;

        public UploadDocumentController(ILogger<UploadDocumentController> logger,
                                        IDocumentLogic documentLogic)
        {
            _logger = logger;
            _documentLogic = documentLogic;
        }


        /// <summary>
        /// Retrieves all uploaded documents
        /// </summary>

        /// <remarks>
        /// Returns all uploaded documents for all the users
        /// to be implemented/repaired
        /// </remarks>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DownloadDocumentDTO>>> GetUploadedDocuments()
        {
            _logger.LogInformation(200, "getting all documents");
            return _documentLogic.GetAll();
            // return await _context.UploadedDocuments.ToListAsync();
        }

        /// <summary>
        /// Retrieves an uploaded document
        /// </summary>

        /// <remarks>
        /// Given a unique ID, retrieves the correspondant document if existing
        /// to be implemented/repaired
        /// </remarks>
        // GET: api/UploadDocument/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DownloadDocumentDTO>> GetUploadDocument(long id)
        {
            _logger.LogInformation(200, "getting single document");
            

            var document = await _documentLogic.GetByIdAsync(id);
            if (document == null) {
                return NotFound();
            } else {
                return document;
            }
        }

        /// <summary>
        /// Updates a target document
        /// </summary>

        /// <remarks>
        /// To update a document, the provided file must have the same ID
        /// to be implemented/repaired
        /// </remarks>
        // PUT: api/UploadDocument/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUploadDocument(UploadDocumentDTO uploadDocument)
        {
            var document = _documentLogic.Put(uploadDocument);
            if (document == null)
            {
                return NotFound();
            }
            else
            {
                return NoContent();
            }
        }

        /// <summary>
        /// Uploads new document
        /// </summary>

        /// <remarks>
        /// Uploads new file into the DB
        /// </remarks>
        // POST: api/UploadDocument
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // TODO: reject if document with same ID exists.
        [HttpPost]
        public async Task<IActionResult> PostUploadDocument([FromForm] UploadDocumentDTO uploadDocument)
        {
            // should await?
            // Passing DTO to Business Layer

            _documentLogic.Add(uploadDocument);

            //_context.UploadedDocuments.Add(uploadDocument);
            //await _context.SaveChangesAsync();

            _logger.LogInformation(200, "we got inside of the controller. Name: " + uploadDocument.Name);

            return Ok("File uploaded successfullly.");
        }

        /// <summary>
        /// Deletes document
        /// </summary>

        /// <remarks>
        /// Given an ID, it deletes the correspondant file
        /// </remarks>
        // DELETE: api/UploadDocument/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUploadDocument(long id)
        {
            if (_documentLogic.PopById(id) == null)
                return NotFound();
            else
                return NoContent();
        }
    }
}
