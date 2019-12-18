#include "stdafx.h"
using namespace cv;
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
	imshow("原始图1", srcImage1);
	imshow("原始图2", srcImage2);

	//【2】定义需要用到的变量和类
	int minHessian = 400;//定义SURF中的hessian阈值特征点检测算子
	SurfFeatureDetector detector(minHessian);//定义一个SurfFeatureDetector（SURF） 特征检测类对象
	vector<KeyPoint> keypoint1, keypoint2;//vector模板类是能够存放任意类型的动态数组，能够增加和压缩数据

	//【3】调用detect函数检测出SURF特征关键点，保存在vector容器中
	detector.detect(srcImage1, keypoint1);
	detector.detect(srcImage2, keypoint2);

	//【4】计算描述符（特征向量）
	SurfDescriptorExtractor extractor;
	Mat descriptors1, descriptors2;
	extractor.compute(srcImage1, keypoint1, descriptors1);
	extractor.compute(srcImage2, keypoint2, descriptors2);

	//【5】使用BruteForce进行匹配
	// 实例化一个匹配器
	BruteForceMatcher< L2<float> > matcher;
	std::vector< DMatch > matches;
	//匹配两幅图中的描述子（descriptors）
	matcher.match(descriptors1, descriptors2, matches);

	//【6】绘制从两个图像中匹配出的关键点
	Mat imgMatches;
	drawMatches(srcImage1, keypoint1, srcImage2, keypoint2, matches, imgMatches);//进行绘制

	//【7】显示效果图
	imshow("匹配图", imgMatches);

	waitKey(0);
	return 0;
}


