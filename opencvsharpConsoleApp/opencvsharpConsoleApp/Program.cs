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
            
            FileStorage cld_date = new FileStorage("0924.yaml", FileStorage.Mode.Read);
            using (var fs = new FileStorage("0924.yaml", FileStorage.Mode.Read))
            {           
                src = fs["vocabulary"].ReadMat();
                using (var window = new Window(src))
                {
                    Cv2.WaitKey();
                }
            }
            Cv2.Split(src, out splitall);

            splitall[2].ConvertTo(channel_depth,MatType.CV_32FC1);
            var window1 = new Window(channel_depth);
            Cv2.WaitKey();

            splitall[3].ConvertTo(channel_gray, MatType.CV_8UC1);
            var window2 = new Window(channel_gray);
            Cv2.WaitKey();

            int imgcols = channel_depth.Cols,imgrows=channel_depth.Rows;
            
            Mat model_calc_gray = Mat.Zeros(channel_depth.Rows,channel_depth.Cols, MatType.CV_32FC1);
            Mat model_gray = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_8UC1);
            Mat model_step1 = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);

            for (int i = 0; i < channel_depth.Rows; i++)
            {
                for (int j = 0; j < channel_depth.Cols; j++)
                {
                    if (channel_depth.At<float>(i,j) < 900)//900时为临界 ==》 0943
                    {
                        model_calc_gray.Set<float>(i, j, channel_gray.At<short>(i, j));//= channel_gray.At<short>(i, j);//char convert to float that could calcaulate
                    }
                    else
                        continue;
                }
            }

            Window windowexcpetlow = new Window(model_calc_gray);
            Cv2.WaitKey();

            Mat Edge_one = model_calc_gray.Clone();
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < model_calc_gray.Cols; j++)
                {
                    Edge_one.Set<float>(i, j,0) ;
                }
            }
            Window.ShowImages(Edge_one);
           // Cv2.WaitKey();

            //取反
            new Scalar(255);
            Mat Edge = Mat.Zeros(channel_depth.Rows, channel_depth.Cols, MatType.CV_32FC1);
            Edge = new Scalar(255)- Edge_one;//
            Window.ShowImages(Edge);
           // Cv2.WaitKey();

            int zero_cout = Cv2.CountNonZero(Edge);//返回矩阵中的非零值个数
            Scalar zero_sum = Cv2.Sum(Edge);//对mat类四个通道求和
            float matMean =(float)( zero_sum[0] / zero_cout);
            float angle = 0.2f;
            for (int i = 0; i < imgcols; i++)
            {
                for (int j = 0; j < imgrows; j++)
                {
                    if (Edge.At<float>(i,j)!= 0)
                        model_step1.Set<float>(i,j,sigmod(Edge.At<float>(i, j), matMean, angle) * 255);
                }
            }
            Window.ShowImages(model_step1);
           // Cv2.WaitKey();

        }       
        public static float sigmod(double x, double mid, double a)
        {
            return (float)(1 / (1 +Math.Exp((a * (mid - x)))));
        }
    }
}
