using System.Drawing;
using Microsoft.DirectX.Direct3D;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character
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
