using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    class PCamera : TgcCamera
    {
        private const float MOVEMENT_SPEED = 600;
        private const float BORDER_WIDTH = 50;

        private const float ZOOM_SPEED = 400;
        private const float DISTANCE_MIN = 100;
        private const float DISTANCE_MAX = 1600;

        private Vector3 center;
        private Vector3 ctpv; //'Center to Position' Versor
        private float distance;

        private Matrix viewMatrix;

        private ITerrain terrain;


        public PCamera(Vector3 _center, ITerrain _terrain)
        {
            this.center = _center;
            this.terrain = _terrain;
            this.ctpv = Vector3.Normalize(new Vector3(0, 2, 1));
            this.distance = (DISTANCE_MAX - DISTANCE_MIN) / 2;
            this.updateViewMatrix();
            GuiController.Instance.CurrentCamera = this;
        }

        public Vector3 getPosition()
        {
            return this.center + (this.ctpv * this.distance);
        }

        public Vector3 getLookAt()
        {
            return this.center;
        }

        private Vector2 lastMousePos;
        private bool rotating = false;
        public void updateCamera()
        {
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            float elapsedTime = GuiController.Instance.ElapsedTime;

            Vector2 mousePos = new Vector2(d3dInput.Xpos, d3dInput.Ypos);
            float viewportHeight = GuiController.Instance.D3dDevice.Viewport.Height;
            float viewportWidth = GuiController.Instance.D3dDevice.Viewport.Width;


            //Desplazamiento

            Vector3 desplazamientoLateral = Vector3.Normalize(Vector3.Cross(this.ctpv, new Vector3(0, 1, 0)));
            Vector3 desplazamientoFrontal = Vector3.Normalize(Vector3.Cross(desplazamientoLateral, new Vector3(0, 1, 0)));

            if (d3dInput.keyDown(Key.RightArrow))
                this.center += desplazamientoLateral * MOVEMENT_SPEED * elapsedTime;
            if (d3dInput.keyDown(Key.LeftArrow))
                this.center -= desplazamientoLateral * MOVEMENT_SPEED * elapsedTime;
            if (d3dInput.keyDown(Key.UpArrow))
                this.center += desplazamientoFrontal * MOVEMENT_SPEED * elapsedTime;
            if (d3dInput.keyDown(Key.DownArrow))
                this.center -= desplazamientoFrontal * MOVEMENT_SPEED * elapsedTime;

            if (mousePos.X > viewportWidth - BORDER_WIDTH) //derecha
                this.center += desplazamientoLateral * MOVEMENT_SPEED * elapsedTime;
            if (mousePos.X < BORDER_WIDTH) //izquierda
                this.center -= desplazamientoLateral * MOVEMENT_SPEED * elapsedTime;
            if (mousePos.Y < BORDER_WIDTH) //arriba
                this.center += desplazamientoFrontal * MOVEMENT_SPEED * elapsedTime;
            if (mousePos.Y > viewportHeight - BORDER_WIDTH) //abajo
                this.center -= desplazamientoFrontal * MOVEMENT_SPEED * elapsedTime;


            //Rotacion

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            {
                if (!this.rotating)
                {
                    this.rotating = true;
                    this.lastMousePos = mousePos;
                }
                else
                {
                    float dx = lastMousePos.X - mousePos.X;
                    if (dx != 0)
                    {
                        dx *= FastMath.PI;
                        dx /= viewportWidth;

                        Vector3 rotationAxis = new Vector3(0, dx, 0);
                        rotationAxis.Normalize();

                        Matrix transMatrix = Matrix.RotationAxis(rotationAxis, FastMath.Abs(dx));
                        this.ctpv.TransformCoordinate(transMatrix);
                    }

                    float dy = lastMousePos.Y - mousePos.Y;
                    if (dy != 0)
                    {
                        dy *= FastMath.PI;
                        dy /= viewportHeight;

                        Vector3 rotationAxis = Vector3.Cross(this.ctpv, new Vector3(0, dy, 0));
                        rotationAxis.Normalize();

                        Matrix transMatrix = Matrix.RotationAxis(rotationAxis, FastMath.Abs(dy));
                        this.ctpv.TransformCoordinate(transMatrix);
                    }

                    this.lastMousePos = mousePos;
                }
            }
            else if (d3dInput.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
                this.rotating = false;


            //Distancia

            if (d3dInput.keyDown(Key.Z) && this.distance < DISTANCE_MAX)
                this.distance += ZOOM_SPEED * elapsedTime;
            if (d3dInput.keyDown(Key.A) && this.distance > DISTANCE_MIN)
                this.distance -= ZOOM_SPEED * elapsedTime;


            //Actualizacion de la matriz de transformacion

            this.updateCenter();
            this.updateViewMatrix();
        }

        private void updateViewMatrix()
        {
            this.viewMatrix = Matrix.LookAtLH(this.getPosition(), this.getLookAt(), new Vector3(0, 1, 0));
        }

        private void updateCenter()
        {
            this.center = terrain.getPosition(this.center.X, this.center.Z);
            this.center.Y /= 2f;
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            d3dDevice.Transform.View = this.viewMatrix;
        }
    }
}
