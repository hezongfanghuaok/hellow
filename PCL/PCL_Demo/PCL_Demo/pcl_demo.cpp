#include<pcl/visualization/cloud_viewer.h>
#include<pcl/io/io.h>
#include<pcl/io/pcd_io.h>//pcd 读写类相关的头文件。
#include<pcl/io/ply_io.h>
#include<pcl/point_types.h> //PCL中支持的点类型头文件。
#include <pcl\io\pcd_io.h>
#include <pcl\point_cloud.h>
#include"stdafx.h"

typedef pcl::PointXYZRGB PointT;
typedef pcl::PointCloud<PointT> PointCloudT;

boost::mutex cloud_mutex;
int user_data;
using std::cout;

#include <pcl\visualization\pcl_visualizer.h>


//定义回调参数结构体 
struct callback_args {
	// structure used to pass arguments to the callback function
	PointCloudT::Ptr clicked_points_3d;
	pcl::visualization::PCLVisualizer::Ptr viewerPtr;
};


void viewerOneOff(pcl::visualization::PCLVisualizer& viewer)
{
	viewer.setBackgroundColor(1.0, 0.5, 1.0);   //设置背景颜色
}
// 回调处理事件  这里是点选取事件
void pp_callback(const pcl::visualization::PointPickingEvent& event, void *args)
{
	struct callback_args * data = (struct callback_args *)args;//点云数据 & 可视化窗口
	if (event.getPointIndex() == -1)
		return;
	PointT current_point;
	event.getPoint(current_point.x, current_point.y, current_point.z);
	data->clicked_points_3d->points.push_back(current_point);
	//Draw clicked points in red:
	pcl::visualization::PointCloudColorHandlerCustom<PointT> red(data->clicked_points_3d, 255, 0, 0);
	data->viewerPtr->removePointCloud("clicked_points");
	data->viewerPtr->addPointCloud(data->clicked_points_3d, red, "clicked_points");
	data->viewerPtr->setPointCloudRenderingProperties(pcl::visualization::PCL_VISUALIZER_POINT_SIZE, 10, "clicked_points");
	std::cout << current_point.x << " " << current_point.y << " " << current_point.z << std::endl;

}
//拾取点坐标
void main()
{
	std::string filename("D:\\ScanData.pcd");
	//visualizer
	pcl::PointCloud<pcl::PointXYZ>::Ptr cloud(new pcl::PointCloud<pcl::PointXYZ>());
	pcl::visualization::PCLVisualizer::Ptr viewer(new pcl::visualization::PCLVisualizer("viewer"));

	if (pcl::io::loadPCDFile(filename, *cloud))
	{
		std::cerr << "ERROR: Cannot open file " << filename << "! Aborting..." << std::endl;
		return;
	}
	std::cout << cloud->points.size() << std::endl;

	cloud_mutex.lock();    // for not overwriting the point cloud

	// Display pointcloud:
	viewer->addPointCloud(cloud, "bunny_source");
	viewer->setCameraPosition(0, 0, -2, 0, -1, 0, 0);

	// Add point picking callback to viewer:
	struct callback_args cb_args;
	PointCloudT::Ptr clicked_points_3d(new PointCloudT);
	cb_args.clicked_points_3d = clicked_points_3d;
	cb_args.viewerPtr = pcl::visualization::PCLVisualizer::Ptr(viewer);

	viewer->registerPointPickingCallback(pp_callback, (void*)&cb_args);


	std::cout << "Shift+click on three floor points, then press 'Q'..." << std::endl;

	// Spin until 'Q' is pressed:
	viewer->spin();
	std::cout << "done." << std::endl;

	cloud_mutex.unlock();

	while (!viewer->wasStopped())
	{
		viewer->spinOnce(100);
		boost::this_thread::sleep(boost::posix_time::microseconds(100000));
	}
}

int main2()
	{

		pcl::PointCloud<pcl::PointXYZ>::Ptr cloud(new pcl::PointCloud<pcl::PointXYZ>);

		char strfilepath[256] = "D:\\ScanData.pcd";//ddg.yaml
		if ( pcl::io::loadPCDFile(strfilepath, *cloud)==-1 ) {
			cout << "error input!" << endl;
			return -1;
	}

	cout << cloud->points.size() << endl;

	for (int i = 0; i < 1000; i++)
	{
		cout << cloud->points[i].x << endl;
		cout << cloud->points[i].y << endl;
		cout << cloud->points[i].z << endl;
	}

	pcl::visualization::CloudViewer viewer("Cloud Viewer");     //创建viewer对象

	viewer.showCloud(cloud);
	viewer.runOnVisualizationThreadOnce(viewerOneOff);
	system("pause");
	return 0;
}

