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
using TgcViewer.Utils.Interpolation;


namespace AlumnoEjemplos.SeaSharp
{
    public static class Sea
    {

        public static QuadTree water;

        public static float time = 0f;
        public static float ambient, diffuse, specular, specularPower;
        public static Texture texture;
        public static Texture texture2;
        public static Color colorMar;
        public static Vector3 shipPos;
        public static InterpoladorVaiven interpolador;
        public static float alfa = 0f;
        
        public static void Load()
        {     
            //Creo el interpolador para los normal map
            interpolador = new InterpoladorVaiven();
            interpolador.Max = 0.9f;
            interpolador.Min = 0;
            interpolador.Speed = 1f;            

            //Genero el agua
            Vector3 center = new Vector3(0, -30, 0);
            water = QuadTree.generateNewQuad(center, 16000, Color.Blue, (int) ConfigParam.Sea.getTamaniotriangulos());
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            texture = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\superficieAgua.tga");
            texture2 = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\superficieAgua2.tga");
            water.Technique = "ParallaxOcclusion";
        }

        internal static void CalculateMovement(float elapsedTime, Vector3 pos)
        {
            time += elapsedTime;
            shipPos = pos;
        }

        public static void Render(CubeTexture surf, TgcFrustum frustum)
        {
            interpolador.update();
       
            Microsoft.DirectX.Direct3D.Device device = GuiController.Instance.D3dDevice;

            //Cargar variables de shader
            water.Effect.SetValue("time", time);
            water.Effect.SetValue("fvLightPosition", TgcParserUtils.vector3ToFloat3Array(ConfigParam.Sea.getLightPos()));
            water.Effect.SetValue("fvEyePosition", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getPosition()));
            water.Effect.SetValue("fvEyeLookAt", TgcParserUtils.vector3ToFloat3Array(GuiController.Instance.RotCamera.getLookAt()));
            water.Effect.SetValue("k_la", ConfigParam.Sea.getAmbient());
            water.Effect.SetValue("k_ld", ConfigParam.Sea.getDiffuse());
            water.Effect.SetValue("k_ls", ConfigParam.Sea.getSpecular());
            water.Effect.SetValue("fSpecularPower", ConfigParam.Sea.getSpecularPower());
            water.Effect.SetValue("amplitudx", ConfigParam.Sea.getAmplitudX());
            water.Effect.SetValue("amplitudz", ConfigParam.Sea.getAmplitudZ());
            water.Effect.SetValue("frecuenciax", ConfigParam.Sea.getFrecuenciaX());
            water.Effect.SetValue("frecuenciaz", ConfigParam.Sea.getFrecuenciaZ());
            water.Effect.SetValue("kx", ConfigParam.Sea.getReflexion());
            water.Effect.SetValue("superficieAgua", texture);
            water.Effect.SetValue("superficieAgua2", texture2);
            water.Effect.SetValue("g_txCubeMap", surf);
            water.Effect.SetValue("colorAgua", ConfigParam.Sea.getColorMar().ToArgb());
            water.Effect.SetValue("shipPos", TgcParserUtils.vector3ToFloat3Array(shipPos));
            water.Effect.SetValue("fHeightMapScale", ConfigParam.Sea.getHeightmapScale());
            water.Effect.SetValue("fTexScale", ConfigParam.Sea.getTexScale());
            water.Effect.SetValue("coeficiente", interpolador.Current);
            water.Effect.SetValue("specularColor", ConfigParam.Sea.getSpecularColor().ToArgb());
            water.Effect.SetValue("diffuseColor", ConfigParam.Sea.getDiffuseColor().ToArgb());
            device.RenderState.AlphaBlendEnable = true;

            water.Render(frustum);

        }
        public static void Close()
        {
            water.Dispose();
        }


    }
}
