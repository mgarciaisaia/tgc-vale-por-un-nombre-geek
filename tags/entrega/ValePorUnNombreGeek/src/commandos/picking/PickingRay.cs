using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking
{
    /// <summary>
    /// Adaptacion de TgcPickingRay, con minimos cambios necesarios para nuestro Commandos 
    /// </summary>
    public class PickingRay
    {
        private TgcRay ray;

        public PickingRay()
        {
            ray = new TgcRay();
        }

        /// <summary>
        /// Ray que representa la acción de Picking
        /// </summary>
        public TgcRay Ray
        {
            get { return ray; }
        }

        /// <summary>
        /// Actualiza el Ray de colisión en base a una posición especificada
        /// </summary>
        public void updateRayByPos(float screenx, float screeny)
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;
            int w = d3dDevice.Viewport.Width;
            int h = d3dDevice.Viewport.Height;
            Matrix matProj = d3dDevice.Transform.Projection;

            Vector3 v = new Vector3();
            v.X = (((2.0f * screenx) / w) - 1) / matProj.M11;
            v.Y = -(((2.0f * screeny) / h) - 1) / matProj.M22;
            v.Z = 1.0f;

            //Transform the screen space pick ray into 3D space
            Matrix m = Matrix.Invert(d3dDevice.Transform.View);
            Vector3 rayDir = new Vector3(
                v.X * m.M11 + v.Y * m.M21 + v.Z * m.M31,
                v.X * m.M12 + v.Y * m.M22 + v.Z * m.M32,
                v.X * m.M13 + v.Y * m.M23 + v.Z * m.M33
                );
            Vector3 rayOrig = new Vector3(m.M41, m.M42, m.M43);


            //Picking Ray creado
            ray.Origin = rayOrig;
            ray.Direction = rayDir;
        }

        /// <summary>
        /// Actualiza el Ray de colisión en base a la posición del mouse en la pantalla
        /// </summary>
        public void updateRayByMouse()
        {
            Device d3dDevice = GuiController.Instance.D3dDevice;

            //Crear Ray en base a coordenadas del mouse
            float sx = GuiController.Instance.D3dInput.Xpos;
            float sy = GuiController.Instance.D3dInput.Ypos;

            this.updateRayByPos(sx, sy);
        }
    }
}
