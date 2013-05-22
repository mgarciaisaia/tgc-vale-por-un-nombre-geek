using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.objects;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain.divisibleTerrain;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Shaders;
using TgcViewer;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.renderzation
{
    class DefaultRenderer:IRenderer
    {
        protected List<Enemy> enemies;
        protected List<Commando> commandos;
        protected List<ILevelObject> objects;
        protected List<TerrainPatch> terrainPatches;
        protected Effect effect;

        public DefaultRenderer()
        {
            effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "shaders.fx");
        }

        public virtual void beginRender()
        {
            enemies = new List<Enemy>();
            commandos = new List<Commando>();
            objects = new List<ILevelObject>();
            terrainPatches = new List<TerrainPatch>();

        }

        public virtual void render(ILevelObject o)
        {
            objects.Add(o);
            o.Effect = effect;
        }

        public virtual void render(Commando c)
        {
            commandos.Add(c);
            c.Effect = effect;
        }

        public virtual void render(Enemy e)
        {
            enemies.Add(e);
            e.Effect = effect;
        }

        public virtual void render(TerrainPatch t)
        {
            terrainPatches.Add(t);
            t.Effect = GuiController.Instance.Shaders.VariosShader;
        }

        public virtual void endRender()
        {
            string technique;


            technique = "DIFFUSE_MAP";


            foreach (TerrainPatch p in terrainPatches)
            {
              
               
                p.Technique = TgcShaders.T_POSITION_TEXTURED;
              
                p.render();

            }

            foreach (ILevelObject o in objects)
            {
                o.Technique = technique;
                o.render();
            }

            foreach (Commando c in commandos)
            {
                c.Technique = technique;
                c.render();
            }
            foreach (Enemy e in enemies)
            {
                e.Technique = technique;
                e.render();
            }

        }

        public virtual void dispose()
        {

        }
    }
}
