using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace StarOJ.Core.Helpers
{
    public static class TextIO
    {
        public static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(false);

        public static Stream ToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }

        public static async Task<string> ToString(Stream str)
        {
            using(var sr = new StreamReader(str))
            {
                return await sr.ReadToEndAsync();
            }
        }

        public static DataPreview GetPreviewInUTF8(string path, int maxLength)
        {
            using (var fs = File.OpenRead(path))
            using (var br = new BinaryReader(fs))
            {
                var bs = br.ReadBytes((int)Math.Min(maxLength, fs.Length));
                return new DataPreview
                {
                    Content = UTF8WithoutBOM.GetString(bs),
                    RemainBytes = fs.Length - bs.Length,
                };
            }
        }

        public static string ReadAllInUTF8(string path)
        {
            return File.ReadAllText(path, UTF8WithoutBOM);
        }

        public static void WriteAllInUTF8(string path, string content)
        {
            File.WriteAllText(path, content, UTF8WithoutBOM);
        }

        public static async Task<string> ReadAllInUTF8Async(string path)
        {
            using (var sr = new StreamReader(path, UTF8WithoutBOM))
                return await sr.ReadToEndAsync();
        }

        public static async Task WriteAllInUTF8Async(string path, string content)
        {
            using (var sw = new StreamWriter(path, false, UTF8WithoutBOM))
                await sw.WriteAsync(content);
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
