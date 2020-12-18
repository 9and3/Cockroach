**Cockroach**
 <p align="center">
  <img width="400" height="410" src="https://github.com/9and3/Cockroach/blob/Cockroach/Cockroach_logo.png">
</p>


Cockroach is a plug-in developed to introduce various commands for point cloud post-processing and meshing into Rhinoceros® environment based on reference functions already existing in the open source library Open3D [1]. 

The pointcloud processing tools focus on:
1. fast and easy-to-use geometric manipulation, characterization and decomposition of point clouds directly in Rhinoceros6 and 7®. 
2. the better link between CAD modelling software (Rhino) and point-cloud processing.
3. integration of point-cloud processing with other frameworks such as easy-to-use .NET programming languages (C#, IronPython, VB) using the interface of Grasshopper, Rhino.
4. unified (as much as possible) approach across multiple backends to help researchers collaborate within academic or industrial partners. At IBOIS - EPFL, among other research lines, we have also being focusing on structures with unpredictable geometries such as raw wood and mineral scraps. These construction elements are scanned and post-processed into low-poly meshes or NURBS for design i.e. 3D timber joinery representation and fabrication tool-paths for 5-axis CNC, ABBIRB6400 Robot and XR manufacture.

We would like to thank Dale Fugier (McNeel) for his help during C++ plugin development into Rhinoceros®.

[1] Zhou, Park, and Koltun, Open3D: A Modern Library for 3D Data Processing.

**Dependecies**

You will need download the Open3D.dll and place it in the same plug-in folder.
There are two ways to get Open3D.dll

1. Download Cockroach from www.food4rhino.com/
2. Compile Open3D as a Dynamic Library from:
Open3D repository: https://github.com/intel-isl/Open3D/blob/master/README.md
Instructions (C++): https://github.com/intel-isl/Open3D/issues/2717

**Core Features**

The first release includes 13 Rhino commands: 

*Cockroach_BallPivoting* : a mesh reconstruction algorithm to reconstruct a triangulated mesh from a point cloud;

*Cockroach_Poisson* : a mesh reconstruction algorithm to obtain water-tight smoothed meshes from point clouds;

*Cockroach_Crop* : to crop a point cloud with a box;

*Cockroach_DBSCANclustering* : a density-based clustering method to obtain separate point clouds from a bigger one;

*Cockroach_Downsample* : reduce the size of a point cloud by a number threshold of points;

*Cockroach_VoxelDownsample* : reduce the number of point clouds by voxelization;

*Cockroach_MeshSmooth* : to uniformly smooth a mesh;

*Cockroach_PlaneSegmentation* : removal of point cloud planes by RANSAC;

*Cockroach_PointCloudNormals* : to estimate and correctly orient normal even for unstructured point clouds;

*Cockroach_PopulatePoints* : to obtain a randomly populated point cloud from a mesh;

*Cockroach_PopulatePointsUniform* : to obtain an uniformly populated point cloud from a mesh;

*Cockroach_RadiusOutlierRemoval* : to filter noise (outlier points) from point clouds by distance radius;

*Cockroach_RemoveStatisticalOutliers* : to filter noise (outlier points) from point clouds by KdTree search;

**Citation**

Please use this citation if you use Cockroach in published work. In addition, please cite also the third-party libraries we used: Open3D (https://github.com/intel-isl/Open3D/blob/master/README.md).

@misc{IBOIS2020,
   author  = {Petras Vestartas and Andrea Settimi},
   title   = {{Cockroach}: {A} plug-in for point cloud post-processing and meshing in {Rhino} environment},
   journal = {EPFL ENAC ICC IBOIS},
   url = {https://github.com/9and3/Cockroach},
   year    = {2020}
}

**License**

Cockroach is released under the MIT license. If you use Cockroach in published work, please cite also the third-party libraries we used: Open3D.
The code is not yet fully open as there are still developments in progress but will be soon. Nevertheless, the code can be made available upon request. We encourage use for both research and commercial purposes, as long as proper attribution is given. Feel free to also send us an email and let us know how Cockroach has been useful to you and how it can be improved. 

**Contact**

For code request or chat i.e. open github issue or contact us by email.
Some data, models or generated code using our research are available from the corresponding authors by request:
Petras Vestartas, petrasvestartas@gmail.com
Andrea Settimi, andrea.settimi@epfl.ch

**Next release**

Cockroach is currently a Rhino plug-in. 
We are planning to make a GH plug-in and release the full source code in a near future.


