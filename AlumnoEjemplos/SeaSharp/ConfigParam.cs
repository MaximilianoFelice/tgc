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
        static float _PreviousSeaDetail = 0;

        public static bool cargado = false;
        public static void Load()
        {
            //Sea
            //GuiController.Instance.Modifiers.addColor("ColorMar",  ConfigParam.colorMar = Color.FromArgb(5, 50, 116));
            GuiController.Instance.Modifiers.addVertex3f("LightPos", new Vector3(-8000, 0, -8000), new Vector3(8000, 20000, 8000), new Vector3(0, 10000, 0));
            //GuiController.Instance.Modifiers.addFloat("Ambient", 0f, 1f, 0.6f);
            //GuiController.Instance.Modifiers.addFloat("Diffuse", 0f, 1f, 1f);
            //GuiController.Instance.Modifiers.addFloat("Specular", 0f, 1f, 1f);
            //GuiController.Instance.Modifiers.addFloat("SpecularPower", 0f, 150f, 50f);
            GuiController.Instance.Modifiers.addFloat("Amplitud", 0f, 300f, 150f);
            GuiController.Instance.Modifiers.addFloat("Frecuencia", 0f, 10f, 2f);
            //GuiController.Instance.Modifiers.addFloat("Reflexion", 0f, 1f, 1f);
            GuiController.Instance.Modifiers.addFloat("Refraccion", 0f, 1f, 0f);
            GuiController.Instance.Modifiers.addFloat("HeightMapScale", 0f, 20f, 1f);
            GuiController.Instance.Modifiers.addFloat("TexScale", 0f, 200f, 20f);


            //Ship
            GuiController.Instance.Modifiers.addFloat("Posicion", 1, 10, 5);
            GuiController.Instance.Modifiers.addBoolean("Normal", "Dibuja normal del barco", false);
            

            //General
            GuiController.Instance.Modifiers.addFloat("tiempo", 1, 10, 3);
            GuiController.Instance.Modifiers.addFloat("tamanioMar", 2000, 8000, 8000);
            GuiController.Instance.Modifiers.addFloat("tamanioTriangulos", 50, 200, 150);


            //Quad
            GuiController.Instance.Modifiers.addFloat("DistLODI", 100, 10000, 2000);
            GuiController.Instance.Modifiers.addFloat("sizeLODI", 50, 400, 150);
            GuiController.Instance.Modifiers.addFloat("DistLODII", 100, 10000, 4000);
            GuiController.Instance.Modifiers.addFloat("sizeLODII", 100, 800, 300);

            //Lluvia
            GuiController.Instance.Modifiers.addBoolean("Tormenta", "Activa lluvia y relampagos",false);

            //Environment
            GuiController.Instance.Modifiers.addBoolean("Terreno", "Activa terreno", false);

        }

        public static void UpdateValues()
        {
            AlumnoEjemplos.SeaSharp.QuadTree.UpdateValues();

            if (_PreviousSeaDetail != Sea.getTamaniotriangulos())
            {
                AlumnoEjemplos.SeaSharp.Sea.Close();
                FPSCounters.Status.Text = "LOADING... PLEASE WAIT   =)";
                AlumnoEjemplos.SeaSharp.Sea.Load();
                _PreviousSeaDetail = Sea.getTamaniotriangulos();
            }
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

            public static bool getNormal()
            {
                return (bool)GuiController.Instance.Modifiers.getValue("Normal");
            }
        }
        #endregion

        #region SEA
        public class Sea
        {
            public static Color getColorMar()
            {
                if (cargado)
                {
                    return (Color)GuiController.Instance.Modifiers.getValue("ColorMar");
                }
                else
                {
                    return Color.AliceBlue;
                }
            }

            public static Color getDiffuseColor()
            {
                if (cargado)
                {
                    return (Color)GuiController.Instance.Modifiers.getValue("diffuseColor");
                }
                else
                {
                    return Color.AliceBlue;
                }
            }

            public static Color getSpecularColor()
            {
                if (cargado)
                {
                    return (Color)GuiController.Instance.Modifiers.getValue("specularColor");
                }
                else
                {
                    return Color.AliceBlue;
                }
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
                if (cargado)
                {
                    return (float)GuiController.Instance.Modifiers.getValue("Ambient");
                }
                else
                {
                    return 1;
                }
            }

            public static float getDiffuse()
            {
                if (cargado)
                {
                    return (float)GuiController.Instance.Modifiers.getValue("Diffuse");
                }
                else
                {
                    return 1;
                }
            }

            public static float getSpecular()
            {
                if (cargado)
                {
                    return (float)GuiController.Instance.Modifiers.getValue("Specular");
                }
                else
                {
                    return 1;
                }
            }

            public static float getSpecularPower()
            {
                if (cargado)
                {
                    return (float)GuiController.Instance.Modifiers.getValue("SpecularPower");
                }
                else
                {
                    return 1;
                }
            }

            public static float getReflexion()
            {
                if (cargado)
                {
                    return (float)GuiController.Instance.Modifiers.getValue("Reflexion");
                }
                else
                {
                    return 1;
                }
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

            public static float getHeightmapScale()
            {
                return (float)GuiController.Instance.Modifiers.getValue("HeightMapScale");
            }

            public static float getTexScale()
            {
                return (float)GuiController.Instance.Modifiers.getValue("TexScale");
            }

            public static void setWater(Color color, float ambient, float diffuse, float specular, float sPower, float reflexion, Color diffuseColor, Color specularColor)
            {
                GuiController.Instance.Modifiers.addColor("ColorMar", color);
                GuiController.Instance.Modifiers.addFloat("Ambient", 0f, 1f, ambient);
                GuiController.Instance.Modifiers.addFloat("Diffuse", 0f, 1f, diffuse);
                GuiController.Instance.Modifiers.addFloat("Specular", 0f, 1f, specular);
                GuiController.Instance.Modifiers.addFloat("SpecularPower", 0f, 150f, sPower);
                GuiController.Instance.Modifiers.addFloat("Reflexion", 0f, 1f, reflexion);
                GuiController.Instance.Modifiers.addColor("diffuseColor", diffuseColor);
                GuiController.Instance.Modifiers.addColor("specularColor", specularColor);
                cargado = true;
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

        public class Lluvia
        {
            public static Boolean getLluvia()
            {
                return (Boolean)GuiController.Instance.Modifiers.getValue("Tormenta");
            }
        }

        public class Environment
        {
            public static Boolean getEnvironment()
            {
                return (Boolean)GuiController.Instance.Modifiers.getValue("Terreno");
            }
        }

    }
}
