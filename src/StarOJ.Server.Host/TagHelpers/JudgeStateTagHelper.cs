using StarOJ.Server.API.Clients;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StarOJ.Core.Judgers;
using StarOJ.Core.Helpers;

namespace StarOJ.Server.Host.TagHelpers
{
    [HtmlTargetElement("judge-state")]
    public class JudgeStateTagHelper : TagHelper
    {
        private readonly IStringLocalizer<JudgeStateTagHelper> _localizer;

        public JudgeStateTagHelper(IStringLocalizer<JudgeStateTagHelper> localizer)
        {
            _localizer = localizer;
        }

        static string GetStateColor(JudgeState state)
        {
            switch (state)
            {
                case JudgeState.Accepted:
                    return "forestgreen";
                case JudgeState.WrongAnswer:
                    return "red";
                case JudgeState.CompileError:
                    return "#004488";
                case JudgeState.TimeLimitExceeded:
                case JudgeState.MemoryLimitExceeded:
                    return "sandybrown";
                case JudgeState.RuntimeError:
                    return "darkorchid";
                case JudgeState.SystemError:
                    return "grey";
                case JudgeState.Pending:
                    return "#6cf";
                case JudgeState.Judging:
                    return "blue";
                case JudgeState.Compiling:
                    return "#f1e05a";
            }
            return "black";
        }

        public JudgeState Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("style", $"color:{GetStateColor(Value)}");

            var icon = new TagBuilder("i");
            switch (Value)
            {
                case JudgeState.Accepted:
                    icon.Attributes["class"] = "fa fa-check";
                    break;
                case JudgeState.WrongAnswer:
                    icon.Attributes["class"] = "fa fa-times";
                    break;
                case JudgeState.TimeLimitExceeded:
                    icon.Attributes["class"] = "fa fa-clock";
                    break;
                case JudgeState.MemoryLimitExceeded:
                    icon.Attributes["class"] = "fa fa-microchip";
                    break;
                case JudgeState.Pending:
                    icon.Attributes["class"] = "fa fa-hourglass-half";
                    break;
                case JudgeState.Judging:
                    icon.Attributes["class"] = "fa fa-cog fa-spin";
                    break;
                case JudgeState.Compiling:
                    icon.Attributes["class"] = "fa fa-cog fa-spin";
                    break;
                case JudgeState.CompileError:
                    icon.Attributes["class"] = "fa fa-code";
                    break;
                case JudgeState.RuntimeError:
                    icon.Attributes["class"] = "fa fa-bomb";
                    break;
                case JudgeState.SystemError:
                    icon.Attributes["class"] = "fa fa-server";
                    break;
            }
            output.Content.AppendHtml(icon);

            var text = new TagBuilder("span");
            text.Attributes["style"] = "margin-left: 10px";
            text.InnerHtml.Append(_localizer[JudgeStateHelper.DisplayNames[Value]]);
            output.Content.AppendHtml(text);
            base.Process(context, output);
        }
    }
}
