using System.Drawing;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;

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


            if (character.Selected)
            {
                this.Effect.SetValue("selectionColor", ColorValue.FromColor(SelectionColor));
                this.Technique = "SELECTED";
            }
            else
                if (character.isDead())        
                    this.Technique = "BLACK_WHITE";                
                else
                    this.Technique = "DIFFUSE_MAP";

            
            base.render();
            
           

        }

     
    }
}
