using LocalJudge.Server.Host.APIClients;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace LocalJudge.Server.Host.TagHelpers
{
    [HtmlTargetElement("prolang-display")]
    public class ProlangDisplayTagHelper : TagHelper
    {
        static string GetLanguageColor(ProgrammingLanguage lang)
        {
            switch (lang)
            {
                case ProgrammingLanguage.CSharp:
                    return "#178600";
                case ProgrammingLanguage.C:
                    return "#555555";
                case ProgrammingLanguage.Cpp:
                    return "#f34b7d";
                case ProgrammingLanguage.Java:
                    return "#b07219";
                case ProgrammingLanguage.Python:
                    return "#3572A5";
            }
            return "white";
        }

        public ProgrammingLanguage Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.TagMode = TagMode.StartTagAndEndTag;

            var color = new TagBuilder("span");
            color.Attributes["style"] = $"display:inline-block; border-radius:50%; height:12px; width:12px; position:relative; background-color:{GetLanguageColor(Value)}";
            output.Content.AppendHtml(color);

            var name = new TagBuilder("span");
            name.Attributes["style"] = "margin-left: 5px";
            switch (Value)
            {
                case ProgrammingLanguage.C:
                    name.InnerHtml.Append("C");
                    break;
                case ProgrammingLanguage.Cpp:
                    name.InnerHtml.Append("C++");
                    break;
                case ProgrammingLanguage.Python:
                    name.InnerHtml.Append("Python");
                    break;
                case ProgrammingLanguage.Java:
                    name.InnerHtml.Append("Java");
                    break;
                case ProgrammingLanguage.CSharp:
                    name.InnerHtml.Append("C#");
                    break;
            }
            output.Content.AppendHtml(name);
            base.Process(context, output);
        }
    }
}
