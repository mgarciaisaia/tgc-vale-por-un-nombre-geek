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

        private void updateRay()
        {
            this.pickingRay.updateRay();
        }

        public TgcRay getRay()
        {
            return this.pickingRay.Ray;
        }

        public bool terrainIntersection(Terrain terrain, out Vector3 position)
        {
            //Version que va "de la tierra al cielo" -> beneficia ENORMEMENTE picking en terrenos bajos
            this.updateRay();

            Vector3 myPoint;
            float terrainY;
            float i0 = (terrain.Position.Y - this.getRay().Origin.Y) / this.getRay().Direction.Y;
            float i = i0;

            while (true)
            {
                myPoint = this.getRay().Origin + i * this.getRay().Direction;

                if (terrain.getY(myPoint.X, myPoint.Z, out terrainY))
                {
                    if (GeneralMethods.isCloseTo(myPoint.Y, terrainY))
                    {
                        //encontramos el punto de interseccion
                        position = myPoint;
                        return true;
                    }
                }
                else if (myPoint.Y >= 255 * terrain.getScaleY())
                    {
                        //ya nos estamos llendo al cielo...
                        position = Vector3.Empty;
                        return false;
                    }

                i--;
            }
        }

        public Vector3 getRayGroundIntersection(Terrain terrain)
        {
            //retorna el punto de colision con el plano y=0
            //(pablo) lo uso para ver si el rayo vario su posicion. es mucho mas rapido que getRayIntersection; salva fps.
            this.updateRay();

            float t0 = (terrain.Position.Y - this.getRay().Origin.Y) / this.getRay().Direction.Y;
            return this.getRay().Origin + t0 * this.getRay().Direction;
        }
    }
}
