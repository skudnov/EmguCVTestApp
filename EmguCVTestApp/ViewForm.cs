using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;

namespace EmguCVTestApp
{
    public partial class ViewForm : Form
    {
        const int FrameWidth = 640;
        const int FrameHeight = 480;
        /// <summary>
        /// Последний захваченный кадр хранится здесь.
        /// </summary>
        Image<Bgr, byte> Frame = new Image<Bgr, byte>(FrameWidth, FrameHeight);
        /// <summary>
        /// Список доступных камер (обновляется автоматически).
        /// </summary>
        CameraList Cameras;
        /// <summary>
        /// Текущая активная камера.
        /// </summary>
        VideoCapture Camera = null;
        /// <summary>
        /// Анализатор кадров - вся логика обработки изображений находится там.
        /// </summary>
        FrameProcessor Proc = null;

        int countQR = 0;
        int countAruco = 0;

        public ViewForm()
        {
            InitializeComponent();
            Cameras = new CameraList(true);
            CameraListCB.DataSource = Cameras;
            CameraListCB.SelectedIndex = -1;
            // При изменении списка камер мы отключаемся от текущей камеры.
            Cameras.ListChanged += (sender, e) => CameraListCB.SelectedIndex = -1;
            ViewBox.Image = Frame;
            // Кадр будет захватываться, если приложению больше нечего делать.
            // Поскольку мы делаем всего лишь преьвю, нам не требуется поддерживать частоту кадров.
            Application.Idle += Camera_ImageGrab; 
        }

        private void ViewForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Отключаем камеру при закрытии формы.
            ShutdownCameraIfNeeded();
        }
        /// <summary>
        /// Захватывает кадр с камеры, если она активна, и обрабатывает его.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Camera_ImageGrab(object sender, EventArgs e)
        {
            if ((Camera != null) && Camera.Grab() && Camera.Retrieve(Frame.Mat))
            {
                FrameResult fr = Proc.ProcessFrame(Frame.Clone()); //обрабатываем кадр
                if (fr.QRLocation != null)
                {
                    lb_qr.Visible = true;
                    countQR += 1;
                    lb_qr.Text = ("QR found:  " + countQR + "times");
                }

                if (fr.ArucoLocation!= null)
                {
                    lb_aruco.Visible = true;
                    countAruco += 1;
                    lb_aruco.Text = ("Aruco found:  " + countAruco + "times");
                }


                // и просто показываем результат
                ViewBox.Image = fr.Visual;
                this.Text = "Data: "+fr.Commentary;
                TiltAngleLbl.Text = double.IsNaN(fr.TiltAngle)
                    ? "------"
                    : string.Format("Наклон {0:f2} гр", fr.TiltAngle * 180.0 / Math.PI);
            }
        }

        /// <summary>
        /// Отключаемся от текущей камеры (если она есть).
        /// </summary>
        private void ShutdownCameraIfNeeded()
        {
            if (Camera != null) // была ли до этого выбрана камера?
            {
                Camera.Dispose(); //уничтожаем объект камеры
                Camera = null;
            }
        }
        /// <summary>
        /// Выбрана камера в выпадающем списке.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraListCB_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ShutdownCameraIfNeeded(); // гасим текущую камеру, если есть
            int index = CameraListCB.SelectedIndex;
            if (index < 0) return; // Если не выбран ни один элемент, то больше ничего не делаем.
            try //пробуем подключиться к выбранной камере
            {
                Camera = new VideoCapture(index);
                Camera.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameWidth, FrameWidth);
                Camera.SetCaptureProperty(Emgu.CV.CvEnum.CapProp.FrameHeight, FrameHeight);
                Proc = new FrameProcessor();
                
            }
            catch (Exception err)
            {
                string message = string.Format("Failed to open camera {0}:\n{1}: {2}", Cameras[index].Name, err.GetType().Name, err.Message);
                MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CameraListCB.SelectedIndex = -1;
            }
        }
    }
}
