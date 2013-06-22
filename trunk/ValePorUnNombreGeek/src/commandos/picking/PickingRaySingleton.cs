﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
{
    class PickingRaySingleton : PickingRay
    {
        private static PickingRaySingleton instance;


        public static PickingRaySingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PickingRaySingleton();
                    //createVars();     
                    
                }
                return instance;
            }
        }



        /// <summary>
        /// Busca la interseccion rayo-heightmap, y devuelve true si existe.
        /// </summary>
        public bool terrainIntersection(ITerrain terrain, out Vector3 position)
        {
            Vector3 aPoint;
            Vector3 terrainPoint;
            float t0 = (terrain.Position.Y - this.Ray.Origin.Y) / this.Ray.Direction.Y;
            float t = t0;
           
            while (true)
            {
                aPoint = this.Ray.Origin + t * this.Ray.Direction;

                if (terrain.getPosition(aPoint.X, aPoint.Z, out terrainPoint))
                    if (GeneralMethods.isCloseTo(aPoint.Y, terrainPoint.Y, 1))
                    {
                        //encontramos el punto de interseccion
                        position = aPoint;
                        return true;
                    }
                else if (aPoint.Y >= terrain.maxY || aPoint.Y < terrain.minY)
                {
                    //ya nos fuimos o muy arriba o muy abajo
                    position = Vector3.Empty;
                    return false;
                }

                t--;
            }
        }


        //public bool skyToGroundAlgorithm(ITerrain terrain, out Vector3 position)
        //{
        //    Vector3 aPoint;
        //    Vector3 terrainPoint;
        //    float t0 = (terrain.Position.Y - this.Ray.Origin.Y) / this.Ray.Direction.Y;
        //    float t = t0;

        //    while (true)
        //    {
        //        aPoint = this.Ray.Origin + t * this.Ray.Direction;

        //        if (terrain.getPosition(aPoint.X, aPoint.Z, out terrainPoint))
        //            if (GeneralMethods.isCloseTo(aPoint.Y, terrainPoint.Y, 1))
        //            {
        //                //encontramos el punto de interseccion
        //                position = aPoint;
        //                return true;
        //            }
        //            else if (aPoint.Y >= terrain.maxY || aPoint.Y < terrain.minY)
        //            {
        //                //ya nos fuimos o muy arriba o muy abajo
        //                position = Vector3.Empty;
        //                return false;
        //            }

        //        t--;
        //    }
        //}


        //public bool groundToSkyAlgorithm(ITerrain terrain, out Vector3 position)
        //{
        //    Vector3 aPoint;
        //    Vector3 terrainPoint;
        //    float t0 = (terrain.Position.Y - this.Ray.Origin.Y) / this.Ray.Direction.Y;
        //    float t = t0;

        //    while (true)
        //    {
        //        aPoint = this.Ray.Origin + t * this.Ray.Direction;

        //        if (terrain.getPosition(aPoint.X, aPoint.Z, out terrainPoint))
        //            if (GeneralMethods.isCloseTo(aPoint.Y, terrainPoint.Y, 1))
        //            {
        //                //encontramos el punto de interseccion
        //                position = aPoint;
        //                return true;
        //            }
        //            else if (aPoint.Y >= terrain.maxY || aPoint.Y < terrain.minY)
        //            {
        //                //ya nos fuimos o muy arriba o muy abajo
        //                position = Vector3.Empty;
        //                return false;
        //            }

        //        t--;
        //    }
        //}
    }
}
