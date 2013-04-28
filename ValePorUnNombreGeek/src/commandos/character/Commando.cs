using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Commando : Character
    {
      

        public Commando(Vector3 _position)
            : base(_position)
        {
            //nothing to do
        }

      
        public override void update(float elapsedTime)
        {
            if (!this.hasTarget()) return;

            this.goToTarget(elapsedTime);

            if (this.isOnTarget()) this.setNoTarget();
        }

        public override bool userCanMove()
        {
            return true;
        }
    }
}
