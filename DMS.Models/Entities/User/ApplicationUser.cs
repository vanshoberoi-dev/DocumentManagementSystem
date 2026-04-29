using Microsoft.AspNetCore.Identity;
using DMS.Models.Entities.Document;

namespace DMS.Models.Entities.User
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<DocumentEntity> Documents { get; set; } = new List<DocumentEntity>();
    }
}