# Object detection with acustic feedback
This repository contains the Unity project for using the motion capture proximity feedback system.
## Starting the Unity project
In order to avoid possible erros, you need to use Unity version <b>2021.3.15f1</b>

Once Unity has opend up the project, go to <b>Assets -> Scenes</b> and double click on <b>"Lab scene"</b> to select the motion capture environment.

## Using the system
In order to use the system you need to include into Unity the real world objects that you want to detect. For doing so, you need to:
- define a new gameobject
- add collider to gameobject
- add optitrack rigid body script
- assign the associated rigid body id from motive or other motion capture framework

At this point, when running the program, the gameobject will be positioned in the same spot as the real one.

If you want to track a person, you need to follow the previous steps, but also add the following:
- add audio source component
- add main script and assign audio clip for each proximity step
- you can remove the collider from the person's gameobject

If you start the scene you should see the the person's gameobject move in the same way as the associated real world person.
