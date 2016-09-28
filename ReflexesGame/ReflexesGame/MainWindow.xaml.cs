using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using System.IO;
using System.Windows.Threading;

namespace ReflexesGame
{
    public partial class MainWindow : Window
    {
        int queJoint;
        private KinectSensor miKinect;
        int timeLeft = 60;
        DispatcherTimer timer;
        Random randomize = new Random();
        double boton1X = 33;
        double boton1Y = 33;
        double boton2X = 169;
        double boton2Y = 126.5;
        double boton3X = 315;
        double boton3Y = 220;
        double boton4X = 461;
        double boton4Y = 126.5;
        double boton5X = 571;
        double boton5Y = 33;
        double boton6X = 33;
        double boton6Y = 220;
        double boton7X = 607;
        double boton7Y = 220;
        double boton8X = 169;
        double boton8Y = 313.5;
        double boton9X = 461;
        double boton9Y = 313.5;
        double boton10X = 33;
        double boton10Y = 407;
        double boton11X = 315;
        double boton11Y = 407;
        double boton12X = 607;
        double boton12Y = 407;

        public MainWindow()
        {
            InitializeComponent();
            MainCanvas.Focusable = true;
            MainCanvas.Focus();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
                timerLabel.Content = "Te quedan " + timeLeft + " segundos";
            }
            else
            {
                timer.Stop();
                timerLabel.Content = "Se acabo del tiempo!";
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (KinectSensor kinectConectado in KinectSensor.KinectSensors)
            {
                if (kinectConectado.Status == KinectStatus.Connected)
                {
                    this.miKinect = kinectConectado;
                    break;
                }
            }
            if (null != this.miKinect)
            {
                this.miKinect.SkeletonStream.Enable();
                this.miKinect.SkeletonFrameReady += this.miKinectSkeletonFrameReady;
                try
                {
                    this.miKinect.Start();
                }
                catch (IOException)
                {
                    this.miKinect = null;
                }
            }

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.miKinect)
            {
                this.miKinect.Stop();
            }
        }
        private void miKinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0)
            {
                foreach (Skeleton skeletonEncontrado in skeletons)
                {
                    if (skeletonEncontrado.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        queJoint = 1;
                        this.seleccionarJoint(skeletonEncontrado);
                        queJoint = 2;
                        this.seleccionarJoint(skeletonEncontrado);
                    }

                }
            }
        }
        private void seleccionarJoint(Skeleton skeleton)
        {
            switch (queJoint)
            {
                case 1:
                    this.obtenerCoordenadaDeJoint(skeleton, JointType.HandLeft);
                    break;
                case 2:
                    this.obtenerCoordenadaDeJoint(skeleton, JointType.HandRight);
                    break;
                default:
                    break;
            }
        }
        private void obtenerCoordenadaDeJoint(Skeleton skeleton, JointType tipoJointDeseado)
        {
            Joint miJoint = skeleton.Joints[tipoJointDeseado];
            if (miJoint.TrackingState == JointTrackingState.Tracked)
            {
                Point coordenadaJoint = this.SkeletonPointToScreen(miJoint.Position);
                double mainX = coordenadaJoint.X;
                double mainY = coordenadaJoint.Y;
                switch (queJoint)
                {
                    case 1:
                        Puntero1.SetValue(Canvas.TopProperty, mainY - 12.5);
                        Puntero1.SetValue(Canvas.LeftProperty, mainX - 12.5);
                        break;
                    case 2:
                        Puntero2.SetValue(Canvas.TopProperty, mainY - 12.5);
                        Puntero2.SetValue(Canvas.LeftProperty, mainX - 12.5);
                        break;
                    default:
                        break;
                }

            }
        }
        private Point SkeletonPointToScreen(SkeletonPoint posicionDeJoint)
        {
            DepthImagePoint depthPoint = this.miKinect.CoordinateMapper.MapSkeletonPointToDepthPoint(posicionDeJoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }

       // if(boton1X >=(double)mano.GetValue(Canvas.LeftProperty))

    }
}
