using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos
{
    class ThingsToRender
    {
        private static ThingsToRender instance;

        public List<TgcBox> boxes = new List<TgcBox>();

        private ThingsToRender()
        {
            //singleton
        }

        public static ThingsToRender getInstace()
        {
            if (instance == null) instance = new ThingsToRender();
            return instance;
        }

        public void render()
        {
            foreach (TgcBox box in this.boxes)
                box.render();
        }
    }
}
