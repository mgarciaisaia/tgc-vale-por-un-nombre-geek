using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class UserCheckBox
    {
        private string description;
        private string id;

        public UserCheckBox(string _description, bool _default)
        {
            this.description = _description;
            this.id = _description;
            GuiController.Instance.Modifiers.addBoolean(this.id, this.description, _default);
        }

        public bool Value
        {
            get { return (bool)GuiController.Instance.Modifiers[this.id]; }
        }
    }
}
