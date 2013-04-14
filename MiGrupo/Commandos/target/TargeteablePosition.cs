using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.Commandos.target
{
    class TargeteablePosition : Targeteable
    {
        private Vector3 position;

        public TargeteablePosition(float x, float y, float z)
        {
            this.position = new Vector3(x, y, z);
        }

        public TargeteablePosition(Vector3 pos)
        {
            this.position = pos;
        }

        public Vector3 getPosition()
        {
            return this.position;
        }
    }
}
