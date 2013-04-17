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
        private float alturaCabeza;
        private const float DEFAULT_VISION_RADIUS = 400;
        private const float DEFAULT_VISION_ANGLE = 60;

        public Enemy(Vector3 _position, Terrain _terrain)
            : base(_position, _terrain)
        {

            crearConoDeVision(DEFAULT_VISION_RADIUS, DEFAULT_VISION_ANGLE);
            

        }

        public Enemy(Vector3 _position)
            : base(_position)
        {

            crearConoDeVision(DEFAULT_VISION_RADIUS, DEFAULT_VISION_ANGLE);


        }


        private void crearConoDeVision(float radius, float angle )
        {
            alturaCabeza = (this.personaje.BoundingBox.PMax.Y - this.personaje.BoundingBox.PMin.Y) * 9 / 10;
            cono = new ConoDeVision(new Vector3(this.getPosition().X, this.getPosition().Y + alturaCabeza, this.getPosition().Z), radius, angle);
            //cono.Enabled = false;
            cono.AutoTransformEnable = false;
        }
        public Enemy(Vector3[] waitpoints, Terrain _terrain)
            : base(waitpoints[0], _terrain)
        {
            this.Waitpoints = waitpoints;
            crearConoDeVision(DEFAULT_VISION_RADIUS, DEFAULT_VISION_ANGLE);
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


            this.cono.Transform = this.personaje.Transform * Matrix.Translation(new Vector3(0,alturaCabeza,0)); //Hacer que el cono se mueva con el personaje
            cono.render();
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