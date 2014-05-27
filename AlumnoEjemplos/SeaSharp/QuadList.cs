using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.SeaSharp
{
    public class QuadList
    {
        public List<TgcQuad> quadList;      //TODO CHANGE SCOPE
        private int _module;
        private int _currentModule;
        private int _density;    // TODO: Implement triangle density constructor

        public QuadList(Vector3 center, int triangleSize, Color color, int module)
        {
            /* Setting the QuadList variables */
            _module = module;
            quadList = new List<TgcQuad>();
            _currentModule = triangleSize;

            /* Creating the first TgcQuad */
            TgcQuad firstQuad = new TgcQuad();

            firstQuad.Center = center;
            firstQuad.Size = new Vector2(triangleSize,triangleSize);
            firstQuad.Color = color;

            firstQuad.updateValues();

            /* Adding Quad to the list */
            quadList.Add(firstQuad);

            /* Applying Triangle Density */
            this.ApplyDensity();

        }

        public void ApplyDensity(){

            do {
                /* Generates auxiliar quadList for removing items */
                List<TgcQuad> auxQuadList = new List<TgcQuad>(quadList);

                foreach (TgcQuad quad in auxQuadList)
                {
                    List<TgcQuad> quadDivisions = quad.SubDivide();
                    quadList.Remove(quad);
                    quadList.AddRange(quadDivisions);

                }

                _currentModule /= 2;

            } while (_currentModule > _module);

        }

        public void Render()
        {
            foreach (TgcQuad quad in quadList) quad.render();
        }

        public void Dispose()
        {
            foreach (TgcQuad quad in quadList) quad.dispose();
        }

        #region SETTERS_AND_GETTERS

        public int density{
            get { return _density; }
            set { _density = value; }
        }

        public Microsoft.DirectX.Direct3D.Effect Effect
        {
            set
            {
                foreach (TgcQuad quad in quadList) quad.Effect = value;
            }
        }

        public String Technique
        {
            set
            {
                foreach (TgcQuad quad in quadList) quad.Technique = value;
            }
        }



        #endregion
    }
}
