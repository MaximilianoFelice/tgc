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
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.SeaSharp
{
    public class QuadTree
    {
        public List<QuadTree> quadList;
        public TgcQuad _quad;
        public static int _module;
        public float _currentModule;
        public static int _density;    // TODO: Implement triangle density constructor
        public static Color _color;
        public Vector3 _center;
        public float LODI = 1000;
        public float LODII = 4000;

        public static QuadTree generateNewQuad(Vector3 center, int triangleSize, Color color, int module)
        {

            /* Setting the QuadList variables */
            _module = module;
            _color = color;

            /* Creating the first leaf */
            QuadTree quadTree = new QuadTree(center, triangleSize);

            return quadTree;


        }

        public QuadTree(Vector3 center, float triangleSize)
        {
            /* Initializing local variables */
            quadList = null;

            _currentModule = triangleSize;

            /* Creating the first TgcQuad */
            _quad = new TgcQuad();

            _quad.Center = center;
            _quad.Size = new Vector2(triangleSize, triangleSize);

            _quad.updateValues();

            /* Applying Triangle Density */
            this.ApplyDensity();
        }


        public void ApplyDensity(){

            if (_currentModule > _module) {

                quadList = new List<QuadTree>();

                _currentModule /= 2;

                /* Generate corresponding leafs */
                List<TgcQuad> newQuads = _quad.SubDivide();

                foreach (TgcQuad quad in newQuads) quadList.Add(new QuadTree(quad.Center, quad.Size.X));

            };

        }

        public void Render(TgcFrustum frustum)
        {
            if (quadList == null)
            {
                _quad.render();
            }
            else
            {
                foreach (QuadTree quadTree in quadList)
                {
                    TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult test = testChildVisibility(quadTree, frustum);
                    if (test == TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult.INSIDE) quadTree.RenderAll();
                    if (test == TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult.INTERSECT) quadTree.Render(frustum);
                }
            }
        }

        public void RenderAll()
        {
            if (quadList == null)
            {
                _quad.render();
            }
            else
            {
                foreach (QuadTree quadTree in quadList)
                {
                   quadTree.RenderAll();
                }
            }
        }

        public TgcCollisionUtils.FrustumResult testChildVisibility(QuadTree quadTree, TgcFrustum frustum)
        {

            Vector3 maxPoint = new Vector3(quadTree._quad.Center.X - quadTree._quad.Size.X / 2, quadTree._quad.Center.Y, quadTree._quad.Center.Z - quadTree._quad.Size.Y / 2);
            Vector3 minPoint = new Vector3(quadTree._quad.Center.X + quadTree._quad.Size.X / 2, quadTree._quad.Center.Y - 1, quadTree._quad.Center.Z + quadTree._quad.Size.Y / 2);
            TgcBoundingBox quadBox = new TgcBoundingBox(maxPoint, minPoint);

            TgcCollisionUtils.FrustumResult res = TgcCollisionUtils.classifyFrustumAABB(frustum, quadBox);

            return res;
        }

        public void Dispose()
        {
            _quad.dispose();
            if (quadList != null) foreach (QuadTree quadTree in quadList) quadTree.Dispose();
        }

        #region SETTERS_AND_GETTERS

        public int density{
            get { return _density; }
            set { _density = value; }
        }

        public Microsoft.DirectX.Direct3D.Effect _effect;

        public Microsoft.DirectX.Direct3D.Effect Effect
        {
            set
            {
                _effect = value;
                _quad.Effect = value;
                if (quadList != null) foreach (QuadTree quadTree in quadList) quadTree.Effect = value;
            }

            get { return _effect; }
        }

        public String Technique
        {
            set
            {
                _quad.Technique = value;
                if (quadList != null) foreach (QuadTree quadTree in quadList) quadTree.Technique = value;
            }
        }


        #endregion
    }
}
