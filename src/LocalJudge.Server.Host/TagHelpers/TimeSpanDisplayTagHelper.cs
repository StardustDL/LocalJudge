using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("timespan-display")]
    public class TimeSpanDisplayTagHelper : TagHelper
    {
        public TimeSpan? Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            if (Value.HasValue)
            {
                var value = Value.Value;
                bool haspre = false;
                if (value.Days > 0)
                {
                    output.Content.Append(string.Format("{0} d", value.Days));
                    haspre = true;
                }
                if (value.Hours > 0)
                {
                    if (haspre) output.Content.Append(" ");
                    output.Content.Append(string.Format("{0} h", value.Hours));
                    haspre = true;
                }
                if (value.Minutes > 0)
                {
                    if (haspre) output.Content.Append(" ");
                    output.Content.Append(string.Format("{0} min", value.Minutes));
                    haspre = true;
                }
                if (value.Seconds > 0)
                {
                    if (haspre) output.Content.Append(" ");
                    output.Content.Append(string.Format("{0} s", value.Seconds));
                    haspre = true;
                }
                if (value.Milliseconds > 0)
                {
                    if (haspre) output.Content.Append(" ");
                    output.Content.Append(string.Format("{0} ms", value.Milliseconds));
                }
                else
                {
                    if (!haspre) output.Content.Append(string.Format("{0} ms", value.Milliseconds));
                }
            }
            else
            {
                output.Content.Append("N/A");
            }
            base.Process(context, output);
        }
    }
}
