using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("timespan-display")]
    public class TimeSpanDisplayTagHelper : TagHelper
    {
        public TimeSpan Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            bool haspre = false;
            if (Value.Days > 0)
            {
                output.Content.Append(string.Format("{0} d", Value.Days));
                haspre = true;
            }
            if (Value.Hours > 0)
            {
                if (haspre) output.Content.Append(" ");
                output.Content.Append(string.Format("{0} h", Value.Hours));
                haspre = true;
            }
            if (Value.Minutes > 0)
            {
                if (haspre) output.Content.Append(" ");
                output.Content.Append(string.Format("{0} min", Value.Minutes));
                haspre = true;
            }
            if (Value.Seconds > 0)
            {
                if (haspre) output.Content.Append(" ");
                output.Content.Append(string.Format("{0} s", Value.Seconds));
                haspre = true;
            }
            if (Value.Milliseconds > 0)
            {
                if (haspre) output.Content.Append(" ");
                output.Content.Append(string.Format("{0} ms", Value.Milliseconds));
            }
            base.Process(context, output);
        }
    }
}
