using System;
using System.Collections.Generic;
using System.Text;
using TgcViewer.Example;
using TgcViewer;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using Microsoft.DirectX;
using TgcViewer.Utils.Modifiers;
using TgcViewer.Utils._2D;
using TgcViewer.Utils;

namespace AlumnoEjemplos
{
    public static class FPSCounters
    {
        public static TgcText2d FPS_Counter;

        public static TgcText2d Status;


        public static void Load()
        {
            FPS_Counter = new TgcText2d();
            FPS_Counter.Align = TgcText2d.TextAlign.LEFT;
            FPS_Counter.Position = new Point(100, 100);
            FPS_Counter.changeFont(new System.Drawing.Font("TimesNewRoman", 12, FontStyle.Bold | FontStyle.Italic));
            FPS_Counter.Size = new Size(300, 100);
            FPS_Counter.Color = Color.Gold;
            FPS_Counter.Text = "Texto inicial";


            Status = new TgcText2d();
            Status.Align = TgcText2d.TextAlign.LEFT;
            Status.Position = new Point(100, 150);
            Status.changeFont(new System.Drawing.Font("TimesNewRoman", 15, FontStyle.Bold | FontStyle.Italic));
            Status.Size = new Size(1000, 300);
            Status.Color = Color.Gold;
            Status.Text = "Texto inicial";

        }

        public static void Render(float elapsedTime)
        {

            FPS_Counter.Text = HighResolutionTimer.Instance.FramesPerSecond.ToString();

            FPS_Counter.render();
            Status.render();

        }

        public static void Close()
        {
            FPS_Counter.dispose();
        }
    }
}
