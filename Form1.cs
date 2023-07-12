using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;

namespace NetDeepLearning
{
    public partial class Form1 : Form
    {
        private HObject ho_Image;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void hWindowControl1_HMouseMove(object sender, HalconDotNet.HMouseEventArgs e)
        {

        }
        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Start_IA_Click(object sender, EventArgs e)
        {
        Start:
            // Initialize necessary HTuple objects.
            HTuple hv_DLSampleBatch = new HTuple(), hv_DLResultBatch = new HTuple(),
                   hv_DLModelHandle = new HTuple(), hv_DLResult = new HTuple(),
                   hv_DetectedClassIDs = new HTuple(), hv_phi_reduced = new HTuple(),
                   hv_box_row = new HTuple(), hv_box_col = new HTuple(),
                   preprocessParams = new HTuple(), hv_phi = new HTuple();

            const string modelPath = "C:\\Users\\tanguy.lebret\\Documents\\model_LR_opt.hdl";
            const string preprocessParamsPath = "C:/Users/tanguy.lebret/Documents/model_LR_opt_dl_preprocess_params.hdict";

            // Load the model
            HOperatorSet.ReadDlModel(modelPath, out hv_DLModelHandle);
            // Open the framegrabber.
            HTuple hv_AcqHandle;
            HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default",
                -1, "false", "default", "000f315c5fde_AlliedVisionTechnologies_MakoG131B5080", 0, -1, out hv_AcqHandle);
            // Set the acquisition parameters.
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "SingleFrame");

            // Get the image from the camera.
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);
            HOperatorSet.GrabImageAsync(out HObject ho_Image, hv_AcqHandle, -1);

            // Load the preprocess parameters
            HOperatorSet.ReadDict(preprocessParamsPath, "json_value_false", "true", out preprocessParams);

            // Apply the preprocess parameters
            Test_inference_ns.Test_inference_pn.preprocess_dl_model_images(ho_Image, out HObject imagesPreprocessed, preprocessParams);
            HOperatorSet.ConvertImageType(imagesPreprocessed, out HObject imageConverted, "real");

            // Generate the DLSampleBatch.
            hv_DLSampleBatch.Dispose();
            Test_inference_ns.Test_inference_pn.gen_dl_samples_from_images(imageConverted, out hv_DLSampleBatch);

            // Inference
            hv_DLResultBatch.Dispose();
            HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(), out hv_DLResultBatch);

            hv_DLResult = hv_DLResultBatch.TupleSelect(0);

            // Data processing
            HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_DetectedClassIDs);

            if (hv_DetectedClassIDs.Length != 0)
            {
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row", out hv_box_row);
                hv_box_row = hv_box_row * (4);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col", out hv_box_col);
                hv_box_col = hv_box_col * (5);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_phi", out hv_phi);
                hv_phi = hv_phi * (180 / Math.PI);

                // Round the values in the hv_phi tuple
                double[] roundedValues = hv_phi.Select(x => Math.Round(x.D, 2)).ToArray();
                HTuple hv_phi_reduced = new HTuple(roundedValues);

                HTuple hv_length1, hv_length2;
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length1", out hv_length1);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length2", out hv_length2);

                // Convert the lengths according to your image size

                HTuple HomMat2D = new HTuple(new double[] { 0.0141965, 0.218402, -495.553, -0.216524, 0.0145446, 99.0114, 0, 0, 1 });
                HTuple X, Y;
                HOperatorSet.AffineTransPoint2d(HomMat2D, hv_box_row, hv_box_col, out X, out Y);

                // Initialize the string
                StringBuilder message = new StringBuilder();


                HWindow window = hSmartWindowControl1.HalconWindow;

                // Effacer le contenu précédent de la fenêtre
                window.ClearWindow();

                // Configurer la taille de la fenêtre d'affichage avec les dimensions de l'image
                HOperatorSet.GetImageSize(imagesPreprocessed, out HTuple width, out HTuple height);
                hSmartWindowControl1.HalconWindow.SetPart(0, 0, height[0].I - 1, width[0].I - 1);

                // Afficher l'image sur laquelle les détections ont été effectuées
                window.DispObj(imagesPreprocessed);


                // Create the string such as {bboxclass.length; transformedRow[0]-transformedColumn[0];transformedRow[1]-transformedColumn[1];....}
                int maxCount = Math.Min(X.Length, 3);

                message.AppendFormat("{0};", maxCount);


                for (int i = 0; i < maxCount; i++)
                {
                    int z = 28;
                    double v = 0 * Math.Sin(hv_phi * Math.PI / 180);
                    double w = 0 * Math.Cos(hv_phi * Math.PI / 180);
                    message.Append($"{X[i].D.ToString("F2")}u{Y[i].D.ToString("F2")}u{hv_phi[i].D:F2}u{z:F2}u{v:F2}u{w:F2}");
                    if (i < maxCount - 1)
                        message.Append(";");

                }

                message.AppendLine();
                message.Replace(',', '.');
                ho_Image.Dispose();
                imagesPreprocessed.Dispose();
                imageConverted.Dispose();
                // SENDTCP(message)
                SendCommandTCP(message.ToString());

                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
                hv_DLSampleBatch.Dispose();
                hv_DLResultBatch.Dispose();
                hv_DLModelHandle.Dispose();
                hv_DLResult.Dispose();
                hv_DetectedClassIDs.Dispose();
                hv_phi_reduced.Dispose();
                hv_box_row.Dispose();
                hv_box_col.Dispose();
                preprocessParams.Dispose();
                hv_phi.Dispose();
                StartListening2();
            }
            else
            {
                // Close the framegrabber.
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);

                // Clean up Halcon objects.
                hv_DLSampleBatch.Dispose();
                hv_DLResultBatch.Dispose();
                hv_DLModelHandle.Dispose();
                hv_DLResult.Dispose();
                hv_DetectedClassIDs.Dispose();
                hv_phi_reduced.Dispose();
                hv_box_row.Dispose();
                hv_box_col.Dispose();
                preprocessParams.Dispose();
                hv_phi.Dispose();
                Thread.Sleep(6000);
                goto Start;
            }


        }


        private void SendCommandTCP(string StrCommand)
        {
            try
            {
                // Adresse IP et numéro de port du destinataire
                IPAddress ip = IPAddress.Parse("192.168.0.2");
                int port = 5016;

                // Création du socket TCP
                TcpClient client = new TcpClient();
                client.Connect(ip, port);
                Thread.Sleep(200);
                // Conversion de la chaîne de caractères en tableau de bytes
                byte[] buffer = Encoding.UTF8.GetBytes(StrCommand);
                // Envoi du message
                NetworkStream stream = client.GetStream();
                Thread.Sleep(200);
                stream.Write(buffer, 0, buffer.Length);
                Thread.Sleep(1000);
                Console.WriteLine("Message envoyé: " + StrCommand);
                // Fermeture de la connexion
                client.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur : " + ex.Message);
            }
        }


        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void label1_Click(object sender, EventArgs e)
        {

        }

        private void hSmartWindowControl1_Load_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            hSmartWindowControl1.HalconWindow.ClearWindow();

            HTuple hv_DLSampleBatch = new HTuple(),
                   hv_DLResultBatch = new HTuple(),
                   hv_DLModelHandle = new HTuple(),
                   hv_DLResult = new HTuple(),
                   hv_DetectedClassIDs = new HTuple(),
                   hv_phi_reduced = new HTuple(),
                   hv_box_row = new HTuple(),
                   hv_box_col = new HTuple(),
                   preprocessParams = new HTuple(),
                   hv_phi = new HTuple();

            const string modelPath = "C:\\Users\\tanguy.lebret\\Documents\\Piece2LR.hdl";
            const string preprocessParamsPath = "C:/Users/tanguy.lebret/Documents/Piece2LR_dl_preprocess_params.hdict";

            // Load the model
            HOperatorSet.ReadDlModel(modelPath, out hv_DLModelHandle);
            // Read the image
            // Open the framegrabber.
            HTuple hv_AcqHandle;
            HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default",
                -1, "false", "default", "000f315c5fde_AlliedVisionTechnologies_MakoG131B5080", 0, -1, out hv_AcqHandle);

            // Set the acquisition parameters.
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "SingleFrame");

            // Start the image acquisition.
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

            // Get the image from the camera.
            HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);

            // Load the preprocess parameters
            HOperatorSet.ReadDict(preprocessParamsPath, "json_value_false", "true", out preprocessParams);

            // Apply the preprocess parameters
            Test_inference_ns.Test_inference_pn.preprocess_dl_model_images(ho_Image, out HObject imagesPreprocessed, preprocessParams);
            HOperatorSet.ConvertImageType(imagesPreprocessed, out HObject imageConverted, "real");

            // Generate the DLSampleBatch.
            hv_DLSampleBatch.Dispose();
            Test_inference_ns.Test_inference_pn.gen_dl_samples_from_images(imageConverted, out hv_DLSampleBatch);

            // Inference
            //const double minConfidence = 0.9;
            //const double maxOverlap = 0.2;
            //const double maxOverlapClassAgnostic = 0.7;
            hv_DLResultBatch.Dispose();
            HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(), out hv_DLResultBatch);

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DLResult = hv_DLResultBatch.TupleSelect(0);
            }

            // Data processing
            HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_DetectedClassIDs);

            if (hv_DetectedClassIDs.Length != 0)
            {
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row", out hv_box_row);
                hv_box_row = hv_box_row * (4);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col", out hv_box_col);
                hv_box_col = hv_box_col * (4);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_phi", out hv_phi);
                hv_phi = hv_phi * (180 / Math.PI);

                // Round the values in the hv_phi tuple
                for (int i = 0; i < hv_phi.Length; i++)
                {
                    double roundedValue = Math.Round(hv_phi[i].D, 2);
                    hv_phi_reduced[i] = roundedValue;
                }

                HTuple hv_length1, hv_length2;
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length1", out hv_length1);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length2", out hv_length2);

                // Convert the lengths according to your image size

                HTuple HomMat2D = new HTuple(new double[] { 0.0141965, 0.218402, -495.553, -0.216524, 0.0145446, 99.0114, 0, 0, 1 });
                HTuple X, Y;
                HOperatorSet.AffineTransPoint2d(HomMat2D, hv_box_row, hv_box_col, out X, out Y);
                X = X - (6 * Math.Cos(hv_phi * (Math.PI / 180)));
                Y = Y - (6 * Math.Sin(hv_phi * (Math.PI / 180)));
                // Initialize the string
                StringBuilder message = new StringBuilder();


                HWindow window = hSmartWindowControl1.HalconWindow;

                // Effacer le contenu précédent de la fenêtre
                window.ClearWindow();

                // Configurer la taille de la fenêtre d'affichage avec les dimensions de l'image
                HOperatorSet.GetImageSize(imagesPreprocessed, out HTuple width, out HTuple height);
                hSmartWindowControl1.HalconWindow.SetPart(0, 0, height[0].I - 1, width[0].I - 1);

                // Afficher l'image sur laquelle les détections ont été effectuées
                window.DispObj(imagesPreprocessed);


                // Create the string such as {bboxclass.length; transformedRow[0]-transformedColumn[0];transformedRow[1]-transformedColumn[1];....}
                int maxCount = Math.Min(X.Length, 3);

                message.AppendFormat("{0};", maxCount);


                for (int i = 0; i < maxCount; i++)
                {
                    int z = 20;
                    double v = 20 * Math.Sin(hv_phi * Math.PI / 180);
                    double w = 20 * Math.Cos(hv_phi * Math.PI / 180);
                    message.Append($"{X[i].D.ToString("F2")}u{Y[i].D.ToString("F2")}u{hv_phi[i].D:F2}u{z:F2}u{v:F2}u{w:F2}");
                    if (i < maxCount - 1)
                        message.Append(";");

                }

                message.AppendLine();
                message.Replace(',', '.');

                // SENDTCP(message)
                SendCommandTCP(message.ToString());
                StartListening2();
                
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            hSmartWindowControl1.HalconWindow.ClearWindow();

            HTuple hv_DLSampleBatch = new HTuple(),
                   hv_DLResultBatch = new HTuple(),
                   hv_DLModelHandle = new HTuple(),
                   hv_DLResult = new HTuple(),
                   hv_DetectedClassIDs = new HTuple(),
                   hv_phi_reduced = new HTuple(),
                   hv_box_row = new HTuple(),
                   hv_box_col = new HTuple(),
                   preprocessParams = new HTuple(),
                   hv_phi = new HTuple();

            const string modelPath = "C:\\Users\\tanguy.lebret\\Documents\\Pièce3LR.hdl";
            const string preprocessParamsPath = "C:/Users/tanguy.lebret/Documents/Pièce3LR_dl_preprocess_params.hdict";

            // Load the model
            HOperatorSet.ReadDlModel(modelPath, out hv_DLModelHandle);
            HOperatorSet.ReadDlModel(modelPath, out hv_DLModelHandle);
            // Read the image
            // Open the framegrabber.
            HTuple hv_AcqHandle;
            HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default",
                -1, "false", "default", "000f315c5fde_AlliedVisionTechnologies_MakoG131B5080", 0, -1, out hv_AcqHandle);

            // Set the acquisition parameters.
            HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "SingleFrame");

            // Start the image acquisition.
            HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

            // Get the image from the camera.
            HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
            HOperatorSet.CloseFramegrabber(hv_AcqHandle);

            // Load the preprocess parameters
            HOperatorSet.ReadDict(preprocessParamsPath, "json_value_false", "true", out preprocessParams);

            // Apply the preprocess parameters
            Test_inference_ns.Test_inference_pn.preprocess_dl_model_images(ho_Image, out HObject imagesPreprocessed, preprocessParams);
            HOperatorSet.ConvertImageType(imagesPreprocessed, out HObject imageConverted, "real");

            // Generate the DLSampleBatch.
            hv_DLSampleBatch.Dispose();
            Test_inference_ns.Test_inference_pn.gen_dl_samples_from_images(imageConverted, out hv_DLSampleBatch);

            // Inference
            //const double minConfidence = 0.9;
            //const double maxOverlap = 0.2;
            //const double maxOverlapClassAgnostic = 0.7;
            hv_DLResultBatch.Dispose();
            HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleBatch, new HTuple(), out hv_DLResultBatch);

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_DLResult = hv_DLResultBatch.TupleSelect(0);
            }

            // Data processing
            HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_DetectedClassIDs);

            if (hv_DetectedClassIDs.Length != 0)
            {
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row", out hv_box_row);
                hv_box_row = hv_box_row * (4);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col", out hv_box_col);
                hv_box_col = hv_box_col * (5);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_phi", out hv_phi);
                hv_phi = hv_phi * (180 / Math.PI);

                // Round the values in the hv_phi tuple
                for (int i = 0; i < hv_phi.Length; i++)
                {
                    double roundedValue = Math.Round(hv_phi[i].D, 2);
                    hv_phi_reduced[i] = roundedValue;
                }

                HTuple hv_length1, hv_length2;
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length1", out hv_length1);
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_length2", out hv_length2);

                // Convert the lengths according to your image size

                HTuple HomMat2D = new HTuple(new double[] { 0.0141965, 0.218402, -495.553, -0.216524, 0.0145446, 99.0114, 0, 0, 1 });
                HTuple X, Y;
                HOperatorSet.AffineTransPoint2d(HomMat2D, hv_box_row, hv_box_col, out X, out Y);
                X = X - (9 * Math.Cos(hv_phi * (Math.PI / 180)));
                Y = Y - (9 * Math.Sin(hv_phi * (Math.PI / 180)));
                // Initialize the string
                StringBuilder message = new StringBuilder();


                HWindow window = hSmartWindowControl1.HalconWindow;

                // Effacer le contenu précédent de la fenêtre
                window.ClearWindow();

                // Configurer la taille de la fenêtre d'affichage avec les dimensions de l'image
                HOperatorSet.GetImageSize(imagesPreprocessed, out HTuple width, out HTuple height);
                hSmartWindowControl1.HalconWindow.SetPart(0, 0, height[0].I - 1, width[0].I - 1);

                // Afficher l'image sur laquelle les détections ont été effectuées
                window.DispObj(imagesPreprocessed);





                // Create the string such as {bboxclass.length; transformedRow[0]-transformedColumn[0];transformedRow[1]-transformedColumn[1];....}
                int maxCount = Math.Min(X.Length, 3);

                message.AppendFormat("{0};", maxCount);


                for (int i = 0; i < maxCount; i++)
                {
                    if (hv_DetectedClassIDs[i].I == 0)
                    {
                        int z = 29;
                        double v = -20;
                        double w = 0;
                        message.Append($"{X[i].D.ToString("F2")}u{Y[i].D.ToString("F2")}u{hv_phi[i].D:F2}u{z:F2}u{v:F2}u{w:F2}");
                    }
                    else
                    {
                        int z = 10;
                        double v = 20;
                        double w = 0;
                        message.Append($"{X[i].D.ToString("F2")}u{Y[i].D.ToString("F2")}u{hv_phi[i].D:F2}u{z:F2}u{v:F2}u{w:F2}");
                    }
                    if (i < maxCount - 1)
                        message.Append(";");

                }

                message.AppendLine();
                message.Replace(',', '.');

                // SENDTCP(message)
                SendCommandTCP(message.ToString());
                StartListening2();
            }
        }

        private void Acq_Image_Click(object sender, EventArgs e)
        {
            try
            {
                // Read the image
                // Open the framegrabber.
                HTuple hv_AcqHandle;

                HOperatorSet.OpenFramegrabber("GigEVision2", 0, 0, 0, 0, 0, 0, "progressive", -1, "default",
                    -1, "false", "default", "000f315c5fde_AlliedVisionTechnologies_MakoG131B5080", 0, -1, out hv_AcqHandle);

                // Set the acquisition parameters.
                HOperatorSet.SetFramegrabberParam(hv_AcqHandle, "AcquisitionMode", "SingleFrame");

                // Start the image acquisition.
                HOperatorSet.GrabImageStart(hv_AcqHandle, -1);

                // Get the image from the camera.
                HOperatorSet.GrabImageAsync(out ho_Image, hv_AcqHandle, -1);
                //HOperatorSet.ReadImage(out ho_Image, @"C:\\Users\\tanguy.lebret\\Documents\\Image\\Lumierenaturelle+backlight\\Normal025");
                HOperatorSet.GetImageSize(ho_Image, out HTuple width, out HTuple height);
                hSmartWindowControl1.HalconWindow.SetPart(0, 0, height[0].I - 1, width[0].I - 1);
                // Display the image
                hSmartWindowControl1.HalconWindow.DispObj(ho_Image);
                HOperatorSet.CloseFramegrabber(hv_AcqHandle);
            }
            catch (Exception ex)
            {
                MessageBox.Show("La caméra n'est pas connectée. Error halcon: " + ex.Message);
            }

        }
        public void ProcessCommand(string command)
        {
            switch (command)
            {
                case "1":
                    Start_IA_Click(null, null);
                    break;
                case "2":
                    button1_Click(null, null);
                    break;
                case "3":
                    button2_Click(null, null);
                    break;
                default:
                    MessageBox.Show("Unknown command received: " + command);
                    break;
            }
        }
        public void StartListening2()
        {
            IPAddress ip = IPAddress.Parse("192.168.0.2");
            int port = 5013;

            TcpClient client = new TcpClient();
            NetworkStream stream = null;
        Start:
            try
            {
                // Tentative de connexion
                client.Connect(ip, port);
                Console.WriteLine("Connectée");
                stream = client.GetStream();

                byte[] data = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(data, 0, client.ReceiveBufferSize);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(data, 0, bytesRead);
                    Console.WriteLine("Message reçu : " + message.Trim('\0')); // Trim les caractères null
                    message = message.Substring(0, message.Length - 1);
                    ProcessCommand(message);

                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                Console.WriteLine($"Une exception a été rencontrée: {ex.Message}");
                goto Start;
            }
        }
    }
}

