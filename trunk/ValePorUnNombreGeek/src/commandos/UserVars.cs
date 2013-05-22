using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class UserVars
    {
        private static UserVars instance;

        private UserVars(Level level, string currentLevel)
        {
            GuiController.Instance.Modifiers.addFile("Level", currentLevel, "-level.xml|*-level.xml");
            GuiController.Instance.Modifiers.addBoolean("Mapa", "ShowCharacters", true);
            GuiController.Instance.Modifiers.addFloat("Zoom", 0.5f, 5, 2);
            GuiController.Instance.Modifiers.addBoolean("Sombras", "Activar", false);
            GuiController.Instance.Modifiers.addBoolean("showCylinder", "Ver cilindros", false);

         

            TerrainPatch[,] patches = level.Terrain.Patches;

            for (int i = 0; i < patches.GetLength(0); i++) for (int j = 0; j < patches.GetLength(1); j++)
            {
                    GuiController.Instance.Modifiers.addBoolean("TerrainPatch[" + i + "," + j + "]", "Mostrar", true);
            }
      

        }

        public static void initialize(Level level, string currentLevel)
        {
            instance = new UserVars(level, currentLevel);
        }

        public static UserVars Instance
        {
            get
            {
                //if (instance == null) instance = new UserVars();
                return instance;
            }
        }

        public bool renderCollisionNormal
        {
            get { return (bool)GuiController.Instance.Modifiers["showCylinder"]; }
        }

        public float zoomMapa { get { return (float)GuiController.Instance.Modifiers.getValue("Zoom"); } }

        public bool showCharacters { get { return (bool)GuiController.Instance.Modifiers.getValue("Mapa"); } }

        public bool sombras { get { return (bool)GuiController.Instance.Modifiers.getValue("Sombras"); } }

        public bool showTerrainPatch(int i, int j){

            return (bool)GuiController.Instance.Modifiers.getValue("TerrainPatch[" + i + "," + j + "]");
        }

      
    }
}
