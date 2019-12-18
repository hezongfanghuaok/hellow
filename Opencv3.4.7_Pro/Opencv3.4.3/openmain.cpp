#include "stdafx.h"
using namespace cv;
using namespace cv::xfeatures2d;
using namespace std;



int main(int argc, char** argv)
{
   
	system("color 2F");


	//【1】载入源图片并显示
	Mat srcImage1 = imread("D:\\github\\box.png", 1);
	Mat srcImage2 = imread("D:\\github\\box_in_scene.png", 1);
	if (!srcImage1.data || !srcImage2.data)//检测是否读取成功
	{
		printf("读取图片错误，请确定目录下是否有imread函数指定名称的图片存在~！ \n"); return false;
	}
	

	
	int minHessian = 400;//定义SURF中的hessian阈值特征点检测算子
	Ptr<SURF> detector = SURF::create(minHessian);
	
	vector<KeyPoint> keypoint1, keypoint2;//vector模板类是能够存放任意类型的动态数组，能够增加和压缩数据

	//【3】调用detect函数检测出SURF特征关键点，保存在vector容器中
	detector ->detect(srcImage1, keypoint1, Mat());
	Mat keypoint_img;
	drawKeypoints(srcImage1, keypoint1, keypoint_img, Scalar::all(-1), DrawMatchesFlags::DEFAULT);
	imshow("KeyPoints Image", keypoint_img);
	waitKey(0);
	return 0;
}


