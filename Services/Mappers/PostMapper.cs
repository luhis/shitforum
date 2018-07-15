using Services.Dtos;

namespace Services.Mappers
{
    public static class PostMapper
    {
        public static PostOverView Map(Domain.Post p, Optional.Option< Domain.File> f)
        {
            return new PostOverView(p.Id, p.Created, p.Name, p.IpAddress, p.Comment, f);
        }
    }
}
