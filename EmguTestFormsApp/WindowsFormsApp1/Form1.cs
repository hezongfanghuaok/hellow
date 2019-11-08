using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using Emgu.CV.CvEnum;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Image<Bgr, byte> imagebgr;
        Image<Gray, byte> imagetest;
        Mat imagemat;
        Mat imagematgray;



        private void button2_Click(object sender, EventArgs e)
        {            
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    imagebgr = new Image<Bgr, byte>(op.FileName);
                    imagemat = new Mat(op.FileName, ImreadModes.AnyColor);
                    if (imagebgr.NumberOfChannels > 1)
                    {
                        imagetest = new Image<Gray, byte>(imagebgr.Width, imagebgr.Height);
                        CvInvoke.CvtColor(imagebgr, imagetest, ColorConversion.Bgr2Gray);
                        imageBox1.Image = imagetest;
                    }
                }
                catch
                {

                }                
            }                
        }

        private void button_turntogray_Click(object sender, EventArgs e)
        {
            if (imagebgr.NumberOfChannels > 1)
            {
                imagetest = new Image<Gray, byte>(imagebgr.Width, imagebgr.Height);
                CvInvoke.CvtColor(imagebgr, imagetest, ColorConversion.Bgr2Gray);
                imageBox1.Image = imagebgr;
            }
        }
        private void roi_Click(object sender, EventArgs e)
        {
            if (imagetest == null)
            {
                MessageBox.Show("image is null");
                return;
            }
            
            try
            {
                Mat matroi = new Mat(imagetest.Mat, new Rectangle(new Point(int.Parse(txt_x.Text), int.Parse(txt_y.Text)), new Size(int.Parse(txt_w.Text), int.Parse(txt_h.Text))));
                imagetest = matroi.ToImage<Gray, byte>();
                imageBox1.Image= imagetest;
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.StackTrace);
            }
           
        }

        private void throd_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> image0_threshold;
            image0_threshold = imagetest.ThresholdBinaryInv(new Gray(30), new Gray(255));
            imagetest = image0_threshold;
            imageBox1.Image = imagetest;
        }
        

        private void canndy_Click(object sender, EventArgs e)
        {

            //image1 = imagetest.Convert<Gray, Single>();
            //image1 = image1.Sobel(1,0,3);
            //image0_canny = image1.Convert<Gray,byte>();

            imagetest = imagetest.Canny(255,100);
            imageBox2.Image = imagetest;
        }

        private void contours_Click(object sender, EventArgs e)
        {
            Image<Bgr, byte> image_contour = new Image<Bgr, byte>(imagetest.Width, imagetest.Height, new Bgr(0, 0, 0));
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(imagetest, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
            //筛选出面积不为0的轮廓并画出
            VectorOfVectorOfPoint use_contours = new VectorOfVectorOfPoint();

            for (int i = 0; i < contours.Size; i++)
            {
                //获取独立的连通轮廓
                VectorOfPoint contour = contours[i];

                //计算连通轮廓的面积
                double area = CvInvoke.ContourArea(contour);
                //进行面积筛选
                if (area > 20)
                {
                    //添加筛选后的连通轮廓
                    use_contours.Push(contour);
                }
            }

            CvInvoke.DrawContours(image_contour, use_contours, -1, new MCvScalar(0, 255, 0), 1);
            int ksize = use_contours.Size;

            double[] m00 = new double[ksize];
            double[] m01 = new double[ksize];
            double[] m10 = new double[ksize];
            Point[] gravity = new Point[ksize];//用于存储轮廓中心点坐标
            MCvMoments[] moments = new MCvMoments[ksize];

            for (int i = 0; i < ksize; i++)
            {
                VectorOfPoint contour = use_contours[i];
                //计算当前轮廓的矩
                moments[i] = CvInvoke.Moments(contour, true);

                m00[i] = moments[i].M00;
                m01[i] = moments[i].M01;
                m10[i] = moments[i].M10;
                int x = Convert.ToInt32(m10[i] / m00[i]);//计算当前轮廓中心点坐标
                int y = Convert.ToInt32(m01[i] / m00[i]);
                gravity[i] = new Point(x, y);
                //image_contour.Draw(new CircleF(gravity[i], 2), new Bgr(0, 255, 0), 2);
                image_contour.Draw(i.ToString(), new Point(gravity[i].X - 10, gravity[i].Y + 30), Emgu.CV.CvEnum.FontFace.HersheyComplexSmall, 1, new Bgr(0, 0, 255));
                image_contour.Draw((gravity[i].X).ToString() + "," + (gravity[i].Y).ToString(), gravity[i], Emgu.CV.CvEnum.FontFace.HersheySimplex, 1, new Bgr(0, 0, 255));
            }
            imageBox1.Image = image_contour;
        }

        private void save_Click(object sender, EventArgs e)
        {
            save1();
        }
        public void save1()
        {
            Image<Gray, byte> images;
            string maindir ="照片";            
            DateTime t = DateTime.Now;
            string dir = t.ToString("yyyyMMdd");
            string path = Set_path(maindir, dir);
            MessageBox.Show(path);            
            string strDefaultFileName = t.ToString("HH-mm-ss");
            string localFilePath = path + "\\" + strDefaultFileName;
            imageBox1.Image.Save(localFilePath + "test.jpg");
          
        }
        public string Set_path(string maindir)
        {
            try
            {
                string programPath = AppDomain.CurrentDomain.BaseDirectory;
                string vol = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(':'));
                string path = vol + ":\\" + maindir + "\\";
                if (false == System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                    MessageBox.Show("创建存储文件夹"+path);

                }
                return path;
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
                return null;
            }
        }
        public string Set_path(string maindir, string subdir)
        {
            try
            {
                string programPath = AppDomain.CurrentDomain.BaseDirectory;
                string vol = programPath.Substring(0, System.Windows.Forms.Application.StartupPath.IndexOf(':'));
                string path = vol + ":\\" + maindir + "\\" + subdir + "\\";
                if (false == System.IO.Directory.Exists(path))
                {

                    System.IO.Directory.CreateDirectory(path);
                    MessageBox.Show("创建存储文件夹" + path);
                }
                return path;
            }
            catch(Exception e1)
            {
                MessageBox.Show(e1.Message);
                return null;
            }
           
        }
        private void smooth_Click(object sender, EventArgs e)
        {
            Image<Gray, Byte> imgbuff = imagetest;
           // imgbuff = imagetest;
            imgbuff = imagetest.SmoothMedian(7);
            imagetest = imgbuff;
             imageBox2.Image = imagetest;
            
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
            Image<Gray, Byte> imgbuff=new Image<Gray, byte>(imagetest.Width,imagetest.Height);            

             CvInvoke.AdaptiveThreshold(imagetest, imgbuff, 255,AdaptiveThresholdType.MeanC,ThresholdType.BinaryInv,3,0);
  

            imagetest = imgbuff;
         
             imageBox2.Image = imagetest;
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        //    try
        //    {
        //        LogHelper.WriteLog("正常日志记录122！");
        //        int a = Convert.ToInt32("lll");
        //    }
        //    catch (Exception ex)
        //    {
        //        //LogHelper.WriteLog("错误日志记录345！", ex);
        //    }

        //  //  LogManager.WriteLog("123","测试信息11111");
           
            
          
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (imagetest == null)
            {
                MessageBox.Show("image is null");
                return;
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                File.Delete(path);
            }

            using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    for (int i = 0; i < imagetest.Rows; i++)
                    {
                        sw.Write("AAAA" + i.ToString() + "   ");
                        for (int j = 0; j < imagetest.Cols; j++)
                        {
                            sw.Write(imagetest.Data[i, j, 0].ToString() + ",");
                        }
                        sw.Write("\r\n");
                    }

                }
            }
        }

        private void canny_button_Click(object sender, EventArgs e)
        {

            Image<Gray, byte> image_erode = new Image<Gray, byte>(imagetest.Width, imagetest.Height);
            Mat matkernal = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
            CvInvoke.MorphologyEx(imagetest, image_erode, Emgu.CV.CvEnum.MorphOp.Erode, matkernal, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
            imagetest = image_erode;
            imageBox2.Image = imagetest;
        }

        private void open_button_Click(object sender, EventArgs e)
        {
            Image<Gray, byte> image_open = new Image<Gray, byte>(imagetest.Width, imagetest.Height);
            Mat matkernal = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
            CvInvoke.MorphologyEx(imagetest, image_open, Emgu.CV.CvEnum.MorphOp.Open, matkernal, new Point(-1, -1), 8, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar(0, 0, 0));
            imagetest = image_open;
            imageBox2.Image = imagetest;
        }

        private void button_dft_Click(object sender, EventArgs e)
        {
            //  Image<Gray, float> image_dft= new Image<Gray, float>(imagetest.Width, imagetest.Height);
            // CvInvoke.Dft(imagetest, image_dft, DxtType.Scale, 0);
            
            // imageBox2.Image = image_dft.Convert<Gray, byte>();
            //Image image = new Image(open.FileName);
            // int b = 0;
            //IntPtr a ->  b;
            // Mat complexImage =new Mat(imagetest.Size, DepthType.Cv32F, 1);
            //CvInvoke.cvSetData(complexImage, IntPtr.Zero,2);
            // Initialize all elements to Zero CvInvoke.cvSetImageCOI(complexImage, 1); 
            // CvInvoke.cvCopy(imagetest, complexImage, IntPtr.Zero);
            // CvInvoke.cvSetImageCOI(complexImage, 0);
            Matrix<double> dft = new Matrix<double>(imagetest.Rows, imagetest.Cols, 1);
           // Mat dft = new Mat(imagetest.Rows, imagetest.Cols, DepthType.Cv64F, 1);
            CvInvoke.Dft(imagetest, dft, DxtType.Forward, 0);
            //The Real part of the Fourier Transform Matrix outReal = new Matrix(image.Size); 
            //The imaginary part of the Fourier Transform Matrix outIm = new Matrix(image.Size); 
            //CvInvoke.cvSplit(dft, outReal, outIm, IntPtr.Zero, IntPtr.Zero);
            ///Show The Data CvInvoke.cvShowImage("Real", outReal); 
            //CvInvoke.cvShowImage("Imaginary ", outIm);
        }

        private void button_CornerHarris_Click(object sender, EventArgs e)
        {
            //  imageBox1.Image = imagebgr;
            // Image<Gray, byte> image_CornerHarris = imagetest;
            Mat imtstoimat = imagetest.Mat;

            Mat cornmat = imagetest.Mat;
            //CvInvoke.CvtColor(imtstoimat, cornmat, ColorConversion.Bgr2Gray);
            //Mat scr = new Mat(@"D:\image_CornerHarris.jpg", ImreadModes.AnyColor);//加载图像图片。 
            // Mat gray_scr = new Mat(@"D:\image_CornerHarris.jpg", ImreadModes.Grayscale);//加载灰度 图像。
            ////CvInvoke.Threshold(gray_scr, gray_scr, 150, 255, ThresholdType.Otsu);//二值 化图像。
            ////CvInvoke.CornerHarris(gray_scr, gray_scr, 3);//二值化图像角点检测。 
            ////CvInvoke.Canny(gray_scr, gray_scr, 120, 150);//canny 处理。 
            ////CvInvoke.CornerHarris(gray_scr, gray_scr, 3);//轮廓角点检测。 
            //CvInvoke.CornerHarris(gray_scr, gray_scr, 3);//灰度图像角点检测 
            //CvInvoke.Normalize(gray_scr, gray_scr, 0, 255, NormType.MinMax);//进行映 射到【0，255】区域中。 
            // gray_scr = gray_scr.ToImage<Gray, byte>().Mat;//把 gray 类型转成 Byte 类型。 
            //imageBox1.Image = scr;//显示输入图像。          
            //imageBox2.Image = gray_scr;//显示角点检测图像

            CvInvoke.CornerHarris(cornmat, cornmat, 3);//灰度图像角点检测 
            CvInvoke.Normalize(cornmat, cornmat, 0, 255, NormType.MinMax);//进行映 射到【0，255】区域中。
            cornmat= cornmat.ToImage<Gray, byte>().Mat;
            imageBox1.Image = imagemat;//显示输入图像。          
            imageBox2.Image = cornmat;//显示角点检测图像
        }


    }
    }


