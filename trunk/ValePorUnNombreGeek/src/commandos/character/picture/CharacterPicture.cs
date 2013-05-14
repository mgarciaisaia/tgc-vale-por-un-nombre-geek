using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using System.Drawing;
using Microsoft.DirectX;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.picture
{
    class CharacterPicture:Picture
    {
        
        protected Character character;

        public Color SelectionColor { get; set; }
        
        public CharacterPicture(Character character, string path):base(path)
        {
           
            this.character = character;
       
            this.SelectionColor = Color.Red;
            
        }

     

        public override void render()
        {


            
            if (character.isDead())
                this.Technique = "BLACK_WHITE";
            else if (character.Selected)
            {
                this.Effect.SetValue("selectionColor", ColorValue.FromColor(SelectionColor));
                this.Technique = "SELECTED";
            }
            else this.Technique = "DIFFUSE_MAP";
            
            base.render();
            
           

        }

     
    }
}
