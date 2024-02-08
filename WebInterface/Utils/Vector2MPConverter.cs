using MessagePack.Formatters;
using MessagePack;
using Microsoft.Xna.Framework;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebInterface.Utils
{
    public class Vector2MPConverter : IMessagePackFormatter<Vector2>
    {
        public Vector2 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
        {

            if (reader.TryReadNil())
            {
                return Vector2.Zero;
            }

            float x = 0, y = 0;

            options.Security.DepthStep(ref reader);

            int count = reader.ReadMapHeader();
            for (int i = 0; i < count; i++)
            {
                string key = reader.ReadString() ?? string.Empty;
                switch (key)
                {
                    case "x":
                        x = reader.ReadSingle();
                        break;
                    case "y":
                        y = reader.ReadSingle();
                        break;
                    default:
                        reader.Skip();
                        break;
                }
            }

            reader.Depth--;
            return new Vector2(x, y);
        }

        public void Serialize(ref MessagePackWriter writer, Vector2 value, MessagePackSerializerOptions options)
        {
            writer.WriteMapHeader(2);
            writer.Write("x");
            writer.Write(value.X);
            writer.Write("y");
            writer.Write(value.Y);
        }
    }


    public class Vector2MPResolver : IFormatterResolver
    {
        readonly Vector2MPConverter defaultConverter = new Vector2MPConverter();

        readonly static Vector2MPResolver instance = new Vector2MPResolver();
        public static Vector2MPResolver Instance => instance;

        IMessagePackFormatter<T>? IFormatterResolver.GetFormatter<T>()
        {
            if (typeof(T) == typeof(Vector2))
            {
                return defaultConverter as IMessagePackFormatter<T>;
            }
            else
            {
                return null;
            }
        }
    }

}
