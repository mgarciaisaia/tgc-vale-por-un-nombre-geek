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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.LevelParser;

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
            Terrain terrain = getTerrain();
            Level level = new Level(terrain);
            foreach (Enemy e in getEnemies(terrain)) level.add(e);
            foreach (Commando c in getCommandos(terrain)) level.add(c);
            foreach (ILevelObject o in getLevelObjects(terrain)) level.add(o);
            return level;
        }

        private Terrain getTerrain()
        {
            XmlNode xmlTerrain = root.GetElementsByTagName("terrain")[0];

            String heightmap = mediaDir + xmlTerrain.Attributes.GetNamedItem("heightmap").InnerText;
            String texture = mediaDir + xmlTerrain.Attributes.GetNamedItem("texture").InnerText;
            float scaleXZ = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleXZ").InnerText);
            float scaleY = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleY").InnerText);

            return new Terrain(heightmap, texture, scaleXZ, scaleY);
        }


        private IEnumerable<ILevelObject> getLevelObjects(Terrain terrain)
        {
            List<ILevelObject> levelObjects = new List<ILevelObject>();

            XmlNodeList objectNodes = root.GetElementsByTagName("levelObject");

            foreach (XmlNode node in objectNodes)
            {
                levelObjects.Add(XMLLevelObject.getLevelObject(node, terrain, mediaDir));
            }

            return levelObjects;
        }


        private IEnumerable<Commando> getCommandos(Terrain terrain)
        {
            List<Commando> commandos = new List<Commando>();

           
            XmlNodeList commandoNodes = root.GetElementsByTagName("commando");

            foreach (XmlNode node in commandoNodes)
            {
               commandos.Add(XMLCommando.getCommando(node, terrain));
            }

            return commandos;
        }


        private IEnumerable<Enemy> getEnemies(Terrain terrain)
        {
           
            List<Enemy> enemies = new List<Enemy>();

           
            XmlNodeList enemyNodes = root.GetElementsByTagName("enemy");
            
            foreach (XmlNode node in enemyNodes)
            {                
                enemies.Add(XMLEnemy.getEnemy(node, terrain));                         
            }

            return enemies;
        }

      
    }
}
