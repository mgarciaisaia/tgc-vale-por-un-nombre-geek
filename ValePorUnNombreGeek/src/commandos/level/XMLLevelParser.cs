using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using Microsoft.DirectX;
using System.IO;
using System.Xml;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcSceneLoader;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level
{
    class XMLLevelParser
    {
        XmlElement root;
        String mediaDir;
        public XMLLevelParser(String filePath, String mediaDir)
        {
            this.root = loadXML(filePath);
            this.mediaDir = mediaDir;
           
           

        }

        private Terrain getTerrain()
        {
            XmlNode xmlTerrain = root.GetElementsByTagName("terrain")[0];

            String heightmap = mediaDir + xmlTerrain.Attributes.GetNamedItem("heightmap").InnerText;
            String texture = mediaDir + xmlTerrain.Attributes.GetNamedItem("texture").InnerText;
            float scaleXZ = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleXZ").InnerText);
            float scaleY = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleY").InnerText);
           
            return new Terrain(heightmap,texture, scaleXZ, scaleY);
        }

        private static XmlElement loadXML(String filePath)
        {
            string str = File.ReadAllText(filePath);
            XmlDocument dom = new XmlDocument();
            dom.LoadXml(str);
            XmlElement root = dom.DocumentElement;
            return root;
        }

        public Level getLevel()
        {
            Level level = new Level(getTerrain());
            return level;
        }

      
    }
}
