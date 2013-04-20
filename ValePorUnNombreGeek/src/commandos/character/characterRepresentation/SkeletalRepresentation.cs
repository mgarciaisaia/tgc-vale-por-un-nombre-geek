using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    class SkeletalRepresentation : ICharacterRepresentation
    {
        protected TgcSkeletalMesh mesh;
        private bool selected;
        private Vector3 meshRotationAxis; //rotacion manual
        private Matrix meshRotationMatrix; //rotacion manual

        public bool Selected
        {

            get { return selected; }
            set { this.selected = value; }
        }
             
              
        public SkeletalRepresentation(Vector3 position)
        {

            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            this.mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                getMesh(),
                getAnimations());

            this.mesh.playAnimation("StandBy", true);
            this.Position = position;

            //rotacion manual
            this.meshRotationAxis = new Vector3(0, 0, -1);
            this.meshRotationMatrix = Matrix.Identity;
            this.AutoTransformEnable = false;

            //por algun motivo hay que volver a actualizar la posicion del personaje
            this.Transform = Matrix.Translation(this.Position); //en este caso transladandolo desde el (0,0,0)
        }

        protected virtual string getMesh()
        {
            return GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\" + "BasicHuman-TgcSkeletalMesh.xml";
        }

        protected virtual string[] getAnimations()
        {
            String myMediaDir = GuiController.Instance.AlumnoEjemplosMediaDir + "ValePorUnNombreGeek\\SkeletalAnimations\\BasicHuman\\Animations\\";
            String exMediaDir = GuiController.Instance.ExamplesMediaDir + "SkeletalAnimations\\BasicHuman\\Animations\\";
            return new string[] { 
                    exMediaDir + "Walk-TgcSkeletalAnim.xml",
                    exMediaDir + "StandBy-TgcSkeletalAnim.xml",
                    exMediaDir + "Jump-TgcSkeletalAnim.xml",
                    myMediaDir + "Die-TgcSkeletalAnim.xml"
                };
        }

  
        public void render()
        {
            this.mesh.animateAndRender();
            if (this.Selected) this.mesh.BoundingBox.render();
        }


        public void dispose()
        {
            this.mesh.dispose();
        }

        //Animaciones

        public void standBy()
        {
            this.mesh.playAnimation("StandBy", true);
        }


        public void die()
        {
            this.mesh.playAnimation("Die", false);
        }

     
        public void walk()
        {
            this.mesh.playAnimation("Walk", true);
        }


        public Vector3 getEyeLevel() 
        {
            return new Vector3(0, (this.mesh.BoundingBox.PMax.Y - this.mesh.BoundingBox.PMin.Y) * 9 / 10, 0);
        }

        //Wrappers de SkeletalMesh
        public Vector3 Position
        {

            get { return this.mesh.Position; }
            set { this.mesh.Position = value; }
        }

        public TgcBoundingBox BoundingBox
        {
            get { return this.mesh.BoundingBox; }
            set { this.mesh.BoundingBox = value; }
        }

        public bool Enabled
        {

            get { return this.mesh.Enabled; }
            set { this.mesh.Enabled = value; }
        }

        public Matrix Transform
        {
            get { return this.mesh.Transform; }
            set { this.mesh.Transform = value; }
        }

        public Vector3 Scale
        {
            get { return this.mesh.Scale; }
            set { this.mesh.Scale = value; }
        }

        public Vector3 Rotation
        {
            get { return this.mesh.Rotation; }
            set { this.mesh.Rotation = value; }
        }


        public bool AutoTransformEnable
        {
            get { return this.mesh.AutoTransformEnable; }
            set { this.mesh.AutoTransformEnable = value; }
        }

        public void move(Vector3 direction)
        {
            this.mesh.move(direction);
            this.faceTo(direction);
        }

        private void faceTo(Vector3 direction) //rotacion manual
        {
            direction.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(this.meshRotationAxis, direction));
            Vector3 axisRotation = Vector3.Cross(this.meshRotationAxis, direction);
            this.meshRotationMatrix = Matrix.RotationAxis(axisRotation, angle);
            this.Transform = this.meshRotationMatrix * Matrix.Translation(this.Position);
        }

        public  void moveOrientedY(float movement){
            
            mesh.moveOrientedY(movement);

        }


      


    }
}
