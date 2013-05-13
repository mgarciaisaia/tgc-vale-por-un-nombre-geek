using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Waiting : OnWaitpoint
    {
        private float waitingTime;
        private float maxWaitingTime;

        public Waiting(Soldier _soldier, float _timeOnWaitpoint)
            : base(_soldier, _timeOnWaitpoint)
        {
            this.waitingTime = 0;
            this.soldier.Representation.standBy();
            this.maxWaitingTime = GeneralMethods.random(1, 3);
        }
        public Waiting(Soldier _soldier, float _timeOnWaitpoint, bool _alert)
            : this(_soldier, _timeOnWaitpoint)
        {
            this.Alert = _alert;
            if(_alert) this.maxWaitingTime = GeneralMethods.random(1, 2);
        }

        public override void onWaitpointUpdate(float elapsedTime)
        {
            this.waitingTime += elapsedTime;

            if (this.waitingTime > this.maxWaitingTime)
            {
                float actualAngle = this.soldier.Representation.FacingAngle;
                float delta = 0.5f * FastMath.PI;
                float desiredAngle;
                bool clockwise;
                if (this.Alert)
                {
                    desiredAngle = actualAngle + FastMath.PI;
                    desiredAngle = GeneralMethods.checkAngle(desiredAngle);
                    clockwise = GeneralMethods.randomBool();
                     
                }
                else
                {
                    desiredAngle = GeneralMethods.random(actualAngle - delta, actualAngle + delta);
                    desiredAngle = GeneralMethods.checkAngle(desiredAngle);
                    clockwise =   this.shallRotateClockwise(desiredAngle);
                }
                
               
                

                this.soldier.setState(new Rotating(this.soldier, desiredAngle, clockwise, this.timeOnWaitpoint, this.Alert));
                return;
            }
        }
    }
}
