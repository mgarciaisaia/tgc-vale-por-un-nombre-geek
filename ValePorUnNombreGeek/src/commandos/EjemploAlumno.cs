using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.commands.orders;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.levelParser;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.level.map;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.renderization;
using TgcViewer.Utils.Sound;
using TgcViewer.Utils;
using System.Drawing;


namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Level level;
        MovementPicking picking;
        Selection selection;
        string currentLevel;
        GraphicalControlPanel controlPanel;
        Renderer defaultRenderer;
        ShadowRenderer shadowRenderer;
        public bool Music { get; set; }

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

  
        #endregion


        public bool Sombras { get { return level.Renderer == shadowRenderer; } set { if (value)level.Renderer = shadowRenderer; else level.Renderer = defaultRenderer; } }
        public string SelectedLevel { get { return currentLevel; } set { loadLevel(value); } }
       
        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            loadLevel(CommandosUI.Instance.SrcDir + "\\niveles\\default-level.xml");
            loadMusic(CommandosUI.Instance.MediaDir+"\\Music\\music.mp3");
            
         
        }

        private void loadMusic(string p)
        {
            Music = true;
            GuiController.Instance.Mp3Player.FileName = p;
            
        }

        #region LoadLevel

        private void loadLevel(string newLevel)
        {
            disposeAll();

            GuiController.Instance.Modifiers.clear();
            GuiController.Instance.UserVars.clearVars();

            currentLevel = newLevel;

            XMLLevelParser levelParser = new XMLLevelParser(newLevel, CommandosUI.Instance.MediaDir);
            level = levelParser.getLevel();


            LevelMap map = level.Map;
            map.setMask(TextureLoader.FromFile(CommandosUI.Instance.d3dDevice, CommandosUI.Instance.MediaDir + "Mapa\\mask.jpg"));
            map.setFrame(TextureLoader.FromFile(CommandosUI.Instance.d3dDevice, CommandosUI.Instance.MediaDir + "Mapa\\frame.png"));
            map.Width = 2 * level.Map.Height;
            map.Height = 1.5f * level.Map.Height;
            map.Position = new Vector2(CommandosUI.Instance.ScreenWidth - level.Map.Width , 0);


            defaultRenderer = level.Renderer;
            shadowRenderer = new ShadowRenderer();

    
            //Movimiento por picking
            picking = new MovementPicking(level.Terrain);

            //Seleccion multiple
            selection = new Selection(level.Characters, level.Terrain);
             
            //Inicializar camara
            CommandosUI.Instance.Camera = new PCamera(new Vector3(0, 0, 150), level.Terrain);


            //Panel de control in game
            controlPanel = new GraphicalControlPanel(CommandosUI.Instance.MediaDir + "Sprites\\panel2.jpg");
            controlPanel.addCommand(new SwitchCrouch(selection.getSelectedCharacters()), CommandosUI.Instance.MediaDir + "Sprites\\crouch.png");
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), CommandosUI.Instance.MediaDir + "Sprites\\cancel.png");
            CommandosUI.Instance.Panel = controlPanel;

            foreach (Commando c in level.Commandos)
                controlPanel.addSelectionButton(c, selection);

            setAndBindModifiers();


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

            GuiController.Instance.Modifiers.addBoolean("Musica", "Reproducir", true);
            Modifiers.Instance.bind("Musica", this, "Music");

            GuiController.Instance.Modifiers.addBoolean("Cilindros", "Ver cilindros", false);
            Modifiers.Instance.bind("Cilindros", typeof(Character), "RenderCylinder");



            GuiController.Instance.Modifiers.addBoolean("Sombras", "Activar", false);
            Modifiers.Instance.bind("Sombras", this, "Sombras");

            GuiController.Instance.Modifiers.addBoolean("Culling", "Activado", false);
            Modifiers.Instance.bind("Culling", level, "CullingEnabled");

            GuiController.Instance.Modifiers.addBoolean("Grilla", "Mostrar", false);
            Modifiers.Instance.bind("Grilla", level.Terrain, "RenderPatchesBB");
          
            for (int i = 0; i < level.Terrain.Patches.GetLength(0); i++) for (int j = 0; j < level.Terrain.Patches.GetLength(1); j++)
                {
                    GuiController.Instance.Modifiers.addBoolean("TerrainPatch[" + i + "," + j + "]", "Mostrar", true);
                    Modifiers.Instance.bind("TerrainPatch[" + i + "," + j + "]", level.Terrain.Patches[i, j], "Enabled");
                }

        }
        #endregion

        const float MAX_ELAPSED_TIME = 0.5f;

        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
          
            Modifiers.Instance.update();

            playMusic();


            

            level.render(elapsedTime);


            controlPanel.render();

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

            level.Map.Technique = "MAPA_VIEJO";
            level.Map.render();

            GuiController.Instance.Text3d.drawText(HighResolutionTimer.Instance.FramesPerSecond + " FPS", 0, 0, Color.Violet);
          
        
          
        }

        private void playMusic()
        {
           
       
            TgcMp3Player player = GuiController.Instance.Mp3Player;
            TgcMp3Player.States currentState = player.getStatus();
            if (Music)
            {
                if (currentState == TgcMp3Player.States.Open)
                

                    player.play(true);
                else if (currentState == TgcMp3Player.States.Paused)
                
                    //Resumir la ejecuci�n del MP3
                    player.resume();
                
            }
            else if (currentState == TgcMp3Player.States.Playing)
            {
                //Pausar el MP3
                player.pause();
            }
          

        }

      

        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            disposeAll();
        }

        private void disposeAll()
        {
            if(controlPanel!=null) controlPanel.dispose();
            if(level!=null)level.dispose();
            Modifiers.clear();
            level = null;
            controlPanel = null;
        }

    }
}
