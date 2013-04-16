using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
{
    class PickingRayHome
    {
        private static PickingRayHome instance;
        TgcPickingRay pickingRay;

        private PickingRayHome()
        {
            this.pickingRay = new TgcPickingRay();
        }

        public static PickingRayHome getInstance()
        {
            if (instance == null) instance = new PickingRayHome();
            return instance;
        }

        public void updateRay()
        {
            this.pickingRay.updateRay();
        }

        public TgcRay getRay()
        {
            return this.pickingRay.Ray;
        }

        public Vector3 getRayIntersection(Terrain terrain)
        {
            return this.getRayIntersectionFromEarthToHeaven(terrain);
        }

        private Vector3 getRayIntersectionFromEarthToHeaven(Terrain terrain)
        {
            //Version que va "de la tierra al cielo" -> beneficia ENORMEMENTE picking en terrenos bajos
            Vector3 aPoint;
            float TERRAIN_MIN_Y = terrain.Position.Y;
            float i0 = (TERRAIN_MIN_Y - this.getRay().Origin.Y) / this.getRay().Direction.Y;
            float i = i0;

            while (true)
            {
                aPoint = this.getRay().Origin + i * this.getRay().Direction;

                if (GeneralMethods.isCloseTo(aPoint.Y, terrain.getHeight(aPoint.X, aPoint.Z)))
                {
                    //encontramos el punto de interseccion
                    return aPoint;
                }

                if (aPoint.Y >= 255 * terrain.getScaleY())
                {
                    //ya nos estamos llendo al cielo...
                    return this.getRay().Origin + i0 * this.getRay().Direction;
                }

                i--;
            }
        }

        private Vector3 getRayIntersectionFromHeavenToEarth(Terrain terrain)
        {
            //Version que va "del cielo a la tierra" -> beneficia en gran medida picking en terrenos altos
            //actualmente fuera de uso.
            Vector3 aPoint;
            float i = 0;

            while (true)
            {
                aPoint = this.getRay().Origin + i * this.getRay().Direction;

                if (GeneralMethods.isCloseTo(aPoint.Y, terrain.getHeight(aPoint.X, aPoint.Z)))
                {
                    //encontramos el punto de interseccion
                    return aPoint;
                }

                if (aPoint.Y <= terrain.Position.Y)
                {
                    //ya nos estamos llendo al subsuelo...
                    return aPoint;
                }

                i++;
            }
        }
    }
}
