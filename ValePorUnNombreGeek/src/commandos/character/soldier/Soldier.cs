using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier
{
    class Soldier : Enemy
    {

        public Vector3[] waitpoints;
        public Vector3[] Waitpoints { 
            get { return waitpoints; } 
            set { 
                waitpoints = value; 
                currentWaitpoint = 0; 
                this.representation.Position = waitpoints[0]; } 
        }
        
        private int currentWaitpoint;

        private SoldierState state;


        public Soldier(Vector3 position)
            : this(new Vector3[] { position })
        {
            
           
        }

      
        public Soldier(Vector3[] waitpoints)
            : base(waitpoints[0])
        {
            this.Waitpoints = waitpoints;
            this.setState(new Waiting(this, 0));
        }

    
         public override bool manageCollision(ILevelObject obj)
        {

            if (obj.Equals(this.Target))
            {
                this.collisionedTarget();
                return true;
            }

          
            Commando commando;
            if (this.canSeeACommando(out commando))
            {
                this.setState(new Chasing(this, commando));
                return true;
            }
               
            

            return false;
          
        }

        public override void update(float elapsedTime)
        {
            if (this.Dead) return;
                     
            this.state.update(elapsedTime);
            this.vision.updatePosition();
           
        }

        private void recorrerWaitpoints(float elapsedTime)
        {
            if (waitpoints != null) this.state.update(elapsedTime);
        }

       


        internal void setNextPositionTarget()
        {
            currentWaitpoint = (currentWaitpoint + 1) % waitpoints.Length;
            this.setPositionTarget(waitpoints[currentWaitpoint]);
        }

        internal void setPreviousPositionTarget()
        {
            currentWaitpoint = currentWaitpoint - 1;
            if (currentWaitpoint < 0) currentWaitpoint = waitpoints.Length - 1;
            this.setPositionTarget(waitpoints[currentWaitpoint]);
        }
        internal Vector3 getNextPositionTarget()
        {
            return waitpoints[(currentWaitpoint + 1) % waitpoints.Length];
        }

        internal void setState(SoldierState _state)
        {
            this.state = _state;
        }
    }
}
