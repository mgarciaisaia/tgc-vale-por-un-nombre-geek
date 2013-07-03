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
                    createVars();     
                    
                }
                return instance;
            }
        }

        private static void createVars()		
        {		
            GuiController.Instance.UserVars.addVar("WorldX");		
            GuiController.Instance.UserVars.addVar("WorldY");		
            GuiController.Instance.UserVars.addVar("WorldZ");		
        }

        /// <summary>
        /// Busca la interseccion rayo-heightmap, y devuelve true si existe.
        /// </summary>
        public bool terrainIntersection(ITerrain terrain, out Vector3 position)
        {
            if (this.Ray.Direction.Y == 0)
            {
                //parche para evitar un ciclo infinito
                position = Vector3.Empty;
                return false;
            }

            if (this.Ray.Direction.Y >= 0)
                //hizo click en un sector del terreno mas alto que la camara
                return this.skyToGroundAlgorithm(terrain, out position);
            else
                //hizo click en un sector del terreno mas bajo que la camara
                return this.groundToSkyAlgorithm(terrain, out position);
        }


        private bool skyToGroundAlgorithm(ITerrain terrain, out Vector3 position)
        {
            Vector3 aPoint;
            Vector3 terrainPoint;
            float t = 0;

            while (true)
            {
                aPoint = this.Ray.Origin + t * this.Ray.Direction;

                if (terrain.getPosition(aPoint.X, aPoint.Z, out terrainPoint))
                {
                    if (GeneralMethods.isCloseTo(aPoint.Y, terrainPoint.Y, 1))
                    {
                        //encontramos el punto de interseccion
                        position = aPoint;

                        try		
	                        {		
	                            GuiController.Instance.UserVars.setValue("WorldX", position.X);		
			
	                        }		
	                        catch (Exception)		
	                        {		
	                            createVars();		
	                            GuiController.Instance.UserVars.setValue("WorldX", position.X);		
	                        }		
	                        GuiController.Instance.UserVars.setValue("WorldY", position.Y);		
	                        GuiController.Instance.UserVars.setValue("WorldZ", position.Z);

                        return true;
                    }
                }
                else if (aPoint.Y >= terrain.maxY || aPoint.Y < terrain.minY)
                {
                    //ya nos fuimos o muy arriba o muy abajo
                    position = Vector3.Empty;
                    return false;
                }

                t++;
            }
        }


        private bool groundToSkyAlgorithm(ITerrain terrain, out Vector3 position)
        {
            Vector3 aPoint;
            Vector3 terrainPoint;
            float t = (terrain.Position.Y - this.Ray.Origin.Y) / this.Ray.Direction.Y;

            while (true)
            {
                aPoint = this.Ray.Origin + t * this.Ray.Direction;

                if (terrain.getPosition(aPoint.X, aPoint.Z, out terrainPoint))
                {
                    if (GeneralMethods.isCloseTo(aPoint.Y, terrainPoint.Y, 1))
                    {
                        //encontramos el punto de interseccion
                        position = aPoint;

                        try
                        {
                            GuiController.Instance.UserVars.setValue("WorldX", position.X);

                        }
                        catch (Exception)
                        {
                            createVars();
                            GuiController.Instance.UserVars.setValue("WorldX", position.X);
                        }
                        GuiController.Instance.UserVars.setValue("WorldY", position.Y);
                        GuiController.Instance.UserVars.setValue("WorldZ", position.Z);

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
    }
}
