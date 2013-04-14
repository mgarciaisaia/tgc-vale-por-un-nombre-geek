using System;
using TgcViewer.Example;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.Pruebas;
using TgcViewer.Utils._2D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.PruebaEscenario
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

            String mediaDir = GuiController.Instance.AlumnoEjemplosMediaDir;

            pathHeightmap = mediaDir + "Heightmaps\\" + "heightmap.jpg";

            pathTextura = mediaDir + "Heightmaps\\" + "TerrainTexture5.jpg";


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


            GuiController.Instance.Modifiers.addBoolean("Terrain", "wireframe", true);
            

        }



      
        public override void render(float elapsedTime)
        {
            Utils.desplazarVistaConMouse(200);

            if(picking(out newPosition))
                collisionPointMesh.Position = newPosition;


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
                    //Proyectar punto en el heightmap
                    //p = new Vector3(colisionPlano.X, terrain.getHeight(colisionPlano), colisionPlano.Z);
                    p = new Vector3(colisionPlano.X, colisionPlano.Y, colisionPlano.Z);

                    float i;

                    //calculamos posicion real en z
                    for (i = pickingRay.Ray.Origin.Z; i <= p.Z; i++)
                    {
                        if ((pickingRay.Ray.Origin.Y / Math.Abs(p.Z - pickingRay.Ray.Origin.Z)) < (terrain.getHeight(p.X, i) / Math.Abs(p.Z - i)))
                        {
                            p = new Vector3(p.X, terrain.getHeight(p.X, i), i);
                            break;
                        }
                    }

                    return true;

                }                    
            }

            p = Vector3.Empty;
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
