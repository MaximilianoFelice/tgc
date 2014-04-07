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

namespace AlumnoEjemplos.TheGroup{

    public class Ship{

        public TgcMesh ship;

        public void Load()
        {

            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            TgcScene shipScene = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Scenes\\Ships\\Others\\BoatFishing-TgcScene.xml");

            ship = shipScene.Meshes[0];

        }

        public Vector3 Position()
        {
            return (ship.Position);
        }

        public void CalculateMovement(float elapsedTime)
        {

            /*
             *          ZONA DE CALCULO
             */

            // TODO: HACER UN NUEVO ALGORITMO PARA ESTO, QUE SEA MUCHO MAS DINAMICO
            //Calcular proxima posicion de la nave segun Input
            float moveForward = 0f;
            float rotate = 0;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            bool moving = false;
            bool rotating = false;
            float jump = 0;

            //Adelante
            if (d3dInput.keyDown(Key.W))
            {
                moveForward = 2;
                moving = true;
            }

            //Atras
            if (d3dInput.keyDown(Key.S))
            {
                moveForward = -2;
                moving = true;
            }

            //Derecha
            if (d3dInput.keyDown(Key.D))
            {
                rotate = -150;
                rotating = true;
            }

            //Izquierda
            if (d3dInput.keyDown(Key.A))
            {
                rotate = 150;
                rotating = true;
            }

            //Si hubo rotacion
            if (rotating)
            {
                //Rotar la nave y la camara, hay que multiplicarlo por el tiempo transcurrido para no atarse a la velocidad el hardware
                float rotAngle = Geometry.DegreeToRadian(rotate * elapsedTime);
                ship.rotateY((-1)*rotAngle);
                //GuiController.Instance.RotCamera.rotateY(rotAngle);
            }

            //Vector de movimiento
            Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Cos(ship.Rotation.Y) * moveForward,
                    jump,
                    -FastMath.Sin(ship.Rotation.Y) * moveForward
                    );
            }
            ship.move(movementVector);


        }

        public void Render()
        {
            /*
            *          ZONA DE RENDERIZADO
            */
            ship.render();
        }

        public void Close()
        {
            ship.dispose();
        }
            
    }

}
