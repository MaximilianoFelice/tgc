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

        public static CustomVertex.PositionColored[] WaterMesh;

        public static float time = 0f;

        //public static void Load()
        //{
        //    WaterMesh = new CustomVertex.PositionColored[4];

        //    WaterMesh[0] = new CustomVertex.PositionColored(-4000, 0, -4000, 255);
        //    WaterMesh[0] = new CustomVertex.PositionColored(4000, 0, -4000, 255);
        //    WaterMesh[0] = new CustomVertex.PositionColored(4000, 0, 4000, 255);
        //    WaterMesh[0] = new CustomVertex.PositionColored(-4000, 0, 4000, 255);

        //    Vector3 center = new Vector3(0,-30,0);
        //    Vector2 size = new Vector2(10000, 10000);
        //    TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\Water01.jpg");

        //    water = new TgcQuad();
        //    water.Center = center;
        //    water.Size = size;
        //    water.Color = Color.Blue;

        //    water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\ColorSwift.fx");
        //    water.Technique = "RenderScene";

        //    water.updateValues();

        //}

        public static void Load()
        {
            Vector3 center = new Vector3(0, -30, 0);

            water = new QuadList(center, 10000, Color.Blue, 1000);
        }

        internal static void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;
        }

        public static void Render()
        {
            //water.Effect.SetValue("time", time);
            water.Render();
        }

        public static void Close()
        {
            water.Dispose();
        }


    }
}
