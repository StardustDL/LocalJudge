using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LocalJudge.Core.Helpers
{
    internal static class TextIO
    {
        public static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(false);

        public static string ReadAllInUTF8(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8);
        }

        public static void WriteAllInUTF8(string path, string content)
        {
            File.WriteAllText(path, content, Encoding.UTF8);
        }

        public static void ConvertToLF(TextReader reader, TextWriter writer)
        {
            while (reader.Peek() != -1)
                writer.Write(reader.ReadLine() + "\n");
        }

        public static string ConvertToLF(TextReader reader)
        {
            StringBuilder sb = new StringBuilder();
            while (reader.Peek() != -1)
                sb.Append(reader.ReadLine() + "\n");
            return sb.ToString();
        }

        public static IEnumerable<string> SplitLines(TextReader reader)
        {
            while (reader.Peek() != -1)
                yield return reader.ReadLine();
        }
    }
}
