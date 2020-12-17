**Cockroach**
 <p align="center">
  <img width="400" height="410" src="https://github.com/9and3/Cockroach/blob/Cockroach/Cockroach_logo.png">
</p>


Cockroach is a plug-in developed to introduce various commands for point cloud post-processing and meshing into Rhinoceros® environment based on reference functions already existing in the open source library Open3D [1]. Its focus is on fast and easy-to-use geometric manipulation, characterization and decomposition of point clouds directly from Rhinoceros®. We would like to thank Dale Fugier (McNeel) for his precious help in implementing these commands into C++ RhinoCommon.

[1] Zhou, Park, and Koltun, Open3D: A Modern Library for 3D Data Processing.


**Core Features**

The first release includes 13 Rhino commands: 

Cockroach_BallPivoting : a mesh reconstruction algorithm to reconstruct a triangulated mesh from a point cloud;
Cockroach_Poisson : a mesh reconstruction algorithm to obtain water-tight smoothed meshes from point clouds;
Cockroach_Crop : to crop a point cloud with a box;
Cockroach_DBSCANclustering : a density-based clustering method to obtain separate point clouds from a bigger one;
Cockroach_Downsample : reduce the size of a point cloud by a number threshold of points;
Cockroach_VoxelDownsample : reduce the number of point clouds by voxelization;
Cockroach_MeshSmooth : to uniformly smooth a mesh;
Cockroach_PlaneSegmentation : removal of point cloud planes by RANSAC;
Cockroach_PointCloudNormals : to estimate and correctly orient normal even for unstructured point clouds;
Cockroach_PopulatePoints : to obtain a randomly populated point cloud from a mesh;
Cockroach_PopulatePointsUniform : to obtain an uniformly populated point cloud from a mesh;
Cockroach_RadiusOutlierRemoval : to filter noise (outlier points) from point clouds by distance radius;
Cockroach_RemoveStatisticalOutliers : to filter noise (outlier points) from point clouds by KdTree search;


**Next release**

Cockroach is for now just a Rhino plug-in. We are planning to make a GH plug-in and release the full source code in a near future.

**Citation**

@misc{IBOIS2020,
   author  = {Petras Vestartas and Andrea Settimi},
   title   = {{Cockroach}: {A} plug-in for point cloud post-processing and meshing in {Rhino} environment},
   journal = {EPFL ENAC ICC IBOIS},
   year    = {2020}
}

**License**

Cockroach is released under the MIT license. If you use Cockroach in published work, please cite also the third-party libraries we used: Open3D.
