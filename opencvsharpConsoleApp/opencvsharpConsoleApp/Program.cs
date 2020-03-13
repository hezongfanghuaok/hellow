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
            var window2 = new Window(channel_depth);
            Cv2.WaitKey();

            Mat model = Mat.Zeros(channel_depth.Rows,channel_depth.Cols, MatType.CV_32FC1);
            Mat model_show = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
            Mat model_z = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);

        }
    }
}
