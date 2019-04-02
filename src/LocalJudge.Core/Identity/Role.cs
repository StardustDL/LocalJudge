namespace LocalJudge.Core.Identity
{
    public class Role : IHasId<string>
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }
}
