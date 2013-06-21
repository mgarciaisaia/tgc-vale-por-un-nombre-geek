using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Xml;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.levelParser
{
    class XMLCommando
    {
        public static Commando getCommando(XmlNode commandoNode, ITerrain terrain)
        {
            
            string commandoClass;

            commandoClass = getClass(commandoNode);

            switch (commandoClass)
            {
                case "commando":
                    return XMLCommando.getDefault(commandoNode, terrain);
                   
            }


            return null;

        }

        private static string getClass(XmlNode commandoNode)
        {
           
            XmlNode classNode = commandoNode.Attributes.GetNamedItem("class");

            if (classNode != null)
            {
                return classNode.InnerText;
            }
            else return "commando";

        }

        private static Commando getDefault(XmlNode commandoNode, ITerrain terrain)
        {
            float[] pos = TgcViewer.Utils.TgcSceneLoader.TgcParserUtils.parseFloat2Array(commandoNode.InnerText);

            Commando commando  = new Commando(terrain.getPosition(pos[0], pos[1]));
            XmlNode pic = commandoNode.Attributes.GetNamedItem("picture");
            if (pic != null)
            {
                commando.Picture = new CharacterPicture(CommandosUI.Instance.MediaDir + pic.InnerText);

            }
            return commando;
        }
    }
}
