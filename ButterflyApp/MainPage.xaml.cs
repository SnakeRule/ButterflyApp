﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ButterflyApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // butterfly
        private Butterfly butterfly;

        // flower
        private Flower flower;
        // audio
        private MediaElement mediaElement;

        // random
        private Random random = new Random();

        // canvas width and height
        private double CanvasWidth;
        private double CanvasHeight;

        // which keys are pressed
        private bool upPressed;
        private bool leftPressed;
        private bool rightPressed;

        // timer "game loop"
        private DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            // window size
            ApplicationView.PreferredLaunchWindowingMode
                = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.PreferredLaunchViewSize = new Size(800, 600);

            // get canvas size
            CanvasWidth = MyCanvas.Width;
            CanvasHeight = MyCanvas.Height;

            //add butterfly
            butterfly = new Butterfly { LocationX = CanvasWidth / 2, LocationY = CanvasHeight / 2 };
            MyCanvas.Children.Add(butterfly);
            butterfly.UpdatePosition();

            //add flower
            AddFlower();

            // load audio
            LoadAudio();

            // key listeners
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

            // initialize game loop
            timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan(0,0,0,0,1000/60); // try 60fps
            timer.Start();
        }

        // load audio from assets
        public async void LoadAudio()
        {
            mediaElement = new MediaElement();
            mediaElement.AutoPlay = false;
            StorageFolder folder =
                await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            StorageFile file =
                await folder.GetFileAsync("tada.wav");
            var stream = await file.OpenAsync(FileAccessMode.Read);
            mediaElement.SetSource(stream, file.ContentType);
        }

        // add a new flower
        public void AddFlower()
        {
            flower = new Flower
            {
                LocationX = random.Next(1, (int)CanvasWidth - 50),
                LocationY = random.Next(1, (int)CanvasHeight - 50)
            };
            // add to canvas
            MyCanvas.Children.Add(flower);
            // location
            flower.UpdatePosition();
        }

        // game loop... 60fps
        private void Timer_Tick(object sender, object e)
        {
            // move
            if (upPressed) butterfly.Move();
            // rotate
            if (leftPressed) butterfly.Rotate(-1); // -1 == left
            if (rightPressed) butterfly.Rotate(1); // 1 == right
            // collision... flower
            CheckCollision();
            // update
            butterfly.UpdatePosition();
        }

        // Check collision between flower and butterfly
        public void CheckCollision()
        {
            // get rects
            Rect r1 = new Rect(butterfly.LocationX, butterfly.LocationY, butterfly.ActualWidth, butterfly.ActualHeight);
            Rect r2 = new Rect(flower.LocationX, flower.LocationY, flower.ActualWidth, flower.ActualHeight);
            // Check for intersection
            r1.Intersect(r2);
            if (!r1.IsEmpty) // not empty -> An intersection has occurred
            {
                // play tada
                mediaElement.Play();
                // remove flower
                MyCanvas.Children.Remove(flower);
                // add a new flower
                AddFlower();
            }
        }

        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    upPressed = false;
                    Debug.WriteLine("UP released");
                    break;
                case VirtualKey.Left:
                    leftPressed = false;
                    break;
                case VirtualKey.Right:
                    rightPressed = false;
                    break;
                default:
                    break;
            }
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    upPressed = true;
                    Debug.WriteLine("UP pressed");
                    break;
                case VirtualKey.Left:
                    leftPressed = true;
                    break;
                case VirtualKey.Right:
                    rightPressed = true;
                    break;
                default:
                    break;
            }
        }
    }
}
