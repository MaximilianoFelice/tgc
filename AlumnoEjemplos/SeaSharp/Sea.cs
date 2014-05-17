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
        public static TgcBox water;   // The soon-to-be-a-scene box.

        public static float time = 0f;

        public static void Load()
        {
            Vector3 center = new Vector3(0,-30,0);
            Vector3 size = new Vector3(10000, 10, 10000);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\Water01.jpg");
            water = TgcBox.fromSize(center, size, texture);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\ColorSwift.fx");
            water.Technique = "RenderScene";

        }

        internal static void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;
        }

        public static void Render()
        {
            water.Effect.SetValue("time", time);
            water.render();
        }

        public static void Close()
        {
            water.dispose();
        }


    }
}
