using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using System.Drawing;
using TgcViewer;
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
    public class ConfigParam
    {
        // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
        //Calcular proxima posicion de la nave segun Input
        
        public static void Load()
        {
            GuiController.Instance.Modifiers.addColor("ColorMar", Color.Aquamarine);
            GuiController.Instance.Modifiers.addVertex3f("LightPos", new Vector3(-8000, 0, -8000), new Vector3(8000, 10000, 8000), new Vector3(6400, 1000, 2400));
        }
        
        public class Ship
        {
            public const float FORWARD = 8;
            public const float ROTATE = 200;
            public const float NITRO = 4.5f;
            public const float DESROTATION = 0.97f;
            public const float DESFORWARD = 0.99f;
        }

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
        }

    }
}
