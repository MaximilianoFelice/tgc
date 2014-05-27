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
    public static class Sea
    {
        public static QuadList water;   // The soon-to-be-a-custom-vertex... thing.

        public static float time = 0f;

        public static void Load()
        {
            Vector3 center = new Vector3(0, -30, 0);

            water = new QuadList(center, 4000, Color.Blue, 100);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\ColorSwift.fx");
            water.Technique = "RenderScene";
        }

        internal static void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;
        }

        public static void Render()
        {
            foreach (TgcQuad quad in water.quadList) quad.Effect.SetValue("time", time);
            //water.Effect.SetValue("time", time);
            water.Render();
        }

        public static void Close()
        {
            water.Dispose();
        }


    }
}
