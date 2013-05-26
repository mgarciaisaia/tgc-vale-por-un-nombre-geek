using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QTHoja : QTNode
    {
        private List<ILevelObject> objectsInside;

        public void addObject(ILevelObject obj)
        {
            this.objectsInside.Add(obj);
        }

        public List<ILevelObject> getObjectsInside()
        {
            return this.objectsInside;
        }
    }
}
