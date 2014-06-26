using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using TgcViewer;
using TgcViewer.Example;
using Microsoft.DirectX.Direct3D;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils.Terrain;
using TgcViewer.Utils.TgcGeometry;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;
using TgcViewer.Utils.TgcSceneLoader;
using TgcViewer.Utils.Shaders;

namespace AlumnoEjemplos.SeaSharp
{
    public static class ConfigParam
    {
        // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
        //Calcular proxima posicion de la nave segun Input
        
        public static void Load()
        {
            //Sea
            GuiController.Instance.Modifiers.addColor("ColorMar", Color.FromArgb(5, 50, 116));
            GuiController.Instance.Modifiers.addVertex3f("LightPos", new Vector3(-8000, 0, -8000), new Vector3(8000, 20000, 8000), new Vector3(6400, 1000, 2400));
            GuiController.Instance.Modifiers.addFloat("Ambient", 0f, 1f, 0.6f);
            GuiController.Instance.Modifiers.addFloat("Diffuse", 0f, 1f, 1f);
            GuiController.Instance.Modifiers.addFloat("Specular", 0f, 1f, 1f);
            GuiController.Instance.Modifiers.addFloat("SpecularPower", 0f, 50f, 16f);
            GuiController.Instance.Modifiers.addFloat("Amplitud", 0f, 300f, 150f);
            GuiController.Instance.Modifiers.addFloat("Frecuencia", 0f, 10f, 2f);
            GuiController.Instance.Modifiers.addFloat("Reflexion", 0f, 1f, 1f);
            GuiController.Instance.Modifiers.addFloat("Refraccion", 0f, 1f, 0f);


            //Ship
            GuiController.Instance.Modifiers.addFloat("Posicion", 1, 10, 5);
            

            //General
            GuiController.Instance.Modifiers.addFloat("tiempo", 1, 10, 3);
            GuiController.Instance.Modifiers.addFloat("tamanioMar", 2000, 8000, 8000);
            GuiController.Instance.Modifiers.addFloat("tamanioTriangulos", 50, 200, 75);


            //Quad
            GuiController.Instance.Modifiers.addFloat("DistLODI", 100, 10000, 2000);
            GuiController.Instance.Modifiers.addFloat("sizeLODI", 50, 400, 150);
            GuiController.Instance.Modifiers.addFloat("DistLODII", 100, 10000, 4000);
            GuiController.Instance.Modifiers.addFloat("sizeLODII", 100, 800, 300);

        }

        public static void UpdateValues()
        {
            AlumnoEjemplos.SeaSharp.QuadTree.UpdateValues();
        }
        
        #region SHIP
        public class Ship
        {
            public const float FORWARD = 0.1f;
            public const float ROTATE = 200;
            public const float NITRO = 4.5f;
            public const float DESROTATION = 0.97f;
            public const float DESFORWARD = 0.99f;

            public static float getFactorPosicion()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Posicion");
            }

            public static float getFactorTiempo()
            {
                return (float)GuiController.Instance.Modifiers.getValue("tiempo");
            }
        }
        #endregion

        #region SEA
        public class Sea
        {
            public static Color getColorMar()
            {
                return (Color)GuiController.Instance.Modifiers.getValue("ColorMar");
            }

            public static Vector3 getLightPos()
            {
                return (Vector3)GuiController.Instance.Modifiers.getValue("LightPos");
            }

            public static float getTamanioMar()
            {
                return (float)GuiController.Instance.Modifiers.getValue("tamanioMar");
            }

            public static float getTamaniotriangulos()
            {
                return (float)GuiController.Instance.Modifiers.getValue("tamanioTriangulos");
            }

            public static float getAmbient()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Ambient");
            }

            public static float getDiffuse()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Diffuse");
            }

            public static float getSpecular()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Specular");
            }

            public static float getSpecularPower()
            {
                return (float)GuiController.Instance.Modifiers.getValue("SpecularPower");
            }

            public static float getReflexion()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Reflexion");
            }

            public static float getRefraccion()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Refraccion");
            }

            public static float getAmplitud()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Amplitud");
            }

            public static float getFrecuencia()
            {
                return (float)GuiController.Instance.Modifiers.getValue("Frecuencia");
            }
        }
        #endregion

        public class QuadTree
        {
            public static float getDistLODI()
            {
                return (float)GuiController.Instance.Modifiers.getValue("DistLODI");
            }

            public static float getSizeLODI()
            {
                return (float)GuiController.Instance.Modifiers.getValue("sizeLODI");
            }

            public static float getDistLODII()
            {
                return (float)GuiController.Instance.Modifiers.getValue("DistLODII");
            }

            public static float getSizeLODII()
            {
                return (float)GuiController.Instance.Modifiers.getValue("sizeLODII");
            }

        }

    }
}
