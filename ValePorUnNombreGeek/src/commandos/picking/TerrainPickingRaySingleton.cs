using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
{
    class TerrainPickingRaySingleton : TgcPickingRay
    {
        private static TerrainPickingRaySingleton instance;


        public static TerrainPickingRaySingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new TerrainPickingRaySingleton();
                    
                    //GuiController.Instance.UserVars.addVar("HeightmapCoords"); //sigo insistiendo, esto no deberia existir.
                    //las coordenadas del heightmap son problema del heightmap. todo se deberia tratar en terminos de coordenadas del mundo
                    GuiController.Instance.UserVars.addVar("WorldCoords");
                }
                return instance;
            }
        }


        public bool terrainIntersection(Terrain terrain, out Vector3 position)
        {
            //Version que va "de la tierra al cielo"
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

                        //GuiController.Instance.UserVars.setValue("HeightmapCoords", coords);
                        GuiController.Instance.UserVars.setValue("WorldCoords", position);

                        return true;
                    }
                }
                else if (myPoint.Y >= terrain.maxY() || myPoint.Y < terrain.minY())
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

            float t0 = (terrain.minY() - this.Ray.Origin.Y) / this.Ray.Direction.Y;
            return this.Ray.Origin + t0 * this.Ray.Direction;
        }
    }
}
