﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace ButterflyApp
{
    public sealed partial class Butterfly : UserControl
    {
        // animate
        private DispatcherTimer timer;
        // offset
        private int currentFrame = 0;
        private int direction = 1; // 1 or -1
        private int frameheight = 132;
        // location
        public double LocationX { get; set; }
        public double LocationY { get; set; }
        // speed
        private readonly double MaxSpeed = 10.0;
        private readonly double Accelerate = 0.5;
        private double speed;
        // Angle
        private double angle = 0;
        private readonly double angleStep = 8;
        public Butterfly()
        {
            this.InitializeComponent();
            // start animation
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            // frame 0,1,2,3,4
            if (direction == 1) currentFrame++;
            else currentFrame--;
            // direction
            if (currentFrame == 0 || currentFrame == 4) direction *= -1;
            // set offset
            SpriteSheetOffset.Y = currentFrame * -frameheight;
        }

        // show butterfly in right position in the canvas
        public void UpdatePosition()
        {
            SetValue(Canvas.LeftProperty, LocationX);
            SetValue(Canvas.TopProperty, LocationY);
        }

        // rotate
        public void Rotate(int angleDirection)
        {
            angle += angleDirection * angleStep; // -5 or 5
            ButterflyRotateAngle.Angle = angle;
        }

        public void Move()
        {
            // more speed
            speed += Accelerate;
            if (speed > MaxSpeed) speed = MaxSpeed;
            // update location x and y
            LocationX -= (Math.Cos(Math.PI / 180 * (angle + 90))) * speed;
            LocationY -= (Math.Sin(Math.PI / 180 * (angle + 90))) * speed;
        }

    }
}
