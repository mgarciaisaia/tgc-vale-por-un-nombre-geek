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
            float i0 = (terrain.Position.Y - this.getRay().Origin.Y) / this.getRay().Direction.Y;
            float i = i0;

            while (true)
            {
                aPoint = this.getRay().Origin + i * this.getRay().Direction;

                if (GeneralMethods.isCloseTo(aPoint.Y, terrain.getHeight(aPoint.X, aPoint.Z)))
                //if (GeneralMethods.isCloseTo(aPoint.Y, this.maxHeight(terrain, aPoint.X, aPoint.Z)))
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

        /*private Vector3 getRayIntersectionFromHeavenToEarth(Terrain terrain)
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
        }*/

        /*private float maxHeight(Terrain terrain, float x, float z)
        {
            //dado un punto en el heightmap, calcula la altura maxima en los puntos mas cercanos
            //se usa para que la caja de seleccion no se vuelva "loca" en algunos lugares del heightmap
            //consume mucho. fuera de uso.
            const int DELTA = 2;
            int i;
            float total = terrain.Position.Y;

            for (i = -DELTA; i <= DELTA; i++)
            {
                total = Math.Max(total, terrain.getHeight(x + i, z));
                total = Math.Max(total, terrain.getHeight(x, z + i));
            }
            return total;
        }*/

        public Vector3 getRayGroundIntersection(Terrain terrain)
        {
            //retorna el punto de colision con el plano y=0
            //(pablo) lo uso para ver si el rayo vario su posicion. es mucho mas rapido que getRayIntersection; salva fps.
            float t0 = (terrain.Position.Y - this.getRay().Origin.Y) / this.getRay().Direction.Y;
            return this.getRay().Origin + t0 * this.getRay().Direction;
        }
    }
}
