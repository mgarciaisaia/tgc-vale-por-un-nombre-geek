using System;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation
{
    interface ISkeletalCharacter
    {
        
             
        void die();
        void standBy();
        void walk();

        bool Selected { get; set; }

        void move(Vector3 direction);

        Vector3 Position { get; set; }
        Matrix Transform { get; set; }
        Vector3 Rotation { get; set; }
        bool AutoTransformEnable { get; set; }

        TgcBoundingBox BoundingBox { get; }

        void render();
        bool Enabled { get; set; }
        void dispose();
           
       
        
    }
}
