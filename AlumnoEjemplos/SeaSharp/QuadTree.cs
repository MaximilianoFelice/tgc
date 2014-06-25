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
    public class QuadTree
    {
        public List<QuadTree> quadList;
        public TgcQuad _quad;
        public static int _module;
        public float _currentModule;
        public static int _density;    // TODO: Implement triangle density constructor
        public static Color _color;
        public Vector3 _center;

        public static float LODI;
        public static float LODI_Module;
        public static float LODII;
        public static float LODII_Module;


        public static void UpdateValues()
        {
            LODI = ConfigParam.QuadTree.getDistLODI();
            LODI_Module = ConfigParam.QuadTree.getSizeLODI();
            LODII = ConfigParam.QuadTree.getDistLODII();
            LODII_Module = ConfigParam.QuadTree.getSizeLODII();
        }

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
                /*Calculate Frustum Distance to this quad */
                float frustum_Distance = getDistanceTo(GuiController.Instance.CurrentCamera.getPosition());

                if (childrenAreVisible(frustum_Distance))
                {
                    foreach (QuadTree quadTree in quadList)
                    {
                        /* Checking child visibility */
                        TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult test = testChildVisibility(quadTree, frustum);
                        if (test == TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult.INSIDE) quadTree.RenderAll();
                        if (test == TgcViewer.Utils.TgcGeometry.TgcCollisionUtils.FrustumResult.INTERSECT) quadTree.Render(frustum);
                    }
                }
                else
                {
                    _quad.render();
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
                /*Calculate Frustum Distance to this quad */
                float frustum_Distance = getDistanceTo(GuiController.Instance.CurrentCamera.getPosition()); 

                if (childrenAreVisible(frustum_Distance))
                {
                    foreach (QuadTree quadTree in quadList)
                    {
                        quadTree.RenderAll();
                    }
                }
                else
                {
                    _quad.render();
                }

            }
        }

        public float getDistanceTo(Vector3 vector)
        {
            Vector3 distVector = _quad.Center - vector;
            float dist = distVector.Length();
            if (dist < 0) dist *= (-1);

            return dist;
        }

        public bool childrenAreVisible(float distance)
        {
            if (distance <= LODI) return true;
            else if ((distance >= LODI) & (distance <= LODII)){
                if (_currentModule >= LODI_Module) return true;
                else return false;
            }
            else
            {
                if (_currentModule >= LODII_Module) return true;
                else return false;
            }
        }

        public TgcCollisionUtils.FrustumResult testChildVisibility(QuadTree quadTree, TgcFrustum frustum)
        {
            float factor = _module / 2;
            float waveMaxHeigth = 500;
            Vector3 maxPoint = new Vector3(quadTree._quad.Center.X - quadTree._quad.Size.X / 2 - factor, quadTree._quad.Center.Y - waveMaxHeigth, quadTree._quad.Center.Z - quadTree._quad.Size.Y / 2 - factor);
            Vector3 minPoint = new Vector3(quadTree._quad.Center.X + quadTree._quad.Size.X / 2 + factor, quadTree._quad.Center.Y + waveMaxHeigth, quadTree._quad.Center.Z + quadTree._quad.Size.Y / 2 + factor);

            //Vector3 maxPoint = new Vector3(quadTree._quad.Center.X - quadTree._quad.Size.X , quadTree._quad.Center.Y, quadTree._quad.Center.Z - quadTree._quad.Size.Y );
            //Vector3 minPoint = new Vector3(quadTree._quad.Center.X + quadTree._quad.Size.X , quadTree._quad.Center.Y + 1, quadTree._quad.Center.Z + quadTree._quad.Size.Y );
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
