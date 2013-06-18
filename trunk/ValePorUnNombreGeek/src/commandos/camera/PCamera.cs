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
    class PCamera : ICamera
    {
        private const float MOVEMENT_SPEED = 600;
        private const float BORDER_WIDTH = 40;

        private const float ZOOM_SPEED = 600;
        private const float DISTANCE_MIN = 200;
        private const float DISTANCE_MAX = 1600;

        private const int ANGLE_MIN = 20;
        private const int ANGLE_MAX = 80;
        private Vector3 maxAngleChecker;
        private Vector3 minAngleChecker;
        //private float ANGLE_SIN_MIN = FastMath.Sin((180 / ANGLE_MIN) * FastMath.PI);
        //private float ANGLE_SIN_MAX = FastMath.Sin((180 / ANGLE_MAX) * FastMath.PI);

        private Vector3 center;
        private Vector3 ctpv; //'Center to Position' Versor
        private float distance;

        private Matrix viewMatrix;

        private ITerrain terrain;


        public PCamera(Vector3 _center, ITerrain _terrain)
        {
            this.terrain = _terrain;

            this.setCenter(this.terrain.getPosition(_center.X, _center.Z));
            this.ctpv = Vector3.Normalize(new Vector3(0, 2, 1));

            //this.maxAngleChecker = new Vector3(0, FastMath.Sin(ANGLE_MAX), FastMath.Cos(ANGLE_MAX));
            //sin t = y / l
            //cos t = x / l

            this.distance = (DISTANCE_MAX - DISTANCE_MIN) / 2;

            this.updateViewMatrix();

            GuiController.Instance.CurrentCamera = this;
        }

        #region Update

        private Point lastRealMousePos;
        private bool rotating = false;
        public void updateCamera()
        {
            if (!CommandosUI.Instance.mouseIsOverViewport()) return;

            var ui = CommandosUI.Instance;
            float elapsedTime = ui.ElapsedTime;

            Vector2 mousePos = ui.ViewportMousePos;
            float viewportHeight = ui.ViewportHeight;
            float viewportWidth = ui.ViewportWidth;


            //Desplazamiento

            Vector3 desplazamientoLateral = Vector3.Normalize(Vector3.Cross(this.ctpv, new Vector3(0, 1, 0)));
            Vector3 desplazamientoFrontal = Vector3.Normalize(Vector3.Cross(desplazamientoLateral, new Vector3(0, 1, 0)));

            if (ui.keyDown(Key.RightArrow))
                this.moveCenter(desplazamientoLateral, 1, elapsedTime);
            if (ui.keyDown(Key.LeftArrow))
                this.moveCenter(-desplazamientoLateral, 1, elapsedTime);
            if (ui.keyDown(Key.UpArrow))
                this.moveCenter(desplazamientoFrontal, 1, elapsedTime);
            if (ui.keyDown(Key.DownArrow))
                this.moveCenter(-desplazamientoFrontal, 1, elapsedTime);

            if (mousePos.X > viewportWidth - BORDER_WIDTH) //derecha
                this.moveCenter(desplazamientoLateral, 1, elapsedTime);
            if (mousePos.X < BORDER_WIDTH) //izquierda
                this.moveCenter(-desplazamientoLateral, 1, elapsedTime);
            if (mousePos.Y < BORDER_WIDTH) //arriba
                this.moveCenter(desplazamientoFrontal, 1, elapsedTime);
            if (mousePos.Y > viewportHeight - BORDER_WIDTH) //abajo
                this.moveCenter(-desplazamientoFrontal, 1, elapsedTime);


            //Distancia

            if (ui.keyDown(Key.Z)) this.zoomOut(1, elapsedTime);
            if (ui.keyDown(Key.A)) this.zoomIn(1, elapsedTime);

            float wheel = ui.DeltaWheelPos;
            if (wheel < 0) this.zoomOut(-wheel * 100, elapsedTime);
            if (wheel > 0) this.zoomIn(wheel * 100, elapsedTime);


            //Rotacion

            Point realMousePos = Mouse.Position;

            if (ui.mouseDown(TgcD3dInput.MouseButtons.BUTTON_MIDDLE))
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
                    {
                        //if (dy > 0 && this.ctpv.Y < ANGLE_SIN_MIN) this.ctpv.Y = ANGLE_SIN_MIN;
                        //else
                        this.rotateCamera
                            (Vector3.Cross(this.ctpv, new Vector3(0, -dy, 0)),
                            dy * FastMath.PI / viewportHeight);
                    }

                    Mouse.Position = this.lastRealMousePos; //mantenemos estatico el cursor
                }
            }
            else if (ui.mouseUp(TgcD3dInput.MouseButtons.BUTTON_MIDDLE)) //finaliza rotacion
            {
                Mouse.show();
                this.rotating = false;
            }

            // Reubico la camara sobre el heightmap si esta debajo
            Vector3 position = this.getPosition();
            Vector3 terrainPosition = terrain.getPosition(position.X, position.Z);
            if (position.Y < terrainPosition.Y + 40)
            {
                this.ctpv.Y = (terrainPosition.Y + 40 - this.center.Y) / this.distance;
                this.ctpv.Normalize();
            }

            //Actualizacion de la matriz de transformacion

            this.updateViewMatrix();
        }

        #endregion

        #region Whatever

        private void updateViewMatrix()
        {
            this.viewMatrix = Matrix.LookAtLH(this.getPosition(), this.getLookAt(), new Vector3(0, 1, 0));
        }

        private void rotateCamera(Vector3 rotationAxis, float angle)
        {
            Vector3 normalizedRotationAxis = Vector3.Normalize(rotationAxis);
            Matrix transMatrix = Matrix.RotationAxis(normalizedRotationAxis, FastMath.Abs(angle));
            this.ctpv.TransformCoordinate(transMatrix);
        }

        private void moveCenter(Vector3 direction, float speed, float elapsedTime)
        {
            Vector3 newCenter = this.center + direction * MOVEMENT_SPEED * speed * elapsedTime;

            if (this.terrain.getPosition(newCenter.X, newCenter.Z, out newCenter))
                this.setCenter(newCenter);
        }

        private void setCenter(Vector3 _center)
        {
            this.center = _center;
            this.center.Y /= 2f;
        }

        private void zoomIn(float speed, float elapsedTime)
        {
            this.distance -= ZOOM_SPEED * speed * elapsedTime;
            if (this.distance < DISTANCE_MIN) this.distance = DISTANCE_MIN;
        }

        private void zoomOut(float speed, float elapsedTime)
        {
            this.distance += ZOOM_SPEED * speed * elapsedTime;
            if (this.distance > DISTANCE_MAX) this.distance = DISTANCE_MAX;
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

        #region ICameraMethods

        public float Distance
        {
            get { return this.distance; }
        }

        public Vector3 Direction
        {
            get { return -this.ctpv; }
        }

        #endregion
    }
}
