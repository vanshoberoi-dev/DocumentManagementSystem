
namespace DMS.Models.Entities.Document
{
    public class DocumentTag
    {
        public Guid DocumentId { get; set; }
        public DocumentEntity? Document { get; set; }

        public Guid TagId { get; set; }
        public Tag.Tag? Tag { get; set; }
    }
}