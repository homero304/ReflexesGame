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
using System.Windows.Threading;
using Microsoft.Kinect;
using System.IO;

namespace ReflexesGame
{
    public partial class MainWindow : Window
    {
        int timeLeft = 60;
        DispatcherTimer timer;
        Random randomize = new Random();
        private KinectSensor miKinect;
        public MainWindow()
        {
            InitializeComponent();
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
                        //this.obtenerCoordenadaDeJoint(skeletonEncontrado, JointType.HandRight);
                        //this.obtenerCoordenadaDeJoint(skeletonEncontrado, JointType.HandLeft);
                    }

                }
            }
        }
        private void obtenerCoordenadaDeJoint(Skeleton skeleton, JointType tipoJointDeseado)
        {
            Joint miJoint = skeleton.Joints[tipoJointDeseado];

            // Si el Joint esta listo
            if (miJoint.TrackingState == JointTrackingState.Tracked)
            {
                if (JointType.HandRight == tipoJointDeseado)
                {
                    //point = this.SkeletonPointToScreen(miJoint.Position); //
                    //guanteX = point.X;

                    //guante.SetValue(Canvas.LeftProperty, guanteX);
                }
                if (JointType.HandLeft == tipoJointDeseado)
                {
                    //point = this.SkeletonPointToScreen(miJoint.Position); //
                    //guante2X = point.X;

                    //guanteIzq.SetValue(Canvas.LeftProperty, guante2X);
                }
            }
        }
        private Point SkeletonPointToScreen(SkeletonPoint posicionDeJoint)
        {
            // Convierte la posicion del Joint a "depth space".  
            // Ajustamos la posicion a una resolucion de  640x480.
            DepthImagePoint depthPoint = this.miKinect.CoordinateMapper.MapSkeletonPointToDepthPoint(posicionDeJoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
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
    }
}