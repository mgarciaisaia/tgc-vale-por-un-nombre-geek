using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QTRama : QTNode
    {
        private QTNode[] children;
        //private int count;

        public QTRama(int _children)
        {
            this.children = new QTNode[_children];
            //count = _children;
        }

        public QTNode child(int index)
        {
            return this.children[index];
        }

        //public int Count { get { return this.count; } }

        public void setChild(QTNode node, int index)
        {
            this.children[index] = node;
        }
    }
}
