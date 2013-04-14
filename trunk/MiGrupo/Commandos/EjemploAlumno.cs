using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking;
using System.Collections.Generic;

namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Sky sky;
        Camera camera;
        List<Character> characters;
        Terrain terrain;

        MovementPicking picking;
        MultipleSelection selection;

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

            //Crear personajes
            this.characters = new List<Character>();
            this.characters.Add(new Character(terrain.getPosition(-200, 200)));
            this.characters.Add(new Character(terrain.getPosition(200, 200)));

            //Inicializar camara
            camera = new Camera(this.terrain.getPosition(0, 150));

            //Movimiento por picking
            picking = new MovementPicking(this.terrain);

            //Seleccion multiple
            selection = new MultipleSelection(this.terrain, this.characters);
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
                foreach (Character ch in selection.getSelectedCharacters())
                {
                    ch.setPositionTarget(pickingPosition);
                }
            }

            camera.update(500);
            sky.render();
            terrain.render();
            foreach (Character ch in this.characters)
            {
                ch.render(elapsedTime);
            }
            selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes
        }

        /// <summary>
        /// Método que se llama cuando termina la ejecución del ejemplo.
        /// Hacer dispose() de todos los objetos creados.
        /// </summary>
        public override void close()
        {
            sky.dispose();
            terrain.dispose();
            selection.dispose();
            foreach (Character ch in this.characters)
            {
                ch.dispose();
            }
        }

    }
}
