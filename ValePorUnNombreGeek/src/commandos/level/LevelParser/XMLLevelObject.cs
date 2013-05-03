﻿using System;
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
        
            string objectClass;
            Vector3 scale, rotation;


            getClassScaleAndRotation(levelObjectNode, out objectClass, out scale, out rotation);

            switch (objectClass)
            {
                case "levelObject":
                    levelObject = XMLLevelObject.getDefault(levelObjectNode, terrain, mediaDir, scale, rotation);
                    break;
                case "tree":
                    levelObject = XMLLevelObject.getTree(levelObjectNode, terrain, scale, rotation);
                    break;
            }


            return levelObject;

        }

        private static void getClassScaleAndRotation(XmlNode levelObjectNode, out string objectClass, out Vector3 scale, out Vector3 rotation)
        {
            XmlNode scaleNode = levelObjectNode.Attributes.GetNamedItem("scale");
            if (scaleNode != null)
            {

                float[] scaleArray = TgcParserUtils.parseFloat3Array(scaleNode.InnerText);
                scale = new Vector3(scaleArray[0], scaleArray[1], scaleArray[2]);

            }
            else
                scale = new Vector3(1, 1, 1);
            
            
            XmlNode rotationNode = levelObjectNode.Attributes.GetNamedItem("rotation");
            if (rotationNode != null)
            {

                float[] rotationArray = TgcParserUtils.parseFloat3Array(rotationNode.InnerText);
                rotation = new Vector3(rotationArray[0], rotationArray[1], rotationArray[2]);

            }
            else rotation = new Vector3(0, 0, 0);


            XmlNode classNode = levelObjectNode.Attributes.GetNamedItem("class");
            if (classNode == null)
            {
                objectClass = "levelObject";
            }
            else objectClass = classNode.InnerText;
        }

       
        private static ILevelObject getTree(XmlNode levelObjectNode, Terrain terrain, Vector3 scale, Vector3 rotation)
        {
            float[] pos = TgcParserUtils.parseFloat2Array(levelObjectNode.InnerText);
                     

            return new Tree(terrain.getPosition(pos[0], pos[1]), scale, rotation);
        }


        private static ILevelObject getDefault(XmlNode levelObjectNode, Terrain terrain, string mediaDir, Vector3 scale, Vector3 rotation)
        {
            float[] pos = TgcParserUtils.parseFloat2Array(levelObjectNode.InnerText);
          
            string path = mediaDir + levelObjectNode.Attributes.GetNamedItem("mesh").InnerText;
                       
            return new LevelObject(path, terrain.getPosition(pos[0], pos[1]), scale, rotation);
        }

    }
}
