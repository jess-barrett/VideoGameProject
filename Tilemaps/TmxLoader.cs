using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject2.Tilemaps
{
    // Custom Content Importer for TMX tilemap files
    public class TmxLoader
    {
        public static Tilemap Load(string tmxFilePath, ContentManager content)
        {
            XDocument doc = XDocument.Load(tmxFilePath);
            XElement mapElement = doc.Element("map");

            Tilemap tilemap = new Tilemap();

            tilemap.Width = int.Parse(mapElement.Attribute("width").Value);
            tilemap.Height = int.Parse(mapElement.Attribute("height").Value);
            tilemap.TileWidth = int.Parse(mapElement.Attribute("tilewidth").Value);
            tilemap.TileHeight = int.Parse(mapElement.Attribute("tileheight").Value);

            XElement tilesetElement = mapElement.Element("tileset");
            if (tilesetElement != null)
            {
                string tilesetSource = tilesetElement.Attribute("source")?.Value;

                if (!string.IsNullOrEmpty(tilesetSource))
                {
                    string contentRoot = Path.GetDirectoryName(Path.GetDirectoryName(tmxFilePath));
                    string tilesetPath = Path.Combine(contentRoot, "Tilemaps", Path.GetFileName(tilesetSource));

                    XDocument tilesetDoc = XDocument.Load(tilesetPath);
                    ProcessTileset(tilesetDoc.Element("tileset"), tilemap, content, Path.GetDirectoryName(tilesetPath));
                }
                else
                {
                    ProcessTileset(tilesetElement, tilemap, content, Path.GetDirectoryName(tmxFilePath));
                }
            }

            foreach (XElement layerElement in mapElement.Elements("layer"))
            {
                TileLayer layer = new TileLayer
                {
                    Name = layerElement.Attribute("name").Value,
                    Width = int.Parse(layerElement.Attribute("width").Value),
                    Height = int.Parse(layerElement.Attribute("height").Value)
                };

                XElement dataElement = layerElement.Element("data");
                string encoding = dataElement.Attribute("encoding")?.Value;

                if (encoding == "csv")
                {
                    string csvData = dataElement.Value.Trim();
                    layer.Tiles = csvData.Split(',')
                        .Select(s => int.Parse(s.Trim()))
                        .ToArray();

                    int nonZeroCount = layer.Tiles.Count(t => t != 0);
                    Console.WriteLine($"Layer '{layer.Name}': {layer.Tiles.Length} total tiles, {nonZeroCount} non-zero");
                }
                else
                {
                    layer.Tiles = dataElement.Elements("tile")
                        .Select(t => int.Parse(t.Attribute("gid").Value))
                        .ToArray();

                    int nonZeroCount = layer.Tiles.Count(t => t != 0);
                    Console.WriteLine($"Layer '{layer.Name}': {layer.Tiles.Length} total tiles, {nonZeroCount} non-zero");
                }

                tilemap.Layers.Add(layer);
            }

            foreach (XElement objectGroupElement in mapElement.Elements("objectgroup"))
            {
                ObjectLayer objectLayer = new ObjectLayer
                {
                    Name = objectGroupElement.Attribute("name").Value
                };

                foreach (XElement objElement in objectGroupElement.Elements("object"))
                {
                    TiledObject obj = new TiledObject
                    {
                        Name = objElement.Attribute("name")?.Value ?? "",
                        Class = objElement.Attribute("class")?.Value ?? objElement.Attribute("type")?.Value ?? "",
                        X = float.Parse(objElement.Attribute("x").Value),
                        Y = float.Parse(objElement.Attribute("y").Value),
                        Width = float.Parse(objElement.Attribute("width")?.Value ?? "0"),
                        Height = float.Parse(objElement.Attribute("height")?.Value ?? "0")
                    };

                    XElement propertiesElement = objElement.Element("properties");
                    if (propertiesElement != null)
                    {
                        foreach (XElement propElement in propertiesElement.Elements("property"))
                        {
                            string propName = propElement.Attribute("name").Value;
                            string propValue = propElement.Attribute("value").Value;
                            obj.Properties[propName] = propValue;
                        }
                    }

                    objectLayer.Objects.Add(obj);
                }

                tilemap.ObjectLayers.Add(objectLayer);
            }

            return tilemap;
        }

        private static void ProcessTileset(XElement tilesetElement, Tilemap tilemap, ContentManager content, string baseDirectory)
        {
            tilemap.TilesetTileWidth = int.Parse(tilesetElement.Attribute("tilewidth").Value);
            tilemap.TilesetTileHeight = int.Parse(tilesetElement.Attribute("tileheight").Value);
            tilemap.TilesetColumns = int.Parse(tilesetElement.Attribute("columns").Value);

            XElement imageElement = tilesetElement.Element("image");
            if (imageElement != null)
            {
                string imagePath = imageElement.Attribute("source").Value;

                string fileName = Path.GetFileNameWithoutExtension(imagePath);

                tilemap.TilesetTexture = content.Load<Texture2D>($"Tilemaps/{fileName}");
            }
        }
    }

    public class Tilemap
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        public List<TileLayer> Layers { get; set; } = new List<TileLayer>();
        public List<ObjectLayer> ObjectLayers { get; set; } = new List<ObjectLayer>();

        public Texture2D TilesetTexture { get; set; }
        public int TilesetColumns { get; set; }
        public int TilesetTileWidth { get; set; }
        public int TilesetTileHeight { get; set; }
    }

    public class TileLayer
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int[] Tiles { get; set; }
    }

    public class ObjectLayer
    {
        public string Name { get; set; }
        public List<TiledObject> Objects { get; set; } = new List<TiledObject>();
    }

    public class TiledObject
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
