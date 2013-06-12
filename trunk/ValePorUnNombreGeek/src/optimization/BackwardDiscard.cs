﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class BackwardDiscard : Culling
    {
        /* BackwardDiscard
         * Optimiza el renderizado dibujando solo los objetos que esten por delante de la
         * ubicacion de la camara (es decir, delante del punto proyeccion de la ubicacion
         * de la camara sobre el plano y = 0).
         * Para objetos y personajes solo se tiene en cuenta su centro.
         * Para sectores del terreno se aplica un offset para dibujarlos un poco antes de
         * su posicion real.
         * 
         * Resultados en mi pc 29/05/2013:
         * Sin optimizacion - 750fps
         * BackwardDiscard (grilla 3x3, viendo todo el mapa) - 750fps
         * BackwardDiscard (grilla 3x3, viendo de a 2 o 3 sectores) - entre 900 y 1100fps
         * 
         * Problemas conocidos:
         * -Cuando inicia el juego no dibuja ningun sector del terreno hasta que se rote
         * la camara al menos minimamente.
         * 
         * Nota: esta tecnica de optimizacion no se debe usar junto con PlaneDiscard.
         */

        protected override void filterAlgorithm()
        {
            TgcCamera camera = GuiController.Instance.CurrentCamera;
            Vector3 cameraSeen = camera.getLookAt() - camera.getPosition();

            Vector3 cameraCut = Vector3.Cross(cameraSeen, new Vector3(0, 1, 0));


            float tpOffsetX = cameraSeen.X / FastMath.Abs(cameraSeen.X);
            float tpOffsetZ = cameraSeen.Z / FastMath.Abs(cameraSeen.Z);


            foreach (TerrainPatch tp in this.patches)
            {
                //primero movemos el centro "lo mas adelante posible respecto de la camara"
                Vector3 tpCenter = tp.BoundingBox.calculateBoxCenter();
                Vector3 tpSize = tp.BoundingBox.calculateSize() * 0.5f;
                Vector3 tpOffset = new Vector3(tpOffsetX * tpSize.X, 0, tpOffsetZ * tpSize.Z);

                if (isInFrontOfCamera(tpCenter + tpOffset, camera.getPosition(), cameraCut))
                    this.filteredPatches.Add(tp);
            }

            foreach (ILevelObject obj in this.objects)
                if (isInFrontOfCamera(obj.Center, camera.getPosition(), cameraCut))
                    this.filteredObjects.Add(obj);

            foreach (Character ch in this.characters)
                if (isInFrontOfCamera(ch.Center, camera.getPosition(), cameraCut))
                    this.filteredCharacters.Add(ch);
        }

        private bool isInFrontOfCamera(Vector3 point, Vector3 cameraPos, Vector3 cameraCut)
        {
            return Vector3.Cross(cameraCut, point - cameraPos).Y > 0;
        }
    }
}