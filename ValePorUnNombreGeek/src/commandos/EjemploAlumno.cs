using TgcViewer.Example;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;
using System.Collections.Generic;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier;


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
        MultipleSelection selection;

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
            selection = new MultipleSelection(this.terrain, this.characters);

            //Movimiento por picking
            picking = new MovementPicking(this.terrain);
       
            //Inicializar camara
            camera = new FreeCamera(this.terrain.getPosition(0, 150), true);


        }


        /// <summary>
        /// M�todo que se llama cada vez que hay que refrescar la pantalla.
        /// Escribir aqu� todo el c�digo referido al renderizado.
        /// Borrar todo lo que no haga falta
        /// </summary>
        /// <param name="elapsedTime">Tiempo en segundos transcurridos desde el �ltimo frame</param>
        public override void render(float elapsedTime)
        {
            Vector3 pickingPosition;
            if (picking.thereIsPicking(out pickingPosition))
            {
                foreach (Character ch in selection.getSelectedCharacters())
                {
                    if(ch.userCanMove()) ch.setPositionTarget(pickingPosition);
                }
            }

            camera.updateCamera();
            sky.render();
            terrain.render();

            foreach (Character ch in this.characters)
            {
                ch.render(elapsedTime);
            }

            selection.update(); //IMPORTANTE: selection.update SE LLAMA DESPUES de renderizar los personajes

            foreach (Enemy enemy in this.enemies)
            {
                enemy.renderVision();
                //IMPORTANTE: enemy.renderVision SE LLAMA DESPUES de renderizar la caja de seleccion
                //Para mas informacion leer el comentario en Enemy.renderVision
            }





            if (GuiController.Instance.D3dInput.keyPressed(Key.X))
            {
                foreach (Character ch in this.characters)
                {
                    ch.die();

                }

            }
        }


        /// <summary>
        /// M�todo que se llama cuando termina la ejecuci�n del ejemplo.
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
