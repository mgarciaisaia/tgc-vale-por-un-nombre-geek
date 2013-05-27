﻿using System;
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
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTree : ICulling
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
         * Resultados en mi pc 26/05/2013:
         * Sin optimizacion - 750fps
         * QuadTree (grilla 3x3, viendo todo el mapa) - 700fps
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


        protected override void filterAlgorithm(TgcFrustum frustum)
        {
            //buscamos los sectores del terreno que ve la camara
            foreach (QTSector sector in this.sectors)
                if (sector.collidesWithFrustum(frustum))
                {
                    this.filteredObjects.AddRange(sector.Objects);
                    this.filteredPatches.Add(sector.TerrainPatch);
                }

            this.filteredCharacters.AddRange(this.characters);
            ////buscamos los personajes que ve la camara
            //foreach (Character ch in this.characters)
            //    if (TgcCollisionUtils.testPointFrustum(frustum, ch.Position))
            //        this.Renderer.render(ch);
        }
    }
}
