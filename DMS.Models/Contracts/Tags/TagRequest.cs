using MediatR;

namespace DMS.Models.Contracts.Tags
{
    public class CreateTagRequest : IRequest<TagResponse>
    {
        public string Name { get; set; } = string.Empty;
    }

    public class GetAllTagsRequest : IRequest<List<TagResponse>>
    {
    }
}