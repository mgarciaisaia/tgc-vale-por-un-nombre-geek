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
        ITerrain terrain;
        public IRenderer Renderer { get; set; }


        public QuadTree(ITerrain terrain, IRenderer renderer)
        {
            this.terrain = terrain;
            this.objects = new List<ILevelObject>();
            this.Renderer = renderer;
        }

        public void add(ILevelObject obstacle)
        {
            objects.Add(obstacle);
        }

        public void render(TgcFrustum frustum, List<Commando> commandos, List<Enemy> enemies)
        {
            HashSet<ILevelObject> objetosARenderizar = new HashSet<ILevelObject>();
            HashSet<Commando> commandosARenderizar = new HashSet<Commando>();
            HashSet<Enemy> enemigosARenderizar = new HashSet<Enemy>();

            float terrainHeight = this.terrain.Height;
            float terrainWidth = this.terrain.Width;
            Vector3 terrainPos = terrain.Position - new Vector3(terrainHeight / 2, 0, terrainWidth / 2);
            //const int PASADAS = 4;

            TgcBoundingBox[,] sectores = new TgcBoundingBox[2,2];

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

                        TgcCollisionUtils.FrustumResult resultado = TgcCollisionUtils.classifyFrustumAABB(GuiController.Instance.Frustum, sectores[ix, iy]);
                        if (resultado != TgcCollisionUtils.FrustumResult.OUTSIDE)
                        {
                            //hay colision
                            foreach (ILevelObject obj in this.objects)
                                if (obj.collidesWith(sectores[ix, iy]))
                                    objetosARenderizar.Add(obj);
                            foreach (Commando ch in commandos)
                                if (ch.collidesWith(sectores[ix, iy]))
                                    commandosARenderizar.Add(ch);
                            foreach (Enemy ch in enemies)
                                if (ch.collidesWith(sectores[ix, iy]))
                                    enemigosARenderizar.Add(ch);
                        }
                    }
                }
            //}

            //El renderer se encarga de renderizarlos en el orden correcto y usar los shaders y pasadas correspondientes.
            this.Renderer.beginRender();

            foreach (ILevelObject obj in objetosARenderizar)
                this.Renderer.render(obj);
            foreach (Commando ch in commandosARenderizar)
                this.Renderer.render(ch);
            foreach (Enemy ch in enemigosARenderizar)
                this.Renderer.render(ch);
            //nota: separo las cosas en objetos, commandos y enemigos por que no se si haciendo
            //una sola coleccion de ILevelObject y llamando a Renderer.render(obj) no entran
            //todos por render(ILevelObject) en vez de render(Commando) y render(Enemy)

            foreach (TerrainPatch p in terrain.Patches) this.Renderer.render(p); //TODO no renderizar todos los sectores

            this.Renderer.endRender();
            
        }

        public void dispose()
        {
            this.Renderer.dispose();

        }


    }
}
