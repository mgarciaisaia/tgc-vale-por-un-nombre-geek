using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking
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
