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


namespace AlumnoEjemplos.SeaSharp
{
    public static class Sea
    {

        public static QuadTree water;   // The soon-to-be-a-custom-vertex... thing.

        public static float time = 0f;
        public static float ambient, diffuse, specular, specularPower;
        public static Texture texture;
        public static Color colorMar;
        
        public static void Load()
        {

            Vector3 center = new Vector3(0, -30, 0);

            water = QuadTree.generateNewQuad(center, (int)ConfigParam.Sea.getTamanioMar(), Color.Blue, (int)ConfigParam.Sea.getTamaniotriangulos());
            water.Effect = TgcShaders.loadEffect(GuiController.Instance.AlumnoEjemplosDir + "SeaSharp\\Shaders\\SeaShader.fx");
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
            texture = TextureLoader.FromFile(d3dDevice, GuiController.Instance.AlumnoEjemplosMediaDir + "Textures\\Water\\superficieAgua.png");
            water.Technique = "RenderCubeMap";

            
            ambient = 1.0f;
            diffuse = 1.0f;
            specular = 1.0f;
            specularPower = 16.0f; 


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
            water.Effect.SetValue("g_txCubeMap", surf);
            water.Effect.SetValue("colorAgua", ConfigParam.Sea.getColorMar().ToArgb());

            device.RenderState.AlphaBlendEnable = true;

            water.Render(frustum);

        }
        public static void Close()
        {
            water.Dispose();
        }


    }
}
