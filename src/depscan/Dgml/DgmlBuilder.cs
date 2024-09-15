using System.Text;
using System.Xml;

namespace Dgml
{
    public class DgmlBuilder()
    {
        public IList<Node> Nodes { get; private set; } = [];
        public IList<Link> Links { get; private set; } = [];

        public void Save(string filename)
        {
            var xmlSettings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true
            };

            using var writer = XmlWriter.Create(filename, xmlSettings);
            writer.WriteStartDocument();
            writer.WriteStartElement("DirectedGraph", "http://schemas.microsoft.com/vs/2009/dgml");
            writer.WriteStartElement("Nodes");
            this.WriteNodes(writer);
            writer.WriteEndElement();

            writer.WriteStartElement("Links");
            this.WriteLinks(writer);
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void WriteNodes(XmlWriter writer)
        {
            foreach (var node in this.Nodes)
            {
                writer.WriteStartElement("Node");
                writer.WriteAttributeString("Id", node.Id);
                writer.WriteAttributeString("Label", node.Name);
                writer.WriteEndElement();
            }
        }

        private void WriteLinks(XmlWriter writer)
        {
            foreach (var link in this.Links)
            {
                writer.WriteStartElement("Link");
                writer.WriteAttributeString("Source", link.Source);
                writer.WriteAttributeString("Target", link.Target);
                writer.WriteEndElement();
            }
        }
    }
}