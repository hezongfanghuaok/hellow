#include "stdafx.h"
using namespace cv;
using namespace std;
int openimage(string a);
int main(int argc, char** argv)
{
	if (argc != 2)
	{
		cout << "usage:display image imagetoloadand display" << endl;
		return -1;
	}
	openimage(argv[1]);		
}
int openimage(string a)
{
	Mat imagemat = imread(a, IMREAD_COLOR);
	if (!imagemat.data)
	{
		cout << "can not open or find image" << endl;
		return -1;
	}
	imshow("Display window", imagemat);
	waitKey(0);
	return 0;
}