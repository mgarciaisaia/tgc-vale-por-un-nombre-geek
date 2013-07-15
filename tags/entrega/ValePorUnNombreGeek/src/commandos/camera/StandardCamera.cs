using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;
using TgcViewer;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    class StandardCamera : TgcCamera
    {
        private const float MOVEMENT_SPEED = 500;

        private Vector3 position;
        private Vector3 center;
        private Matrix viewMatrix;

        public StandardCamera()
        {
            this.center = new Vector3(0, 0, 0);
            this.position = new Vector3(0, 500, 500);
            GuiController.Instance.CurrentCamera = this;
        }

        public Microsoft.DirectX.Vector3 getPosition()
        {
            return this.position;
        }

        public Microsoft.DirectX.Vector3 getLookAt()
        {
            return this.center;
        }

        public void updateCamera()
        {
            float speed = MOVEMENT_SPEED * CommandosUI.Instance.ElapsedTime;

            Vector3 movement = new Vector3(0, 0, 0);

            if (CommandosUI.Instance.keyDown(Key.Up)) movement += new Vector3(0, 0, -speed);
            if (CommandosUI.Instance.keyDown(Key.Down)) movement += new Vector3(0, 0, speed);
            if (CommandosUI.Instance.keyDown(Key.Right)) movement += new Vector3(-speed, 0, 0);
            if (CommandosUI.Instance.keyDown(Key.Left)) movement += new Vector3(speed, 0, 0);

            this.position += movement;
            this.center += movement;

            Vector3 direction = this.center - this.position;
            direction.Normalize();

            this.position += direction * MOVEMENT_SPEED * CommandosUI.Instance.DeltaWheelPos * 0.5f;

            viewMatrix = Matrix.LookAtLH(this.position, this.center, new Vector3(0, 1, 0));
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            d3dDevice.Transform.View = this.viewMatrix;
        }
    }
}
