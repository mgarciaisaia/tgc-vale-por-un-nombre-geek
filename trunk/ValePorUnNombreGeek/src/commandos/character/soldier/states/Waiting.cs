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
            this.maxWaitingTime = GeneralMethods.random(1, 3);
        }

        public override void onWaitpointUpdate(float elapsedTime)
        {
            this.waitingTime += elapsedTime;

            if (this.waitingTime > this.maxWaitingTime)
            {
                float actualAngle = this.soldier.Representation.FacingAngle;
                float delta = (float)2 * FastMath.PI;
                float desiredAngle = GeneralMethods.random(actualAngle - delta, actualAngle + delta);
                desiredAngle = GeneralMethods.checkAngle(desiredAngle);
                
                bool clockwise;

                float a = actualAngle;
                float b = desiredAngle;

                if (a < b) //TODO hacer estos if mas expresivos y claros
                    if (b - a < 2 * FastMath.PI - b + a)
                        clockwise = true;
                    else
                        clockwise = false;
                else
                    if (a - b < 2 * FastMath.PI - a + b)
                        clockwise = false;
                    else
                        clockwise = true;

                this.soldier.setState(new Rotating(this.soldier, desiredAngle, clockwise, this.timeOnWaitpoint));
                return;
            }
        }
    }
}
