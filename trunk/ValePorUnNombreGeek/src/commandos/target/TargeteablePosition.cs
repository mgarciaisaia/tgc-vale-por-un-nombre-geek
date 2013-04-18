using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target
{
    class TargeteablePosition : ITargeteable
    {
        private Vector3 position;


        public Vector3 Position
        {
            get { return this.position;}
            set { this.position = value; }
        }

        public TargeteablePosition(float x, float y, float z)
        {
            this.position = new Vector3(x, y, z);
        }

        public TargeteablePosition(Vector3 p)
        {
            this.position = p;
        }
       
       

    }
}
