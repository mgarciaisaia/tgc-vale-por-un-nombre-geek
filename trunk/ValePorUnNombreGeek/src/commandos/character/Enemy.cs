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
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
   
    class Enemy : Character
    {
        private ConoDeVision cono;

     
        private float alturaCabeza;
        private const float DEFAULT_VISION_RADIUS = 400;
        private const float DEFAULT_VISION_ANGLE = 60;
        public float VisionAngle { get { return cono.Angle; } set { cono.Angle = value; } }
        public float VisionRadius { get { return cono.Radius; } set { cono.Radius = value; } }
        public bool ConeEnabled { get { return cono.Enabled; } set { cono.Enabled = value; } }

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

        protected override void loadCharacterRepresentation(Vector3 position)
        {
            this.representation = new EnemyRepresentation(position);
        }


        private void crearConoDeVision(float radius, float angle )
        {
            alturaCabeza = (this.representation.BoundingBox.PMax.Y - this.representation.BoundingBox.PMin.Y) * 9 / 10;
            cono = new ConoDeVision(new Vector3(this.Position.X, this.Position.Y + alturaCabeza, this.Position.Z), radius, angle);
            //cono.Enabled = false;
            cono.AutoTransformEnable = false;
        }

       

        public bool canSee(TgcBox target)
        {
            
            return cono.colisionaCon(target);
        }


        protected bool watch()
        {
            //Si ve a un character lo setea como objetivo y devuelve true
            return false;
        }


        public override void render(float elapsedTime) 
        {
            base.render(elapsedTime);
            //Aplico las mismas modificaciones al cono(mas la modificacion para la altura). En la nueva version del cono se van a aplicar solas.
            this.cono.Transform = this.representation.Transform * Matrix.Translation(new Vector3(0, alturaCabeza, 0));
            cono.renderWireframe();
        }

       

        }



}