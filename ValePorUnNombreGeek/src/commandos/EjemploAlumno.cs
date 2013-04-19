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



            //Crear personajes seleccionables
            List<Character> selectableCharacters = new List<Character>();
            selectableCharacters.Add(new Character(terrain.getPosition(-200, 200), terrain));
            selectableCharacters.Add(new Character(terrain.getPosition(200, 200), terrain));

            //Seleccion multiple
            selection = new MultipleSelection(this.terrain, selectableCharacters);

            //Crear el resto de los personajes
            this.characters = new List<Character>();
            this.characters.AddRange(selectableCharacters);

            Vector3[] waitpoints = new Vector3[3];
            /*waitpoints[0] = new Vector3(560, 0, 560);
            waitpoints[1] = new Vector3(-560, 0, 560);
            waitpoints[2] = new Vector3(-560, 0, -560);*/
            terrain.heightmapCoordsToXYZ(new Vector2(73, 81), out waitpoints[0]);
            terrain.heightmapCoordsToXYZ(new Vector2(22, 80), out waitpoints[1]);
            terrain.heightmapCoordsToXYZ(new Vector2(10, 37), out waitpoints[2]);
            this.characters.Add(new Soldier(waitpoints, terrain));

            //Movimiento por picking
            picking = new MovementPicking(this.terrain);
       
            //Inicializar camara
            camera = new StaticCamera(this.terrain.getPosition(0, 150));


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
