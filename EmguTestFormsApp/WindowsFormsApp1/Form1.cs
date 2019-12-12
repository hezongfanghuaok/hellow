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
using Emgu.CV.Features2D;
using System.IO;
using Emgu.CV.CvEnum;
using Emgu.CV.XFeatures2D;
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
                    imagematgray = new Mat(op.FileName, ImreadModes.Grayscale);
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
            Image<Gray, byte> image0_threshold=new Image<Gray, byte>(imagetest.Width,imagetest.Height);
            CvInvoke.Threshold(imagetest,image0_threshold,0,255,ThresholdType.Otsu);
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

        private void button_Detect_Click(object sender, EventArgs e)
        {
            Mat scr = imagemat;
            Mat result = imagemat.Clone();
            #region Detect()代码
            /*
            GFTTDetector _gftd = new GFTTDetector();//以默认参数创建 GFTTDetector 类。 
            MKeyPoint[] keypoints = _gftd.Detect(scr, null);//检测关键点，返回 MKeyPoint[]。
            foreach (MKeyPoint keypoint in keypoints)//遍历 MKeyPoint[]数组。
            {
                Point point = Point.Truncate(keypoint.Point);//获得关键点的坐 标位置，以 Point 类型。
                CvInvoke.Circle(result, point, 3, new MCvScalar(0, 0, 255), 1);//绘 制关键点的位置，以 Circle 形式。
            }
            */
            #endregion
            #region DetectRaw() code
            GFTTDetector _gftd = new GFTTDetector();//以默认参数创建 GFTTDetector 类。 
            VectorOfKeyPoint vector_keypoints = new VectorOfKeyPoint();//创建 VectorOfKeyPoint 类型，存储关键点集合。
            _gftd.DetectRaw(scr, vector_keypoints, null);//检测关键点。
            foreach (MKeyPoint keypoint in vector_keypoints.ToArray())//遍历 MKeyPoint[]数组。 
            {
                Point point = Point.Truncate(keypoint.Point);//获得关键点的坐 标位置，以 Point 类型。 
                CvInvoke.Circle(result, point, 3, new MCvScalar(255, 255, 0), 1);//绘制关键点的位置，以 Circle 形式。 
            }
            #endregion
            imageBox1.Image = scr;//显示输入图像。 
            imageBox2.Image = result;//显示角点检测图像。
        }

        private void button_akaz_Click(object sender, EventArgs e)
        {
            Mat scr = imagemat;
            Mat result = imagemat.Clone();
            VectorOfKeyPoint vector_keypoints = new VectorOfKeyPoint();// 创 建 VectorOfKeyPoint 类型，存储关键点集合。
            #region akaz
            //AKAZE _akaze = new AKAZE();//以默认参数创建 AKAZE 类。            
            // _akaze.DetectRaw(scr, vector_keypoints,null);
            #endregion
            # region Brisk
           // Brisk _brisk = new Brisk(30, 1, 1f); ;//以默认参数创建 BriskE 类。
           // _brisk.DetectRaw(scr, vector_keypoints, null);//检测关键点。
            #endregion
            # region ORBDetector scalefactor影响因子 调大调小得出不同的检测数量
            ORBDetector _orbdetector = new ORBDetector(186, 1.9f, 10,30,1,8,ORBDetector.ScoreType.Harris,31,20); ;//以默认参数创建 BriskE 类。
            _orbdetector.DetectRaw(scr, vector_keypoints, null);//检测关键点。
            #endregion
            Features2DToolbox.DrawKeypoints(scr, vector_keypoints, result, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.NotDrawSinglePoints);//指定 参数 绘 制关键点
            #region 
            //相应的描述算子还有 FastDetector  MSERDetector ORBDetector
            #endregion
            imageBox1.Image = scr;//显示输入图像。 
            imageBox2.Image = result;//显示角点检测图像。
        }

        private void hisbox_button_Click(object sender, EventArgs e)//直方图相关
        {
            histogramBox1.ClearHistogram();
            histogramBox1.GenerateHistograms(imagematgray, 256);
            histogramBox1.Refresh();
            histogramBox1.Show();
            #region 计算直方图数据
            //DenseHistogram dense = new DenseHistogram(256, new RangeF(0, 255));
            //Image<Bgr, byte> image = new Image<Bgr, byte>(300, 300, new Bgr(0, 0, 0));
            //dense.Calculate(new Image<Gray, Byte>[] { imagematgray.ToImage<Gray, byte>()}, true, null);
            //float[] data = dense.GetBinValues();
            //float max = data[0];
            //for (int i = 1; i < data.Length; i++)
            //{
            //    if (data[i] > max)
            //    {
            //        max = data[i];
            //    }
            //}
            //for (int i = 0; i < data.Length; i++)
            //{
            //    data[i] = data[i] * 256 / max;
            //    image.Draw(new LineSegment2DF(new PointF(i + 20, 255), new PointF(i + 21, 255 - data[i])), new Bgr(255, 255, 255), 2);
            //}
            #endregion

            Mat dst = new Mat();
            CvInvoke.EqualizeHist(imagematgray, dst);
            imageBox1.Image = imagematgray;
            imageBox2.Image = dst;
            histogramBox2.ClearHistogram();
            histogramBox2.GenerateHistograms(dst, 256);
            histogramBox2.Refresh();
            histogramBox2.Show();
        }

        private void template_button_Click(object sender, EventArgs e)//模板匹配
        {
            Mat scr = imagemat.Clone();//待搜索图片
            Mat temp = new Mat(@"D:\github\temp.jpg", ImreadModes.AnyColor);//匹配模板
            Mat result = new Mat(new Size(scr.Width - temp.Width + 1, scr.Height - temp.Height + 1), DepthType.Cv32F, 1);//创建mat存储输出结果
            CvInvoke.MatchTemplate(scr, temp, result, Emgu.CV.CvEnum.TemplateMatchingType.SqdiffNormed);//采用匹配法，匹配值越大越掘金准确图片
            CvInvoke.Normalize(result, result, 255, 0, Emgu.CV.CvEnum.NormType.MinMax); //把数据进行以最大值 255 最小值 0 进行归一化。
            double max = 0, min = 0; //创建 double 的极值。
            Point max_loc = new Point(0, 0), min_loc = new Point(0, 0); //创建 dPoint 类型，表示极值的坐标。
            CvInvoke.MinMaxLoc(result, ref min, ref max, ref min_loc, ref max_loc); //获得极值及其坐标。
            CvInvoke.Rectangle(scr, new Rectangle(max_loc, temp.Size), new MCvScalar(0, 0, 255), 3); //绘制矩形，匹配得到的效果。
            imageBox1.Image = temp; //显示模板图片。
            imageBox2.Image = scr.ToImage<Bgr, byte>();//显示找到模板图片的带搜索图片。
                                                       //imageBox3.Image = result.ToImage<Gray, byte>(); //显示匹配结果，这边如果 imageBox3.Image = result 显示 为全黑，必须转成把类型转成 Byte。
            result = result.ToImage<Gray, byte>().Mat; //result 类型转成 Byte 类型。
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint(); //创建 VectorOfVectorOfPoint 类型保存轮廓。
            int threshold = 60; //设置阈值。
            CvInvoke.Threshold(result, result, threshold, 255, Emgu.CV.CvEnum.ThresholdType.BinaryInv); //阈值操作。
            CvInvoke.FindContours(result, contours, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple); //存储轮廓。
            for (int i = 0; i < contours.Size; i++)//遍历每个连通 域。
            {
                VectorOfPoint contour = contours[i];
                MCvMoments moment = CvInvoke.Moments(contour); //获得连通域的矩
                Point p = new Point((int)(moment.M10 / moment.M00), (int)(moment.M01 / moment.M00)); //获得连通域的中心
                CvInvoke.Rectangle(scr, new Rectangle(p, temp.Size), new MCvScalar(0, 0, 255), 4);

            }
            imageBox1.Image = temp; //显示模板图片。
            imageBox2.Image = scr.ToImage<Bgr, byte>();//显示找到模板图片的带搜索图片。
        }

        private void featuremach_button_Click(object sender, EventArgs e)
        {
            Mat scr = new Mat(@"D:\github\box.png", ImreadModes.AnyColor);//指定目录创 建输入图像。 
            Mat dst = new Mat(@"D:\github\box_in_scene.png", ImreadModes.AnyColor);//指 定目录创建输入图像。 
            ORBDetector orb = new ORBDetector();//默认方式实例化 ORBDetector 类。
            VectorOfKeyPoint scr_key_point = new VectorOfKeyPoint();//实例化一 个存储 scr 关键点的 VectorOfKeyPoint 类。 
            VectorOfKeyPoint dst_key_point = new VectorOfKeyPoint();//实例化一 个存储 dst 关键点的 VectorOfKeyPoint 类。
            orb.DetectRaw(scr, scr_key_point);//对 scr 的特征点进行检测。 
            orb.DetectRaw(dst, dst_key_point);//对 dst 的特征点进行检测。
            Features2DToolbox.DrawKeypoints(scr, scr_key_point, scr, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.Default);//绘制特征点。 
            Features2DToolbox.DrawKeypoints(dst, dst_key_point, dst, new Bgr(0, 0, 255), Features2DToolbox.KeypointDrawType.Default);//绘制特征点。
            imageBox1.Image = scr;//显示图像。 
            imageBox2.Image = dst;//显示图像。
            BriefDescriptorExtractor brief = new BriefDescriptorExtractor(); //默认参数实例化 BriefDescriptorExtractor 类。
            Mat scr_descriptor = new Mat();//实例化 Mat 存储 scr 图像检 测到的描述子。
            Mat dst_descriptor = new Mat();//实例化 Mat 存储 dst 图像检 测到的描述子。
            brief.Compute(scr, scr_key_point, scr_descriptor);// 使 用特 定参数进行 scr 描述子的提取。
            brief.Compute(dst, dst_key_point, dst_descriptor);//使用特定 参数进行 dst 描述子的提取。
            BFMatcher match = new BFMatcher(DistanceType.Hamming);//汉明距离创 建特征匹配类。
            match.Add(scr_descriptor);//添加模型描述子。
            VectorOfVectorOfDMatch matches = new VectorOfVectorOfDMatch();//创建 类，存储比较结果。
            match.KnnMatch(dst_descriptor, matches, 2, null);//进行描述子匹配。
            Mat result = new Mat();//绘制特征点和描述子输出图像。 
            Features2DToolbox.DrawMatches(scr, scr_key_point, dst, dst_key_point, matches, result, new MCvScalar(255, 255, 255), new MCvScalar(0, 0, 255)); //绘制关键点及描述子。
            imageBox1.Image = result;//显示图像。


        }
    }
    }


