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
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int queJoint;
        /// <summary>
        /// miKinect,  este objeto repesenta el Kinect conectado a la computadora
        /// por medio de este se accesa los datos de los diferentes streams (Video, Depth, Skeleton)
        /// </summary>
        private KinectSensor miKinect;
        int timeLeft = 60;
        DispatcherTimer timer;
        Random randomize = new Random();
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
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
        /// <summary>
        /// Esta rutina se ejececuta al cargar la ventana
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Buscamos el Kinect conectado a la computadora, mediante la propiedad KinectSensors, al descubrir el primer elemento con el estado Connected
            // lo asignamos a nuestro objeto declarado al inicio (KinectSensor miKinect)
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
                // Habilitamos el Stream de Skeleton
                this.miKinect.SkeletonStream.Enable();

                // Asignamos el event handler que se llamara cada vez que SkeletonStream tenga un frame de datos disponible 
                this.miKinect.SkeletonFrameReady += this.miKinectSkeletonFrameReady;

                // Iniciamos miKinect
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

        /// <summary>
        /// Esta rutina se ejecuta al cerrar la ventana
        /// </summary>
        /// <param name="sender">object sending the event</param>
        /// <param name="e">event arguments</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (null != this.miKinect)
            {
                this.miKinect.Stop();
            }
        }

        /// <summary>
        /// Manejador del evento SkeletonFrameReady
        /// </summary>
        private void miKinectSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //Creamos el objeto Skeleton que usaremos para recibir el frame 
            Skeleton[] skeletons = new Skeleton[0];

            //Abrimos el frame que se ha recibido y lo copiamos a nuestro objeto skeletons
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

        /// <summary>
        /// Selecciona el Joint del cual se quiere las coordenadas
        /// </summary>
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

        /// <summary>
        /// Obtiene las coordenadas del Joint seleccionado
        /// </summary>
        private void obtenerCoordenadaDeJoint(Skeleton skeleton, JointType tipoJointDeseado)
        {
            //Creamos un Joint para acceder al Joint especificado por tipoJointDeseado y obtener sus propiedades (Position, con esta podemos abtener las coordenadas)
            Joint miJoint = skeleton.Joints[tipoJointDeseado];

            // Si el Joint esta listo obtenemos sus coordenadas y pasamos a visualizalo en el Canvas
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
                        // Get Head Coordinate
                 
                        break;
                }

            }
        }
        /// <summary>
        /// Mapea un SkeletonPoint ajustandolo a las dimensiones de deseamos
        /// </summary>
        private Point SkeletonPointToScreen(SkeletonPoint posicionDeJoint)
        {
            // Convierte la posicion del Joint a "depth space".  
            // Ajustamos la posicion a una resolucion de  640x480.
            DepthImagePoint depthPoint = this.miKinect.CoordinateMapper.MapSkeletonPointToDepthPoint(posicionDeJoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(depthPoint.X, depthPoint.Y);
        }
    }
}
