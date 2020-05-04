using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace opencvsharpConsoleApp
{
    class Program
    {

        static void Main(string[] args)
        {
            #region
            Mat[] splitall;
            Mat src;
            Mat channel_depth = new Mat();
            Mat channel_gray = new Mat();
            Mat channel_three = new Mat();
            Mat element3 = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 1));
            Mat element5 = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));

            FileStorage cld_date = new FileStorage("0924.yaml", FileStorage.Mode.Read);//101161kk.yaml
            using (var fs = new FileStorage("0924.yaml", FileStorage.Mode.Read))//0924.yaml
            {
                src = fs["vocabulary"].ReadMat();
            }
            Cv2.Split(src, out splitall);

            splitall[2].ConvertTo(channel_depth, MatType.CV_32FC1);
            //var window1 = new Window("depth",channel_depth);
            //Cv2.WaitKey();

            splitall[3].ConvertTo(channel_gray, MatType.CV_8UC1);
            //using (var window = new Window("原始图", WindowMode.Normal, channel_gray))
            //{
            //    Cv2.WaitKey();
            //}


            int imgcols = channel_depth.Cols, imgrows = channel_depth.Rows;

            Mat model_calc_gray = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);
            Mat model_gray = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_8UC1);
            Mat model_step1 = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);

            for (int i = 0; i < channel_depth.Rows; i++)
            {
                for (int j = 0; j < channel_depth.Cols; j++)
                {
                    if (channel_depth.At<float>(i, j) < 900)//900时为临界 ==》 0943
                    {
                        model_calc_gray.Set<float>(i, j, channel_gray.At<Byte>(i, j));//= channel_gray.At<short>(i, j);//char convert to float that could calcaulate
                    }//方便后面sigmoid计算
                    else
                        continue;
                }
            }
            Mat Edge_one = model_calc_gray.Clone();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < model_calc_gray.Cols; j++)
                {
                    Edge_one.Set<float>(i, j, 0);
                }
            }


            //取反
            Mat Edge = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);
            Edge = new Scalar(255) - Edge_one;//

            int zero_cout = Cv2.CountNonZero(Edge);//返回矩阵中的非零值个数
            Scalar zero_sum = Cv2.Sum(Edge);//对mat类四个通道求和
            float matMean = (float)(zero_sum[0] / zero_cout);//对非0像素求均值
            float angle = 0.2f;

            for (int i = 0; i < imgrows; i++)
            {
                for (int j = 0; j < imgcols; j++)
                {
                    if (Edge.At<float>(i, j) != 0)
                    {

                        model_step1.Set<float>(i, j, sigmod(Edge.At<float>(i, j), matMean, angle));
                    }
                }
            }

            Mat show_change_two = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_8UC1);
            Cv2.Normalize(model_step1, show_change_two, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            using (var window = new Window("转换展示图", WindowMode.Normal, show_change_two))
            {
                Cv2.WaitKey();
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Close, element5, new Point(-1, -1), 10);
            using (var window = new Window("5次模糊", WindowMode.Normal, show_change_two))
            {
                Cv2.WaitKey();
            }
            

            Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Dilate, element5, new Point(-1, -1), 8);
            using (var window = new Window("5次模糊", WindowMode.Normal, show_change_two))
            {
                Cv2.WaitKey();
            }
            #region 注释代码
            //Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Erode, element5, new Point(-1, -1), 2);
            //using (var window = new Window("5次模糊", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            //Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Open, element5, new Point(-1, -1), 10);
            //using (var window = new Window("5次模糊", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            
            //for (int num1 = 0; num1 < 5; num1++)
            //    //Cv2.MedianBlur(show_change_two, show_change_two, 5);
            // Cv2.GaussianBlur(show_change_two, show_change_two, new Size(3, 1), MatType.CV_8UC1);
            //using (var window = new Window("10次中值", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            //Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Close, element5, new Point(-1, -1), 3);
            //using (var window = new Window("5比原算", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            //for (int num1 = 0; num1 < 10; num1++)
            //    Cv2.GaussianBlur(show_change_two, show_change_two, new Size(1, 3), MatType.CV_8UC1);
            //using (var window = new Window("5次模糊", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            //Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Dilate, element3, new Point(-1, -1), 3);
            ////调试结果较好，二值化之前都不要对图像进行滤波，会丧失边界
            //Cv2.Dilate(show_change_two, show_change_two, element3, new Point(-1, -1), 5);
            //using (var window = new Window("5膨胀", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            // Cv2.Dilate(show_change_two, show_change_two, element5, new Point(-1, -1), 5);
            //using (var window = new Window("5次腐蚀", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}

            // for (int num1 = 0; num1 < 10; num1++)
            //     Cv2.MedianBlur(show_change_two, show_change_two, 5);
            //// Cv2.GaussianBlur(show_change_two, show_change_two, new Size(1, 3), MatType.CV_8UC1);
            // using (var window = new Window("10次中值", WindowMode.Normal, show_change_two))
            // {
            //     Cv2.WaitKey();
            // }
            // int a = 0;


            //Cv2.Threshold(show_change_two, show_change_two, 0, 255,ThresholdTypes.Otsu);
            //PixelConnectivity pixelConnectivity =new PixelConnectivity();

            //Cv2.ConnectedComponents(show_change_two, show_change_two, 4);
            //////Cv2.MedianBlur(show_change_two, show_change_two, 5);
            //using (var window = new Window("结果二值", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}

            //for (int num1 = 0; num1 < 10; num1++)
            //    Cv2.MedianBlur(show_change_two, show_change_two, 5);
            //// Cv2.GaussianBlur(show_change_two, show_change_two, new Size(1, 3), MatType.CV_8UC1);
            //using (var window = new Window("10次中值", WindowMode.Normal, show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
            //int a = 0;
            #endregion
            #endregion
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            Mat Sobel_Edge = new Mat();
            Mat Sobel_result = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            Cv2.Sobel(show_change_two, Sobel_Edge, MatType.CV_16SC1, 4, 0, 5, 1, 0, BorderTypes.Default);
            Cv2.ConvertScaleAbs(Sobel_Edge, Sobel_result);
            Cv2.Threshold(Sobel_result, Sobel_result, 20, 255, ThresholdTypes.Otsu);
            using (var window = new Window("sobel结果二值", WindowMode.Normal, Sobel_result))
            {
                Cv2.WaitKey();
            }

           

            //for (int num1 = 0; num1 < 3; num1++)
            ////Cv2.MedianBlur(Sobel_result, Sobel_result, 3);
            //  Cv2.GaussianBlur(Sobel_result, Sobel_result, new Size(1, 3), MatType.CV_8UC1);
            //using (var window = new Window("10次模糊", WindowMode.Normal, Sobel_result))
            //{
            //    Cv2.WaitKey();
            //}

            //Mat img_step2 = new Mat();
            //Cv2.MorphologyEx(Sobel_result, Sobel_result, MorphTypes.Open, element5, new Point(-1, -1), 1);
            //using (var window = new Window("sobel5后膨胀", WindowMode.Normal, Sobel_result))
            //{
            //    Cv2.WaitKey();
            //}

            Mat result = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            Point[][] contours_one;
            HierarchyIndex[] hierarchy_one;
            Cv2.FindContours(Sobel_result.Clone(), out contours_one,out hierarchy_one, RetrievalModes.External,ContourApproximationModes.ApproxSimple , new Point(0, 0));

     
            List<Point[]> afterFilter=new List<Point[]>();
            Console.WriteLine( "轮廓数量"+contours_one.Length);
            for (int c = 0; c < contours_one.Length; c++)
            {
                Console.WriteLine("轮廓"+c+"长度"+contours_one[c].Length);
            }

            //vector<vector<Point>>::iterator itc = contours_one.begin();
            for (int c = 0; c < contours_one.Length; c++)
            {
                double area = Cv2.ContourArea(contours_one[c]);
                
                Console.WriteLine(area);
                if (area > 800)
                    afterFilter.Add(contours_one[c]);
            }

            Cv2.DrawContours(result, afterFilter, -1, new Scalar(255), -1);          
            using (var window = new Window("去除小面积结果图", WindowMode.Normal, result))
            {
                Cv2.WaitKey();
            }


            ////for (int num = 0; num < 5; num++)
            ////    Cv2.Dilate(result, result, element3);
            //using (var window = new Window("连接下面的部分5次膨胀结果图", WindowMode.Normal, result))
            //{
            //    Cv2.WaitKey();
            //}

            Cv2.MorphologyEx(result, result, MorphTypes.Close, element3, new Point(-1, -1), 5);
            using (var window = new Window("闭运算再次迭代10次结果图", result))
            {
                Cv2.WaitKey();
            }


            Mat result1 = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            Point[][] contours_one1;
            HierarchyIndex[] hierarchy_one1;
            Cv2.FindContours(result.Clone(), out contours_one1, out hierarchy_one1, RetrievalModes.External, ContourApproximationModes.ApproxSimple, new Point(0, 0));
            List<Point[]> afterFilter1 = new List<Point[]>();
            Console.WriteLine(contours_one1.Length);
            //vector<vector<Point>>::iterator itc = contours_one.begin();
            for (int c = 0; c < contours_one1.Length; c++)
            {
                double area = Cv2.ContourArea(contours_one1[c]);
                Console.WriteLine(area);
                if (area > 3000)
                    afterFilter1.Add(contours_one1[c]);
            }
            Cv2.DrawContours(result1, afterFilter1, -1, new Scalar(255), -1);
            using (var window = new Window("再次去除小面积结果图", WindowMode.Normal, result1))
            {
                Cv2.WaitKey();
            }


            Mat result_uchar = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            result1.ConvertTo(result_uchar, MatType.CV_8UC1);
            Point[][] contours_three;
            HierarchyIndex[] hierarchy_three;
            Cv2.FindContours(result_uchar.Clone(), out contours_three, out hierarchy_three, RetrievalModes.External, ContourApproximationModes.ApproxSimple, new Point(0, 0));
            Mat rectangle_one = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC3);
            Rect[] boundRect_one=new Rect[contours_three.Length];  //定义外接矩形集合
            RotatedRect[] box_one=new RotatedRect[ contours_three.Length];
            Point2f[] rect_one=new Point2f[4];
            Console.WriteLine("最终边界数量："+contours_three.Length);

            List<Point2f[]> rec_vec = new List<Point2f[]>(contours_three.Length);
            float[] center_one_x=new float[contours_three.Length];
            float[] center_one_y = new float[contours_three.Length];
            for (int i = 0; i < contours_three.Length; i++)
            {
                box_one[i] = Cv2.MinAreaRect(contours_three[i]);  //计算外接旋转矩形
                boundRect_one[i] =Cv2.BoundingRect(contours_three[i]);//计算每个轮廓最小外接矩形
                Cv2.Circle (rectangle_one,new Point( box_one[i].Center.X, box_one[i].Center.Y), 5, new Scalar(0, 255, 0), -1);  //绘制最旋转矩形的中心点
                rect_one=box_one[i].Points(); //把最小外接矩形四个端点复制给rect数组  复制构造
                Cv2.Rectangle(rectangle_one,boundRect_one[i], new Scalar(0, 255, 0),5);//画最小外接矩形
                center_one_x[i] = box_one[i].Center.X;
                center_one_y[i] = box_one[i].Center.Y;
                //cout << "end" <<center_one.size() << endl;
                for (int j = 0; j < 4; j++)
                {
                    Cv2.Line(rectangle_one, (Point) rect_one[j], (Point)rect_one[(j + 1) % 4], new Scalar(0, 0, 255),2);  //绘制旋转矩形每条边
                   // rec_vec[i].push_back(rect_one[j]);          /*cout << "第"<<j<<"个角点"<< rect[j] << endl;*/
                }
                using (var window = new Window("绘制最小外接矩形结果图", WindowMode.Normal, rectangle_one))
                {
                    Cv2.WaitKey();
                }



            }
           
            int[] ind= new int[center_one_x.Length];
            BubbleSort(center_one_x, ind);
            for (int i = 0; i < contours_three.Length - 1; i++)
            {
               // cout << "ind" << ind[i] << endl;
                Point point_one;
                point_one.X = boundRect_one[ind[i]].X + boundRect_one[ind[i]].Width/2;
                point_one.Y = boundRect_one[ind[i]].Y;
                Point point_two;
                point_two.X = boundRect_one[ind[i + 1]].X + boundRect_one[ind[i + 1]].Width /2;
                point_two.Y = boundRect_one[ind[i + 1]].Y + boundRect_one[ind[i + 1]].Height;
                Point point_three;
                point_three = point_two - point_one;
                point_three.X = Math.Abs(point_three.X);
                point_three.Y = Math.Abs(point_three.Y);
                Rect rect=new Rect(point_one.X, point_one.Y, point_three.X, point_three.Y);
                Mat capture_one = channel_gray[rect];
                //imshow("截图第一幅图结果图", capture_one);
                using (var window = new Window("截图第一幅图结果图", WindowMode.Normal, capture_one))
                {
                    Cv2.WaitKey();
                }
                Point point_four;
                point_four.X = point_one.X + point_three.X /2;
                point_four.Y = point_one.Y + 100;
                Cv2.Circle(channel_gray, point_four, 9,new Scalar(0, 0, 255));
                Console.WriteLine("贴标X："+splitall[0].At<float>(point_four.Y, point_four.X));
                Console.WriteLine("贴标Y：" + splitall[1].At<float>(point_four.Y, point_four.X));
                Console.WriteLine("贴标Z：" + splitall[2].At<float>(point_four.Y, point_four.X));
                using (var window = new Window("截图第一幅圈圈", WindowMode.Normal, channel_gray))
                {
                    Cv2.WaitKey();

                }

                Point point_five;
                point_five.X = point_one.X + point_three.Y /2;
                point_five.Y = point_one.Y + 200;
                Cv2.Circle(channel_gray, point_five, 9, new Scalar(0, 0, 255));
                Console.WriteLine("喷码X：" + splitall[0].At<float>(point_five.Y, point_five.X));
                Console.WriteLine("喷码Y：" + splitall[1].At<float>(point_five.Y, point_five.X));
                Console.WriteLine("喷码Z：" + splitall[2].At<float>(point_five.Y, point_five.X));
                using (var window = new Window("截图第二幅圈圈", WindowMode.Normal, channel_gray))
                {
                    Cv2.WaitKey();
                }
            }

        }


        static void BubbleSort(float[] p, int[] ind_diff)
        {
            int length = p.Length;
            for (int m = 0; m < length; m++)
            {
                ind_diff[m]=m;
            }

            for (int j = 0; j < length; j++)
            {
                Console.WriteLine("ind_diff:"+ ind_diff[j].ToString());
            }
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length - i - 1; j++)
                {
                    if (p[j] > p[j + 1])
                    {
                        float temp = p[j];
                        p[j] = p[j + 1];
                        p[j + 1] = temp;

                        int ind_temp = ind_diff[j];
                        ind_diff[j] = ind_diff[j + 1];
                        ind_diff[j + 1] = ind_temp;
                    }
                }
            }
        }
            public static void jiaodian(Mat result)
        {
            Mat feature_test = new Mat();
            result.ConvertTo(feature_test, MatType.CV_8UC1);
            Mat feature_result = Mat.Zeros(result.Width, result.Height, MatType.CV_8UC3);
            Point2f[] cornersPoint = Cv2.GoodFeaturesToTrack(result, 30, 0.1, 30, new Mat(), 3, false, 0.04);
            foreach (var item in cornersPoint)
            {
                Cv2.Circle(result, Convert.ToInt16(item.X), Convert.ToInt16(item.Y), 10, Scalar.Wheat, 2);
                using (var window = new Window("feature结果图", WindowMode.Normal, result))
                {
                    Cv2.WaitKey();
                }
                Console.WriteLine(item);
            }
        }
        static void drawhis(Mat src0, string windowname)
        {
            Mat[] src = { src0 };
            Mat dstHist=new Mat();       // 在cv中用CvHistogram *hist = cvCreateHist
            int dims = 1;
            Rangef[] ranges = { new Rangef(0,256)};   // 这里需要为const类型

            int[] histsize = { 256 };//每个维度的直方图尺寸
            int[] channels = {0};
            Mat mask = new Mat();
            //【3】计算图像的直方图
            Cv2.CalcHist(src,channels, mask,dstHist, dims, histsize, ranges);    // cv 中是cvCalcHist
            int scale = 8;

            Mat dstImage=new Mat(histsize[0] * scale, 256,MatType.CV_8U, new Scalar(0));
           // Cv2.Normalize(dstHist, dstHist, 0, 255, NormTypes.MinMax);

        

            //【5】绘制出直方图
            //int hpt = saturate_cast<int>(0.9 * size);
            for (int i = 0; i < histsize[0]; i++)
            {
                float binValue = dstHist.At<float>(i); //   注意hist中是float类型 
                Cv2.Rectangle(dstImage, new Point(i * scale, 255 - binValue), new Point((i + 1) * scale - 1, 255) , new Scalar(255));
            }
            using (var window = new Window("直方图", WindowMode.Normal, dstImage))
            {
                Cv2.WaitKey();
            }

        }
        static void fillHole(Mat srcBw, Mat dstBw)
        {
            Size m_Size = srcBw.Size();
            Mat Temp = Mat.Zeros(m_Size.Height + 10, m_Size.Width + 10, srcBw.Type());//延展图像
            Rect rect1 = new Rect(5, 5, m_Size.Width, m_Size.Height);
            srcBw.CopyTo(Temp[rect1]);
            using (var window = new Window("扩展图", WindowMode.Normal, Temp))
            {
                Cv2.WaitKey();
            }
            Cv2.FloodFill(Temp, new Point(0, 0), new Scalar(255));

            Mat cutImg=new Mat();//裁剪延展的图像
            Temp[new Range(1, m_Size.Height + 1), new Range(1, m_Size.Width + 1)].CopyTo(cutImg);

            dstBw = srcBw | (~cutImg);
        }
        public static float sigmod(float x, float mid, float a)
        {
            return (float)(1 / (1 +Math.Exp((a * (mid - x)))));
        }
    }
}
