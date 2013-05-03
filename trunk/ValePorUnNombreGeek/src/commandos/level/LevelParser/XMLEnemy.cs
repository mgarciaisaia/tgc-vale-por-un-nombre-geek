using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.LevelParser
{
    class XMLEnemy
    {
     

        public static Enemy getEnemy(XmlNode enemyNode, Terrain terrain)
        {
           
            XmlNode at = enemyNode.Attributes.GetNamedItem("class");
            string enemyClass;

            enemyClass = getClass(enemyNode);

            switch (enemyClass)
            {
                case "soldier": 
                    return XMLEnemy.getSoldier(enemyNode, terrain);
                    
                
            }

            return null;

        }

        private static string getClass(XmlNode enemyNode)
        {
           
            XmlNode classNode = enemyNode.Attributes.GetNamedItem("class");
            
            if (classNode != null)
            {
               return classNode.InnerText;
            }
            else return "soldier";

        }

        private static Enemy getSoldier(XmlNode enemyNode, Terrain terrain)
        {
            
            List<Vector3> waitpoints = new List<Vector3>();
            foreach (XmlNode wn in enemyNode.ChildNodes)
            {
                if (wn.NodeType == XmlNodeType.Element)
                {
                    float[] pos = TgcParserUtils.parseFloat2Array(wn.InnerText);
                    waitpoints.Add(terrain.getPosition(pos[0], pos[1]));
                }
            }

            return new Soldier(waitpoints.ToArray<Vector3>());
        }
    }
}
