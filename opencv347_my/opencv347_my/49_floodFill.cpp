#include "stdafx.h"
using namespace cv;  

int g_nStructElementSize = 3;//�ṹԪ��(�ں˾���)�ĳߴ�

int main3( )
{    
	Mat src = imread("1.jpg"); 
	imshow("��ԭʼͼ��",src);
	Rect ccomp;
	floodFill(src, Point(50,300), Scalar(155, 255,55), &ccomp, Scalar(20, 20, 20),Scalar(20, 20, 20));
	imshow("��Ч��ͼ��",src);
	waitKey(0);
	Mat src1;
	cvtColor( src, src1,COLOR_RGB2GRAY);
	imshow("�Ҷ�ͼ", src1);
	waitKey(0);

	Mat g_dstImage;
	Mat element = getStructuringElement(MORPH_RECT, Size(2 * g_nStructElementSize + 1, 2 * g_nStructElementSize + 1), Point(g_nStructElementSize, g_nStructElementSize));
	dilate(src1, g_dstImage, element);//����
	imshow("��ʴͼ", g_dstImage);
	waitKey(0);

	return 0;    
}  