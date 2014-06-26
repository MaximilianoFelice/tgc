using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.TgcGeometry;

namespace AlumnoEjemplos.SeaSharp
{
    public static class Commons
    {
        #region ::VERSORES::
        /// <summary>
        /// Versor (1,0,0)
        /// </summary>
        public static readonly Vector3 Vector3X = new Vector3(1f, 0f, 0f);
        /// <summary>
        /// Versor (0,1,0)
        /// </summary>
        public static readonly Vector3 Vector3Y = new Vector3(0f, 1f, 0f);
        /// <summary>
        /// Versor (0,0,1)
        /// </summary>
        public static readonly Vector3 Vector3Z = new Vector3(0f, 0f, 1f);
        /// <summary>
        /// Versor (0,0,0)
        /// </summary>
        public static readonly Vector3 Vector30 = new Vector3(0f, 0f, 0f);
        /// <summary>
        /// Versor (1,1,1)
        /// </summary>
        public static readonly Vector3 Vector31 = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Versor (1,0)
        /// </summary>
        public static readonly Vector2 Vector2X = new Vector2(1f, 0f);
        /// <summary>
        /// Versor (0,1)
        /// </summary>
        public static readonly Vector2 Vector2Y = new Vector2(0f, 1f);
        /// <summary>
        /// Versor (0,0)
        /// </summary>
        public static readonly Vector2 Vector20 = new Vector2(0f, 0f);
        /// <summary>
        /// Versor (1,1)
        /// </summary>
        public static readonly Vector2 Vector21 = new Vector2(1f, 1f);
        #endregion

    }


    /* Redefinition zone */

    /* Agrego metodos a TgcScene */
    #region TGC_SCENE_NEW_METHODS
    public static class MovableTgcScene
    {
        /* Funciones que manejan la scene como conjunto */
        #region TGC_SCENE HANDLER
        /* Mueve todos los meshes de una scene */
        public static void Move(this TgcScene Scene, Vector3 MovementVector)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.move(MovementVector);
        }

        /* Reposiciona todos los meshes de una scene */
        public static void Position(this TgcScene Scene, Vector3 NewPosition)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.Position = NewPosition;
        }

        /* Rota a todos los meshes de una scene */
        public static void RotateY(this TgcScene Scene, float Angle)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.rotateY(Angle);
        }

        public static void RotateX(this TgcScene Scene, float Angle)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.rotateX(Angle);
        }

        public static void RotateZ(this TgcScene Scene, float Angle)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.rotateZ(Angle);
        }

        public static void RotatationZ(this TgcScene Scene, float Angle)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.Rotation = new Vector3(Mesh.Rotation.X, Mesh.Rotation.Y, Angle);
        }

        public static void copyShipRotationY(this TgcScene Scene, Ship targetShip)
        {
            //foreach (TgcMesh Mesh in Scene.Meshes)
                for (int i = 0; i < Scene.Meshes.Count; i++)
                {
                    Scene.Meshes[i].Rotation = new Vector3(-targetShip.ship.Meshes[i].Rotation.X, -targetShip.ship.Meshes[i].Rotation.Y + Geometry.DegreeToRadian(90), targetShip.ship.Meshes[i].Rotation.Z);
                }
                
        }

        /* Retorna los valores de rotacion del scene */
        public static float RotationY(this TgcScene Scene)
        {
            return (Scene.Meshes[0].Rotation.Y);
        }

        public static float RotationX(this TgcScene Scene)
        {
            return (Scene.Meshes[0].Rotation.X);
        }

        public static float RotationZ(this TgcScene Scene)
        {
            return (Scene.Meshes[0].Rotation.Z);
        }

        public static void Scale(this TgcScene Scene, Vector3 ScaleVector)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.Scale = (ScaleVector);
        }

        public static void SetNormal(this TgcScene Scene, Vector3 Normal)
        {
            foreach (TgcMesh Mesh in Scene.Meshes) Mesh.Rotation = new Vector3(Normal.X, Mesh.Rotation.Y, Normal.Z); // FIXME HAY ALGO RARISIMO ACA.

            //foreach (TgcMesh Mesh in Scene.Meshes) Mesh.Rotation = new Vector3(Normal.X, FastMath.Cos(Mesh.Rotation.Y) * Normal.Y, Normal.Z); // FIXME HAY ALGO RARISIMO ACA.
        }

        #endregion
    #endregion


    }

    /* Added methods to TgcQuad */
    public static class DivisibleTgcQuad
    {
        public static List<TgcQuad> SubDivide(this TgcQuad ParentQuad)
        {

            List<TgcQuad> dividedQuads = new List<TgcQuad>();

            /* Useful Values for calculations */
            float newSize = ParentQuad.Size.X / 2;
            float newCenterDisplacement = newSize / 2;      // Displacement from the previous centers to the new ones.

            /* Calculating size for each new quad would have the same result */
            Vector2 newQuadSize = new Vector2(newSize, newSize);

            Vector3 Normal = new Vector3(0, 1, 0);
            
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++){

                    TgcQuad newQuad = new TgcQuad();

                    /* New Quad Properties calculations */
                    float xCenterPosition = ParentQuad.Center.X + ((float) Math.Pow(-1,i) * newCenterDisplacement);
                    float zCenterPosition = ParentQuad.Center.Z + ((float) Math.Pow(-1,j) * newCenterDisplacement);
                    Vector3 newQuadCenter = new Vector3(xCenterPosition, ParentQuad.Center.Y, zCenterPosition);

                    /* New Quad Properties assignments */
                    newQuad.Center = newQuadCenter;
                    newQuad.Size = newQuadSize;
                    newQuad.Color = ParentQuad.Color;
                    newQuad.Normal = Normal;
                    newQuad.updateValues();

                    /* Adding New Quad to the list */
                    dividedQuads.Add(newQuad);

                }
            }

            return dividedQuads;
        }

    }

}