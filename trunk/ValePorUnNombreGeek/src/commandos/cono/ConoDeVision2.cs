using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character.characterRepresentation;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.cono
{
    //Intento de mejora del cono, cuando terminemos con la reestructuracion lo voy a probar, ahora no porq estan cambiando cosas
    class ConoDeVision2 : Cono
    {

        float verticalAngle;
        public float MaxVerticalAngle { get { return this.verticalAngle; } set { this.verticalAngle = value; } }
        public float MaxHorizontalAngle { get { return this.angle; } set { this.angle = value; } }
        ICharacterRepresentation characterRepresentation;

        public float MaxDistance { get { return this.radius; } set { this.radius = value; } }

        public ConoDeVision2(ICharacterRepresentation characterRepresentation, float maxDistance, float maxHorizontalAngle, float maxVerticalAngle)
            : base(characterRepresentation.Position + characterRepresentation.getEyeLevel(), maxDistance, maxHorizontalAngle)
        {

            this.AutoTransformEnable = false;
            this.characterRepresentation = characterRepresentation;
            GuiController.Instance.Modifiers.addBoolean("Cono", "Visible", false);
        }

        public override void render()
        {
            this.update();

            base.render();
        }

        private void update()
        {
            this.Enabled = (bool)GuiController.Instance.Modifiers.getValue("Cono");
            this.Transform = this.characterRepresentation.Transform + Matrix.Translation(this.characterRepresentation.getEyeLevel());
        }

        public override void renderWireframe()
        {
            update();
            base.renderWireframe();
        }


        public bool isInsideVisionRange(TgcBox target)
        {
            return false;
        }

        protected override void crearCircunferencia(float radiusA, int cantPuntos)
        {
            float theta;
            float dtheta = 2 * FastMath.PI / cantPuntos;
            int i;

            float radiusB = radius * FastMath.Tan(FastMath.ToRad(MaxVerticalAngle));

            circunferencia = new Vector3[cantPuntos];

            for (i = 0, theta = 0; i < cantPuntos; i++, theta += dtheta)
            {

                circunferencia[i] = new Vector3(
                          radiusA * FastMath.Cos(theta),
                          (float)(radiusB * FastMath.Sin(theta)),
                          -radius
                     );
            }
        }

    }

}