using System;
using System.IO;
using System.Xml;
using Microsoft.Web.XmlTransform;

namespace transform_config
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                Console.Error.WriteLine("Usage: transform-config <config-path> <transform-path> [<output-path>]");
                return 1;
            }

            var configPath = args[0];
            var transformPath = args[1];
            var outputPath = args.Length == 3 ? args[2] : default;

            var (result, xml) = TransformConfig(configPath, transformPath);

            if (result == 1)
            {
                Console.Error.WriteLine("Transformation failed");
                return 1;
            }

            // TODO: Option to return XML as a string in stdout
            // Console.WriteLine(xml.AsXmlString());

            xml.Save(outputPath ?? configPath);

            return 0;
        }

        private static (int result, XmlTransformableDocument xml) TransformConfig(
            string configPath,
            string transformPath)
        {
            var document = new XmlTransformableDocument {
                PreserveWhitespace = true
            };

            document.Load(configPath);

            var transformation = new XmlTransformation(transformPath);

            return transformation.Apply(document)
                ? (0, document)
                : (1, default);
        }

        public static string AsXmlString(this XmlDocument document)
        {
            using (var sw = new StringWriter())
            using (var xw = XmlWriter.Create(sw))
            {
                document.WriteTo(xw);
                xw.Flush();

                return sw.GetStringBuilder().ToString();
            }
        }
    }
}
