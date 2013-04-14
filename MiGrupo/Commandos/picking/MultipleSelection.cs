using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.picking
{
    class MultipleSelection
    {
        private bool selecting = false;
        private Vector3 initSelectionPoint;
        TgcPickingRay pickingRay;
        TgcBox selectionBox;

        private const float SELECTION_BOX_HEIGHT = 400;


        public MultipleSelection()
        {
            //Crear caja para marcar en que lugar hubo colision
            selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            selectionBox.BoundingBox.setRenderColor(Color.Red);

            //Iniciarlizar PickingRay
            pickingRay = new TgcPickingRay();
        }

        public void update()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;


            //Si hacen clic con el mouse, ver si hay colision con el suelo
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Detectar nuevo punto de colision con el piso
                pickingRay.updateRay();

                Vector3 origin;
                Vector3 direction;
                float t;

                origin = pickingRay.Ray.Origin;
                direction = pickingRay.Ray.Direction;

                t = -(origin.Y / direction.Y);




                //primera vez
                if (!selecting)
                {
                    pickingRay.updateRay();


                    this.initSelectionPoint = origin + t * direction;

                    selecting = true;
                }
                //Si se está seleccionado, generar box de seleccion
                else
                {
                    Vector3 collisionPoint = origin + t * direction;
                    
                    Vector3 min = Vector3.Minimize(initSelectionPoint, collisionPoint);
                    Vector3 max = Vector3.Maximize(initSelectionPoint, collisionPoint);
                    min.Y = 0;
                    max.Y = SELECTION_BOX_HEIGHT;

                    //Configurar BOX
                    selectionBox.setExtremes(min, max);
                    selectionBox.updateValues();

                    selectionBox.BoundingBox.render();
                }
            }




            //Solto el clic del mouse, terminar la selección
            if (GuiController.Instance.D3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                selecting = false;
                /*
                //Ver que modelos quedaron dentro del area de selección seleccionados
                foreach (TgcMesh mesh in modelos)
                {
                    //Colisión de AABB entre área de selección y el modelo
                    if (TgcCollisionUtils.testAABBAABB(selectionBox.BoundingBox, mesh.BoundingBox))
                    {
                        modelosSeleccionados.Add(mesh);
                    }
                }
                 */
            }
        }
    }
}
