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
        DispatcherTimer timerPuntaje;
        Random randomize = new Random();
        
        bool hit = false;
        const int maxPoints = 300;
        int pointsCounter = 0;
        const double boton1X = 67;
        const double boton1Y = 33;
        const double boton2X = 169;
        const double boton2Y = 126.5;
        const double boton3X = 315;
        const double boton3Y = 220;
        const double boton4X = 461;
        const double boton4Y = 126.5;
        const double boton5X = 555;
        const double boton5Y = 33;
        const double boton6X = 67;
        const double boton6Y = 220;
        const double boton7X = 555;
        const double boton7Y = 220;
        const double boton8X = 169;
        const double boton8Y = 313.5;
        const double boton9X = 461;
        const double boton9Y = 313.5;
        const double boton10X = 67;
        const double boton10Y = 407;
        const double boton11X = 315;
        const double boton11Y = 407;
        const double boton12X = 555;
        const double boton12Y = 407;
        const double botonWidth = 46;
        const double botonHeight = 46;
        Point[] puntos = new Point[12] {
            new Point(boton1X, boton1Y),
            new Point(boton2X, boton2Y),
            new Point(boton3X, boton3Y),
            new Point(boton4X, boton4Y),
            new Point(boton5X, boton5Y),
            new Point(boton6X, boton6Y),
            new Point(boton7X, boton7Y),
            new Point(boton8X, boton8Y),
            new Point(boton9X, boton9Y),
            new Point(boton10X, boton10Y),
            new Point(boton11X, boton11Y),
            new Point(boton12X, boton12Y)
    };
        Image[] imagenes = new Image[12];
        Point puntoSeleccionado = new Point();
        int indiceSeleccionado = 0;


        public MainWindow()
        {
            InitializeComponent();
            MainCanvas.Focusable = true;
            MainCanvas.Focus();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            timer.Tick += new EventHandler(timer_Tick);
            timer.IsEnabled = true;
            timerPuntaje = new DispatcherTimer();
            timerPuntaje.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timerPuntaje.Tick += new EventHandler(timerPuntaje_Tick);
            timerPuntaje.IsEnabled = true;
        }
        private bool detect_collision(Point point1, Point point2)
        {
            bool retValue = (
                           (point1.X >= (point2.X - botonWidth / 2) )
                        && (point1.X <= (point2.X + botonWidth / 2))
                        && (point1.Y >= (point2.Y - botonHeight / 2))
                        && (point1.Y <= (point2.Y + botonHeight / 2))
                        );

            return retValue;
        }
        private void timerPuntaje_Tick(object sender, EventArgs e)
        {
            if (hit)
            {
                imagenes[indiceSeleccionado].Visibility = Visibility.Hidden;
                if (pointsCounter < maxPoints)
                {
                    pointsCounter += 1;
                    puntaje.Content = Convert.ToString(pointsCounter);
                    hit = false;
                    indiceSeleccionado = randomize.Next(0, 11);
                    puntoSeleccionado = puntos[indiceSeleccionado];
                    imagenes[indiceSeleccionado].Visibility = Visibility.Visible;
                }
            }
            if (timeLeft == 0)
            {
                timerPuntaje.Stop();
                imagenes[indiceSeleccionado].Visibility = Visibility.Hidden;
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft = timeLeft - 1;
               
                timerLabel.Content = timeLeft + " segundos";
            }
            else
            {
                timer.Stop();
                timerLabel.Content = "Se acabo el tiempo!";

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
            imagenes[0] = CP1;
            imagenes[1] = CP2;
            imagenes[2] = CP3;
            imagenes[3] = CP4;
            imagenes[4] = CP5;
            imagenes[5] = CP6;
            imagenes[6] = CP7;
            imagenes[7] = CP8;
            imagenes[8] = CP9;
            imagenes[9] = CP10;
            imagenes[10] = CP11;
            imagenes[11] = CP12;
            indiceSeleccionado = randomize.Next(0, 11);
            puntoSeleccionado = puntos[indiceSeleccionado];
            imagenes[indiceSeleccionado].Visibility = Visibility.Visible;
            
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
                if (detect_collision(coordenadaJoint, puntoSeleccionado) && !hit) {
                    hit = true;
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
