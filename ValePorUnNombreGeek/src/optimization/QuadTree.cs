using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.optimization
{
    class QuadTree : AlumnoEjemplos.ValePorUnNombreGeek.src.optimization.IQuadTree
    {
        List<ILevelObject> objects;
        List<Commando> commandos;
        List<Enemy> enemies;
        ITerrain terrain;

        private QTNode firstNode;
        private const int DIVISIONESX = 2;
        private const int DIVISIONESY = 2;
        private const int GRADO = DIVISIONESX * DIVISIONESY;
        private const int PROFUNDIDAD = 2; //debe ser > 0

        public IRenderer Renderer { get; set; }


        private QTRama crearArbol(int profundidad, int grado, int divisionesx, int divisionesy, Vector3 position, Vector3 size)
        {
            QTRama ret = new QTRama(grado);
            ret.setBounds(position, position + size);

            Vector3 childSize = size;
            childSize.X /= divisionesx;
            childSize.Z /= divisionesy;

            int i = 0;

            for (int x = 0; x < divisionesx; x++)
                for (int y = 0; y < divisionesy; y++)
                {
                    Vector3 childPos = position;
                    childPos.X += ((float)x / divisionesx) * size.X;
                    childPos.Z += ((float)y / divisionesy) * size.Z;

                    if (profundidad == 0) //su hijo es una hoja
                    {
                        ret.setChild(new QTHoja(), i);
                        ret.child(i).setBounds(childPos, childPos + childSize);
                    }
                    else
                        ret.setChild(crearArbol(profundidad - 1, grado, divisionesx, divisionesy, childPos, childSize), i);

                    i++;
                }
            return ret;
        }

        public void renderizarArbol(QTNode raiz, int profundidad, int grado)
        {
            raiz.BoundingBox.render();

            if (profundidad > 0) //tiene hijos
                for (int i = 0; i < grado; i++)
                    renderizarArbol(((QTRama)raiz).child(i), profundidad - 1, grado);
        }



        public QuadTree(ITerrain terrain, IRenderer renderer)
        {
            this.terrain = terrain;
            this.objects = new List<ILevelObject>();
            this.commandos = new List<Commando>();
            this.enemies = new List<Enemy>();
            this.Renderer = renderer;

            float terrainHeight = this.terrain.Height;
            float terrainWidth = this.terrain.Width;
            Vector3 terrainPos = terrain.Position - new Vector3(terrainHeight / 2, 0, terrainWidth / 2);

            firstNode = this.crearArbol(PROFUNDIDAD - 1, GRADO, DIVISIONESX, DIVISIONESY, terrainPos, new Vector3(terrainHeight, terrain.maxY, terrainWidth));

            //recorremos las hojas y agregamos los objetos
            for (int i = 0; i < GRADO; i++)
            {
                //TODO
            }
        }

        public void add(ILevelObject obstacle)
        {
            this.objects.Add(obstacle);
            //GuiController.Instance.UserVars.addVar("obj " + obstacle.GetHashCode().ToString());
        }

        public void add(Commando commando)
        {
            this.commandos.Add(commando);
            GuiController.Instance.UserVars.addVar("commando " + commando.GetHashCode().ToString());
        }

        public void add(Enemy enemy)
        {
            this.enemies.Add(enemy);
            GuiController.Instance.UserVars.addVar("enemy " + enemy.GetHashCode().ToString());
        }

        public void render(TgcFrustum frustum)
        {
            HashSet<ILevelObject> objetosARenderizar = new HashSet<ILevelObject>();
            HashSet<Commando> commandosARenderizar = new HashSet<Commando>();
            HashSet<Enemy> enemigosARenderizar = new HashSet<Enemy>();

            float terrainHeight = this.terrain.Height;
            float terrainWidth = this.terrain.Width;
            Vector3 terrainPos = terrain.Position - new Vector3(terrainHeight / 2, 0, terrainWidth / 2);

            this.renderizarArbol(this.firstNode, PROFUNDIDAD, GRADO);
            //const int PASADAS = 4;

            /*TgcBoundingBox[,] sectores = new TgcBoundingBox[2,2];

            //for (int pasada = 0; pasada < PASADAS; pasada++)
            //{
                for (int ix = 0; ix < 2; ix++)
                {
                    for (int iy = 0; iy < 2; iy++)
                    {
                        Vector3 pmin = terrainPos + new Vector3(terrainHeight / 2 * ix, 0, terrainWidth / 2 * iy);
                        Vector3 pmax = terrainPos + new Vector3(terrainHeight / 2 * (ix + 1), 300, terrainWidth / 2 * (iy + 1));
                        sectores[ix, iy] = new TgcBoundingBox(pmin, pmax);
                        sectores[ix, iy].render();

                        TgcCollisionUtils.FrustumResult resultado = TgcCollisionUtils.classifyFrustumAABB(frustum, sectores[ix, iy]);
                        if (resultado != TgcCollisionUtils.FrustumResult.OUTSIDE)
                        {
                            //hay colision
                            foreach (ILevelObject obj in this.objects)
                                if (obj.collidesWith(sectores[ix, iy]))
                                    objetosARenderizar.Add(obj);
                            foreach (Commando ch in this.commandos)
                                if (TgcCollisionUtils.sqDistPointAABB(ch.Position, sectores[ix, iy]) == 0)
                                    commandosARenderizar.Add(ch);
                            foreach (Enemy ch in this.enemies)
                                if (TgcCollisionUtils.sqDistPointAABB(ch.Position, sectores[ix, iy]) == 0)
                                    enemigosARenderizar.Add(ch);
                        }
                    }
                }
            //}
            */

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();
            
            /*foreach (ILevelObject asd in this.objects)
                GuiController.Instance.UserVars.setValue("obj " + asd.GetHashCode().ToString(), false);
            foreach (Commando asd in this.commandos)
                GuiController.Instance.UserVars.setValue("commando " + asd.GetHashCode().ToString(), false);
            foreach (Enemy asd in this.enemies)
                GuiController.Instance.UserVars.setValue("enemy " + asd.GetHashCode().ToString(), false);

            foreach (ILevelObject obj in objetosARenderizar)
            {
                this.Renderer.render(obj);
                GuiController.Instance.UserVars.setValue("obj " + obj.GetHashCode().ToString(), true);
            }
            foreach (Commando ch in commandosARenderizar)
            {
                this.Renderer.render(ch);
                GuiController.Instance.UserVars.setValue("commando " + ch.GetHashCode().ToString(), true);
            }
            foreach (Enemy ch in enemigosARenderizar)
            {
                this.Renderer.render(ch);
                GuiController.Instance.UserVars.setValue("enemy " + ch.GetHashCode().ToString(), true);
            }*/

            foreach (Enemy e in enemies) this.Renderer.render(e);
            foreach (TerrainPatch p in terrain.Patches) this.Renderer.render(p);
            foreach (ILevelObject o in objects) this.Renderer.render(o);
            foreach (Commando c in commandos) this.Renderer.render(c);

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
