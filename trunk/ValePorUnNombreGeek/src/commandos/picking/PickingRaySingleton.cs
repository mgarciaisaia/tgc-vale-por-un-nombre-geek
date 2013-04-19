using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
{
    class PickingRaySingleton : TgcPickingRay
    {
        private static PickingRaySingleton instance;


        public static PickingRaySingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PickingRaySingleton();
                    
                    GuiController.Instance.UserVars.addVar("WorldX");
                    GuiController.Instance.UserVars.addVar("WorldY");
                    GuiController.Instance.UserVars.addVar("WorldZ");
                    GuiController.Instance.UserVars.addVar("hmX");
                    GuiController.Instance.UserVars.addVar("hmY");
                }
                return instance;
            }
        }


        /// <summary>
        /// Busca la interseccion rayo-heightmap, y devuelve true si existe.
        /// </summary>
        public bool terrainIntersection(Terrain terrain, out Vector3 position)
        {
            this.updateRay();

            Vector3 myPoint;
            float terrainY;
            float i0 = (terrain.Position.Y - this.Ray.Origin.Y) / this.Ray.Direction.Y;
            float i = i0;
            while (true)
            {
                myPoint = this.Ray.Origin + i * this.Ray.Direction;

                if (terrain.getY(myPoint.X, myPoint.Z, out terrainY))
                {
                    if (GeneralMethods.isCloseTo(myPoint.Y, terrainY))
                    {
                        //encontramos el punto de interseccion
                        position = myPoint;

                        //muestra el punto encontrado
                        GuiController.Instance.UserVars.setValue("WorldX", position.X);
                        GuiController.Instance.UserVars.setValue("WorldY", position.Y);
                        GuiController.Instance.UserVars.setValue("WorldZ", position.Z);

                        Vector2 hmCoords;
                        terrain.xzToHeightmapCoords(myPoint.X, myPoint.Z, out hmCoords);
                        GuiController.Instance.UserVars.setValue("hmX", hmCoords.X);
                        GuiController.Instance.UserVars.setValue("hmY", hmCoords.Y);

                        return true;
                    }
                }
                else if (myPoint.Y >= terrain.maxY() || myPoint.Y < terrain.minY())
                {
                    //ya nos fuimos o muy arriba o muy abajo
                    position = Vector3.Empty;
                    return false;
                }

                i--;
            }
        }

        /// <summary>
        /// Busca la interseccion rayo-plano y=0.
        /// </summary>
        public Vector3 getRayGroundIntersection(Terrain terrain)
        {
            //retorna el punto de colision con el plano y=0
            //(pablo) lo uso para ver si el rayo vario su posicion. es mucho mas rapido que getRayIntersection; salva fps.
            this.updateRay();

            float t0 = (terrain.minY() - this.Ray.Origin.Y) / this.Ray.Direction.Y;
            return this.Ray.Origin + t0 * this.Ray.Direction;
        }
    }
}
