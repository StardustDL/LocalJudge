using Microsoft.AspNetCore.Razor.TagHelpers;
using StarOJ.Core.Helpers;

namespace StarOJ.Server.Host.TagHelpers
{
    [HtmlTargetElement("memory-display")]
    public class MemoryDisplayTagHelper : TagHelper
    {
        public long? Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (!Value.HasValue || Value < 0)
                output.Content.Append("N/A");
            else if (Value < MemoryValueHelper.KB)
                output.Content.Append(string.Format("{0} B", Value));
            else if (Value < MemoryValueHelper.MB)
                output.Content.Append(string.Format("{0:f2} KB", (double)Value / MemoryValueHelper.KB));
            else if (Value < MemoryValueHelper.GB)
                output.Content.Append(string.Format("{0:f2} MB", (double)Value / MemoryValueHelper.MB));
            else
                output.Content.Append(string.Format("{0:f2} GB", (double)Value / MemoryValueHelper.GB));
            base.Process(context, output);
        }
    }
}
