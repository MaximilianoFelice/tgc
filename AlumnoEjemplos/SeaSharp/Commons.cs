using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;
using TgcViewer.Utils.TgcSceneLoader;

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
        #endregion
    }

}