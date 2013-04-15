using System;
using TgcViewer.Example;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils._2D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.pruebas.PruebaEscenario
{
    /// <summary>
    /// Prueba de picking en Heightmap usando TgcBox
    /// </summary>
    public class PruebaPickingEnHeightmapUsandoTgcBox : TgcExample
    {
        Terrain terrain;
        string pathHeightmap;
        string pathTextura;

        //Picking
        TgcPickingRay pickingRay;
        Vector3 newPosition;
        TgcBox collisionPointMesh;
        TgcBox planeCollisionPointMesh;
        TgcBox planoAuxiliar;
    


        public override string getCategory()
        {
            return "AlumnoEjemplos";
        }

        /// <summary>
        /// Completar nombre del grupo en formato Grupo NN
        /// </summary>
        public override string getName()
        {
            return "PruebaPickingHeightmapTgcBox";
        }

        /// <summary>
        /// Completar con la descripción del TP
        /// </summary>
        public override string getDescription()
        {
            return "Prueba de picking en heightmap usando un tgcBox debajo";
        }

        public override void init()
        {

            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir+ "ValePorUnNombreGeek\\";

            pathHeightmap = mediaDir + "Heightmaps\\" + "heightmap.jpg";

            pathTextura = mediaDir + "Heightmaps\\" + "TerrainTexture5.jpg";

            GuiController.Instance.UserVars.addVar("PuntoClick");
            GuiController.Instance.UserVars.addVar("Caja");
           
            //Cargar heightmap          
            terrain = new Terrain();
            terrain.loadHeightmap(pathHeightmap, 20f, 2f, new Vector3(0, 0, 0));
            terrain.loadTexture(pathTextura);



            //Picking
            planoAuxiliar = TgcBox.fromSize(new Vector3(0, 0, 0), new Vector3(terrain.getWidth()*terrain.getScaleXZ(), 0.1f, terrain.getLength()*terrain.getScaleXZ()), Color.Blue);
            pickingRay = new TgcPickingRay();
            collisionPointMesh = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Red);
            planeCollisionPointMesh = TgcBox.fromSize(new Vector3(30, 10, 30), Color.Green);


            //Configurar camara en Tercer Persona
            GuiController.Instance.ThirdPersonCamera.Enable = true;
            GuiController.Instance.ThirdPersonCamera.setCamera(new Vector3(0, terrain.getHeight(0,0),0), 500, -120);


            

        }



      
        public override void render(float elapsedTime)
        {
            Utils.desplazarVistaConMouse(200);

            if (picking(out newPosition))
            {
                Vector2 coords;
                terrain.xzToHeightmapCoords(newPosition.X, newPosition.Z, out coords);
                       
                collisionPointMesh.Position = newPosition;
                GuiController.Instance.UserVars.setValue("Caja", newPosition);
                GuiController.Instance.UserVars.setValue("PuntoClick", coords);
               
            }
            if ((bool)GuiController.Instance.Modifiers.getValue("Terrain"))
            {
                planoAuxiliar.render();
                planeCollisionPointMesh.render();
                
                terrain.renderWireframe();
            }
            else terrain.render();

            collisionPointMesh.render();
          
        }

        private bool picking(out Vector3 p)
        {
            //Si hacen clic con el mouse, ver si hay colision con el suelo
            if (GuiController.Instance.D3dInput.buttonPressed(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Actualizar Ray de colisión en base a posición del mouse
                pickingRay.updateRay();

                Vector3 colisionPlano;

               

                //Detectar colisión Ray-AABB
                if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, planoAuxiliar.BoundingBox, out colisionPlano)){

                    planeCollisionPointMesh.Position = colisionPlano;

                    p = new Vector3(colisionPlano.X, colisionPlano.Y, colisionPlano.Z);

                    Vector3 origen = pickingRay.Ray.Origin;
                    Vector3 director = pickingRay.Ray.Direction;

                    float i = 0;

                    Vector3 unPunto;

                    while (true)
                    {
                        unPunto = origen + i * director;
                        if (estaCerca(unPunto.Y, terrain.getHeight(unPunto.X, unPunto.Z)))
                        {
                            //encontramos el punto de interseccion
                            p = unPunto;
                            break;
                        }

                        if (unPunto.Y < colisionPlano.Y) break;

                        i++;
                    }
                    return true;
                }                    
            }

            p = Vector3.Empty;
            return false;
        }

        private bool estaCerca(float a, float b)
        {
            if (a < b + 1 && a < b + 1) return true;
            return false;
        }

        public override void close()
        {
            terrain.dispose();
            planeCollisionPointMesh.dispose();
            collisionPointMesh.dispose();
            planoAuxiliar.dispose();
        }


    }



}
