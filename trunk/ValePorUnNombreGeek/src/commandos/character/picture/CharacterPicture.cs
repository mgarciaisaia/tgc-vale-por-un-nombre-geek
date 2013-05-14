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
            this.Effect = character.Effect;
            this.Technique = "CHARACTER_PICTURE";
            this.SelectionColor = Color.Red;
            
        }

     

        public override void render()
        {

            string oldTechnique = this.Technique;         
            
            if (character.isDead())
                this.Technique = this.Technique + "_DEAD";
            else if (character.Selected)
            {
                this.Effect.SetValue("selectionColor", ColorValue.FromColor(SelectionColor));
                this.Technique = this.Technique + "_SELECTED";
            }
            
            base.render();
            
            this.Technique = oldTechnique;

        }

     
    }
}
