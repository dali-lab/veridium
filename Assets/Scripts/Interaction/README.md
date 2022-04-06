## veridium
# Interaction Scripts

The interaction scripts handle the user's interaction with the scene, and many of the basic functionalities of the element structures.

## Usage

A podium can be created with the `ElementLoader.cs`, `StructureBase.cs`, and `StructureController.cs` scripts. See the `StructureBase` prefab for this configuration.

`HandDistanceGrabbable.cs` and `HandDistanceGrabber.cs` are useful for making distance grabbable objects and interactors. Use them in place of the XR interaction toolkit Direct interactor and grabbable components. Distance grabbable object must be on the `DistanceGrab` layer. Add a `RayCaster.cs` component to the interactor to enable the visualization.

`ExitSceneTile.cs` can be added to a grab interactable object to create a scene exit tile. 

`XRGrabInteractable_Lockable` adds a locking functionality to the XRGrabInteractable component. New grab interactables should inherit from this class.

## Implementation

### `ElementLoader.cs`

ElementLoader is a socket that takes element tiles and then prompts the StructureBase to load them into the scene with `StructureBase.ElementAdded()`. It also handles the removal of the element with `StructureBase.ElementRemoved()`.

### `StructureBase.cs`

StructureBase handles the creation and destruction of the element structure as well as switching the structure between view modes (single celled, multi celled, and infinite) as well as changing between close packed and ball and stick layouts. 

Structure base should be used only with the `StructureBase` prefab. This prefab includes a `Structure` GameObject, which will be the object that is held by the user. It includes a `StructureConroller.cs` component which handles the user's interaction with the structure. The game object `StructureBuilder` is the actual parent of the spheres and cylinders that make up the structure, and is responsible for building them. `Attach` is an empty game object that is used by the StructureController to keep the structure's relative position to the user when grabbed. The `ScaleGrabber` game object is not active unless the structure is already held, and it enables a second grabbing point so that the user can two hand grab the structure. 

### `StructureController.cs`

StructureController manages mainly two things: maintaining the grab offset during one hand grab, and managing the rotation and scale of the structure during two hand grab. It should be given a min scale and max scale and references to the two direct interactors that are used to grab the structure.