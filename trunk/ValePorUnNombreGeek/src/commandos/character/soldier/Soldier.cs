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
        
        //public int maxWaitingTime = 5;
        //private float waitingTime;
        //private bool waiting = false;
        private int currentWaitpoint;

        private SoldierState state;


        public Soldier(Vector3 position, Terrain _terrain)
            : base(position, _terrain)
        {
            this.setState(new Waiting(this));
        }

        public Soldier(Vector3[] waitpoints, Terrain _terrain)
            : this(waitpoints[0], _terrain)
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
            if (waitpoints != null) 
            {

                /*if (waiting)
                {
                    //Si espero suficiente tiempo, fijar el proximo waitpoint como objetivo
                    waitingTime += GuiController.Instance.ElapsedTime;
                    if (waitingTime > maxWaitingTime)
                    {
                        currentWaitpoint = (currentWaitpoint + 1) % waitpoints.Length;
                        waiting = false;
                        this.setPositionTarget(waitpoints[currentWaitpoint]);
                    }
                }
                else
                {
                    this.setPositionTarget(waitpoints[currentWaitpoint]);
                    goToTarget(elapsedTime);
                    if (!this.hasTarget())
                    { //Si llego a un waitpoint, esperar.
                        waiting = true;
                        waitingTime = 0;
                    }
                } */
                this.state.update(elapsedTime);
            }

            /*Pablo: no se podria hacer un state?
             * con tres estados:
             * -llendo al waitpoint
             * -esperando
             * -(y proximamente) persiguiendo personaje
             * 
             * Daniela: si
            */
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
