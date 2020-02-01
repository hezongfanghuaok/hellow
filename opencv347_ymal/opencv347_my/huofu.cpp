// ReadYaml.cpp : 定义控制台应用程序的入口点。
#include "stdafx.h"
using namespace cv;
using namespace std;

#define N 100;
#define M 200;
void readMat(Mat &dst, string path)
{
	FileStorage fs(path, FileStorage::READ);
	fs["vocabulary"] >> dst;
	fs.release();
}
void getFiles(string path, vector<string>& files)
{
	//文件句柄  
	intptr_t hFile = 0;
	//文件信息  
	struct _finddata_t fileinfo;
	string p;
	if ((hFile = _findfirst(p.assign(path).append("\\*").c_str(), &fileinfo)) != -1)
	{
		do
		{	//如果是目录,迭代之  
			//如果不是,加入列表  
			if ((fileinfo.attrib &  _A_SUBDIR))
			{
				if (strcmp(fileinfo.name, ".") != 0 && strcmp(fileinfo.name, "..") != 0)
					getFiles(p.assign(path).append("\\").append(fileinfo.name), files);
			}
			else
			{
				files.push_back(p.assign(path).append("\\").append(fileinfo.name));
			}
		} while (_findnext(hFile, &fileinfo) == 0);
		_findclose(hFile);
	}
}
//void BubbleSort(float  *p, int length, int * ind_diff)
//{
//	for (int m = 0; m < length; m++)
//	{
//		ind_diff[m] = m;

//	}
//	for (int i = 0; i < length; i++)
//	{
//		for (int j = 0; j < length - i - 1; j++)
//		{
//			if (p[j] > p[j + 1])
//			{
//				float temp = p[j];
//				p[j] = p[j + 1];
//				p[j + 1] = temp;
//				int ind_temp = ind_diff[j];
//				ind_diff[j] = ind_diff[j + 1];
//				ind_diff[j + 1] = ind_temp;
//			}
//		}
//	}
//}
void BubbleSort(vector<float>p, vector<int>&ind_diff)
{
	int length = p.size();
	for (int m = 0; m < length; m++)
	{
		ind_diff.push_back(m);
	}

	for (size_t j = 0; j < length; j++)
	{
		cout << ind_diff[j] << endl;
	}
	cout << endl;
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

	for (size_t j = 0; j < length; j++)
	{
		cout << ind_diff[j] << endl;
	}
}
template<typename T>
float sigmod(T x, T mid, T a)
{
	return float(1 / (1 + exp((a * (mid - x)))));
}

int main()
{
	const char * filePath = "D:\\testimage";
	vector<string> pic_namelist;
	getFiles(filePath, pic_namelist);
	for (int k = 0; k < pic_namelist.size(); k++)
	{
		Mat src;
		Mat channel_one;
		Mat channel_two;
		vector<Mat> sprit_all;
		readMat(src, pic_namelist[k]);
		int n = 1;
		split(src, sprit_all);
		sprit_all[3].convertTo(channel_one, CV_8UC1);
		imshow("gray", channel_one);
		waitKey(50);
		sprit_all[2].convertTo(channel_two, CV_32FC1);
		imshow("high", channel_two);
	
		Mat model = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		Mat model_show = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		Mat model_z = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		Mat element = getStructuringElement(MORPH_RECT, Size(5, 5));
		for (int i = 0; i < channel_one.rows; i++)
		{
			for (int j = 0; j < channel_one.cols; j++)
			{
				if (channel_two.at<float>(i, j) <1060)
				{
					model_show.at<float>(i, j) = channel_one.at<uchar>(i, j);
					model_z.at<float>(i, j) = sprit_all[2].at<float>(i, j);
				}
				else
					continue;
			}
		}
		imshow("模型展示", model_show);
		waitKey(0);
		imshow("模型展示2", model_z);
		waitKey(0);
		Mat imgdst_one;
		resize(model_show, imgdst_one, Size(500, 500));
		imshow("suoxiao", imgdst_one);
		waitKey(0);
		Mat Edge_one = model_show.clone();
		for (int i = 0; i < 150; i++)
		{
			for (int j = 0; j <channel_two.cols; j++)
			{
				Edge_one.at<float>(i, j) = 0;
			}
		}
		imshow("模型展示赋值为零图", Edge_one);
		waitKey(10);
		Mat Edge = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		for (int i = 0; i < channel_two.rows; i++)
		{
			for (int j = 0; j < channel_two.cols; j++)
			{
				Edge.at<float>(i, j) = 255 - Edge_one.at<float>(i, j);
			}
		}
		imshow("模型展示图", Edge);
		waitKey(0);

		int zero_cout = countNonZero(Edge.clone());
		Scalar zero_sum = sum(Edge);
		float matMean = zero_sum[0] / zero_cout;
		int zero_coutZ = countNonZero(model_z.clone());
		Scalar zero_sumZ = sum(model_z);
		float matMeanZ = 450; zero_sum[0] / zero_coutZ;
		//cout << model_z << endl;
		float angle = 0.01;
		Mat show = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		Mat showZ = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		for (size_t i = 0; i < model_show.cols; i++)
		{
			for (size_t j = 0; j < model_show.rows; j++)
			{
				if (Edge.at<float>(Point(i, j)) != 0)
				{
					show.at<float>(Point(i, j)) = sigmod(Edge.at<float>(Point(i, j)), matMean, angle) * 255;
					showZ.at<uchar>(Point(i, j)) = int(sigmod(model_z.at<float>(Point(i, j)), matMeanZ, angle) * 255);
				}
			}
		}
		imshow("Z轴扩大阈值后展示图", showZ);
		waitKey(0);
		imshow("扩大阈值后展示图", show);
		waitKey(0);
		//Mat resutl_two;
		//medianBlur(show, resutl_two,5);
		//imshow("中值滤波展示图", resutl_two);
		//waitKey(0);
		//Mat resutl_three;
		//medianBlur(resutl_two, resutl_three, 5);
		//imshow("中值滤波展示图", resutl_three);
		//waitKey(0);
		//Mat resutl_four;
		//medianBlur(resutl_three, resutl_four, 5);
		//imshow("中值滤波展示图", resutl_four);
		//waitKey(0);
		Mat show_uchar;
		show.convertTo(show_uchar, CV_8UC1);
		imshow("转换展示图", show_uchar);
		waitKey(0);
		morphologyEx(show_uchar, show_uchar, MORPH_CLOSE, element, Point(-1, -1), 10);
		imshow("闭运算迭代3次结果图", show_uchar);
		waitKey(0);
		for (int num = 0; num < 6; num++)
			dilate(show_uchar, show_uchar, element);
		imshow("6次膨胀结果图", show_uchar);
		waitKey(0);
		Mat show_change_two;
		//show_uchar.convertTo(show_change_two, CV_8UC1);
		//imshow("转换展示图", show_change_two);
		//waitKey(0);
		Canny(show_uchar, show_change_two, 1, 3);
		imshow("canny模糊展示图", show_change_two);
		waitKey(0);
		for (int num = 0; num < 5; num++)
			dilate(show_change_two, show_change_two, element);
		imshow("5次膨胀结果图", show_change_two);
		waitKey(0);

		Mat show_train = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		morphologyEx(show_change_two, show_train, MORPH_CLOSE, element, Point(-1, -1), 10);
		imshow("闭运算迭代10次结果图", show_train);
		waitKey(0);

		for (int num = 0; num < 1; num++)
			erode(show_train, show_train, element);
		imshow("1次腐蚀结果图", show_train);
		waitKey(0);


		Mat Edge_result = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		Sobel(show_train, Edge_result, 0, 2, 0, 3, 1, 1, BORDER_DEFAULT);
		imshow("第一幅图进行边缘处理结果图", Edge_result);
		waitKey(0);
		Mat Edge_result_two;
		threshold(Edge_result, Edge_result_two,100, 255, THRESH_BINARY);
		imshow("二值重新赋值结果图", Edge_result_two);
		waitKey(0);
		for (int num = 0; num <1; num++)
			dilate(Edge_result_two, Edge_result_two, element);
		imshow("再次进行2次膨胀结果图", Edge_result_two);
		waitKey(0);


		Mat result = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		vector<vector<Point>> contours_one;
		vector<Vec4i>hierarchy_one;
		findContours(Edge_result_two.clone(), contours_one, hierarchy_one, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
		vector<vector<Point>> afterFilter;
		cout << contours_one.size() << endl;
		vector<vector<Point>>::iterator itc = contours_one.begin();
		for (size_t c = 0; c <contours_one.size(); c++)
		{
			double area = contourArea(contours_one[c]);
			cout << area << endl;
			if (area > 500)
				afterFilter.push_back(contours_one[c]);
		}
		drawContours(result, afterFilter, -1, Scalar(255), CV_FILLED);
		imshow("去除小面积结果图", result);
		waitKey(0);
		for (int num = 0; num < 5; num++)
			dilate(result, result, element);

		imshow("连接下面的部分5次膨胀结果图", result);
		morphologyEx(result, result, MORPH_CLOSE, element, Point(-1, -1), 10);
		imshow("闭运算再次迭代10次结果图", result);
		waitKey(0);

		Mat result_two = Mat::zeros(model_show.rows, model_show.cols, CV_8UC1);
		vector<vector<Point>> contours_two;
		vector<Vec4i>hierarchy_two;
		//Mat result_uchar = Mat::zeros(result.rows, result.cols, CV_8UC1);
		//result.convertTo(result_uchar, CV_8UC1);
		findContours(result.clone(), contours_two, hierarchy_two, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
		vector<vector<Point>> afterFilter_three;
		for (size_t c = 0; c <contours_two.size(); c++)
		{
			double area = contourArea(contours_two[c]);
			cout << area << endl;
			if (area > 5000)
				afterFilter_three.push_back(contours_two[c]);
		}
		drawContours(result_two, afterFilter_three, -1, Scalar(255), CV_FILLED);
		imshow("去除小面积结果图", result_two);
		waitKey(10);
		Mat result_uchar = Mat::zeros(result.rows, result.cols, CV_8UC1);
		result_two.convertTo(result_uchar, CV_8UC1);
		vector<vector<Point> > contours_three;
		vector<Vec4i> hierarchy_three;
		findContours(result_uchar.clone(), contours_three, hierarchy_three, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
		Mat rectangle_one = Mat::zeros(model_show.rows, model_show.cols, CV_8UC3);
		vector<Rect> boundRect_one(contours_three.size());  //定义外接矩形集合
		vector<RotatedRect> box_one(contours_three.size());
		Point2f rect_one[4];
		cout << "contours_three.size()" << contours_three.size() << endl;

		vector<vector<Point2f>> rec_vec(contours_three.size());
		vector<float>center_one(contours_three.size());
		for (int i = 0; i < contours_three.size(); i++)
		{
			box_one[i] = minAreaRect(Mat(contours_three[i]));  //计算每个轮廓最小外接矩形
			boundRect_one[i] = boundingRect(Mat(contours_three[i]));
			circle(rectangle_one, Point(box_one[i].center.x, box_one[i].center.y), 5, Scalar(0, 255, 0), -1, 8);  //绘制最小外接矩形的中心点
			box_one[i].points(rect_one);  //把最小外接矩形四个端点复制给rect数组
			rectangle(rectangle_one, Point(boundRect_one[i].x, boundRect_one[i].y), Point(boundRect_one[i].x + boundRect_one[i].width, boundRect_one[i].y + boundRect_one[i].height), Scalar(0, 255, 0), 2, 8);
			//cout << "start" <<center_one.size() << endl;
			center_one[i] = box_one[i].center.x;
			//cout << "end" <<center_one.size() << endl;
			for (int j = 0; j < 4; j++)
			{
				line(rectangle_one, rect_one[j], rect_one[(j + 1) % 4], Scalar(0, 0, 255), 2, 8);  //绘制最小外接矩形每条边
				rec_vec[i].push_back(rect_one[j]);			/*cout << "第"<<j<<"个角点"<< rect[j] << endl;*/
			}
			imshow("绘制最小外接矩形结果图", rectangle_one);
			waitKey(10);
		}
		vector<int>ind;
		BubbleSort(center_one, ind);
		for (size_t i = 0; i < contours_three.size() - 1; i++)
		{
			cout << "ind" << ind[i] << endl;
			Point point_one;
			point_one.x = boundRect_one[ind[i]].x + boundRect_one[ind[i]].width*0.5;
			point_one.y = boundRect_one[ind[i]].y;
			Point point_two;
			point_two.x = boundRect_one[ind[i + 1]].x + boundRect_one[ind[i + 1]].width*0.5;
			point_two.y = boundRect_one[ind[i + 1]].y + boundRect_one[ind[i + 1]].height;
			Point point_three;
			point_three = point_two - point_one;
			Rect rect(point_one.x, point_one.y, point_three.x, point_three.y);
			Mat capture_one = channel_one(rect);
			imshow("截图第一幅图结果图", capture_one);
			waitKey(0);
			Point point_four;
			point_four.x = point_one.x + point_three.x*0.5;
			point_four.y = point_one.y + N;
			circle(channel_one, point_four, 9, Scalar(0, 0, 255));
			cout << "贴标签x坐标值" << "= " << sprit_all[0].at<float>(point_four.y, point_four.x) << endl;
			cout << "贴标签y坐标值" << "= " << sprit_all[1].at<float>(point_four.y, point_four.x) << endl;
			cout << "贴标签z坐标值" << "= " << sprit_all[2].at<float>(point_four.y, point_four.x) << endl;
			imshow("moxingtu", channel_one);
			waitKey(0);
			Point point_five;
			point_five.x = point_one.x + point_three.x*0.5;
			point_five.y = point_one.y + M;
			circle(channel_one, point_five, 9, Scalar(0, 0, 255));
			cout << "喷码x坐标值" << "= " << sprit_all[0].at<float>(point_five.y, point_five.x) << endl;
			cout << "喷码y坐标值" << "= " << sprit_all[1].at<float>(point_five.y, point_five.x) << endl;
			cout << "喷码z坐标值" << "= " << sprit_all[2].at<float>(point_five.y, point_five.x) << endl;
			imshow("moxingtu", channel_one);
			waitKey(0);
		}
		waitKey(0);
	}
	return 0;
}



