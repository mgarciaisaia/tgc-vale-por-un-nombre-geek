using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.Direct3D;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    interface ICharacterRepresentation
    {
        void talk();
        void die();
        void standBy();
        void walk();
        void crouch();
        bool Selected { get; set; }

        void move(Vector3 direction);
        void moveOrientedY(float movement);

        Vector3 Position { get; set; }
        Vector3 Left { get; }
        Vector3 Right { get; }
        Vector3 Front { get; }
        Matrix Transform { get; }
        //Vector3 Scale { get; set; }
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


        Effect Effect { get; set; }
        string Technique { get; set; }

        Vector3 getEyeLevel();


        float Radius { get; }

        Vector3 Center { get; }

        bool isCrouched();



        string Prefix { get;}
    }
}
