using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.panel.graphical
{
    class SelectionButton : IButton
    {
        private Character ch;
        private Selection selectionRef;

        public SelectionButton(Character _ch, Selection _selection)
        {
            this.ch = _ch;
            this.selectionRef = _selection;
        }

        public void click()
        {
            this.selectionRef.deselectIfNotShift();
            this.selectionRef.selectCharacter(this.ch);
        }

        public Microsoft.DirectX.Vector2 Position
        {
            get { return this.ch.Life.Position; }
            set { this.ch.Life.Position = value; }
        }

        public float Height
        {
            get { return this.ch.Life.Height; }
        }

        public float Width
        {
            get { return this.ch.Life.Width; }
        }

        public void render()
        {
            this.ch.Life.render();
        }

        public void dispose()
        {
            //Nada, el character libera su vida.
        }
    }
}
