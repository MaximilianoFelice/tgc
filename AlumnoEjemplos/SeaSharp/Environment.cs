using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;

namespace AlumnoEjemplos.SeaSharp
{
    public static class Environment
    {
        public static List<Island> islands = new List<Island>();

        // public static int limit = 250; // Establece el limite del skybox y lo ajusta a la escala (20)

        // public static int differential = 35; // Establece la diferencia entre islas, esta en funcion de la escala de las mismas (20)

        public static void Load()
        {

            islands.Add(new Island(new Vector3(100, 0, 100)));
            islands.Add(new Island(new Vector3(-70, 0, -150)));


            /* LOGICA DEPRECIADA: Reducia mucho los FPS*/
            // TODO: Borrar cuando llegue el nuevo terrain.

            //int upper_limit = limit;
            //int side_limit = limit;

            //for (; upper_limit > (-1) * limit; upper_limit -= differential)
            //{
            //    islands.Add(new Island (new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; side_limit > (-1) * limit; side_limit -= differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; upper_limit < limit; upper_limit += differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}

            //for (; side_limit < limit; side_limit += differential)
            //{
            //    islands.Add(new Island(new Vector3(upper_limit, 0, side_limit)));
            //}
        }

        public static void Render(){

            foreach (Island island in islands) island.Render();

        }

        public static void Close()
        {
            foreach (Island island in islands) island.Close();
        }
    }
}
