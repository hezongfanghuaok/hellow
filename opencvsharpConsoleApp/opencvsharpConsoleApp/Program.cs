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
            Mat[] splitall;
            Mat src;
            Mat channel_depth=new Mat();
            Mat channel_gray = new Mat();
            Mat channel_three = new Mat();
            Mat element3 = Cv2.GetStructuringElement(MorphShapes.Rect,new Size(3, 3));
            Mat element5 = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(5, 5));

            FileStorage cld_date = new FileStorage("0924.yaml", FileStorage.Mode.Read);//101161kk.yaml
            using (var fs = new FileStorage("0924.yaml", FileStorage.Mode.Read))//0924.yaml
            {           
                src = fs["vocabulary"].ReadMat();
                //using (var window = new Window("原始图像",src))
                //{
                //    Cv2.WaitKey();
                //}
            }
            Cv2.Split(src, out splitall);

            splitall[2].ConvertTo(channel_depth,MatType.CV_32FC1);
            //var window1 = new Window("depth",channel_depth);
            //Cv2.WaitKey();

            splitall[3].ConvertTo(channel_gray, MatType.CV_8UC1);
            //var window2 = new Window("gray",channel_gray);
            //Cv2.WaitKey();

            int imgcols = channel_depth.Cols,imgrows=channel_depth.Rows;
            
            Mat model_calc_gray = Mat.Zeros(channel_depth.Rows,channel_depth.Cols, MatType.CV_32FC1);
            Mat model_gray = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_8UC1);
            Mat model_step1 = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);

            for (int i = 0; i < channel_depth.Rows; i++)
            {
                for (int j = 0; j < channel_depth.Cols; j++)
                {
                    Vec3f a = channel_depth.At<Vec3f>(i, j);
                    float a1 = a.Item0;
                    float a2 = a.Item1;
                    float a3 = a.Item2;
                    float b = channel_depth.At<float>(i, j);
                    if (channel_depth.At<float>(i,j) < 900)//900时为临界 ==》 0943
                    {
                        int aa=channel_gray.At<byte>(i, j);
                        model_calc_gray.Set<float>(i, j, channel_gray.At<byte>(i, j));//= channel_gray.At<short>(i, j);//char convert to float that could calcaulate
                    }//方便后面sigmoid计算
                    else
                        continue;
                }
            }          
            //Window windowexcpetlow = new Window(model_calc_gray);
            //Cv2.WaitKey();

            Mat Edge_one = model_calc_gray.Clone();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < model_calc_gray.Cols; j++)
                {
                    Edge_one.Set<float>(i, j,0) ;
                }
            }
            //using (var window = new Window("edge img", Edge_one))
            //{
            //    Cv2.WaitKey();
            //}
            

            //取反
            new Scalar(255);
            Mat Edge = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);
            Edge = new Scalar(255)- Edge_one;//
            //using (var window = new Window("revser img", Edge))
            //{
            //    Cv2.WaitKey();
            //}

            int zero_cout = Cv2.CountNonZero(Edge);//返回矩阵中的非零值个数
            Scalar zero_sum = Cv2.Sum(Edge);//对mat类四个通道求和
            float matMean =(float)( zero_sum[0] / zero_cout);//对非0像素求均值
            float angle = 0.2f;
            for (int i = 0; i < imgcols; i++)
            {
                for (int j = 0; j < imgrows; j++)
                {
                    if (Edge.At<float>(i,j)!= 0)
                    {
                        model_step1.Set<float>(i, j, sigmod(Edge.At<float>(i, j), matMean, angle));
                    }
                }
            }
            Mat show_change_two = new Mat();
            Cv2.Normalize(model_step1, show_change_two, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            using (var window = new Window("转换展示图", show_change_two))
            {
                Cv2.WaitKey();
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            for (int num1 = 0; num1 < 5; num1++)
                //Cv2.GaussianBlur(show_change_two, show_change_two, new Size(1, 3), 0, 0);
                Cv2.BoxFilter(show_change_two, show_change_two, MatType.CV_8UC1, new Size(1, 3));
            using (var window = new Window("原始图3次模糊", show_change_two))
            {
                Cv2.WaitKey();
            }

            for (int num = 0; num <5; num++)
                Cv2.Dilate(show_change_two, show_change_two, element3);
            using (var window = new Window("二值化前5次膨胀", show_change_two))
            {
                Cv2.WaitKey();
            }

                for (int num1 = 0; num1 < 5; num1++)
                    //Cv2.GaussianBlur(show_change_two, show_change_two, new Size(1, 3), 0, 0);
                    Cv2.BoxFilter(show_change_two, show_change_two, MatType.CV_8UC1, new Size(1, 5));
                using (var window = new Window("5次模糊", show_change_two))
                {
                    Cv2.WaitKey();
                }

                Cv2.Threshold(show_change_two, show_change_two, 20, 255, ThresholdTypes.Binary);
                using (var window = new Window("结果二值", show_change_two))
                {
                    Cv2.WaitKey();
                }





            //Cv2.MorphologyEx(show_change_two, show_change_two, MorphTypes.Close, element5, new Point(-1, -1), 1);//
            //using (var window = new Window("1次开", show_change_two))
            //{
            //    Cv2.WaitKey();
            //}
           




            Mat Sobel_Edge = new Mat();
            Mat Sobel_result = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            Cv2.Sobel(show_change_two, Sobel_Edge, MatType.CV_16SC1, 2, 0, 3, 1, 1, BorderTypes.Default);
            Cv2.ConvertScaleAbs(Sobel_Edge, Sobel_result);
            Cv2.Threshold(Sobel_result, Sobel_result, 100, 255, ThresholdTypes.Binary);
            using (var window = new Window("sobel结果二值", Sobel_result))
            {
                Cv2.WaitKey();
            }

            Mat img_step2 = new Mat();
            for (int num = 0; num < 3; num++)
                Cv2.Dilate(Sobel_result, img_step2, element3);
            using (var window = new Window("sobel后膨胀", img_step2))
            {
                Cv2.WaitKey();
            }

            Mat result = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            Point[][] contours_one;
            HierarchyIndex[] hierarchy_one;
            Cv2.FindContours(img_step2.Clone(), out contours_one,out hierarchy_one, RetrievalModes.External,ContourApproximationModes.ApproxSimple , new Point(0, 0));
            List<Point[]> afterFilter=new List<Point[]>();
            Console.WriteLine( contours_one.Length);
            //vector<vector<Point>>::iterator itc = contours_one.begin();
            for (int c = 0; c < contours_one.Length; c++)
            {
                double area = Cv2.ContourArea(contours_one[c]);
                Console.WriteLine(area);
                if (area > 800)
                    afterFilter.Add(contours_one[c]);
            }
           Cv2.DrawContours(result, afterFilter, -1, new Scalar(255), -1);          
            using (var window = new Window("去除小面积结果图", result))
            {
                Cv2.WaitKey();
            }

            
            

            // for (int num = 0; num < 5; num++)
            //     Cv2.Dilate(result, result, element);
            // using (var window = new Window("连接下面的部分5次膨胀结果图", result))
            // {
            //     Cv2.WaitKey();
            // }
            // Cv2.MorphologyEx(result, result, MorphTypes.Close, element, new Point(-1, -1), 10);
            // using (var window = new Window("闭运算再次迭代10次结果图", result))
            // {
            //     Cv2.WaitKey();
            // }


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
            using (var window = new Window("再次去除小面积结果图", result1))
            {
                Cv2.WaitKey();
            }


            Mat result_uchar = Mat.Zeros(imgrows, imgcols, MatType.CV_8UC1);
            result.ConvertTo(result_uchar, MatType.CV_8UC1);
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
                using (var window = new Window("绘制最小外接矩形结果图", rectangle_one))
                {
                    Cv2.WaitKey();
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


        public static float sigmod(double x, double mid, double a)
        {
            return (float)(1 / (1 +Math.Exp((a * (mid - x)))));
        }
    }
}
