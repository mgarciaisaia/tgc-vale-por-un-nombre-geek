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
            Enemy enemy = null;
            XmlNode at = enemyNode.Attributes.GetNamedItem("class");
            switch (enemyNode.Attributes.GetNamedItem("class").InnerText)
            {
                case "soldier": 
                    enemy = XMLEnemy.getSoldier(enemyNode, terrain);
                    break;
                
            }
            return enemy;

        }

        private static Enemy getSoldier(XmlNode enemyNode, Terrain terrain)
        {
               //Cargo los waitpoints
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
