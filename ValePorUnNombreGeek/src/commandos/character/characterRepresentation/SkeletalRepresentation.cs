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
        private Vector3 angleZeroVector; //rotacion manual
        private Matrix meshRotationMatrix; //rotacion manual
        private float meshFacingAngle; //hacia donde mira

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
            this.angleZeroVector = new Vector3(0, 0, -1);
            this.meshRotationMatrix = Matrix.Identity;
            this.AutoTransformEnable = false;
            this.meshFacingAngle = 0;

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

        public void Talk()
        {
            this.mesh.playAnimation("Talk", true);
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

        public float FacingAngle
        {
            get { return this.meshFacingAngle; }
        }

        public Vector3 getAngleZeroVector()
        {
            return this.angleZeroVector;
        }

        /*public Vector3 Rotation
        {
            get { return this.mesh.Rotation; }
            set { this.mesh.Rotation = value; }
        }*/


        public bool AutoTransformEnable
        {
            get { return this.mesh.AutoTransformEnable; }
            set { this.mesh.AutoTransformEnable = value; }
        }

        public void move(Vector3 direction)
        {
            this.mesh.move(direction);
            this.setRotation(direction);
        }

        public void setRotation(Vector3 direction)
        {
            direction.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(this.angleZeroVector, direction));
            Vector3 rotationAxis = Vector3.Cross(this.angleZeroVector, direction);
            this.meshRotationMatrix = Matrix.RotationAxis(rotationAxis, angle);

            //guardamos la direccion en la que miramos ahora
            this.meshFacingAngle = angle;

            this.applyTransformations();
        }

        public void setRotation(float angle, bool clockwise)
        {
            float rotationAxisY = Convert.ToSingle(clockwise) * 2 - 1; //convierte el bool en true = 1; false = -1
            Vector3 rotationAxis = new Vector3(0, rotationAxisY, 0);

            this.meshRotationMatrix = Matrix.RotationAxis(rotationAxis, angle);
            this.meshFacingAngle = angle;

            this.applyTransformations();
        }

        public void rotate(float angle, bool clockwise)
        {
            float modifier = Convert.ToSingle(clockwise) * 2 - 1; //convierte el bool en true = 1; false = -1
            this.meshFacingAngle += modifier * angle;

            this.meshFacingAngle = GeneralMethods.checkAngle(this.meshFacingAngle);

            this.setRotation(this.meshFacingAngle, true);
        }

        private void applyTransformations()
        {
            this.Transform = this.meshRotationMatrix * Matrix.Translation(this.Position);
        }

        public  void moveOrientedY(float movement){
            
            mesh.moveOrientedY(movement);

        }


      


    }
}
