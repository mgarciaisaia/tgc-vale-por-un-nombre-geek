using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcSkeletalAnimation;
using TgcViewer;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.target;
using TgcViewer.Utils.TgcGeometry;
using System.Drawing;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
{
    abstract class Character : Targeteable
    {
        protected TgcSkeletalMesh personaje;

        public bool selected;
        private bool dead;
        

        public Character(string _meshFilePath, string[] _animationsFilePath, Vector3 _position)
        {
            TgcSkeletalLoader skeletalLoader = new TgcSkeletalLoader();
            this.personaje = skeletalLoader.loadMeshAndAnimationsFromFile(
                _meshFilePath,
                _animationsFilePath);

            this.personaje.playAnimation("StandBy", true);
            this.personaje.Position = _position;
            
            this.selected = false;
            this.dead = false;
        }

        public void render(float elapsedTime)
        {
            this.update();

            personaje.updateAnimation();
            personaje.render();
            if (this.selected && !dead) personaje.BoundingBox.render();
        }

        protected abstract void update();

        public void die()
        {
            this.dead = true;
            this.personaje.playAnimation("Die", false);
        }

        public void dispose()
        {
            personaje.dispose();
        }

        public Vector3 getPosition()
        {
            return personaje.Position;
        }

        public TgcBoundingBox BoundingBox()
        {
            return this.personaje.BoundingBox;
        }
    }
}
