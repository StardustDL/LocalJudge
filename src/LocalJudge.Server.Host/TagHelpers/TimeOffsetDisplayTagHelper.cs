using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("timeoffset-display")]
    public class TimeOffsetDisplayTagHelper : TagHelper
    {
        public DateTimeOffset Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            var tspan = DateTimeOffset.Now - Value;
            if (tspan.TotalDays > 60)
            {
                output.Content.Append(Value.ToString("F"));
            }
            else if (tspan.TotalDays > 30)
            {
                output.Content.Append("1 month ago");
            }
            else if (tspan.TotalDays > 14)
            {
                output.Content.Append("2 weeks ago");
            }
            else if (tspan.TotalDays > 7)
            {
                output.Content.Append("1 weeks ago");
            }
            else if (tspan.TotalDays > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalDays)} days ago");
            }
            else if (tspan.TotalHours > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalHours)} hours ago");
            }
            else if (tspan.TotalMinutes > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalMinutes)} minutes ago");
            }
            else if (tspan.TotalSeconds > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalSeconds)} seconds ago");
            }
            else
            {
                output.Content.Append("just");
            }
            base.Process(context, output);
        }
    }
}
