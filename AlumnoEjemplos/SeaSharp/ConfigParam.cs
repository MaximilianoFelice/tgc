using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.SeaSharp
{
    public class ConfigParam
    {
        // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
        //Calcular proxima posicion de la nave segun Input
        public class Ship
        {
            public const float FORWARD = 8;
            public const float ROTATE = 200;
            public const float NITRO = 4.5f;
            public const float DESROTATION = 0.97f;
            public const float DESFORWARD = 0.99f;
        }
    }
}
