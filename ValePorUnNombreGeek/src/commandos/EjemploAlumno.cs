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
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;


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
        Renderer defaultRenderer;
        ShadowRenderer shadowRenderer;

        #region Details

        /// <summary>
        /// Categoría a la que pertenece el ejemplo.
        /// Influye en donde se va a haber en el árbol de la derecha de la pantalla.
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
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Implementación del Commandos";
        }

        public static string MediaDir = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\";
        public static string SrcDir = GuiController.Instance.AlumnoEjemplosDir + "ValePorUnNombreGeek\\";
        public static string ShadersDir = MediaDir + "Shaders\\";

  
        #endregion


        public bool Sombras { get { return level.Renderer == shadowRenderer; } set { if (value)level.Renderer = shadowRenderer; else level.Renderer = defaultRenderer; } }
        public string SelectedLevel { get { return currentLevel; } set { loadLevel(value); } }
       
        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //SkyBox
            sky = new Sky();

            loadLevel(EjemploAlumno.SrcDir + "\\niveles\\default-level.xml");
            
            
            //Panel de control in game
            controlPanel = new GraphicalControlPanel(EjemploAlumno.MediaDir + "Sprites\\panel2.jpg");
            controlPanel.addCommand(new Talk(selection.getSelectedCharacters()), EjemploAlumno.MediaDir + "Sprites\\emptyp.png");
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), EjemploAlumno.MediaDir + "Sprites\\cancelp.png");
            /*controlPanel = new TextControlPanel();
            controlPanel.addCommand(new Talk(selection.getSelectedCharacters()), Key.D1);
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), Key.D2);*/
         
        }

        #region LoadLevel

        private void loadLevel(string newLevel)
        {
            if (level != null) level.dispose();

            GuiController.Instance.Modifiers.clear();
            GuiController.Instance.UserVars.clearVars();

            currentLevel = newLevel;

            XMLLevelParser levelParser = new XMLLevelParser(newLevel, EjemploAlumno.MediaDir);
            level = levelParser.getLevel();


            LevelMap map = level.Map;
            map.setMask(TextureLoader.FromFile(GuiController.Instance.D3dDevice, EjemploAlumno.MediaDir + "Mapa\\mask.jpg"));
            map.setFrame(TextureLoader.FromFile(GuiController.Instance.D3dDevice, EjemploAlumno.MediaDir + "Mapa\\frame.png"));
            map.Width = 2 * level.Map.Height;
            map.Height = 1.5f * level.Map.Height;
            map.Position = new Vector2(GuiController.Instance.Panel3d.Width / 2 - level.Map.Width / 2, GuiController.Instance.Panel3d.Height - level.Map.Height);


            defaultRenderer = level.Renderer;
            shadowRenderer = new ShadowRenderer();
            shadowRenderer.objects = level.Objects;
            shadowRenderer.characters = level.Characters;
            shadowRenderer.patches = level.Patches;

            setAndBindModifiers();


            //Movimiento por picking
            picking = new MovementPicking(level.Terrain);

            //Seleccion multiple
            selection = new Selection(level.Characters, level.Terrain);
             
            //Inicializar camara
            CommandosUI.Instance.Camera = new PCamera(new Vector3(0, 0, 150), level.Terrain);
        }

        private void setAndBindModifiers()
        {
            Modifiers.initialize();

            GuiController.Instance.Modifiers.addFile("Level", currentLevel, "-level.xml|*-level.xml");
            Modifiers.Instance.bind("Level", this, "SelectedLevel");


            GuiController.Instance.Modifiers.addBoolean("Mapa", "ShowCharacters", true);
            Modifiers.Instance.bind("Mapa", level.Map, "ShowCharacters");

            GuiController.Instance.Modifiers.addFloat("Zoom", 0.5f, 5, 2);
            Modifiers.Instance.bind("Zoom", level.Map, "Zoom");


            GuiController.Instance.Modifiers.addBoolean("showCylinder", "Ver cilindros", false);
            Modifiers.Instance.bind("showCylinder", typeof(Character), "RenderCylinder");



            GuiController.Instance.Modifiers.addBoolean("Sombras", "Activar", false);
            Modifiers.Instance.bind("Sombras", this, "Sombras");

            float K = 900;
            GuiController.Instance.Modifiers.addVertex3f("LightLookFrom", new Vector3(-K, -K, -K), new Vector3(K, 2 * K, K), new Vector3(80, K, -210));
            Modifiers.Instance.bind("LightLookFrom", shadowRenderer, "LightLookFrom");
            
            GuiController.Instance.Modifiers.addVertex3f("LightLookAt", new Vector3(-K, -K, -K), new Vector3(K, K, K), new Vector3(0, 0, 0));
            Modifiers.Instance.bind("LightLookAt", shadowRenderer, "LightLookAt");
          
            for (int i = 0; i < level.Terrain.Patches.GetLength(0); i++) for (int j = 0; j < level.Terrain.Patches.GetLength(1); j++)
                {
                    GuiController.Instance.Modifiers.addBoolean("TerrainPatch[" + i + "," + j + "]", "Mostrar", true);
                    Modifiers.Instance.bind("TerrainPatch[" + i + "," + j + "]", level.Terrain.Patches[i, j], "Enabled");
                }

        }
        #endregion

        const float MAX_ELAPSED_TIME = 0.5f;

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            if (elapsedTime > MAX_ELAPSED_TIME)
            {
                GuiController.Instance.Logger.log("Ignoramos un retardo de " + elapsedTime + " s");
                return;
            }

            Modifiers.Instance.update();



            

            level.render(elapsedTime);

            sky.render();

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
            level.Map.render();

        
          
        }

      

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            
            controlPanel.dispose();
            sky.dispose();
            level.dispose();
            level = null;
                   
            
        }

    }
}
