namespace StarOJ.Core.Identity
{
    public class RoleMetadata : IHasId<string>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
