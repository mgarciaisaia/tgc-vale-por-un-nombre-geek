using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTree : Culling
    {
        private List<QTSector> sectors;


        /* QuadTree
         * Optimiza el renderizado dibujando solo los sectores del terreno que colisionan
         * con el frustum de la camara.
         * Si hablamos de los objetos, cada uno esta relacionado a un sector, de manera
         * que si un sector no se renderiza, sus objetos tampoco.
         * Por el lado de los personajes, se busca la colision punto-frustum de su posicion
         * para determinar si se renderiza o no.
         * 
         * Resultados en mi pc 27/05/2013:
         * Sin optimizacion - 750fps
         * QuadTree (grilla 3x3, viendo todo el mapa) - entre 700 y 750fps
         * QuadTree (grilla 3x3, viendo de a 2 o 3 sectores) - entre 850 y 1000fps
         */

        #region Constructor

        public QuadTree(ITerrain terrain)
            : base()
        {
            sectors = new List<QTSector>();

            foreach(TerrainPatch tp in terrain.Patches)
                sectors.Add(new QTSector(tp));
        }

        public void add(ILevelObject obj)
        {
            foreach (QTSector sector in this.sectors)
                sector.addObjectIfCollides(obj);
        }

        #endregion


        protected override void fillAlgorithm()
        {
            TgcFrustum frustum = CommandosUI.Instance.CameraFrustum;

            //if (CommandosUI.Instance.Camera.Distance > 1000)
            //{
            //    //como la camara esta lejos del terreno, no optimizamos nada por que es muy costoso
            //    this.filteredPatches.AddRange(this.patches);
            //    this.filteredObjects.AddRange(this.objects);
            //    this.filteredCharacters.AddRange(this.characters);
            //    CommandosUI.Instance.log("lejos!" + CommandosUI.Instance.ElapsedTime.ToString());
            //    return;
            //}

            //buscamos los sectores del terreno que ve la camara
            foreach (QTSector sector in this.sectors)
                if (this.patches.Contains(sector.TerrainPatch) &&
                    sector.collidesWithFrustum(frustum))
                {
                    this.filteredPatches.Add(sector.TerrainPatch);

                    //buscamos los objetos del sector
                    foreach(ILevelObject obj in sector.Objects)
                        if(this.objects.Contains(obj))
                            this.filteredObjects.Add(obj);

                    //buscamos los personajes sobre el sector
                    foreach (Character ch in this.characters)
                        if (GeneralMethods.pointIsOverAABB(ch.Position, sector.TerrainPatch.BoundingBox))
                            this.filteredCharacters.Add(ch);
                }
        }
    }
}
