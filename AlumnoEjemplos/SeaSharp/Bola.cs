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

namespace AlumnoEjemplos.SeaSharp
{
    public class Bola
    {
        TgcMesh bola;
        float i;
        //bool press = false;

        public void Load()
        {
            Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;

            TgcSceneLoader loader = new TgcSceneLoader();
            bola = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Pelota\\Sphere-TgcScene.xml").Meshes[0];

            bola.Scale = new Vector3(1, 1, 1);
            i = 1f;
            //createTime = System.DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public void Render()
        {
            bola.render();
        }

        public void Close()
        {
            bola.dispose();
        }

        public bool CalculatePath(float elapsedTime)
        {
            //TgcD3dInput d3dInput = GuiController.Instance.D3dInput;
            float moveForward = 0f;
            bool moving = true;
            float jump = 0;

            
            bola.move(3, 3 - 5*i*elapsedTime, 3);

            
            
            
            if(bola.Position == new Vector3 (100,100,10) || i > 500)
            {
                bola.dispose();
                /*TgcSceneLoader loader = new TgcSceneLoader();
                bola = loader.loadSceneFromFile(GuiController.Instance.AlumnoEjemplosMediaDir + "Pelota\\Sphere-TgcScene.xml").Meshes[0];
                bola.Scale = new Vector3(1, 1, 1);*/
                return false;
                
            }
            else
            {
                i++;
                return true;
            }
            
            /*Vector3 movementVector = Vector3.Empty;
            if (moving)
            {
                //Aplicar movimiento, desplazarse en base a la rotacion actual del personaje
                movementVector = new Vector3(
                    FastMath.Cos(90) * moveForward,
                    elapsedTime,
                    -FastMath.Sin(90) * moveForward
                    );
            }
            //bola.move(1, createTime*elapsedTime-0.5f*FastMath.Pow2(elapsedTime), 0);
         */
            

        }
    }
}
