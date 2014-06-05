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

        public static int irender = 1;
        public static QuadList water;   // The soon-to-be-a-custom-vertex... thing.
        public static QuadList water2;   // The soon-to-be-a-custom-vertex... thing.
        public static QuadList water3;   // The soon-to-be-a-custom-vertex... thing.
        public static QuadList water4;   // The soon-to-be-a-custom-vertex... thing.

        public static float time = 0f;
        public static float ambient, diffuse, specular, specularPower;
        public static Vector3 lightPos;
        public static Texture texture;

        public static void Load()
        {

            Vector3 center = new Vector3(-2000, 0, -2000);

            water = new QuadList(center, 4000, Color.Blue, 200);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            texture = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\superficieAgua.png");
            water.Technique = "RenderCubeMap";


            water2 = new QuadList(center + new Vector3(0, 0, 4000), 4000, Color.Blue, 200);
            water2.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            water2.Technique = "RenderCubeMap";

            water3 = new QuadList(center + new Vector3(4000, 0, 0), 4000, Color.Blue, 200);
            water3.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            water3.Technique = "RenderCubeMap";

            water4 = new QuadList(center + new Vector3(4000, 0, 4000), 4000, Color.Blue, 200);
            water4.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            water4.Technique = "RenderCubeMap";

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

        public static void Render(CubeTexture surf, bool aux)
        {         
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;
            
            SetWaterValues(ref water, surf, aux);
            SetWaterValues(ref water2, surf, aux);
            SetWaterValues(ref water3, surf, aux);
            SetWaterValues(ref water4, surf, aux);

            device.RenderState.AlphaBlendEnable = true;

            // Alternamos los renderizados (no se percibe por la velocidad)
            if (irender == 1)
            {
                irender = 0;
                // Renderizamos las aguas de la izquierda
                water2.Render();
                water4.Render(); 
            }
            else if (irender == 0)
            {
                irender = 1;
                // Renderizamos las aguas de la derecha
                water.Render();
                water3.Render();
            }
            
            

        }

        private static void SetWaterValues(ref QuadList water, CubeTexture surf, bool aux)
        {
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
            if (!aux)
            {
                water.Effect.SetValue("g_txCubeMap", surf);
                //water.Technique = "RenderCubeMap";
                //water.Render();
            }
        }
        public static void Close()
        {
            water.Dispose();
        }


    }
}
