
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private List<Symbol> Extract(Stream stream)
        {
            List<Symbol> list = new List<Symbol>();
            Stack<Symbol> stack = new Stack<Symbol>();
            byte[] buffer = new byte[1];
            int cursor = 0;
            stream.Position = 0L;
            while (stream.Read(buffer, 0, 1) > 0)
            {
                if (buffer[0] == 0x7b)
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
                    Symbol symbol = stack.Peek();
                    if (!symbol.IsValid())
                    {
                        stack.Pop();
                    }
                    else if (symbol.IsComplete())
                    {
                        list.Add(stack.Pop());
                    }
                    else
                    {
                        symbol.Append(buffer[0]);
                    }
                }
                cursor++;
            }
            return list;
        }

        public string Parse(Stream stream, Dictionary<string, string> settings)
        {
            Func<Symbol, bool> predicate = null;
            Func<Symbol, bool> func2 = null;
            string str;
            List<Symbol> source = Extract(stream);
            using (MemoryStream stream2 = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(stream2))
                {
                    writer.AutoFlush = true;
                    stream.Position = Bom.GetCursor(stream);
                    byte[] buffer = new byte[1];
                    while (stream.Read(buffer, 0, 1) > 0)
                    {
                        if (predicate == null)
                        {
                            predicate = p => p.IsWithin(stream.Position);
                        }
                        if (source.Any(predicate))
                        {
                            if (func2 == null)
                            {
                                func2 = p => p.Cursor == stream.Position;
                            }
                            foreach (Symbol symbol in source.Where(func2))
                            {
                                if (!settings.ContainsKey(symbol.Key))
                                {
                                    throw new KeyNotFoundException($"A setting has not been provided for key {symbol.Key} in the json file");
                                }
                                writer.Write(new[] { Convert.ToChar(buffer[0]) });
                                stream.Position += symbol.Length;
                                writer.Write(settings[symbol.Key]);
                            }
                        }
                        else
                        {
                            writer.Write(_encoding.GetChars(buffer));
                        }
                    }
                    str = _encoding.GetString(stream2.ToArray());
                }
            }
            return str;
        }

        private static class Bom
        {
            public static int GetCursor(Stream stream)
            {
                byte[] match = new byte[4];
                match[2] = 0xfe;
                match[3] = 0xff;
                if (IsMatch(stream, match))
                {
                    return 4;
                }
                match = new byte[4];
                match[0] = 0xff;
                match[1] = 0xfe;
                if (IsMatch(stream, match))
                {
                    return 4;
                }
                if (IsMatch(stream, new byte[] { 0xfe, 0xff }))
                {
                    return 2;
                }
                if (IsMatch(stream, new byte[] { 0xff, 0xfe }))
                {
                    return 2;
                }
                if (IsMatch(stream, new byte[] { 0xef, 0xbb, 0xbf }))
                {
                    return 3;
                }
                return 0;
            }

            private static bool IsMatch(Stream stream, byte[] match)
            {
                stream.Position = 0L;
                byte[] buffer = new byte[match.Length];
                stream.Read(buffer, 0, buffer.Length);
                return !buffer.Where((t, i) => t != match[i]).Any();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Symbol
        {
            public readonly int Cursor;
            public readonly List<byte> Data;
            public readonly Encoding Encoding;
            public Symbol(Encoding encoding, int cursor, byte key)
            {
                Encoding = encoding;
                Cursor = cursor;
                List<byte> list = new List<byte> {
                    key
                };
                Data = list;
            }

            public int Length => Data.Count;
            public void Append(byte value)
            {
                Data.Add(value);
            }

            public bool IsValid() => Value.StartsWith("{{");

            public bool IsComplete() => Value.Contains("}}");

            private string Value => Encoding.GetString(Data.ToArray());

            public string Key => Value.Replace("{{", string.Empty).Replace("}}", string.Empty);

            public bool IsWithin(long cursor) => (cursor >= Cursor) && (cursor <= Cursor + Length);
        }
    }
}

