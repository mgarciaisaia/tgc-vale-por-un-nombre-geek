using System.Drawing;
using Microsoft.DirectX.Direct3D;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picture
{
    class CharacterPicture:Picture
    {

        public Character Character { get; set; }
      

        public Color SelectionColor { get; set; }
        
        public CharacterPicture(string path):base(path)
        {
           
       
            this.SelectionColor = Color.Red;
            
        }

        public CharacterPicture(Texture texture, float width, float height):base(texture, width, height)
        {
            this.SelectionColor = Color.Red;
        }
        

        public override void render()
        {


            if (Character.Selected)
            {
                this.Effect.SetValue("selectionColor", ColorValue.FromColor(SelectionColor));
                this.Technique = "SELECTED";
            }
            else
                if (Character.isDead())        
                    this.Technique = "BLACK_WHITE";                
                else
                    this.Technique = "DIFFUSE_MAP";

            
            base.render();
            
           

        }

        public CharacterPicture Clone()
        {
            return new CharacterPicture(this.texDiffuseMap, this.width, this.height) { Character = this.Character };

        }

     
    }
}
