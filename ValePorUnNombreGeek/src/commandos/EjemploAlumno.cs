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

        #endregion


        /// <summary>
        /// M�todo que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aqu� todo el c�digo de inicializaci�n: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //Crear SkyBox
            sky = new Sky();

            XMLLevelParser levelParser= new XMLLevelParser(
              GuiController.Instance.AlumnoEjemplosDir + "ValePorUnNombreGeek\\niveles\\" + "nivel1.xml",
              GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\"
              );
          
            //Cargar nivel
            level = levelParser.getLevel();
           
            //Crear personajes
            Vector3[] waitpoints = new Vector3[3];
            level.Terrain.heightmapCoordsToXYZ(new Vector2(73, 81), out waitpoints[0]);
            level.Terrain.heightmapCoordsToXYZ(new Vector2(22, 80), out waitpoints[1]);
            level.Terrain.heightmapCoordsToXYZ(new Vector2(10, 37), out waitpoints[2]);


         
            level.add(new Soldier(waitpoints));
            level.add(new Commando(level.Terrain.getPosition(-200, 200)));
            level.add(new Commando(level.Terrain.getPosition(200, 200)));
          

            //Seleccion multiple
            selection = new Selection(level.Characters, level.Terrain);

            //Movimiento por picking
            picking = new MovementPicking(level.Terrain);
       
            //Inicializar camara
            camera = new FreeCamera(level.Terrain.getPosition(0, 150), true);


        }


        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            picking.update(selection.getSelectedCharacters());

            camera.updateCamera();
            sky.render();
            level.render(elapsedTime);


            selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes




           
        }


        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            sky.dispose();
            level.dispose();
        }

    }
}
