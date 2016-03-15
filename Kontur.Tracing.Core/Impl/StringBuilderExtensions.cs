using System.Collections.Generic;
using System.Text;

namespace Kontur.Tracing.Core.Impl
{
    internal static class StringBuilderExtensions
    {
        public static StringBuilder AppendPlainObject(this StringBuilder builder, string name, IEnumerable<KeyValuePair<string, string>> fields)
        {
            builder
                .AppendQuotedString(name)
                .Append(':')
                .Append('{');

            bool isFirst = true;

            foreach (var pair in fields)
            {
                if (isFirst)
                {
                    isFirst = false;
                }
                else
                {
                    builder.AppendComma();
                }

                builder.AppendKeyValuePair(pair.Key, pair.Value);
            }

            return builder.Append('}');
        }

        public static StringBuilder AppendKeyValuePair(this StringBuilder builder, string key, string value)
        {
            return builder
                .AppendQuotedString(key)
                .Append(':')
                .AppendQuotedString(value);
        }

        public static StringBuilder AppendQuotedString(this StringBuilder builder, string value)
        {
            return builder
                .Append('"')
                .Append(value)
                .Append('"');
        }

        public static StringBuilder AppendComma(this StringBuilder builder)
        {
            return builder.Append(',');
        }
    }
}