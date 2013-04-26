using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.multiple;
using System.Collections.Generic;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle;


namespace AlumnoEjemplos.ValePorUnNombreGeek
{
    /// <summary>
    /// Ejemplo del alumno
    /// </summary>
    public class EjemploAlumno : TgcExample
    {
        Sky sky;
        Terrain terrain;

        List<Character> characters;
        List<Enemy> enemies;

        MovementPicking picking;
        Selection selection;

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

        #endregion


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
            Vector3[] waitpoints = new Vector3[3];
            terrain.heightmapCoordsToXYZ(new Vector2(73, 81), out waitpoints[0]);
            terrain.heightmapCoordsToXYZ(new Vector2(22, 80), out waitpoints[1]);
            terrain.heightmapCoordsToXYZ(new Vector2(10, 37), out waitpoints[2]);

            this.enemies = new List<Enemy>();
            this.enemies.Add(new Soldier(waitpoints, terrain));

            this.characters = new List<Character>();
            this.characters.Add(new Commando(terrain.getPosition(-200, 200), terrain));
            this.characters.Add(new Commando(terrain.getPosition(200, 200), terrain));
            this.characters.AddRange(this.enemies);

            //Seleccion multiple
            selection = new Selection(this.characters, this.terrain);

            //Movimiento por picking
            picking = new MovementPicking(this.terrain);
       
            //Inicializar camara
            camera = new FreeCamera(this.terrain.getPosition(0, 150), true);


        }


        /// <summary>
        /// Método que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aquí todo el código referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el último frame</param>
        public override void render(float elapsedTime)
        {
            picking.update(selection.getSelectedCharacters());

            camera.updateCamera();
            sky.render();
            terrain.render();

            foreach (Character ch in this.characters)
            {
                ch.render(elapsedTime);
            }

            foreach (Enemy enemy in this.enemies)
            {
                foreach (Character ch in this.characters)
                {
                    if(enemy.canSee(ch)) break;
                }
            }


            selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes




            if (GuiController.Instance.D3dInput.keyPressed(Key.X))
            {
                foreach (Character ch in selection.getSelectedCharacters())
                {
                    ch.die();

                }

            }
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
