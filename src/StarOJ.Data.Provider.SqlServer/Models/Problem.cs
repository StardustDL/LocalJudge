namespace StarOJ.Data.Provider.SqlServer.Models
{
    public class Problem
    {
        #region Metadata

        public int Id { get; set; }

        public string Name { get; set; }

        public int UserId { get; set; }

        public string Source { get; set; }

        #endregion

        #region Description

        public string Description { get; set; }

        public string Input { get; set; }

        public string Output { get; set; }

        public string Hint { get; set; }

        #endregion
    }
}
