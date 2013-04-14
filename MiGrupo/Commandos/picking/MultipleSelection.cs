﻿using System;
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
        Terrain terrain;

        private const float SELECTION_BOX_HEIGHT = 75;


        public MultipleSelection(Terrain _terrain)
        {
            //Crear caja para marcar en que lugar hubo colision
            this.selectionBox = TgcBox.fromSize(new Vector3(3, SELECTION_BOX_HEIGHT, 3), Color.Red);
            this.selectionBox.AlphaBlendEnable = true;
            this.selectionBox.Color = Color.FromArgb(150, Color.Green);

            //Iniciarlizar PickingRay
            this.pickingRay = new TgcPickingRay();

            this.terrain = _terrain;
        }

        public void update()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;


            //Si hacen clic con el mouse, ver si hay colision con el suelo
            if (GuiController.Instance.D3dInput.buttonDown(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //Detectar nuevo punto de colision con el piso
                pickingRay.updateRay();


                //primera vez
                if (!selecting)
                {
                    this.initSelectionPoint = GeneralMethods.intersectionPoint(pickingRay.Ray.Origin, pickingRay.Ray.Direction, this.terrain);
                    selecting = true;
                }
                //Si se está seleccionado, generar box de seleccion
                else
                {
                    Vector3 pointA = this.initSelectionPoint;
                    Vector3 pointB = GeneralMethods.intersectionPoint(pickingRay.Ray.Origin, pickingRay.Ray.Direction, this.terrain);
                    float selectionBoxHeight = GeneralMethods.max(pointA.Y, pointB.Y) + SELECTION_BOX_HEIGHT;

                    pointA.Y = 0;
                    pointB.Y = 0;

                    Vector3 min = Vector3.Minimize(pointA, pointB);
                    Vector3 max = Vector3.Maximize(pointA, pointB);
                    min.Y = 0;
                    max.Y = selectionBoxHeight;

                    //Configurar BOX
                    selectionBox.setExtremes(min, max);
                    selectionBox.updateValues();

                    selectionBox.render();
                }
            }




            //Solto el clic del mouse, terminar la selección
            if (GuiController.Instance.D3dInput.buttonUp(TgcViewer.Utils.Input.TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                selecting = false;
                
                //TODO select items

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

        public void dispose()
        {
            selectionBox.dispose();
        }
    }
}
