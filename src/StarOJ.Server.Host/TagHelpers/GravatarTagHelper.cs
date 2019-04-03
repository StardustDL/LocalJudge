using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace StarOJ.Server.Host.TagHelpers
{
    [HtmlTargetElement("img", Attributes = nameof(Gravatar))]
    public class GravatarTagHelper : TagHelper
    {
        static string ComputeHash(string input)
        {
            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();
            byte[] inputArray = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hashedArray = MD5.ComputeHash(inputArray);
            MD5.Clear();
            return BitConverter.ToString(hashedArray).Replace("-", "");
        }

        public string Gravatar { get; set; }

        public uint Size { get; set; } = 80;

        public string Default { get; set; } = "mp";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string email = Gravatar.Trim().ToLower();
            var src = $"https://www.gravatar.com/avatar/{ComputeHash(email).ToLower()}?size={Size}&d={Default}";
            output.Attributes.SetAttribute("src", src);
            base.Process(context, output);
        }
    }
}
