using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states
{
    class Rotating : OnWaitpoint
    {
        private bool clockwise;
        private float desiredAngle;
        private float ROTATION_ANGLE = 2 * FastMath.PI;

        public Rotating(Soldier _soldier, float _desiredAngle, bool _clockwise, float _timeOnWaitpoint)
            : base(_soldier, _timeOnWaitpoint)
        {
            this.desiredAngle = _desiredAngle;
            this.clockwise = _clockwise;
        }

        public override void onWaitpointUpdate(float elapsedTime)
        {
            /*try
            {
                GuiController.Instance.UserVars.setValue("actualAngle", this.soldier.Representation.FacingAngle);
                GuiController.Instance.UserVars.setValue("desiredAngle", this.desiredAngle);
            }
            catch (Exception e)
            {
                GuiController.Instance.UserVars.addVar("actualAngle");
                GuiController.Instance.UserVars.addVar("desiredAngle");
            }*/

            float deltaAngle = elapsedTime * ROTATION_ANGLE;
            this.soldier.Representation.rotate(deltaAngle, this.clockwise);

            if (GeneralMethods.isCloseTo(this.soldier.Representation.FacingAngle, this.desiredAngle, deltaAngle))
            {
                this.soldier.setState(new Waiting(this.soldier, this.timeOnWaitpoint));
            }
        }
    }
}
