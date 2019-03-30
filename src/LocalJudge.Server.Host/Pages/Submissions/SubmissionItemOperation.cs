namespace LocalJudge.Server.Host.Pages.Submissions
{
    public enum SubmissionItemOperationType
    {
        Rejudge,
        Delete,
    }

    public class SubmissionItemOperation
    {
        public SubmissionItemOperationType Type { get; set; }

        public string ID { get; set; }
    }
}