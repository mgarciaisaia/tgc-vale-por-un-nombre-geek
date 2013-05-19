﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.character;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.TgcGeometry;
using AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.terrain;

namespace AlumnoEjemplos.ValePorUnNombreGeek.src.commandos.picking.selection.rectangle
{
    class WorldProjection : ScreenProjection
    {

        //TODO pablo - este metodo de seleccion funciona pero no de la manera mas sana. por ahora lo dejo en puntos suspensivos.


        private Terrain terrain;

        public WorldProjection(Terrain _terrain, List<Character> _selectableCharacters)
            : base(_selectableCharacters)
        {
            this.terrain = _terrain;
        }

        private List<Character> getCharactersInBox(TgcBox _selectionBox)
        {
            List<Character> ret = new List<Character>();
            Vector3 n; //useless

            foreach (Character ch in this.selectableCharacters)
                if(ch.collidesWith(_selectionBox.BoundingBox, out n))
                    ret.Add(ch);

            return ret;
        }

        #region Update

        private Vector3 initTerrainPoint;
        public override bool canBeginSelection()
        {
            if (!PickingRaySingleton.Instance.terrainIntersection(this.terrain, out this.initTerrainPoint))
                return false;

            return base.canBeginSelection();
        }


        private Vector3 actualTerrainPoint;
        public override void updateSelection()
        {
            Vector3 intersectionPoint;
            if (PickingRaySingleton.Instance.terrainIntersection(this.terrain, out intersectionPoint))
                this.actualTerrainPoint = intersectionPoint;
            else
                return;

            base.updateSelection();
        }


        public override List<Character> endAndRetSelection()
        {
            Vector3 min = Vector3.Minimize(initTerrainPoint, actualTerrainPoint);
            Vector3 max = Vector3.Maximize(initTerrainPoint, actualTerrainPoint);
            min.Y = this.terrain.minY;
            max.Y = this.terrain.maxY;

            TgcBox selectionBox = TgcBox.fromExtremes(min, max);
            return this.getCharactersInBox(selectionBox);
        }

        #endregion
    }
}