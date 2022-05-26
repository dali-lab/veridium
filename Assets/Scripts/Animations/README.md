## veridium
# Animation Scripts

The Animation system is used to create interactive animated lectures for chemistry and physics visualization. The design philosophy behind this system is high modularity, code reuse, and designer-friendly control in unity's GUIs.

## Usage

Animations can be used in two ways. 
* `AnimationBase.cs` can be extended to create animations, and those can be instantiated through an AnimPlayer and attached to a GameObject
* `AnimSequence_OLD.cs` can chain audio and `AnimationBase` instances and children to create entire lectures

### Animations
`AnimationBase` instance typically affect the GameObject they are attached to, but may occasionally cause behaviors on other objects, especially when one script needs to affect multiple GameObjects. If the Animation is highly specific to one lecture, it should be named "Lec1_x", where the number denotes the lecture's identity. Highly specific animations should, however, be avoided. Animations should be small and modular, for example moving an object, adding a set of atoms to a structure, or highlighting one or more objects. Animations being small and modular increases the likelihood that they might be reused within a lecture or used for multiple lectures. `AnimationBase` has the following parameters:
* `bool indefiniteDuration`, whether the animation should ever stop playing on its own
* `float duration`, how long the animation should play before it stops. Ignored if `indefiniteDuration` is true

### Creating a new Animation
When creating a new Animation, duplicate the `AnimationTemplate.cs` class. It comes with the following life cycle functions:
* `public override void Play()` Called when an animation starts playing or resumes playing. Setup code should happen here
* `public override void End()` Called when an animation reaches its end. Cleanup code should happen here
* `public override void Pause()` Called when animation is paused or reaches its end
* `protected override void ResetChild()` Called when animation restarts. This should undo whatever the animation did
* `protected override void UpdateAnim()` Called every frame while animation is playing
You also have access to the following properties:
* `float elapsedTime` Time in seconds since the animation started playing
* `float elapsedTimePercent` elapstedTime divided by duration. This is useful as a 0-1 measure of progress
* `bool playing` whether the animation is currently playing
* `bool beginPlaying` whether the animation has started and not been reset. True even if ended or paused
* `AnimationManger manager` a reference to either the `AnimSequence` or the `AnimPlayer` that controls the animation. Should almost never be null.

Duplicate the file `AnimationTemplate.cs` and rename it to make a new animation. Then simply fill in the life cycle functions.

### Reversibility
A note about design philosophy when it comes to animations: animations should ideally be deterministic and analytical. That is to say, the state of an animation should be a function of time, rather than a dynamic system in which the state in each frame depends on the state of the previous frame. The reasoning behind this is reversibility. AnimSequences can be "scrubbed" or moved back and forth through at will, so in dynamic systems, errors will accumulate with repeated reversal. 

### Creating Lectures
To create a lecture using `AnimSequence`, create a prefab and put an `AnimSequence` component on it. Also add an `AudioSource` component and ensure that `AnimSequence`'s reference to that audio source is set. Then, fill in the list of segments with lecture segments. Each segment includes:
* A reference to an audio clip which will form the instruction of the lecture
* A list of actions, each with a `timing` attribute and a `actionType` attribute.
The timing attribute is a float that determines the time in seconds after the beginning of the segment (the audio clip) that the action should occur.

To define the behavior of an action, select one of the following action types:
* `AnimationScript` allows you to select from a list of customizable `AnimationBase`s to play
* `Await` allows you to await a certain action from the user, from a pre-defined list of await options
* `UnityEvent` allows you to trigger functions on objects in the scene
* `Animator` allows you to set parameters in an animator object in the scene
* `AnimPlayerReference` references an AnimPlayer, which allows you to use `AnimationBase` objects that are on other GameObjects

If you select `AnimationScript` or `Await`, you will be given the option to choose from a list of available types.
AnimationScript types:
* `Add_Atoms` adds atoms to a structure in a set of steps. You can define an animation that plays when the atoms are added.
* `Fade` fades the opacity of a GameObject over the duration.
* `Glow` adds a constant emissive glow effect to a GameObject.
* `Glow_Pulse` adds a pulsing emissive glow effect to a GameObject.
* `Move_To` moves a GameObject to a certain position, rotation, and scale over the duration. You can modify some or all of those attributes.
* `Spin_Up` creates a satisfying spawn-in animation for a gameobject.
* `Play_On_Atoms` plays an animation on a set of atoms. You can define an animation that plays when the atoms are added, and it will be copied onto them.
* `Play_On_Object` allows you to define an animation, and it will copy that animation onto a target gameObject for the duration.

Await types:
* `Await_Continue` waits for the user to grab the continue button.
* `Await_Grab` waits for the user to grab some XRGrabInteractable.
* `Await_Insert` waits for a certain element to be put in a socket.
* `Await_Release` waits for the user to let go of some XRGrabInteractable.
* `Await_Any` takes a list of Awaits, and completes when any one of them is completed.
* `Await_Sequence` takes a list of Awaits, and completes when each of them is completed in order. Allows for more complex input.

AnimSequence will automatically move to the next segment when the audio clip finishes playing, unless there is still an AnimationBase playing or yet to play. An AnimationBase will always block the next segment from playing until it finishes unless it is set to `indefiniteDuration`. If you want to have user interaction block the transitions between segments, use `AwaitUserBase`, discussed in another section.

The `AnimSequence` will play the segments in order. The audio of each segment will play, and each animation will be played `timing` seconds after the beginning of the segment. For example, a segment could be created with a voice audio 25 seconds long, and if an animation is added with a `timing` value of 5, that animation will run five seconds after the audio starts playing. This can be used to sync animations with speech, so that visuals can accompany lecture content.

### Using AwaitUserBase

Sometimes you want to have user interaction block the transitions between segments. For example, if you want to have the user click a button to continue, you can use a child of `AwaitUserBase` to block the transition until the user clicks the button.

`AwaitUserBase` is simply an `AnimationBase` that does nothing but keep "playing" until some action is performed. When it is triggered, it will stop blocking the transition to the next segment and the lecture can continue.

To create your own behaviours with this script, create a child and call `CompleteAction()` when the desired action is performed.

### Easing
If you are not sure what an easing function is, visit [this page](https://easings.net). Easing functions are important for animation, so some common ones are included as static functions of `Easing`. Easing functions can be accessed through:
* `Easing.EaseIn(float x, EasingType easingType)`
* `Easing.EaseOut(float x, EasingType easingType)`
* `Easing.EaseFull(float x, EasingType easingType)`
Where x is a value between 0 and 1. These can be used to access the following easing types:
* Linear (no easing)
* Quadratic (best for simple movement, simulated constant acceleration/deceleration)
* Exponential (best for fading)
* Back (overshoots target slightly before settling)
* Elastic (simulates damped spring)
* Bounce (simulates bouncing like a ball)
* Pointer (satisfying jump forward and settle back)

Feel free to add your own easing functions to `Easing` if you want to use them in your animations.