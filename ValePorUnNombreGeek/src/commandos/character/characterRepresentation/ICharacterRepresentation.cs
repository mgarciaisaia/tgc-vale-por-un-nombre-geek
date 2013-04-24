using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    interface ICharacterRepresentation
    {
                    
        void die();
        void standBy();
        void walk();

        bool Selected { get; set; }

        void move(Vector3 direction);
        void moveOrientedY(float movement);

        Vector3 Position { get; set; }
        Matrix Transform { get; set; }
        //Vector3 Rotation { get; set; }
        Vector3 Scale { get; set; }
        bool AutoTransformEnable { get; set; }
        float FacingAngle { get; }

        TgcBoundingBox BoundingBox { get; }

        void render();
        bool Enabled { get; set; }
        void dispose();
        void setRotation(Vector3 direction);
        void setRotation(float angle, bool clockwise);
        void rotate(float angle, bool clockwise);
        Vector3 getAngleZeroVector();

        Vector3 getEyeLevel();
    }
}
