using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("memory-display")]
    public class MemoryDisplayTagHelper : TagHelper
    {
        const long KB = 1024, MB = 1024 * 1024, GB = 1024 * 1024 * 1024;

        public long Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            if(Value <= 0)
                output.Content.Append("N/A");
            else if (Value < KB)
                output.Content.Append(string.Format("{0:f2} B", (double)Value));
            else if (Value < MB)
                output.Content.Append(string.Format("{0:f2} KB", (double)Value / KB));
            else if (Value < GB)
                output.Content.Append(string.Format("{0:f2} MB", (double)Value / MB));
            else
                output.Content.Append(string.Format("{0:f2} GB", (double)Value / GB));
            base.Process(context, output);
        }
    }
}
