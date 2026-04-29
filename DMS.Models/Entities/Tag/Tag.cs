using DMS.Models.Entities.Document;

namespace DMS.Models.Entities.Tag
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;

        public ICollection<DocumentTag> DocumentTags { get; set; } = new List<DocumentTag>();
    }
}