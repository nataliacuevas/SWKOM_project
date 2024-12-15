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
using log4net;

namespace sws.SL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadDocumentController : ControllerBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(UploadDocumentController));
        private readonly IDocumentLogic _documentLogic;

        public UploadDocumentController(IDocumentLogic documentLogic)
        {
           
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
            log.Info("Request to retrieve all documents received.");
            try
            {
                var documents = _documentLogic.GetAll();
                if (documents == null || !documents.Any())
                {
                    log.Warn("No documents found.");
                    return Ok(new List<DownloadDocumentDTO>());
                }

                log.Info($"Successfully retrieved {documents.Count} documents.");
                return Ok(documents);
            }
            catch (BusinessLogicException ex)
            {
                log.Error("Business logic error while retrieving all documents.", ex);
                throw new ServiceException("Error in retrieving all documents in the service layer.", ex);
            }
            catch (Exception ex)
            {
                log.Error("Unexpected error while retrieving all documents.", ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Retrieves an uploaded document
        /// </summary>

        /// <remarks>
        /// Given a unique ID, retrieves the correspondant document if existing
        /// </remarks>
        [HttpGet("{id}")]
        public async Task<ActionResult<DownloadDocumentDTO>> GetUploadDocument(long id)
        {
            log.Info($"Request to retrieve document with ID {id}.");
            try
            {
                var document = await _documentLogic.GetByIdAsync(id);
                if (document == null)
                {
                    log.Warn($"Document with ID {id} not found.");
                    return NotFound();
                }

                log.Info($"Successfully retrieved document with ID {id}.");
                return Ok(document); // Explicitly wrap the result in OkObjectResult
            }
            catch (Exception ex)
            {
                log.Error($"Error retrieving document with ID {id}.", ex);
                return StatusCode(500, "Internal server error while fetching document.");
            }
        }


        /// <summary>
        /// Uploads new document
        /// </summary>

        /// <remarks>
        /// Uploads new file into the DB
        /// </remarks>
        // POST: api/UploadDocument
        [HttpPost]
        public async Task<IActionResult> PostUploadDocument([FromForm] UploadDocumentDTO uploadDocument)
        {
            // Passing DTO to Business Layer
            log.Info("Request to upload a new document.");
            try
            {
                await _documentLogic.Add(uploadDocument);
                log.Info($"Document '{uploadDocument.Name}' uploaded successfully.");
                return Ok("File uploaded successfully");
            }
            catch (BusinessLogicException ex)
            {
                log.Error("Business logic error during document upload.", ex);
                return StatusCode(500, "Internal server error in BusinessLogic.");
            }
            catch (Exception ex)
            {
                log.Error("Error uploading document.", ex);
               
                return StatusCode(500, "Internal server error while uploading document.");
            }
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
            log.Info($"Request to delete document with ID {id}.");
            try
            {
                if (_documentLogic.PopById(id) == null)
                {
                    log.Warn($"Document with ID {id} not found for deletion.");
                    return NotFound();
                }
                log.Info($"Document with ID {id} deleted successfully.");
                return NoContent();

            }
            catch(Exception ex)
            {
                log.Error($"Error deleting document with ID {id}.", ex);
                return StatusCode(500, "Internal server error while deleting document.");
            }
       
        }
    }
}
