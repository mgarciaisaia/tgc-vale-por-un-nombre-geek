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
            Terrain terrain = getTerrain();
            Level level = new Level(terrain);
            foreach (Enemy e in getEnemies(terrain)) level.add(e);
            foreach (Commando c in getCommandos(terrain)) level.add(c);
            foreach (ILevelObject o in getLevelObjects(terrain)) level.add(o);
            return level;
        }

        private IEnumerable<ILevelObject> getLevelObjects(Terrain terrain)
        {
            List<ILevelObject> levelObjects = new List<ILevelObject>();

            //Obtengo lista de nodos levelobject
            XmlNodeList objectNodes = root.GetElementsByTagName("levelObject");

            foreach (XmlNode node in objectNodes)
            {
                float[] pos = TgcParserUtils.parseFloat2Array(node.InnerText);
                string path = mediaDir + node.Attributes.GetNamedItem("mesh").InnerText;
                float[] scale = TgcParserUtils.parseFloat3Array(node.Attributes.GetNamedItem("scale").InnerText);
                levelObjects.Add(new LevelObject(path,terrain.getPosition(pos[0], pos[1]), new Vector3(scale[0],scale[1],scale[2])));
            }

            return levelObjects;
        }

        private IEnumerable<Commando> getCommandos(Terrain terrain)
        {
            List<Commando> commandos = new List<Commando>();

            //Obtengo lista de nodos commando
            XmlNodeList commandoNodes = root.GetElementsByTagName("commando");

            foreach (XmlNode node in commandoNodes)
            {
                float[] pos = TgcParserUtils.parseFloat2Array(node.InnerText);
                commandos.Add(new Commando(terrain.getPosition(pos[0], pos[1])));
            }

            return commandos;
        }


        private IEnumerable<Enemy> getEnemies(Terrain terrain)
        {
           
            List<Enemy> enemies = new List<Enemy>();

            //Obtengo lista de nodos soldier
            XmlNodeList soldierNodes = root.GetElementsByTagName("soldier");
            foreach (XmlNode node in soldierNodes)
            {
                
                            
                //Cargo los waitpoints
                List<Vector3> waitpoints = new List<Vector3>();
                foreach (XmlNode wn in node.ChildNodes)
                {
                    if(wn.NodeType == XmlNodeType.Element){ 
                        float[] pos = TgcParserUtils.parseFloat2Array(wn.InnerText);
                        waitpoints.Add(terrain.getPosition(pos[0], pos[1]));
                    }
                }

                enemies.Add(new Soldier(waitpoints.ToArray<Vector3>()));
            }

            return enemies;
        }

      
    }
}
