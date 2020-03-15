#include"stdafx.h"

#define N 100;
#define M 200;
using namespace std;
using namespace cv;
clock_t startTime, endTime;
void drawhis(Mat src,string windowname)
{
	Mat dstHist;       // 在cv中用CvHistogram *hist = cvCreateHist
	int dims = 1;
	float hranges[] = { 0, 255 };
	const float *ranges[] = { hranges };   // 这里需要为const类型
	int size = 32;//每个维度的直方图尺寸
	int channels = 0;

	//【3】计算图像的直方图
	calcHist(&src, 1, &channels, noArray(), dstHist, dims, &size, ranges);    // cv 中是cvCalcHist
	int scale = 8;

	Mat dstImage(size * scale, 256, CV_8U, Scalar(0));
	cv::normalize(dstHist, dstHist, 0, 255, cv::NORM_MINMAX);

	//【5】绘制出直方图
	int hpt = saturate_cast<int>(0.9 * size);
	for (int i = 0; i < size; i++)
	{
		float binValue = dstHist.at<float>(i); //   注意hist中是float类型 
		rectangle(dstImage, Point(i*scale, 255), Point((i + 1)*scale - 1, 256 - binValue), Scalar(255));
	}
	imshow(windowname, dstImage);
}
bool loadParams(std::string filename, std::vector<cv::Mat>& vcameraMatrix, std::vector<cv::Mat>& vdistCoeffs)
{
	fprintf(stderr, "loadParams.\n");
	cv::FileStorage fs(filename, cv::FileStorage::READ);
	if (!fs.isOpened()) {
		fprintf(stderr, "%s:%d:loadParams falied. 'camera.yml' does not exist\n", __FILE__, __LINE__);
		return false;
	}

	int n_camera = (int)fs["n_camera"];
	vcameraMatrix.resize(n_camera);
	vdistCoeffs.resize(n_camera);
	for (int i = 0; i < n_camera; i++) {
		char buf1[100];
		sprintf(buf1, "cam_%d_matrix", i);
		char buf2[100];
		sprintf(buf2, "cam_%d_distcoeffs", i);
		fs[buf1] >> vcameraMatrix[i];
		fs[buf2] >> vdistCoeffs[i];
	}
	fs.release();

	return true;
}
void readMat(Mat &dst, string path)
{
	FileStorage fs(path, FileStorage::READ);
	fs["vocabulary"] >> dst;
	fs.release();
}
template<class T>
void BubbleSort1(vector<T> p)
{
	int length = p.size();
	for (int i = 0; i < length; i++)
	{
		for (int j = 0; j < length - i - 1; j++)
		{
			if (p[j] > p[j + 1])
			{
				float temp = p[j];
				p[j] = p[j + 1];
				p[j + 1] = temp;

			}
		}
	}
	for (int i = 0; i < length; i++)
		cout << p.back << endl;
	
}
template<class T>
T findmax(vector<T> p)
{
	T temp = 0;
	int length = p.size();
	for (int i = 0; i < length-1; i++)
	{		
		if (p[i] > p[i + 1])
		{
			temp = p[i];
		}		
	}
	return temp;
}
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
#pragma region 前处理
	Mat src;
	Mat channel_one;
	Mat channel_two;
	Mat channel_three;
	Mat channel_four;
	Mat channel_s;
	FileStorage fs2("D:\\github\\testimg\\0924.yaml", FileStorage::READ);//101161kk
	if (!fs2.isOpened())
	{
		fprintf(stderr, "%s:%d:loadParams falied. 'camera.yml' does not exist\n", __FILE__, __LINE__);
		return false;
	}

	fs2["vocabulary"] >> src;
	vector<Mat> sprit_all;
	int n = 1;
	split(src, sprit_all);
	
	sprit_all[3].convertTo(channel_one, CV_8UC1);
	namedWindow("gray", WINDOW_NORMAL);
	imshow("gray", channel_one);
	waitKey();
	sprit_all[2].convertTo(channel_two, CV_32FC1);
	imshow("high", channel_two);
	//channel_s = channel_one.clone();
//	float a0 = findmax(vector<float>(channel_two.reshape(1, 1)));
	Mat model = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
	Mat model_show = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
	Mat model_z = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
	Mat element = getStructuringElement(MORPH_RECT, Size(3,3));
	for (int i = 0; i < channel_one.rows; i++)
	{
		for (int j = 0; j < channel_one.cols; j++)
		{
			if (channel_two.at<float>(i, j) < 900)//900时为临界 ==》 0943
			{

				model_show.at<float>(i, j) = channel_one.at<uchar>(i, j);//char convert to float that could calcaulate
				
				/*if (model_show.at<float>(i, j) != 0)
				{
					uchar temp = channel_one.at<uchar>(i, j);
					float tempf = model_show.at<float>(i, j);
				}*/
			}
			else
				continue;
		}
	}
	namedWindow("滤除底座模型展示", WINDOW_NORMAL);
	imshow("滤除底座模型展示", model_show);
	waitKey(0);

	Mat Edge_one = model_show.clone();
	for (int i = 0; i < 100; i++)
	{
		for (int j = 0; j < channel_two.cols; j++)
		{
			Edge_one.at<float>(i, j) = 0;
		}
	}
	imshow("模型展示赋值为零图", Edge_one);
	waitKey(0);

	//取反
	Mat Edge = Mat::zeros(channel_two.rows, channel_two.cols, CV_32FC1);
	Edge = Scalar(255) -Edge_one;//
	endTime = clock();//计时开始
	imshow("模型展示图", Edge);
	
	waitKey(0);

	//

	int zero_cout = countNonZero(Edge.clone());//返回矩阵中的非零值个数
	Scalar zero_sum = sum(Edge);//对mat类四个通道求和
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


	Mat show_uchar = show.clone();
	morphologyEx(show_uchar, show_uchar, MORPH_CLOSE, element, Point(-1, -1), 10);//
	imshow("闭运算迭代10次结果图", show_uchar);
	drawhis(show_uchar,"闭运算结果图");
	waitKey(0);
	
	Mat show_change_two;
	show_uchar.convertTo(show_change_two, CV_8UC1);
	imshow("转换展示图", show_change_two);
	waitKey(0);
	
	for (int num = 0; num < 5; num++)
		dilate(show_change_two, show_change_two, element);
	imshow("二值后膨胀", show_change_two);
	waitKey(0);

	threshold(show_change_two, show_change_two, 90, 255, THRESH_BINARY);
	imshow("threshold展示图", show_change_two);
	waitKey(0);

	

	Mat Edge_result;// = Mat::zeros(channel_two.rows, channel_two.cols, CV_16S);
	Mat Edge_result_abs= Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
	Sobel(show_change_two, Edge_result, CV_16S, 2, 0, 3, 1, 1, BORDER_DEFAULT);
	//Canny(show_change_two, Edge_result_abs,100,255);
	convertScaleAbs(Edge_result, Edge_result_abs);
	namedWindow("Sobel第一幅图进行边缘处理结果图", WINDOW_NORMAL);
	imshow("Sobel第一幅图进行边缘处理结果图", Edge_result_abs);	
	waitKey(0);

	Mat Edge_result_two;
	threshold(Edge_result_abs, Edge_result_two, 100, 255, THRESH_BINARY);
	namedWindow("二值重新赋值结果图", WINDOW_NORMAL);
	imshow("二值重新赋值结果图", Edge_result_two);
	//drawhis(Edge_result_two, "erzhitu");
	waitKey(0);


	//Mat Edge_result111 = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
	for (int num = 0; num < 2; num++)
		dilate(Edge_result_two, Edge_result_two, element);
	namedWindow("再次进行2次膨胀结果图", WINDOW_NORMAL);
	imshow("再次进行2次膨胀结果图", Edge_result_two);
	
	waitKey(0);/**/
#pragma endregion

#pragma region 边框处理
	Mat result = Mat::zeros(channel_two.rows, channel_two.cols, CV_8UC1);
	vector<vector<Point>> contours_one;
	vector<Vec4i>hierarchy_one;
	findContours(Edge_result_two.clone(), contours_one, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE, Point(0, 0));
	vector<vector<Point>> afterFilter;
	cout << contours_one.size() << endl;
	vector<vector<Point>>::iterator itc = contours_one.begin();
	for (size_t c = 0; c < contours_one.size(); c++)
	{
		double area = contourArea(contours_one[c]);
		cout << area << endl;
		if (area > 800)
			afterFilter.push_back(contours_one[c]);
	}
	drawContours(result, afterFilter, -1, Scalar(255), FILLED);
	namedWindow("去除小面积结果图", WINDOW_NORMAL);
	imshow("去除小面积结果图", result);
	waitKey(0);



	for (int num = 0; num < 5; num++)
		dilate(result, result, element);
	imshow("连接下面的部分5次膨胀结果图", result);
	morphologyEx(result, result, MORPH_CLOSE, element, Point(-1, -1), 10);
	namedWindow("闭运算再次迭代10次结果图", WINDOW_NORMAL);
	imshow("闭运算再次迭代10次结果图", result);
	waitKey(0);



	Mat result_two = Mat::zeros(model_show.rows, model_show.cols, CV_8UC1);
	vector<vector<Point>> contours_two;
	vector<Vec4i>hierarchy_two;
	//Mat result_uchar = Mat::zeros(result.rows, result.cols, CV_8UC1);
	//result.convertTo(result_uchar, CV_8UC1);
	findContours(result.clone(), contours_two, hierarchy_two, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE, Point(0, 0));
	vector<vector<Point>> afterFilter_three;
	for (size_t c = 0; c < contours_two.size(); c++)
	{
		double area = contourArea(contours_two[c]);
		cout << area << endl;
		if (area > 5000)
			afterFilter_three.push_back(contours_two[c]);
	}
	drawContours(result_two, afterFilter_three, -1, Scalar(255), FILLED);
	namedWindow("再次去除去除小面积结果图", WINDOW_NORMAL);
	imshow("再次去除去除小面积结果图", result_two);
	waitKey(0);
#pragma endregion

	

	//转灰度画框
	Mat result_uchar = Mat::zeros(result.rows, result.cols, CV_8UC1);
	result_two.convertTo(result_uchar, CV_8UC1);
	vector<vector<Point> > contours_three;
	vector<Vec4i> hierarchy_three;
	findContours(result_uchar.clone(), contours_three, hierarchy_three, RETR_EXTERNAL, CHAIN_APPROX_SIMPLE, Point(0, 0));
	Mat rectangle_one = Mat::zeros(model_show.rows, model_show.cols, CV_8UC3);
	vector<Rect> boundRect_one(contours_three.size());  //定义外接矩形集合
	vector<RotatedRect> box_one(contours_three.size());
	Point2f rect_one[4];
	cout << "contours_three.size()" << contours_three.size() << endl;

	vector<vector<Point2f>> rec_vec(contours_three.size());
	vector<float>center_one(contours_three.size());
	vector<float>center_one_y(contours_three.size());
	for (int i = 0; i < contours_three.size(); i++)
	{
		box_one[i] = minAreaRect(Mat(contours_three[i]));  //计算外界旋转矩形
		boundRect_one[i] = boundingRect(Mat(contours_three[i]));//计算每个轮廓最小外接矩形
		circle(rectangle_one, Point(box_one[i].center.x, box_one[i].center.y), 5, Scalar(0, 255, 0), -1, 8);  //绘制最旋转矩形的中心点
		box_one[i].points(rect_one);  //把最小外接矩形四个端点复制给rect数组  复制构造
		rectangle(rectangle_one, boundRect_one[i], Scalar(0, 255, 0), 2, 8);//Point(boundRect_one[i].x, boundRect_one[i].y), Point(boundRect_one[i].x + boundRect_one[i].width, boundRect_one[i].y + boundRect_one[i].height)
		//cout << "start" <<center_one.size() << endl;	
		center_one[i] = box_one[i].center.x;
		center_one_y[i] = box_one[i].center.y;
		//cout << "end" <<center_one.size() << endl;
		for (int j = 0; j < 4; j++)
		{
			line(rectangle_one, rect_one[j], rect_one[(j + 1) % 4], Scalar(0, 0, 255), 2, 8);  //绘制旋转矩形每条边
			rec_vec[i].push_back(rect_one[j]);			/*cout << "第"<<j<<"个角点"<< rect[j] << endl;*/
		}
		namedWindow("绘制最小外接矩形结果图", WINDOW_NORMAL);
		imshow("绘制最小外接矩形结果图", rectangle_one);
		waitKey(0);
	}

	
	/*//通过矩计算中心距
	vector<Moments> moment_all(contours_three.size());
	vector<Point2f> momentcent(contours_three.size());
	for (int i = 0; i < contours_three.size(); i++)
	{
		moment_all[i] = moments(contours_three[i],false);
		momentcent[i].x = moment_all[i].m10 / moment_all[i].m00;
		momentcent[i].y = moment_all[i].m01 / moment_all[i].m00;
		circle(rectangle_one, Point(momentcent[i].x, momentcent[i].y), 10, Scalar(255,0 , 0), -1, 8);  //绘制最旋转矩形的中心点
	}
	imshow("绘制最小外接矩形结果图", rectangle_one);
	waitKey(0);*/

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
		point_three.x = fabsf(point_three.x);
		point_three.y = fabsf(point_three.y);
		Rect rect(point_one.x, point_one.y, point_three.x, point_three.y);
		Mat capture_one = channel_one(rect);
		//imshow("截图第一幅图结果图", capture_one);
		waitKey(0);
		Point point_four;
		point_four.x = point_one.x + point_three.x*0.5;
		point_four.y = point_one.y + N;
		circle(channel_one, point_four, 9, Scalar(0, 0, 255));
		cout << "贴标签x坐标值" << "= " << sprit_all[0].at<float>(point_four.y, point_four.x) << endl;
		cout << "贴标签y坐标值" << "= " << sprit_all[1].at<float>(point_four.y, point_four.x) << endl;
		cout << "贴标签z坐标值" << "= " << sprit_all[2].at<float>(point_four.y, point_four.x) << endl;
		namedWindow("moxingtu", WINDOW_NORMAL);
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
	fs2.release();

}

