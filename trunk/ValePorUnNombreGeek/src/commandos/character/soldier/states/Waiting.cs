using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Waiting : OnWaitpoint
    {
        private float waitingTime;
        private float maxWaitingTime;

        private Random rnd = new Random();

        public Waiting(Soldier _soldier, float _timeOnWaitpoint)
            : base(_soldier, _timeOnWaitpoint)
        {
            this.waitingTime = 0;
            this.maxWaitingTime = this.random(1, 3);
        }

        public override void onWaitpointUpdate(float elapsedTime)
        {
            this.waitingTime += elapsedTime;

            if (this.waitingTime > this.maxWaitingTime)
            {
                float actualAngle = this.soldier.Representation.FacingAngle;
                float delta = (float)0.5 * FastMath.PI;
                float desiredAngle = this.random(actualAngle - delta, actualAngle + delta);
                desiredAngle = GeneralMethods.checkAngle(desiredAngle);
                
                bool clockwise = false;
                if (desiredAngle > actualAngle &&
                    desiredAngle - actualAngle < FastMath.PI) clockwise = true; //TODO revisar. a veces gira para el otro lado

                this.soldier.setState(new Rotating(this.soldier, desiredAngle, clockwise, this.timeOnWaitpoint));
                return;
            }
        }

        private float random(float min, float max)
        {
            float delta = max - min;
            return min + delta * (float)rnd.NextDouble();
        }
    }
}
