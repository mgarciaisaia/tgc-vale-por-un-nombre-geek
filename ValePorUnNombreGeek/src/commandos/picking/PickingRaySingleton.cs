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
        private static bool varsInitizalized = false;


        public static PickingRaySingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PickingRaySingleton();
                    
                  
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

            Vector3 aPoint;
            Vector3 foundedPoint;
            float t0 = (terrain.Position.Y - Instance.Ray.Origin.Y) / Instance.Ray.Direction.Y;
            float t = t0;
            while (true)
            {
                aPoint = Instance.Ray.Origin + t * Instance.Ray.Direction;

                if (terrain.getPosition(aPoint.X, aPoint.Z, out foundedPoint))
                {
                    if (GeneralMethods.isCloseTo(aPoint.Y, foundedPoint.Y, 1))
                    {
                        //encontramos el punto de interseccion
                        position = aPoint;
                        if (!varsInitizalized) initVars();
                        //muestra el punto encontrado
                        GuiController.Instance.UserVars.setValue("WorldX", position.X);
                        GuiController.Instance.UserVars.setValue("WorldY", position.Y);
                        GuiController.Instance.UserVars.setValue("WorldZ", position.Z);

                        Vector2 hmCoords;
                        terrain.xzToHeightmapCoords(aPoint.X, aPoint.Z, out hmCoords);
                        GuiController.Instance.UserVars.setValue("hmX", hmCoords.X);
                        GuiController.Instance.UserVars.setValue("hmY", hmCoords.Y);

                        return true;
                    }
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

        private void initVars()
        {
            GuiController.Instance.UserVars.addVar("WorldX");
            GuiController.Instance.UserVars.addVar("WorldY");
            GuiController.Instance.UserVars.addVar("WorldZ");
            GuiController.Instance.UserVars.addVar("hmX");
            GuiController.Instance.UserVars.addVar("hmY");
            varsInitizalized = true;
        }

        /// <summary>
        /// Busca la interseccion rayo-plano y=0.
        /// </summary>
        public Vector3 getRayGroundIntersection(Terrain terrain)
        {
            //retorna el punto de colision con el plano y=0
            //(pablo) lo uso para ver si el rayo vario su posicion. es mucho mas rapido que getRayIntersection; salva fps.
            this.updateRay();

            float t0 = (terrain.minY - this.Ray.Origin.Y) / this.Ray.Direction.Y;
            return this.Ray.Origin + t0 * this.Ray.Direction;
        }
    }
}
