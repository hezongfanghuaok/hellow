#include "stdafx.h"
using namespace cv;
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
	imshow("ԭʼͼ1", srcImage1);
	imshow("ԭʼͼ2", srcImage2);

	//��2��������Ҫ�õ��ı�������
	int minHessian = 400;//����SURF�е�hessian��ֵ������������
	SurfFeatureDetector detector(minHessian);//����һ��SurfFeatureDetector��SURF�� ������������
	vector<KeyPoint> keypoint1, keypoint2;//vectorģ�������ܹ�����������͵Ķ�̬���飬�ܹ����Ӻ�ѹ������

	//��3������detect��������SURF�����ؼ��㣬������vector������
	detector.detect(srcImage1, keypoint1);
	detector.detect(srcImage2, keypoint2);

	//��4������������������������
	SurfDescriptorExtractor extractor;
	Mat descriptors1, descriptors2;
	extractor.compute(srcImage1, keypoint1, descriptors1);
	extractor.compute(srcImage2, keypoint2, descriptors2);

	//��5��ʹ��BruteForce����ƥ��
	// ʵ����һ��ƥ����
	BruteForceMatcher< L2<float> > matcher;
	std::vector< DMatch > matches;
	//ƥ������ͼ�е������ӣ�descriptors��
	matcher.match(descriptors1, descriptors2, matches);

	//��6�����ƴ�����ͼ����ƥ����Ĺؼ���
	Mat imgMatches;
	drawMatches(srcImage1, keypoint1, srcImage2, keypoint2, matches, imgMatches);//���л���

	//��7����ʾЧ��ͼ
	imshow("ƥ��ͼ", imgMatches);

	waitKey(0);
	return 0;
}


