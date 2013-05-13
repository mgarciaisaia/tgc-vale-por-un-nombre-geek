using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Rotating : OnWaitpoint
    {
        private bool clockwise;
        private float desiredAngle;
        private float ROTATION_ANGLE;

        public Rotating(Soldier _soldier, float _desiredAngle, bool _clockwise, float _timeOnWaitpoint)
            : base(_soldier, _timeOnWaitpoint)
        {
            this.desiredAngle = _desiredAngle;
            this.clockwise = _clockwise;
            ROTATION_ANGLE = 2 * FastMath.PI;
        }

        public Rotating(Soldier _soldier, float _desiredAngle, bool _clockwise, float _timeOnWaitpoint, bool _alert)
            : this(_soldier, _desiredAngle, _clockwise, _timeOnWaitpoint)
        {
            this.Alert = _alert;
            if (this.Alert) ROTATION_ANGLE = 4 * FastMath.PI;
        }

        public override void onWaitpointUpdate(float elapsedTime)
        {
            float deltaAngle = elapsedTime * ROTATION_ANGLE;
            this.soldier.Representation.rotate(deltaAngle, this.clockwise);

            if (GeneralMethods.isCloseTo(this.soldier.Representation.FacingAngle, this.desiredAngle, deltaAngle))
                this.onDesiredAngle();
        }

        protected virtual void onDesiredAngle()
        {
            this.soldier.setState(new Waiting(this.soldier, this.timeOnWaitpoint, this.Alert));
        }
    }
}
