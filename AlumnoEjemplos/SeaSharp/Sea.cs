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
        public static TgcBox lightBox;

        public static float time = 0f;
        public static float ambient, diffuse, specular, specularPower;
        public static Vector3 lightPos;

        public static void Load()
        {
            Vector3 center = new Vector3(0,-30,0);
            Vector3 size = new Vector3(10000, 10, 10000);
            TgcTexture texture = TgcTexture.createTexture(GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\Water01.jpg");
            water = TgcBox.fromSize(center, size, Color.Blue);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\IluminacionAgua.fx");
            water.Technique = "DefaultTechnique";

            lightPos = new Vector3(0, -10000, -100000);
            //GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -1000, -100), new Vector3(10000, 1000, 10000), new Vector3(0, 40, 0));
            //lightBox = TgcBox.fromSize(lightPos, new Vector3(5, 5, 5), Color.Yellow);
            ambient = 0.1f;
            diffuse = 0.9f;
            specular = 1.0f;
            specularPower = 50.0f; 


        }

        internal static void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;
        }

        public static void Render()
        {
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            //Vector3 lightPosition = (Vector3)GuiController.Instance.Modifiers["LightPosition"];

            //Cargar variables de shader
            water.Effect.SetValue("time", time);
            water.Effect.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPos));
            water.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getPosition()));
            water.Effect.SetValue("k_la", ambient);
            water.Effect.SetValue("k_ld", diffuse);
            water.Effect.SetValue("k_ls", specular);
            water.Effect.SetValue("fSpecularPower", specularPower);
            device.RenderState.AlphaBlendEnable = true;

            water.render();
            /*
            Blend ant_src = device.RenderState.SourceBlend;
            Blend ant_dest = device.RenderState.DestinationBlend;
            bool ant_alpha = device.RenderState.AlphaBlendEnable;
            device.RenderState.AlphaBlendEnable = true;
            device.RenderState.SourceBlend = Blend.SourceColor;
            device.RenderState.DestinationBlend = Blend.InvSourceColor;
            ((TgcBox)water).render();
            device.RenderState.SourceBlend = ant_src;
            device.RenderState.DestinationBlend = ant_dest;
            device.RenderState.AlphaBlendEnable = ant_alpha;*/
            //lightBox.render();

        }
        public static void Close()
        {
            water.dispose();
            lightBox.dispose();
        }


    }
}
