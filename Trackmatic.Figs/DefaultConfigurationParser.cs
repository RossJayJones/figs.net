using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Trackmatic.Figs
{
    public class DefaultConfigurationParser : IParseConfiguration
    {
        private readonly Encoding _encoding;

        public DefaultConfigurationParser(Encoding encoding)
        {
            _encoding = encoding;
        }

        public string Parse(Stream stream, Dictionary<string, string> settings)
        {
            var symbols = Extract(stream);
            using (var output = new MemoryStream())
            using (var writer = new StreamWriter(output))
            {
                writer.AutoFlush = true;
                stream.Position = Bom.GetCursor(stream);
                var buffer = new byte[1];
                while (stream.Read(buffer, 0, 1) > 0)
                {
                    if (symbols.Any(p => p.IsWithin(stream.Position)))
                    {
                        foreach (var symbol in symbols.Where(p => p.Cursor == stream.Position))
                        {
                            if (!settings.ContainsKey(symbol.Key))
                                throw new KeyNotFoundException(string.Format("A setting has not been provided for key {0} in the json file", symbol.Key));
                            writer.Write(new[] { (char)buffer[0] });
                            stream.Position += symbol.Length;
                            writer.Write(settings[symbol.Key]);
                        }
                    }
                    else
                    {
                        writer.Write(_encoding.GetChars(buffer));
                    }
                }
                return _encoding.GetString(output.ToArray());
            }
        }

        private List<Symbol> Extract(Stream stream)
        {
            var symbols = new List<Symbol>();
            var stack = new Stack<Symbol>();
            var buffer = new byte[1];
            int cursor = 0;
            stream.Position = 0;
            while (stream.Read(buffer, 0, 1) > 0)
            {
                if (buffer[0] == '{')
                {
                    if (stack.Count > 0)
                    {
                        stack.Peek().Append(buffer[0]);
                    }
                    else
                    {
                        stack.Push(new Symbol(_encoding, cursor, buffer[0]));
                    }
                }
                else if (stack.Count > 0)
                {
                    var current = stack.Peek();

                    if (!current.IsValid())
                    {
                        stack.Pop();
                    }
                    else if (current.IsComplete())
                    {
                        symbols.Add(stack.Pop());
                    }
                    else
                    {
                        current.Append(buffer[0]);
                    }
                }

                cursor++;
            }

            return symbols;
        }

        private static class Bom
        {
            public static int GetCursor(Stream stream)
            {
                // UTF-32, big-endian
                if (IsMatch(stream, new byte[] {0x00, 0x00, 0xFE, 0xFF}))
                    return 4;
                // UTF-32, little-endian
                if (IsMatch(stream, new byte[] { 0xFF, 0xFE, 0x00, 0x00 }))
                    return 4;
                // UTF-16, big-endian
                if (IsMatch(stream, new byte[] { 0xFE, 0xFF }))
                    return 2;
                // UTF-16, little-endian
                if (IsMatch(stream, new byte[] { 0xFF, 0xFE }))
                    return 2;
                // UTF-8
                if (IsMatch(stream, new byte[] { 0xEF, 0xBB, 0xBF }))
                    return 3;
                return 0;
            }

            private static bool IsMatch(Stream stream, byte[] match)
            {
                stream.Position = 0;
                var buffer = new byte[match.Length];
                stream.Read(buffer, 0, buffer.Length);
                return !buffer.Where((t, i) => t != match[i]).Any();
            }
        }

        private struct Symbol
        {
            public readonly int Cursor;

            public readonly List<byte> Data;

            public readonly Encoding Encoding;

            public Symbol(Encoding encoding, int cursor, byte key)
            {
                Encoding = encoding;
                Cursor = cursor;
                Data = new List<byte> {key};
            }

            public int Length
            {
                get { return Data.Count; }
            }

            public void Append(byte value)
            {
                Data.Add(value);
            }

            public bool IsValid()
            {
                return Value.StartsWith("{{");
            }

            public bool IsComplete()
            {
                return Value.Contains("}}");
            }

            public string Value
            {
                get { return Encoding.GetString(Data.ToArray()); }
            }

            public string Key
            {
                get { return Value.Replace("{{", string.Empty).Replace("}}", string.Empty); }
            }

            public bool IsWithin(long cursor)
            {
                return cursor >= Cursor && cursor <= (Cursor + Length);
            }
        }
    }
}
