using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.soldier.states;

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
            : base(position)
        {
            this.setState(new Waiting(this, 0));
        }

      
        public Soldier(Vector3[] waitpoints)
            : this(waitpoints[0])
        {
            this.Waitpoints = waitpoints;
        }

    

        protected override void update(float elapsedTime)
        {

            if (this.watch())
            {
                //waiting = false;
                chase(elapsedTime);

            }
            else
            {
                recorrerWaitpoints(elapsedTime);
            }
           
        }

        private void recorrerWaitpoints(float elapsedTime)
        {
            if (waitpoints != null) this.state.update(elapsedTime);
        }

        protected void chase(float elapsedTime)
        {
            goToTarget(elapsedTime);
            //Intentar matarlo(?)
        }

        internal void setNextPositionTarget()
        {
            currentWaitpoint = (currentWaitpoint + 1) % waitpoints.Length;
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
