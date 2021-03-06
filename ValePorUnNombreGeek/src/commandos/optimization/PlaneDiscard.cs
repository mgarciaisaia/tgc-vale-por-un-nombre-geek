﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.optimization
{
    class PlaneDiscard : Culling
    {
        /* PlaneDiscard
         * Optimiza el renderizado dibujando solo los objetos que esten por delante de la
         * ubicacion de la camara (es decir, delante del plano de corte de la camara).
         * Para eso solo se tiene en cuenta la posicion de dichos objetos.
         * 
         * Resultados en mi pc 26/05/2013:
         * Sin optimizacion - 750fps
         * PlaneDiscard (grilla 3x3, viendo todo el mapa) - 750fps
         * PlaneDiscard (grilla 3x3, viendo de a 2 o 3 sectores) - entre 900 y 1100fps
         * 
         * Nota: esta tecnica de optimizacion no se debe usar junto con BackwardDiscard.
         */

        protected override void fillAlgorithm()
        {
            ICamera camera = CommandosUI.Instance.Camera;

            //buscamos el plano de la camara
            Vector3 n = camera.Direction;
            float d = Vector3.Dot(n, camera.getPosition());


            //filtramos los personajes ubicados por detras del plano
            foreach (Character ch in this.characters)
                if (pointIsInFrontOfPlane(ch.Center, n, d))
                    this.filteredCharacters.Add(ch);

            //filtramos los objetos ubicados por detras del plano
            foreach (ILevelObject o in this.objects)
                if (pointIsInFrontOfPlane(o.Center, n, d))
                    this.filteredObjects.Add(o);

            //ahora filtramos los sectores del terreno
            float signX;
            if (n.X != 0) signX = n.X / FastMath.Abs(n.X); else signX = 0;
            float signZ;
            if (n.Z != 0) signZ = n.Z / FastMath.Abs(n.Z); else signZ = 0;

            foreach (TerrainPatch tp in this.patches)
            {
                //primero movemos el centro "lo mas adelante posible respecto de la camara"
                Vector3 tpCenter = tp.BoundingBox.calculateBoxCenter();
                Vector3 tpSize = tp.BoundingBox.calculateSize() * 0.5f;
                Vector3 tpOffset = new Vector3(signX * tpSize.X, 0, signZ * tpSize.Z);

                //checkeamos que ese punto este por deltante de la camara
                if (pointIsInFrontOfPlane(tpCenter + tpOffset, n, d))
                    this.filteredPatches.Add(tp);
            }
        }

        private bool pointIsInFrontOfPlane(Vector3 point, Vector3 n, float d)
        {
            return Vector3.Dot(n, point) > d;
        }
    }
}
