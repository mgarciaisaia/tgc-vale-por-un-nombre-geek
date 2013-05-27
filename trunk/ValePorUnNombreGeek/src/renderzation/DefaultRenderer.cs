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
    class DefaultRenderer:Renderer
    {
        protected Effect effect;

        public DefaultRenderer()
        {
            effect = TgcShaders.loadEffect(EjemploAlumno.ShadersDir + "shaders.fx");
        }

        public override void render()
        {
            string technique;


            technique = "DIFFUSE_MAP";


            foreach (TerrainPatch p in this.patches)
            {
                p.Effect = GuiController.Instance.Shaders.VariosShader;
                p.Technique = TgcShaders.T_POSITION_TEXTURED;
                p.render();
            }

            foreach (ILevelObject o in this.objects)
            {
                o.Effect = effect;
                o.Technique = technique;
                o.render();
            }

            foreach (Character c in this.characters)
            {
                c.Effect = effect;
                c.Technique = technique;
                c.render();
            }

        }

        public override void dispose()
        {
            //...
        }
    }
}
