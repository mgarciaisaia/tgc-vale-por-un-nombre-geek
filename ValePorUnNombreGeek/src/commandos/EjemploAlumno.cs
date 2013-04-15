using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using System.Collections.Generic;


namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Sky sky;
        StaticCamera camera;
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



            //Crear personajes seleccionables
            List<Character> selectableCharacters = new List<Character>();
            selectableCharacters.Add(new Character(terrain.getPosition(-200, 200)));
            selectableCharacters.Add(new Character(terrain.getPosition(200, 200)));

            //Seleccion multiple
            selection = new MultipleSelection(this.terrain, selectableCharacters);

            //Crear el resto de los personajes
            this.characters = new List<Character>(selectableCharacters);
            this.characters.Add(new Enemy(terrain.getPosition(400, 200)));

            //Movimiento por picking
            picking = new MovementPicking(this.terrain);
       
            //Inicializar camara
            camera = new StaticCamera(this.terrain.getPosition(0, 150));
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
            foreach (Character ch in this.characters)
            {
                ch.dispose();
            }
        }

    }
}
