using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Walker
    {
        private ConoDeVision cono;
        public Vector3[] waitpoints;
        public int maxWaitingTime = 5;
        private float waitingTime;
        private bool waiting=false;
        private bool chasing=false;
        private int currentWaitpoint;

        public Enemy(Vector3 _position)
            : base(_position)
        {

            crearConoDeVision();
            

        }

        private void crearConoDeVision()
        {
            float alturaCabeza = (this.personaje.BoundingBox.PMax.Y - this.personaje.BoundingBox.PMin.Y) * 9 / 10;
            cono = new ConoDeVision(new Vector3(this.getPosition().X, this.getPosition().Y + alturaCabeza, this.getPosition().Z), 30, 30);
            cono.Enabled = false;
            cono.AutoTransformEnable = false;
        }
        public Enemy(Vector3[] waitpoints)
            :base(waitpoints[0])
        {
            this.Waitpoints = waitpoints;
            crearConoDeVision();
        }
        protected new static string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "CS_Arctic-TgcSkeletalMesh.xml";
      
        }

        public bool puedeVer(TgcBox target)
        {
            
            return cono.colisionaCon(target);
        }

        protected override void update()
        {

            if (waitpoints != null && !chasing) //Si no esta persiguiendo
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
                else if (GeneralMethods.isCloseTo(this.getPosition(),(waitpoints[currentWaitpoint])))
                { //Si llego a un waitpoint, esperar.
                    waiting = true;
                    waitingTime = 0;
                }
            }

            base.update();


            this.cono.Transform = this.personaje.Transform; //Hacer que el cono se mueva con el personaje
            if (cono.Enabled) cono.render();
        }


        

        public float VisionAngle { get { return cono.Angle; } set { cono.Angle = value; } }

        public float VisionRadius { get { return cono.Radius; } set { cono.Radius = value; } }

        public bool ConeEnabled { get { return cono.Enabled; } set { cono.Enabled = value; } }

        public Vector3[] Waitpoints { get { return waitpoints; } set { waitpoints = value; currentWaitpoint = 0; this.personaje.Position = waitpoints[0]; waitingTime = 0; } }
    }


    /* class EnemyFactory : TgcSkeletalLoader.IMeshFactory
    {
        public TgcSkeletalMesh createNewMesh(Mesh d3dMesh, string meshName, TgcSkeletalMesh.MeshRenderType renderType, TgcSkeletalBone[] bones)
        {
            return new Enemy(d3dMesh, meshName, renderType, bones);
        }
    }*/

}