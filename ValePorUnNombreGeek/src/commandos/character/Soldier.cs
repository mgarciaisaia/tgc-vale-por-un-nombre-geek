using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    class Soldier:Enemy
    {

        public Vector3[] waitpoints;
        public Vector3[] Waitpoints { 
            get { return waitpoints; } 
            set { 
                waitpoints = value; 
                currentWaitpoint = 0; 
                this.representation.Position = waitpoints[0]; 
                waitingTime = 0; 
                waiting = true; } 
        }
 
        public int maxWaitingTime = 5;
        private float waitingTime;
        private bool waiting = false;
        private int currentWaitpoint;

        public Soldier(Vector3[] waitpoints, Terrain _terrain)
            : base(waitpoints[0], _terrain)
        {
            this.Waitpoints = waitpoints;
          
        }
        protected override void update()
        {

            if (this.watch())
            {
                waiting = false;
                chase();

            }
            else
            {
                recorrerWaitpoints();
            }
           
        }

          private void recorrerWaitpoints()
        {
            if (waitpoints != null) 
            {

                if (waiting)
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
                    goToTarget();
                    if (!this.hasTarget())
                    { //Si llego a un waitpoint, esperar.
                        waiting = true;
                        waitingTime = 0;
                    }
                } 
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

          protected void chase()
          {
              goToTarget();
              //Intentar matarlo(?)
          }
    
    }
}
