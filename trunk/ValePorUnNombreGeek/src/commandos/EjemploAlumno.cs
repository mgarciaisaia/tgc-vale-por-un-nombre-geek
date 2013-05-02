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
        TextControlPanel controlPanel;

        FreeCamera camera;

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

        public string getMediaDir()
        {
            return GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\";
        }

        #endregion

       
        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            string initialLevel = GuiController.Instance.AlumnoEjemplosDir + "ValePorUnNombreGeek\\niveles\\" + "default-level.xml";
           
            GuiController.Instance.Modifiers.addFile("Level", initialLevel, "-level.xml|*-level.xml");
            //Crear SkyBox
            sky = new Sky();

            loadLevel(initialLevel);
            
            
           
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

            XMLLevelParser levelParser = new XMLLevelParser(newLevel, this.getMediaDir());
            level = levelParser.getLevel();
            
            //Movimiento por picking
            picking = new MovementPicking(level.Terrain);
               

            //Inicializar camara
            camera = new FreeCamera(level.Terrain.getPosition(0, 150), true);

            //Seleccion multiple
            selection = new Selection(level.Characters, level.Terrain);
            

            //Panel de control in game
            controlPanel = new TextControlPanel();
            controlPanel.addCommand(new Talk(selection.getSelectedCharacters()), Key.D1);
            controlPanel.addCommand(new StandBy(selection.getSelectedCharacters()), Key.D2);
        }
        #endregion

        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            string selectedPath = (string)GuiController.Instance.Modifiers["Level"];
            
            checkLoadLevel(selectedPath);

            picking.update(selection.getSelectedCharacters());

           
            sky.render();
            level.render(elapsedTime);


            selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes

            controlPanel.render();
        }

      

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            sky.dispose();
            level.dispose();
        }

    }
}
