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

namespace sws.SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDocumentController : ControllerBase
    {
        private readonly UploadDocumentContext _context;
        private readonly ILogger _logger;
        private readonly IDocumentLogic _documentLogic;

        public UploadDocumentController(UploadDocumentContext context,
                                        ILogger<UploadDocumentController> logger,
                                        IDocumentLogic documentLogic)
        {
            _context = context;
            _logger = logger;
            _documentLogic = documentLogic;
        }

        // GET: api/UploadDocument
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UploadDocument>>> GetUploadedDocuments()
        {
            _logger.LogInformation(200, "getting all documents");
            return await _context.UploadedDocuments.ToListAsync();
        }

        // GET: api/UploadDocument/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UploadDocument>> GetUploadDocument(long id)
        {
            var uploadDocument = await _context.UploadedDocuments.FindAsync(id);

            if (uploadDocument == null)
            {
                return NotFound();
            }

            return uploadDocument;
        }

        // PUT: api/UploadDocument/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUploadDocument(long id, UploadDocument uploadDocument)
        {
            if (id != uploadDocument.Id)
            {
                return BadRequest();
            }

            _context.Entry(uploadDocument).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UploadDocumentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UploadDocument
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UploadDocumentDTO>> PostUploadDocument(UploadDocumentDTO uploadDocument)
        {
            // should await?
            // Passing DTO to Business Layer
            _documentLogic.Add(uploadDocument);
            //_context.UploadedDocuments.Add(uploadDocument);
            //await _context.SaveChangesAsync();

            return CreatedAtAction("GetUploadDocument", new { id = uploadDocument.Id }, uploadDocument);
        }

        // DELETE: api/UploadDocument/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUploadDocument(long id)
        {
            var uploadDocument = await _context.UploadedDocuments.FindAsync(id);
            if (uploadDocument == null)
            {
                return NotFound();
            }

            _context.UploadedDocuments.Remove(uploadDocument);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UploadDocumentExists(long id)
        {
            return _context.UploadedDocuments.Any(e => e.Id == id);
        }
    }
}
