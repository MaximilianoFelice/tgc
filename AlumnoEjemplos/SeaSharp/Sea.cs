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

        public static QuadTree water;   // The soon-to-be-a-custom-vertex... thing.

        public static float time = 0f;
        public static float ambient, diffuse, specular, specularPower;
        public static Vector3 lightPos;
        public static Texture texture;

        public static void Load()
        {

            Vector3 center = new Vector3(0, -30, 0);

            water = QuadTree.generateNewQuad(center, 8000, Color.Blue, 150);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            texture = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\superficieAgua.png");
            water.Technique = "RenderCubeMap";

            lightPos = new Vector3(0, 10000, 0);
            ambient = 0.7f;
            diffuse = 1.0f;
            specular = 1.0f;
            specularPower = 50.0f; 

        }

        internal static void CalculateMovement(float elapsedTime)
        {
            time += elapsedTime;
        }

        public static void Render(CubeTexture surf, TgcFrustum frustum)
        {         
                   
            water.Effect.SetValue("time", time);
       
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            //Cargar variables de shader
            water.Effect.SetValue("time", time);
            water.Effect.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(lightPos));
            water.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getPosition()));
            water.Effect.SetValue("fvEyeLookAt", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getLookAt()));
            water.Effect.SetValue("k_la", ambient);
            water.Effect.SetValue("k_ld", diffuse);
            water.Effect.SetValue("k_ls", specular);
            water.Effect.SetValue("fSpecularPower", specularPower);
            water.Effect.SetValue("superficieAgua", texture);
            water.Effect.SetValue("g_txCubeMap", surf);

            device.RenderState.AlphaBlendEnable = true;

            water.Render(frustum);

        }
        public static void Close()
        {
            water.Dispose();
        }


    }
}
