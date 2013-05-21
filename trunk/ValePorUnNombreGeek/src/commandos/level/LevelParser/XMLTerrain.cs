using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TgcViewer.Utils.TgcSceneLoader;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.levelParser
{
    class XMLTerrain
    {

        
           
    
        public static ITerrain getTerrain(XmlNode xmlTerrain, string mediaDir)
        {
            String heightmap = mediaDir + xmlTerrain.Attributes.GetNamedItem("heightmap").InnerText;
            String texture = mediaDir + xmlTerrain.Attributes.GetNamedItem("texture").InnerText;
            float scaleXZ = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleXZ").InnerText);
            float scaleY = TgcParserUtils.parseFloat(xmlTerrain.Attributes.GetNamedItem("scaleY").InnerText);
            XmlNode formatAttr = xmlTerrain.Attributes.GetNamedItem("format");
            float[] format = new float[] { 1, 1 };
            
            if (formatAttr != null) { format = TgcParserUtils.parseFloat2Array(formatAttr.InnerText); }
            
            return new DivisibleTerrain(heightmap, texture, scaleXZ, scaleY, new Vector2(format[0], format[1]));
            
        }
    }
}
