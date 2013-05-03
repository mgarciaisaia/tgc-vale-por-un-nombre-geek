using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.LevelParser
{
    class XMLLevelObject
    {
        public static ILevelObject getLevelObject(XmlNode levelObjectNode, Terrain terrain, string mediaDir)
        {
            ILevelObject levelObject = null;
            XmlNode objectClass = levelObjectNode.Attributes.GetNamedItem("class");
            XmlNode scaleNode = levelObjectNode.Attributes.GetNamedItem("scale");
            Vector3 scale;

            if(scaleNode != null){

                 float[] scaleArray = TgcParserUtils.parseFloat3Array(scaleNode.InnerText);
                 scale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);
   
            }else 
                scale = new Vector3(1, 1, 1);

            if (objectClass == null)
            {
                return XMLLevelObject.getDefault(levelObjectNode, terrain, mediaDir, scale);
            }

            switch (objectClass.InnerText)
            {
                case "levelObject":
                    levelObject = XMLLevelObject.getDefault(levelObjectNode, terrain, mediaDir, scale);
                    break;
            }


            return levelObject;

        }


        private static ILevelObject getDefault(XmlNode node, Terrain terrain, string mediaDir, Vector3 scale)
        {
            float[] pos = TgcParserUtils.parseFloat2Array(node.InnerText);
          
            string path = mediaDir + node.Attributes.GetNamedItem("mesh").InnerText;
                       
            return new LevelObject(path, terrain.getPosition(pos[0], pos[1]), scale);
        }

    }
}
