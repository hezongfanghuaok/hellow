// ReadYaml.cpp : 定义控制台应用程序的入口点。
#include "stdafx.h"
using namespace cv;
using namespace std;

#define N 100;
#define M 200;


//////////////////////////////////////////////////
//输入为一个3*n的矩阵Mat分别为x、y、z的坐标
//平面方程z = ax + by + c, 返回值为[a, b, c]
//斜率 x = b , y  = a; z = - a / b

Mat get_normal_vector(Mat src)
{
	Mat z = src.col(src.cols - 1);
	Mat xy_baise = src.clone();
	Mat ones = Mat::ones(Size(1, src.rows), src.type());
	Mat baise_temp = xy_baise.col(src.cols - 1);
	ones.copyTo(baise_temp);
	return ((xy_baise.t())*xy_baise).inv()*xy_baise.t()*z;
}

//void contours(Mat src, vector<vector<Point>> contours, int num_of_contour, int num_of_point)
//{
//	Mat temp = Mat::ones(num_of_point, 3, CV_32FC1);
//	vector<Mat> sprit_all;
//	split(src, sprit_all);
//
//	for (size_t i = 0; i < num_of_point; i++)
//	{
//		Point temp_p = contours[num_of_contour][int(((contours[num_of_contour].size()) / num_of_contour) * i)];
//		temp.at<float>(i, 0) = sprit_all[0].at<float>(temp_p);
//		temp.at<float>(i, 1) = sprit_all[1].at<float>(temp_p);
//		temp.at<float>(i, 2) = sprit_all[2].at<float>(temp_p);
//	}
//	Mat norm_vec = get_normal_vector(temp);
//	cout << norm_vec << endl;
//}

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
		cout << ind_diff[j]<< endl;
	}
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

int main3()
{
	const char * filePath = "C:\\Users\\Administrator\\Desktop\\gg";
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
		Mat model_again = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		Mat element = getStructuringElement(MORPH_RECT, Size(5, 5));
		for (int i = 0; i < channel_one.rows; i++)
		{
			for (int j = 0; j < channel_one.cols; j++)
			{
				if (channel_two.at<float>(i, j) < 475)
				{
					model_show.at<float>(i, j) = channel_one.at<uchar>(i, j);
				}
				else
					continue;
			}
		}
		imshow("模型展示", model_show);
		waitKey(0);

		Mat Edge_one = model_show.clone();
		for (int i = 0; i < 100; i++)
		{
			for (int j = 0; j <channel_two.cols; j++)
			{
				Edge_one.at<float>(i,j) = 0;
			}
		}
		imshow("模型展示赋值为零图", Edge_one);
		waitKey(0);
		Mat Edge = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		for (int i = 0; i < channel_two.rows; i++)
		{
			for (int j = 0; j < channel_two.cols; j++)
			{				
				Edge.at<float>(i, j) = 255 - Edge_one.at<float>(i, j);
				//cout << "model_show "<<model_show.at<float>(i, j) << endl;
				//cout << "Edge " <<Edge.at<float>(i, j) << endl;
			}
		}
		imshow("模型展示图", Edge);
		waitKey(0);
		int zero_cout = countNonZero(Edge.clone());
		Scalar zero_sum = sum(Edge);
		float matMean = zero_sum[0] / zero_cout;
		float angle = 0.2;
		Mat show = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
		for (size_t i = 0; i < model_show.cols; i++)
		{
			for (size_t j = 0; j < model_show.rows; j++)
			{
				if (Edge.at<float>(Point(i, j)) != 0)
					show.at<float>(Point(i, j)) = sigmod(Edge.at<float>(Point(i, j)), matMean, angle) * 255;
			}
		}
		imshow("扩大阈值后展示图", show);
		waitKey(0);
		
		for (int num = 0; num < 3; num++)
			dilate(show, show, element);
		imshow("3次膨胀结果图", show);
		waitKey(0);

		Mat show_uchar;
		show.convertTo(show_uchar, CV_8UC1);
		imshow("转换展示图", show_uchar);
		waitKey(0);

		morphologyEx(show_uchar, show_uchar, MORPH_OPEN, element, Point(-1, -1), 3);
		imshow("开运算迭代3次结果图", show_uchar);
		waitKey(0);

		Mat Edge_result_one;
		threshold(show_uchar, Edge_result_one, 200, 255, THRESH_BINARY);
		imshow("二值赋值结果图", Edge_result_one);
		waitKey(0);

		Mat show_blur_two = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		for (size_t i = 0; i < model_show.cols; i++)
		{
			for (size_t j = 0; j < model_show.rows; j++)
			{
				show_blur_two.at<uchar>(Point(i, j)) = 255 - Edge_result_one.at<uchar>(Point(i, j));
			}
		}
		imshow("反转图像结果图", show_blur_two);
		waitKey(0);

		for (int num = 0; num < 1; num++)
			dilate(show_blur_two, show_blur_two, element);
		imshow("反转后3次膨胀结果图", show_blur_two);
		waitKey(0);

		Mat Edge_result = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		Sobel(show_blur_two, Edge_result, 0, 2, 0, 3, 1, 1, BORDER_DEFAULT);
		imshow("第一幅图进行边缘处理结果图", Edge_result);
		waitKey(0);

		Mat show_change = Edge_result.clone();
		for (int num = 0; num<1; num++)
			dilate(show_change, show_change, element);
		imshow("边缘处理后1次膨胀结果图", show_change);
		waitKey(0);

		Mat Edge_result_two;
		threshold(show_change, Edge_result_two, 200, 255, THRESH_BINARY);
		imshow("再次进行二值赋值结果图", Edge_result_two);
		waitKey(0);

		Mat result = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
		vector<vector<Point>> contours_one;
		vector<Vec4i>hierarchy_one;
		findContours(Edge_result_two.clone(), contours_one, hierarchy_one, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
		vector<vector<Point>> afterFilter;
		cout << contours_one.size() << endl;
		for (size_t c = 0; c <contours_one.size(); c++)
		{  
			double area = contourArea(contours_one[c]);
			if (area >500)
				afterFilter.push_back(contours_one[c]);
		}
		drawContours(result, afterFilter, -1, Scalar(255), CV_FILLED);
		imshow("去除小面积轮廓区域结果图", result);
		waitKey(0);
		for (int num = 0; num<3; num++)
			dilate(result, result, element);
		imshow("又进行3次膨胀结果图", result);
		waitKey(0); 
		morphologyEx(result, result, MORPH_CLOSE, element, Point(-1, -1), 3);
		imshow("闭运算3次迭代结果图", result);
		waitKey(0);
		for (int num = 0; num < 4; num++)
			dilate(result, result, element);
		imshow("连接下面的部分4次膨胀结果图", result);
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
			if (area > 2000)
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
		cout << "contours_three.size()"<<contours_three.size() << endl;
		vector<vector<Point2f>> rec_vec(contours_three.size());
		vector<float>center_one(contours_three.size());
		for (int i = 0; i <  contours_three.size(); i++)
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
		for (size_t i = 0; i < contours_three.size()-1; i++)
		{
			cout << "ind" << ind[i] << endl;
			Point point_one;
			point_one.x = boundRect_one[ind[i]].x + boundRect_one[ind[i]].width*0.5;
			point_one.y = boundRect_one[ind[i]].y;
			Point point_two;
			point_two.x = boundRect_one[ind[i+1]].x + boundRect_one[ind[i+1]].width*0.5;
			point_two.y = boundRect_one[ind[i+1]].y + boundRect_one[ind[i+1]].height;
			Point point_three;
			point_three = point_two - point_one;
			Rect rect(point_one.x, point_one.y, point_three.x, point_three.y);
			Mat capture_one = channel_one(rect);
			imshow("截图第一幅图结果图", capture_one);
			waitKey(0);
			Point point_four;
			point_four.x = point_one.x + point_three.x*0.5;
			point_four.y = point_one.y + N;
			circle(channel_one, point_four, 9 , Scalar(0,0,255));
			cout << "贴标签x坐标值" << "= " << sprit_all[0].at<float>( point_four.y, point_four.x) << endl;
			cout << "贴标签y坐标值" << "= " << sprit_all[1].at<float>( point_four.y,point_four.x) << endl;
			cout << "贴标签z坐标值" << "= " << sprit_all[2].at<float>( point_four.y,point_four.x) << endl;
			imshow("moxingtu", channel_one);
			waitKey(0);
			Point point_five;
			point_five.x = point_one.x + point_three.x*0.5;
			point_five.y = point_one.y + M;
			circle(channel_one, point_five, 9, Scalar(0, 0, 255));
			cout << "喷码x坐标值" << "= " << sprit_all[0].at<float>(point_five.y,point_five.x) << endl;
			cout << "喷码y坐标值" << "= " << sprit_all[1].at<float>(point_five.y,point_five.x) << endl;
			cout << "喷码z坐标值" << "= " << sprit_all[2].at<float>(point_five.y,point_five.x) << endl;
			imshow("moxingtu", channel_one);
			waitKey(0);

			Mat A = Mat::zeros(4, 3, CV_32FC1);
			A.at<float>(0, 0) = sprit_all[0].at<float>(point_five.y, point_five.x);
			A.at<float>(0, 1) = sprit_all[1].at<float>(point_five.y, point_five.x);
			A.at<float>(0, 2) = sprit_all[2].at<float>(point_five.y, point_five.x);

			A.at<float>(1, 0) = sprit_all[0].at<float>(point_five.y + 10, point_five.x + 10);
			A.at<float>(1, 1) = sprit_all[1].at<float>(point_five.y + 10, point_five.x + 10);
			A.at<float>(1, 2) = sprit_all[2].at<float>(point_five.y + 10, point_five.x + 10);

			A.at<float>(2, 0) = sprit_all[0].at<float>(point_five.y + 20 , point_five.x + 34);
			A.at<float>(2, 1) = sprit_all[1].at<float>(point_five.y + 20, point_five.x  + 34);
			A.at<float>(2, 2) = sprit_all[2].at<float>(point_five.y + 20, point_five.x + 34);

			A.at<float>(3, 0) = sprit_all[0].at<float>(point_five.y + 14, point_five.x + 25);
			A.at<float>(3, 1) = sprit_all[1].at<float>(point_five.y + 14, point_five.x + 25);
			A.at<float>(3, 2) = sprit_all[2].at<float>(point_five.y + 14, point_five.x + 25);
			get_normal_vector(A);
			cout << "A" << A << endl;
			cout << "get_normal_vector(A)"<< get_normal_vector(A) << endl;
			waitKey(0);
			cout << "get_normal_vector(A).at<float>(0,1)" << get_normal_vector(A).at<float>(1,0 ) << endl;

			if ( get_normal_vector(A).at<float>(1,0 ) ==  0)
			{
				for (int i = 0; i < channel_one.rows; i++)
				{
					for (int j = 0; j < channel_one.cols; j++)
					{
						if (channel_two.at<float>(i, j) < 485 && channel_two.at<float>(i, j) > 475)
						{
							model_again.at<float>(i, j) = channel_one.at<uchar>(i, j);
						}
						else
							continue;
					}
				}
				imshow("模型展示一条线", model_again);
				waitKey(0);

				Mat uchar_change;
				model_again.convertTo(uchar_change, CV_8UC1);
				imshow("把一条线转换成8位字符型图", uchar_change);
				waitKey(0);

				Mat Edge_deal;
				threshold(uchar_change, Edge_deal, 5, 255, THRESH_BINARY);
				imshow("把低灰度值转成二值化图", Edge_deal);
				waitKey(0);
				morphologyEx(Edge_deal, Edge_deal, MORPH_CLOSE, element, Point(-1, -1), 3);
				imshow("把低灰度值转成二值化图进行开运算迭代3次结果图", Edge_deal);
				waitKey(0);
				for (int num = 0; num <2; num++)
					dilate(Edge_deal, Edge_deal, element);
				imshow("把边缘进行3次膨胀结果图", Edge_deal);
				waitKey(0);

				Mat result_edge = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
				vector<vector<Point>> contours_edge;
				vector<Vec4i>hierarchy_edge;
				findContours(Edge_deal.clone(), contours_edge, hierarchy_edge, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
				vector<vector<Point>> afterFilter_edge;
				cout << contours_edge.size() << endl;
				for (size_t c = 0; c <contours_edge.size(); c++)
				{
					double area = contourArea(contours_edge[c]);
					if (area >2000)
						afterFilter_edge.push_back(contours_edge[c]);
				}
				drawContours(result_edge, afterFilter_edge, -1, Scalar(255), CV_FILLED);
				imshow("去除小面积轮廓区域结果图", result_edge);
				waitKey(0);
				for (int num = 0; num <2; num++)
					erode(result_edge, result_edge, element);
				imshow("把边缘进行3次腐蚀结果图", result_edge);
				waitKey(0);

				vector<vector<Point>> contours_edge_one;
				vector<Vec4i>hierarchy_edge_one;
				findContours(result_edge.clone(), contours_edge_one, hierarchy_edge_one, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_SIMPLE, Point(0, 0));
				Mat rectangle_edge = Mat::zeros(model_show.rows, model_show.cols, CV_8UC3);
				vector<Rect> boundRect_edge(contours_edge_one.size());  //定义外接矩形集合
				vector<RotatedRect> box_edge(contours_edge_one.size());
				Point2f rect_edge[4];
				cout << "contours_three.size()" << contours_edge_one.size() << endl;
				vector<vector<Point2f>> rec_edge(contours_edge_one.size());
				vector<float>center_edge(contours_edge_one.size());
				for (int i = 0; i < 1; i++)
				{
					box_edge[i] = minAreaRect(Mat(contours_edge_one[i]));  //计算每个轮廓最小外接矩形
					boundRect_edge[i] = boundingRect(Mat(contours_edge_one[i]));
					circle(rectangle_edge, Point(box_edge[i].center.x, box_edge[i].center.y), 5, Scalar(0, 255, 0), -1, 8);  //绘制最小外接矩形的中心点
					box_edge[i].points(rect_edge);  //把最小外接矩形四个端点复制给rect数组
					rectangle(rectangle_edge, Point(boundRect_edge[i].x, boundRect_edge[i].y), Point(boundRect_edge[i].x + boundRect_edge[i].width, boundRect_edge[i].y + boundRect_edge[i].height), Scalar(0, 255, 0), 2, 8);
					center_edge[i] = box_edge[i].center.x;
					for (int j = 0; j < 4; j++)
					{
						line(rectangle_edge, rect_edge[j], rect_edge[(j + 1) % 4], Scalar(0, 0, 255), 2, 8);  //绘制最小外接矩形每条边
						rec_edge[i].push_back(rect_edge[j]);			/*cout << "第"<<j<<"个角点"<< rect[j] << endl;*/
					}
					circle(rectangle_edge, Point(boundRect_edge[i].x, boundRect_edge[i].y), 5, Scalar(0, 255, 0), -1, 8);
					circle(rectangle_edge, Point(boundRect_edge[i].x + boundRect_edge[i].width, boundRect_edge[i].y), 5, Scalar(0, 255, 0), -1, 8);
					imshow("第一个绘制最小外接矩形结果图", rectangle_edge);
					waitKey(0);
				}
				Mat result_point = Mat::zeros(2, 2, CV_32FC1);
				for (size_t i = boundRect_edge[0].y; i < boundRect_edge[0].y + 200; i++)
				{
					for (size_t j = boundRect_edge[0].x; j < boundRect_edge[0].x + boundRect_edge[0].width; j++)
					{
						if (result_edge.at<uchar>(i, j) == 255)
						{
							if (i == boundRect_edge[0].y + 100)
							{
								result_point.at<float>(0, 0) = sprit_all[0].at<float>(j, i);
								result_point.at<float>(0, 1) = sprit_all[1].at<float>(j, i);
								cout << "i" << "=" << i << endl;
								cout << "j" << "=" << j << endl;
							}
							else if (i == boundRect_edge[0].y + 190)
							{
								result_point.at<float>(1, 0) = sprit_all[0].at<float>(j, i);
								result_point.at<float>(1, 1) = sprit_all[1].at<float>(j, i);
								cout << "i" << "=" << i << endl;
								cout << "j" << "=" << j << endl;
							}
						}
						else
							continue;
					}
				}
				cout << "A" << "=" << result_point << endl;
				waitKey(0);
			}
			else
				continue;		
		}
		waitKey(0);
	}
	return 0;
}




