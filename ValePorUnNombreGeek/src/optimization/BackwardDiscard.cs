using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.Input;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class BackwardDiscard : IQuadTree
    {
        private List<ILevelObject> objects;
        private List<Character> characters;
        private ITerrain terrain;


        /* BackwardDiscard
         * Optimiza el renderizado dibujando solo los objetos que esten por delante de la
         * ubicacion de la camara (es decir, delante del plano de corte de la camara).
         * Para eso solo se tiene en cuenta la posicion de dichos objetos.
         * 
         * Resultados en mi pc 26/05/2013:
         * Sin optimizacion - 750fps
         * BackwardDiscard (grilla 3x3, viendo todo el mapa) - 750fps
         * BackwardDiscard (grilla 3x3, viendo de a 2 o 3 sectores) - entre 900 y 1100fps
         * 
         * Problemas a resolver:
         * El dibujado de un sector del terreno tiene en cuenta su centro, por lo tanto cuando
         * la camara sobrepasa este, no se renderiza el sector, quedando un agujero negro (xD).
         */

        #region Constructor

        public BackwardDiscard(ITerrain _terrain, IRenderer _renderer)
        {
            this.Renderer = _renderer;
            this.terrain = _terrain;
            this.objects = new List<ILevelObject>();
            this.characters = new List<Character>();
        }

        public void add(ILevelObject obstacle)
        {
            this.objects.Add(obstacle);
        }

        public void add(Character ch)
        {
            this.characters.Add(ch);
        }

        public IRenderer Renderer { get; set; }

        #endregion


        public void render(TgcFrustum frustum)
        {
            this.Renderer.beginRender();

            TgcCamera camera = GuiController.Instance.CurrentCamera;

            //buscamos la direccion en la que mira la camara
            Vector3 cameraSeen = camera.getLookAt() - camera.getPosition();
            //hallamos su perpendicular
            Vector3 cameraCut = Vector3.Cross(cameraSeen, new Vector3(0, 1, 0));


            foreach (Character ch in this.characters)
            {
                //ubicacion del personaje respecto de la camara
                Vector3 chCameraPos = ch.Position - camera.getPosition();

                //vemos si esta detras de la direccion en la que mira
                Vector3 asd = Vector3.Cross(cameraCut, chCameraPos);
                if (asd.Y > 0)
                    this.Renderer.render(ch);
            }

            foreach (TerrainPatch p in this.terrain.Patches)
            {
                Vector3 tpCameraPos = p.BoundingBox.calculateBoxCenter() - camera.getPosition();
                Vector3 asd = Vector3.Cross(cameraCut, tpCameraPos);
                if (asd.Y > 0)
                    this.Renderer.render(p);
            }

            foreach (ILevelObject o in this.objects)
            {
                Vector3 oCameraPos = o.Center - camera.getPosition();
                Vector3 asd = Vector3.Cross(cameraCut, oCameraPos);
                if (asd.Y > 0)
                    this.Renderer.render(o);
            }

            this.Renderer.endRender();
        }


        public void dispose()
        {
            this.Renderer.dispose();
        }
    }
}
