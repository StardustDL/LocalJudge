using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("timeoffset-display")]
    public class TimeOffsetDisplayTagHelper : TagHelper
    {
        private readonly IStringLocalizer<TimeOffsetDisplayTagHelper> _localizer;

        public TimeOffsetDisplayTagHelper(IStringLocalizer<TimeOffsetDisplayTagHelper> localizer)
        {
            _localizer = localizer;
        }

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
                output.Content.Append($"1 {_localizer["month ago"]}");
            }
            else if (tspan.TotalDays > 14)
            {
                output.Content.Append($"2 {_localizer["weeks ago"]}");
            }
            else if (tspan.TotalDays > 7)
            {
                output.Content.Append($"1 {_localizer["weeks ago"]}");
            }
            else if (tspan.TotalDays > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalDays)} {_localizer["days ago"]}");
            }
            else if (tspan.TotalHours > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalHours)} {_localizer["hours ago"]}");
            }
            else if (tspan.TotalMinutes > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalMinutes)} {_localizer["minutes ago"]}");
            }
            else if (tspan.TotalSeconds > 1)
            {
                output.Content.Append($"{(int)Math.Floor(tspan.TotalSeconds)} {_localizer["seconds ago"]}");
            }
            else
            {
                output.Content.Append($"{_localizer["just"]}");
            }
            base.Process(context, output);
        }
    }
}
