namespace sws.SL.DTOs

{
    public class UploadDocumentDTO
    {
        // TODO erase
        //public long Id { get; set; }
        public string? Name { get; set; }
        //public string? Content { get; set; }
        public IFormFile File { get; set; }
    }
}

