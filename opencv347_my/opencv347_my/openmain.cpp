#include "stdafx.h"
using namespace cv;
using namespace std;
using namespace xfeatures2d;

const int LOOP_NUM = 10;
const int GOOD_PTS_MAX = 50;
const float GOOD_PORTION = 0.15f;


int openimage(string a);
void surf1();
template<class KPMatcher>
struct SURFMatcher
{	
	KPMatcher  matcher;
	template<class T>
	
	void match(const T& in1, const T& in2, std::vector<cv::DMatch>& matches)
	{
		matcher.match(in1, in2, matches);
	}
};

static Mat drawGoodMatches(
	const Mat& img1,
	const Mat& img2,
	const std::vector<KeyPoint>& keypoints1,
	const std::vector<KeyPoint>& keypoints2,
	std::vector<DMatch>& matches,
	std::vector<Point2f>& scene_corners_
)
{
	//-- Sort matches and preserve top 10% matches
	std::sort(matches.begin(), matches.end());
	std::vector< DMatch > good_matches;
	double minDist = matches.front().distance;
	double maxDist = matches.back().distance;

	const int ptsPairs = std::min(GOOD_PTS_MAX, (int)(matches.size() * GOOD_PORTION));
	for (int i = 0; i < ptsPairs; i++)
	{
		good_matches.push_back(matches[i]);
		
	}
	for (int i = 0; i < good_matches.size(); i++)
	{
		std::cout << "\good_matches distance"<<i<< ":" << good_matches[i].distance << ":" << good_matches[i] .queryIdx << ":" << good_matches[i].trainIdx<< std::endl;
	}

	std::cout << "\nMax distance: " << maxDist << std::endl;
	std::cout << "Min distance: " << minDist << std::endl;

	std::cout << "Calculating homography using " << ptsPairs << " point pairs." << std::endl;

	// drawing the results
	Mat img_matches;

	drawMatches(img1, keypoints1, img2, keypoints2,
		good_matches, img_matches, Scalar::all(-1), Scalar::all(-1),
		std::vector<char>(), DrawMatchesFlags::NOT_DRAW_SINGLE_POINTS);

	//-- Localize the object
	std::vector<Point2f> obj;
	std::vector<Point2f> scene;

	for (size_t i = 0; i < good_matches.size(); i++)
	{
		//-- Get the keypoints from the good matches
		obj.push_back(keypoints1[good_matches[i].queryIdx].pt);
		scene.push_back(keypoints2[good_matches[i].trainIdx].pt);
	}
	//-- Get the corners from the image_1 ( the object to be "detected" )
	std::vector<Point2f> obj_corners(4);
	obj_corners[0] = Point(0, 0);
	obj_corners[1] = Point(img1.cols, 0);
	obj_corners[2] = Point(img1.cols, img1.rows);
	obj_corners[3] = Point(0, img1.rows);
	std::vector<Point2f> scene_corners(4);

	Mat H = findHomography(obj, scene, RANSAC);
	perspectiveTransform(obj_corners, scene_corners, H);

	scene_corners_ = scene_corners;

	//-- Draw lines between the corners (the mapped object in the scene - image_2 )
	line(img_matches,
		scene_corners[0] + Point2f((float)img1.cols, 0), scene_corners[1] + Point2f((float)img1.cols, 0),
		Scalar(0, 255, 0), 2, LINE_AA);
	line(img_matches,
		scene_corners[1] + Point2f((float)img1.cols, 0), scene_corners[2] + Point2f((float)img1.cols, 0),
		Scalar(0, 255, 0), 2, LINE_AA);
	line(img_matches,
		scene_corners[2] + Point2f((float)img1.cols, 0), scene_corners[3] + Point2f((float)img1.cols, 0),
		Scalar(0, 255, 0), 2, LINE_AA);
	line(img_matches,
		scene_corners[3] + Point2f((float)img1.cols, 0), scene_corners[0] + Point2f((float)img1.cols, 0),
		Scalar(0, 255, 0), 2, LINE_AA);
	return img_matches;
}

int main(int argc, char** argv)
{
	surf1();
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
void surf1()
{
	Mat img_1 = imread("D:\\myface.jpg", 0);
	Mat img_2 = imread("D:\\my.jpg", 0);

	Ptr<Feature2D> sift = xfeatures2d::SURF::create(100.0);

	vector<KeyPoint> keypoints_1, keypoints_2;
	Mat descriptors_1, descriptors_2;
	vector<DMatch> matches;//特征匹配结果存储类
	SURFMatcher<BFMatcher> matcher;//特征匹配类


	sift->detectAndCompute(img_1, noArray(), keypoints_1, descriptors_1);
	sift->detectAndCompute(img_2, noArray(), keypoints_2, descriptors_2);
	//keypoints_1
	matcher.match(descriptors_1, descriptors_2, matches);
	
	std::vector<Point2f> corner;
	Mat img_matches = drawGoodMatches(img_1, img_2, keypoints_1, keypoints_2, matches, corner);

	imshow("Match_Result", img_matches);

	
	waitKey(0);

}