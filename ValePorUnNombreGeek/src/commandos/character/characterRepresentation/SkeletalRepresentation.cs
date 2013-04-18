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
        TgcSkeletalMesh mesh;
        private bool selected;

        public bool Selected
        {

            get { return selected; }
            set { this.selected = value; }
        }

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


      
        public SkeletalRepresentation(Vector3 position)
        {

            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            this.mesh = skeletalLoader.loadMeshAndAnimationsFromFile(
                getMesh(),
                getAnimations());

            this.mesh.playAnimation("StandBy", true);
            this.mesh.Position = position;


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

        public void die()
        {
            this.mesh.playAnimation("Die", false);
        }

     
        public void walk()
        {
            this.mesh.playAnimation("Walk", true);
        }

        public void move(Vector3 direction)
        {
            this.mesh.move(direction);
        }

        public void standBy()
        {
            this.mesh.playAnimation("StandBy", true);
        }

        public void dispose()
        {
            this.mesh.dispose();
        }

    }
}
