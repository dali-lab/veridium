## seeing-is-believing
# Core Scripts

The Core module implements the functionality needed to procedurally generate and display Bravais lattice structures in a Unity scene.

## Usage

`StructureBuilder.cs` is the Unity Monobehavior GameObject script used to build and render the lattice structure from within the scene. To use StructureBuilder, call its public `BuildCell` method:

```c#
public void BuildCell(CellType type, CellVariation variation, CrystalState state, float sideLength, float sphereRadius, int atomicNumber = 0);
```
BuildCell takes in the following parameters:

* **type**: A `CellType` enum corresponding to the crystal structure of the lattice. There are 7 possible values (defined in `UnitCell6.cs`).
* **variation** A `CellVariation` enum corresponding to the structure variation present in the structure.
* **state** A `CrystalState` enum that describes the display configuration for the molecule: options include `CrystalState.SINGLECELL` and `CrystalState.INFINITE` for single celled and infinite views respectively
* **sideLength** This parameter is currently unused. In previous versions it was used to describe the size of cubic unit cells. The function needs to be updated in coordination with the Interactions module
* **sphereRadius** The radius of the atom objects in the structure
* **atomicNumber** Optional parameter used to instantiate the atomic number property of the atoms in the structure.

In its current implementation, BuildCell will draw the crystal described by these parameters at the location of the StructureBuilder's GameObject. The sideLength parameter isn't used. Instead, the default values in `Constants.cs` are used to size the dimensions and internal angles of the Unit Cell.

## Implementation

### Prerequisite Background

You should be familiar with the basic concepts behind Bravais structures and Unit Cells to understand the logic behind this module.

### Atom and Bond

The `Atom` class is used to store positioning information for a single Atom/Vertex in a Bravais Crystal structure. It also has functionality to Render itself in the VR scene. The Bond class does the exact same thing for unit cell bonds.

### UnitCell

This module draws a distinction between 8-sided (hexagonal) and 6-sided (all other) Unit Cells because they differ in terms of vertex numbering and layout. However, the `UnitCell` abstract class defines a set of common interfacing functionality that the two classes (`UnitCell6` and `UnitCell8`) share. By instantiating all UnitCells into this abstract definition, we can store them in the same data structures and iterate over them using the same functions.

The `UnitCell` abstract class defines the following functions which are in turn implemented by extending classes:

```c#
public abstract void AddVertices(Dictionary<Vector3, Atom> crystalAtoms);
public abstract void AddBonds(Dictionary<Vector3, Bond> crystalBonds);
public abstract Atom[] GetVertices();
public abstract List<Bond> GetBonds();
public abstract void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder);
public abstract void GenerateNeighbors(Dictionary<Vector3, Atom> crystalAtoms, Dictionary<Vector3, Bond> crystalBonds, Dictionary<Vector3, UnitCell> crystalCells);
public abstract List<Atom> GetMillerAtoms(int h, int k, int l);
public abstract string Debug();
```

* **AddVertices** Initializes the vertices in the cell by building Atom objects at the locations specified by the cell's structure. Adds the Atoms to a provided position dictionary.
* **AddBonds** Initializes bonds between the vertices. Adds the Bonds to a provided position dictionary.
* **GetVertices** returns an array of the cell's vertices
* **GetBonds** returns a list of the cell's bonds
* **Draw** renders the cell to the screen
* **GenerateNeighbors** is designed to be used inside a `Crystal` object instance. It generates duplicates of the UnitCell exactly adjacent to every side so that at least 4 of the vertices of the original are shared with the duplicate. It then adds the newly generated atoms, bonds, and Unit Cells to the Crystal's dictionaries.
* **GetMillerAtoms** Returns a list of Atoms corresponding to the Miller indices passed in as parameters.
* **Debug** Returns a string used to debug the cell's properties

#### UnitCell6

The `UnitCell6` class is the specific implementation of UnitCells for 6 sided Bravais lattice structures (all but Hexagonal). The following points are notable in their uniqueness to the UnitCell6 class:

* Since many six sided unit cells can have variable values for both their side lengths and internal angles, the `UnitCell6` constructor takes in values for a, b, and c which represent the three side lengths of the six-sided lattice, and alpha, beta, and gamma which represent the internal angles of the cell. The constructor contains an extensive switch statement to verify that the these values are valid for the type and variation of the cell. Invalid values are replaced with valid ones automatically.
* Since the UnitCell6 class encompasses cells with various angular constraints and sizing, its `AddVertices` implementation needs to calculate the positioning of its Atoms in 3D space with these constraints in mind. This calculation starts with the `Constants.cell6BasicPositions`, a class constant array that maps vertices from their indices to their *untransformed relative positions* within the Unit Cell. These untransformed relative position values are not the actual positions of the vertices in the cell. Instead they serve as references for where the vertices should be relative to the center of the cell. For example, a value of (1, 1, 1) means that the vertex is in the upper corner of the cell in the positive x/y/z octant of a 3D cartesian coordinate system centered around the Unit Cell. The actual position is then calculated from this relative position using the dimensions and angles of the specific unit cell. Specifically, the `GenerateVertexPosition` function of UnitCell6 generates these positions by applying angular transforms to the three primitive vectors that describe the cell. To learn more about Unit Cell primitive vectors, go [here](https://www.csub.edu/~adzyubenko/Phys313/AM_Ch4.pdf).


#### UnitCell8

* The UnitCell8 constructor takes only a baseLength, and a height to generate the cell's dimensions. Since the base must be a perfect hexagon, this is all that's required.
* The `AddVertices` function in UnitCell8 also relies on transformations of untransformed relative positions. However, since the angles are constant, these transformations are preformed through direct scaling.


### Crystal

The overarching structure used to create and render complete Bravais lattices is defined in the Crystal class. The class contains three Dictionaries, each mapping from positions to objects in the lattice. The `atoms` dictionary maps from the Vector3 positions of  each atom in the lattice to the actual `Atom` instance at that location. The `bonds` dictionary maps from positions to bonds and the `unitCells` dictionary maps from positions to unit cells. The Crystal defines the following functionality.

```c#
public void Construct(CellType type, CellVariation variation, float a, float b, float c, float alpha, float beta, float gamma, int atomicNumber, int constructionDepth);
public void Draw(GameObject atomPrefab, GameObject linePrefab, GameObject builder);
public void ClearCrystal(GameObject builder);
GetMillerAtoms(int h, int k , int l);
public string Debug();
```

* **Construct** Creates an infinite crystal at a specified recursion depth from a single unit cell. It instantiates the UnitCell of the specified type and variation at the crystal's centerpoint, adding the new atoms, bonds, and cell to their corresponding dictionaries. Then, it calls `GenerateNeighbors` on every UnitCell in the dictionaries as many times as the recursion depth specifies.
* **Draw** Renders all the atoms and bonds to the scene
* **ClearCrystal** Removes the atoms and bonds from the scene and resets the local dictionaries
* **GetMillerAtoms** Returns all the miller atoms within the crystal for the given miller indices
* **Debug** Returns a string that describes the crystal for debugging.

### Miller

`Miller.cs` contains the functions used to verify whether a point is on a plane specified by some miller indices, and retrieve the miller indices for a cell:

```c#
public static bool PointInMillerPlane(Vector3 point, int h, int k, int l, Vector3 origin, Vector3 a1, Vector3 a2, Vector3 a3, float planarSeparation);
public static List<Vector3> GetMillerIndecesForCell(CellType type, CellVariation variation);
```

* **PointInMillerPlane** uses a reciprocal lattice transform to construct a plane defined by three miller indices. It returns true if the provided point is on that plane.
* **GetMillerIndicesForCell** Returns the valid miller indices for each cell type and variation. This function is incomplete and needs more work.

### Constants

`Constants.cs` is a static class where class constants are stored.

### CoreTests

`CoreTests.cs` defines a set of tests for each Crystal/UnitCell type and variation.

## Authors

* Siddharth Hathi '24, Developer
* Andy Kotz '24, Developer
