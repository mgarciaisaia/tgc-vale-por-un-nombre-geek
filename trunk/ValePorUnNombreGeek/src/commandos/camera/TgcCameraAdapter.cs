using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.Input;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.camera
{
    class TgcCameraAdapter : ICamera
    {
        /*
         * TgcCameraAdapter:
         * Permite usar cualquier TgcCamera en el Commandos.
         * Lo que hace es implementar los metodos Distance y Direction.
         * 
         * Nota: no se llama Adapter por el patron de diseno :p
         */

        private TgcCamera camera;

        public TgcCameraAdapter(TgcCamera _camera)
        {
            this.camera = _camera;
        }

        public Vector3 getPosition()
        {
            return this.camera.getPosition();
        }

        public Vector3 getLookAt()
        {
            return this.camera.getLookAt();
        }

        public void updateCamera()
        {
            this.camera.updateCamera();
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            this.camera.updateViewMatrix(d3dDevice);
        }

        public float Distance
        {
            get { return (this.getLookAt() - this.getPosition()).Length(); }
        }

        public Vector3 Direction
        {
            get { return Vector3.Normalize(this.getLookAt() - this.getPosition()); }
        }
    }
}
