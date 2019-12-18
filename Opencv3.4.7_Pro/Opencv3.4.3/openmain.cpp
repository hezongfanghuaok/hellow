#include "stdafx.h"
using namespace cv;
using namespace cv::xfeatures2d;
using namespace std;



int main(int argc, char** argv)
{
   
	system("color 2F");


	//��1������ԴͼƬ����ʾ
	Mat srcImage1 = imread("D:\\github\\box.png", 1);
	Mat srcImage2 = imread("D:\\github\\box_in_scene.png", 1);
	if (!srcImage1.data || !srcImage2.data)//����Ƿ��ȡ�ɹ�
	{
		printf("��ȡͼƬ������ȷ��Ŀ¼���Ƿ���imread����ָ�����Ƶ�ͼƬ����~�� \n"); return false;
	}
	

	
	int minHessian = 400;//����SURF�е�hessian��ֵ������������
	Ptr<SURF> detector = SURF::create(minHessian);
	
	vector<KeyPoint> keypoint1, keypoint2;//vectorģ�������ܹ�����������͵Ķ�̬���飬�ܹ����Ӻ�ѹ������

	//��3������detect��������SURF�����ؼ��㣬������vector������
	detector ->detect(srcImage1, keypoint1, Mat());
	Mat keypoint_img;
	drawKeypoints(srcImage1, keypoint1, keypoint_img, Scalar::all(-1), DrawMatchesFlags::DEFAULT);
	imshow("KeyPoints Image", keypoint_img);
	waitKey(0);
	return 0;
}


