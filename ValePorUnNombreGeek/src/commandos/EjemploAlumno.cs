using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands.orders;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.levelParser;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map;


namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Sky sky;
       
        Level level;
        MovementPicking picking;
        Selection selection;
        string currentLevel;
        GraphicalControlPanel controlPanel;

        FreeCamera camera;

        #region Details

        /// <summary>
        /// Categor�a a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el �rbol de la derecha de la pantalla.
        /// </summary>
        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "VALE_POR_UN_NOMBRE_GEEK";
        }

        /// <summary>
        /// Completar con la descripci�n del TP
        /// </summary>
        public override string getDescription()
        {
            return "Implementaci�n del Commandos";
        }

        public static string getMediaDir()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\";
        }

        #endregion

       
        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            string mediaPath = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\";
            string initialLevel = GuiController.Instance.AlumnoEjemplosDir + "ValePorUnNombreGeek\\niveles\\default-level.xml";
            
            GuiController.Instance.Modifiers.addFile("Level", initialLevel, "-level.xml|*-level.xml");
            //Crear SkyBox
            sky = new Sky();
            this.level = null;
            loadLevel(initialLevel);
            LevelMap map = level.Map;
            map.Width = 2 * level.Map.Height;
            map.Height = 1.5f * level.Map.Height;
            map.Position = new Vector2(GuiController.Instance.Panel3d.Width - 10 - level.Map.Width, level.Map.Position.Y);
         

            UserVars.initialize();
            
            //Panel de control in game
            controlPanel = new GraphicalControlPanel(GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\Sprites\\panel2.jpg");
            controlPanel.addCommand(new Talk(selection.getSelectedCharacters()), mediaPath + "Sprites\\emptyp.png");
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), mediaPath + "Sprites\\cancelp.png");
            /*controlPanel = new TextControlPanel();
            controlPanel.addCommand(new Talk(selection.getSelectedCharacters()), Key.D1);
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), Key.D2);*/
           
        }

        #region LoadLevel
        private void checkLoadLevel(string selectedPath)
        {
            if (selectedPath != currentLevel) loadLevel(selectedPath);
        }

        private void loadLevel(string newLevel)
        {
            if (level != null) level.dispose();
            
            currentLevel = newLevel;

            XMLLevelParser levelParser = new XMLLevelParser(newLevel, getMediaDir());
            level = levelParser.getLevel();
            
            //Movimiento por picking
            picking = new MovementPicking(level.Terrain);
               

            //Inicializar camara
            camera = new FreeCamera(level.Terrain.getPosition(0, 150), true);

            //Seleccion multiple
            selection = new Selection(level.Characters, level.Terrain);
        }
        #endregion

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            string selectedPath = (string)GuiController.Instance.Modifiers["Level"];
            
            checkLoadLevel(selectedPath);

            sky.render();

            Character.RenderCylinder = UserVars.Instance.renderCollisionNormal;
            level.render(elapsedTime);

            if (controlPanel.mouseIsOverPanel())
            {
                selection.cancelSelection(); //cancelamos la seleccion si se estaba seleccionando
                controlPanel.update(); //permitimos que el panel ejecute su logica
            }
            else
            {
                picking.update(selection.getSelectedCharacters());
                selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes
            }

            controlPanel.render();
            level.Map.Technique = "MAPA_VIEJO";
            level.Map.Zoom = UserVars.Instance.zoomMapa;
            level.Map.ShowCharacters = UserVars.Instance.showCharacters;
            level.Map.render();

            //Prueba barrita de vida
            level.Commandos[0].Life.decrement(elapsedTime * 5);
            level.Commandos[0].Life.render();
        }

      

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            
            controlPanel.dispose();
            sky.dispose();
            level.dispose();
          
            
        }

    }
}
