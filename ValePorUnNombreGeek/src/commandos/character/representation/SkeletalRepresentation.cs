using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using Microsoft.DirectX;
using TgcViewer;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.collision;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    class SkeletalRepresentation : ICharacterRepresentation
    {
        protected TgcSkeletalMesh mesh;
        private bool selected;
        private Vector3 angleZeroVector; //rotacion manual
        private float meshFacingAngle; //hacia donde mira
        protected float radius;

        private Cylinder boundingCylinder;

        public bool Selected
        {

            get { return selected; }
            set { this.selected = value; }
        }

        public float Radius
        {
            get { return this.radius; }
        }

        public Vector3 Center
        {
            get { return this.BoundingBox.calculateBoxCenter(); }
        }

        public SkeletalRepresentation(Vector3 position)
        {

            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            this.mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                getMesh(),
                getAnimations());

            this.mesh.playAnimation("StandBy", true);

            this.Position = position;
            this.radius = mesh.BoundingBox.calculateBoxRadius();

            //this.mesh.AutoUpdateBoundingBox = false;
            //seguimos actualizando la bb por que de momento la necesitamos para seleccionar al personaje
            Vector3 size = this.BoundingBox.calculateSize();
            this.boundingCylinder = new Cylinder(this.Position, size.Y, size.X / 2);

            //rotacion manual
            this.AutoTransformEnable = false;
            this.angleZeroVector = new Vector3(0, 0, -1);
            this.setRotation(this.angleZeroVector);
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
                    exMediaDir + "Talk-TgcSkeletalAnim.xml",
                    exMediaDir + "StandBy-TgcSkeletalAnim.xml",
                    exMediaDir + "Jump-TgcSkeletalAnim.xml",
                    myMediaDir + "Die-TgcSkeletalAnim.xml",
                    exMediaDir + "CrouchWalk-TgcSkeletalAnim.xml"
                };
        }


        public void render()
        {
            this.mesh.animateAndRender();
            this.boundingCylinder.render();
        }


        public void dispose()
        {
            this.mesh.dispose();
            this.boundingCylinder.dispose();
        }

        #region Animations

        public void standBy()
        {
            this.mesh.playAnimation("StandBy", true);
        }

        public void talk()
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

        public void crouch()
        {
            this.mesh.playAnimation("CrouchWalk", true);

        }

        public bool isCrouched()
        {
            return this.mesh.CurrentAnimation.Name.Equals("CrouchWalk");
        }
        #endregion

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
        }

        public Cylinder BoundingCylinder
        {
            get { return this.boundingCylinder; }
        }

        public bool Enabled
        {
            get { return this.mesh.Enabled; }
            set { this.mesh.Enabled = value; }
        }

        public Matrix Transform
        {
            get { return this.mesh.Transform; }
            private set { this.mesh.Transform = value; }
        }

        /*public Vector3 Scale
        {
            get { return this.mesh.Scale; }
            set { this.mesh.Scale = value; }
        }*/

        public float FacingAngle
        {
            get { return this.meshFacingAngle; }
        }

        public Vector3 getAngleZeroVector()
        {
            return this.angleZeroVector;
        }


        public bool AutoTransformEnable
        {
            get { return this.mesh.AutoTransformEnable; }
            set { this.mesh.AutoTransformEnable = value; }
        }

        public void move(Vector3 direction)
        {
            this.mesh.move(direction);
            this.setRotation(direction);
            this.boundingCylinder.Position = this.Position;
        }

        #region Rotation

        public void setRotation(Vector3 direction)
        {
            direction.Normalize();
            float angle = FastMath.Acos(Vector3.Dot(this.angleZeroVector, direction));
            Vector3 rotationAxis = Vector3.Cross(this.angleZeroVector, direction);
            Matrix rotationMatrix = Matrix.RotationAxis(rotationAxis, angle);

            //guardamos la direccion en la que miramos ahora
            this.meshFacingAngle = angle;

            this.applyTransformations(rotationMatrix);
        }

        public void setRotation(float angle, bool clockwise)
        {
            float rotationAxisY = Convert.ToSingle(clockwise) * 2 - 1; //convierte el bool en true = 1; false = -1
            Vector3 rotationAxis = new Vector3(0, rotationAxisY, 0);

            Matrix rotationMatrix = Matrix.RotationAxis(rotationAxis, angle);

            //guardamos la direccion en la que miramos ahora
            this.meshFacingAngle = angle;

            this.applyTransformations(rotationMatrix);
        }

        public void rotate(float angle, bool clockwise)
        {
            float modifier = Convert.ToSingle(clockwise) * 2 - 1; //convierte el bool en true = 1; false = -1
            this.meshFacingAngle += modifier * angle;

            this.meshFacingAngle = GeneralMethods.checkAngle(this.meshFacingAngle);

            this.setRotation(this.meshFacingAngle, true);
        }

        public Vector3 Left { get { return left; } }
        public Vector3 Right { get { return right; } }
        public Vector3 Front { get { return front; } }

        Vector3 left = new Vector3(1,0,0);
        Vector3 front = new Vector3(0,0,-1);
        Vector3 right = new Vector3(-1,0,0);
        
        private void applyTransformations(Matrix _rotationMatrix)
        {
            this.Transform = _rotationMatrix * Matrix.Translation(this.Position);
            this.left = Vector3.TransformCoordinate(new Vector3(1,0,0), _rotationMatrix);
            this.right = Vector3.TransformCoordinate(new Vector3(-1,0,0), _rotationMatrix);
            this.front = Vector3.TransformCoordinate(new Vector3(0, 0, -1), _rotationMatrix);
        }

        #endregion

        public void moveOrientedY(float movement)
        {

            mesh.moveOrientedY(movement);

        }
        public string Prefix{ get{ return "SKELETAL";}}
        public Effect Effect { get { return this.mesh.Effect; } set {this.mesh.Effect = value ;} }
        public string Technique { get { return this.mesh.Technique; } set { this.mesh.Technique = value; } }
    }
}
