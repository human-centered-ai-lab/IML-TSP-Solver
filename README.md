# IML-TSP-Solver

More information of the iML project: http://hci-kdd.org/project/iml/

Ant algorithm readings:
	- Ant colony system: a cooperative learning approach to the traveling salesman 
 	  problem" - 1996
	- "An  investigation  of  some  properties of an Ant algorithm" - 1992

Simple C# implementation (currently in an Unity project) of some Ant Algorithms with iML elements.
With this implementation it is possible to include some real world agents(humans) in the algorithm loop. 

currently supported algorithms:
	- AS
	- ACS

Usage:
    AntAlgorithmChooser aac = new AntAlgorithmChooser(...)
    AntAlgorithm aa = aac.getAlgorithm(); 

Please make sure, that you are using the proper parameter setting, which is explained in the literature about the ant algorithms.
