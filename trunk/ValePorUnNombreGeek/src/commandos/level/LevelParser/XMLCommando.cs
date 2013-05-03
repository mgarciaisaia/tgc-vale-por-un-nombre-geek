using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Xml;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;


namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.LevelParser
{
    class XMLCommando
    {
        public static Commando getCommando(XmlNode commandoNode, Terrain terrain)
        {
            Commando commando = null;
            XmlNode commandoClass = commandoNode.Attributes.GetNamedItem("class");

            if (commandoClass == null)
            {
                return XMLCommando.getDefault(commandoNode, terrain);
            }

            switch (commandoClass.InnerText)
            {
               
            }


            return commando;

        }

        private static Commando getDefault(XmlNode commandoNode, Terrain terrain)
        {
            float[] pos = TgcViewer.Utils.TgcSceneLoader.TgcParserUtils.parseFloat2Array(commandoNode.InnerText);

            return new Commando(terrain.getPosition(pos[0], pos[1]));
        }
    }
}
