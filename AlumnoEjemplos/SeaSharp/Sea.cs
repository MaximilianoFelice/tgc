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
            water = TgcBox.fromSize(center, size, texture);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\IluminacionAgua.fx");
            water.Technique = "DefaultTechnique";

            lightPos = new Vector3(0, -1000, 7000);
            //GuiController.Instance.Modifiers.addVertex3f("LightPosition", new Vector3(-100, -1000, -100), new Vector3(10000, 1000, 10000), new Vector3(0, 40, 0));
            //lightBox = TgcBox.fromSize(lightPos, new Vector3(5, 5, 5), Color.Yellow);
            ambient = 0.7f;
            diffuse = 0.4f;
            specular = 1.0f;
            specularPower = 16.84f; 


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
            

            water.render();
            //lightBox.render();

        }
        public static void Close()
        {
            water.dispose();
            lightBox.dispose();
        }


    }
}
