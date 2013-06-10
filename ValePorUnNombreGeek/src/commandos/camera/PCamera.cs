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
using System.Windows.Forms;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    class PCamera : TgcCamera
    {
        private const float MOVEMENT_SPEED = 600;
        private const float BORDER_WIDTH = 80;

        private const float ZOOM_SPEED = 600;
        private const float DISTANCE_MIN = 100;
        private const float DISTANCE_MAX = 1600;

        private Vector3 center;
        private Vector3 ctpv; //'Center to Position' Versor
        private float distance;

        private Matrix viewMatrix;

        private ITerrain terrain;


        public PCamera(Vector3 _center, ITerrain _terrain)
        {
            this.terrain = _terrain;

            this.center = _center;
            this.ctpv = Vector3.Normalize(new Vector3(0, 2, 1));
            this.distance = (DISTANCE_MAX - DISTANCE_MIN) / 2;

            this.updateCenter();
            this.updateViewMatrix();

            GuiController.Instance.CurrentCamera = this;
        }

        #region Update

        private Point lastRealMousePos;
        private bool rotating = false;
        public void updateCamera()
        {
            if (!Mouse.isOverViewport()) return;

            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            float elapsedTime = GuiController.Instance.ElapsedTime;

            Vector2 mousePos = Mouse.ViewportPosition;
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

            Point realMousePos = Mouse.Position;

            if (d3dInput.buttonDown(TgcD3dInput.MouseButtons.BUTTON_MIDDLE))
            {
                if (!this.rotating) //inicia rotacion
                {
                    this.rotating = true;
                    this.lastRealMousePos = Mouse.Position;
                    Mouse.hide();
                }
                else //esta rotando
                {
                    float dx = lastRealMousePos.X - realMousePos.X;
                    if (dx != 0) //hay rotacion en x
                        this.rotateCamera
                            (new Vector3(0, -dx, 0),
                            dx * FastMath.PI / viewportWidth);

                    float dy = lastRealMousePos.Y - realMousePos.Y;
                    if (dy != 0) //hay rotacion en y
                        this.rotateCamera
                            (Vector3.Cross(this.ctpv, new Vector3(0, -dy, 0)),
                            dy * FastMath.PI / viewportWidth);

                    Mouse.Position = this.lastRealMousePos; //mantenemos estatico el cursor
                }
            }
            else if (d3dInput.buttonUp(TgcD3dInput.MouseButtons.BUTTON_MIDDLE)) //finaliza rotacion
            {
                this.rotating = false;
                Mouse.show();
            }


            //Distancia

            if (d3dInput.keyDown(Key.Z) && this.distance < DISTANCE_MAX)
                this.distance += ZOOM_SPEED * elapsedTime;
            if (d3dInput.keyDown(Key.A) && this.distance > DISTANCE_MIN)
                this.distance -= ZOOM_SPEED * elapsedTime;


            //Actualizacion de la matriz de transformacion

            this.updateCenter();
            this.updateViewMatrix();
        }

        #endregion

        #region Whatever

        private void updateViewMatrix()
        {
            this.viewMatrix = Matrix.LookAtLH(this.getPosition(), this.getLookAt(), new Vector3(0, 1, 0));
        }

        private void updateCenter()
        {
            this.center = terrain.getPosition(this.center.X, this.center.Z);
            this.center.Y /= 2f;
        }

        private void rotateCamera(Vector3 rotationAxis, float angle)
        {
            Vector3 normalizedRotationAxis = Vector3.Normalize(rotationAxis);
            Matrix transMatrix = Matrix.RotationAxis(normalizedRotationAxis, FastMath.Abs(angle));
            this.ctpv.TransformCoordinate(transMatrix);
        }

        #endregion

        #region TgcCameraMethods

        public Vector3 getPosition()
        {
            return this.center + (this.ctpv * this.distance);
        }

        public Vector3 getLookAt()
        {
            return this.center;
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            d3dDevice.Transform.View = this.viewMatrix;
        }

        #endregion
    }
}
