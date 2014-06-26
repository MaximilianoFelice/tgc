﻿using System;
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
        public static Texture diffuseMap;
        public static Color colorMar;
        public static Random rand;
        public static int r;
        public static Vector3 shipPos;
        public static InterpoladorVaiven interpolador;
        public static float alfa = 0f;
        
        public static void Load()
        {
            
            Vector3 center = new Vector3(0, -30, 0);
            interpolador = new InterpoladorVaiven();

            water = QuadTree.generateNewQuad(center, 8000, Color.Blue,150);
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            texture = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\aaa.tga");
            diffuseMap = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\aaaa.tga");
            water.Technique = "ParallaxOcclusion";

            interpolador.Max = 0.9f;
            interpolador.Min = 0;
            interpolador.Speed = 1f;
            //interpolador.reset();
            
            
            ambient = 1.0f;
            diffuse = 1.0f;
            specular = 1.0f;
            specularPower = 16.0f; 
        }

        internal static void CalculateMovement(float elapsedTime, Vector3 pos)
        {
            time += elapsedTime;
            shipPos = pos;
            //alfa = interpolador.Current;
        }

        public static void Render(CubeTexture surf, TgcFrustum frustum, int r)
        {
            //Random rand = new Random();
            

            water.Effect.SetValue("rand", (int)r);
            interpolador.update();
            alfa = interpolador.Current;
       
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
            water.Effect.SetValue("amplitud", ConfigParam.Sea.getAmplitud());
            water.Effect.SetValue("frecuencia", ConfigParam.Sea.getFrecuencia());
            water.Effect.SetValue("kx", ConfigParam.Sea.getReflexion());
            water.Effect.SetValue("kc", ConfigParam.Sea.getRefraccion());
            water.Effect.SetValue("superficieAgua", texture);
            water.Effect.SetValue("texDiffuseMap", diffuseMap);
            water.Effect.SetValue("g_txCubeMap", surf);
            water.Effect.SetValue("colorAgua", ConfigParam.Sea.getColorMar().ToArgb());
            water.Effect.SetValue("shipPos", TgcParserUtils.vector3ToFloat3Array(shipPos));
            water.Effect.SetValue("fHeightMapScale", ConfigParam.Sea.getHeightmapScale());
            water.Effect.SetValue("fTexScale", ConfigParam.Sea.getTexScale());
            water.Effect.SetValue("coeficiente", interpolador.Current);
            device.RenderState.AlphaBlendEnable = true;

            water.Render(frustum);

        }
        public static void Close()
        {
            water.Dispose();
        }


    }
}
