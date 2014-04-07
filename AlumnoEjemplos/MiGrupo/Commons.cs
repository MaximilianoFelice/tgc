using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TgcViewer;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.TheGroup
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
        public static readonly Vector3 Versor3Y = new Vector3(0f, 1f, 0f);
        /// <summary>
        /// Versor (0,0,1)
        /// </summary>
        public static readonly Vector3 Versor3Z = new Vector3(0f, 0f, 1f);
        /// <summary>
        /// Versor (0,0,0)
        /// </summary>
        public static readonly Vector3 Versor30 = new Vector3(0f, 0f, 0f);
        /// <summary>
        /// Versor (1,1,1)
        /// </summary>
        public static readonly Vector3 Versor31 = new Vector3(1f, 1f, 1f);
        /// <summary>
        /// Versor (1,0)
        /// </summary>
        public static readonly Vector2 Versor2X = new Vector2(1f, 0f);
        /// <summary>
        /// Versor (0,1)
        /// </summary>
        public static readonly Vector2 Versor2Y = new Vector2(0f, 1f);
        /// <summary>
        /// Versor (0,0)
        /// </summary>
        public static readonly Vector2 Versor20 = new Vector2(0f, 0f);
        /// <summary>
        /// Versor (1,1)
        /// </summary>
        public static readonly Vector2 Versor21 = new Vector2(1f, 1f);
        #endregion
    }
}
