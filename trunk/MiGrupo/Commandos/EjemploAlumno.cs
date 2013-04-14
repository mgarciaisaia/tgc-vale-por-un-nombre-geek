using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking;

namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Sky sky;
        Camera camera;
        Character character;
        Terrain terrain;
        MovementPicking picking;
        MultipleSelection seleccionMultiple;

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
            return "Grupo 99";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "MiIdea - Descripcion de la idea";
        }

        /// <summary>
        /// Método que se llama una sola vez,  al principio cuando se ejecuta el ejemplo.
        /// Escribir aquí todo el código de inicialización: cargar modelos, texturas, modifiers, uservars, etc.
        /// Borrar todo lo que no haga falta
        /// </summary>
        public override void init()
        {
            //Crear SkyBox
            sky = new Sky();

            //Cargar HeightMap
            terrain = new Terrain();

            //Picking
            picking = new MovementPicking(terrain);

            //Crear personaje
            character = new Character(terrain.getPosition(0, 0));

            //Inicializar camara
            camera = new Camera(character.getPosition());

            //Inicializar seleccion multiple
            seleccionMultiple = new MultipleSelection();
        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            Vector3 pickingPosition;
            if (picking.thereIsPicking(out pickingPosition))
            {
                character.setPositionTarget(pickingPosition);
            }

            camera.update(500);
            sky.render();
            terrain.render();
            character.render(elapsedTime);
            seleccionMultiple.update();
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            sky.dispose();
            terrain.dispose();
            character.dispose();
        }

    }
}
