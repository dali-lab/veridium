## veridium
# Animation Scripts

The Animation system is used to create interactive animated lectures for chemistry and physics visualization. The design philosophy behind this system is high modularity, code reuse, and designer-friendly control in unity's GUIs.

## Usage

Animations can be used in two ways. 
* `AnimationBase.cs` can be extended to create animations, and those can be attached to GameObjects for desired behavior
* `AnimSequence.cs` can chain audio and `AnimationBase` instances and children to create entire lectures

### Animations
`AnimationBase` instances typically affect the GameObject they are attached to, but may occasionally cause behaviors on other objects, especially when one script needs to affect multiple GameObjects. `AnimationBase` children are named by convention "Anim_x", where x is the action that the animation performs. If the Animation is highly specific to one lecture, it should be named "Lec1_x", where the number denotes the lecture's identity. Highly specific animations should, however, be avoided. Animations should be small and modular, for example moving an object, adding a set of atoms to a structure, or highlighting one or more objects. Animations being small and modular increases the likelihood that they might be reused within a lecture or used for multiple lectures. `AnimationBase` has the following parameters:
* `bool playOnStart`, whether the animation should start playing right away
* `bool indefiniteDuration`, whether the animation should ever stop playing on its own
* `float duration`, how long the animation should play before it stops. Ignored if `indefiniteDuration` is true

### Creating a new Animation
When creating a new Animation, duplicate the `Anim_Template.cs` class. It comes with the following life cycle functions:
* `public override void Play()` Called when an animation starts playing or resumes playing. Setup code should happen here
* `public override void End()` Called when an animation reaches its end. Cleanup code should happen here
* `public override void Pause()` Called when animation is paused or reaches its end
* `protected override void ResetChild()` Called when animation restarts. This should undo whatever the animation did
* `protected override void UpdateAnim()` Called every frame while animation is playing

You also have access to the following properties:
* `float elapsedTime` Time in seconds since the animation started playing
* `float elapsedTimePercent` elapsedTime divided by duration. This is useful as a 0-1 measure of progress
* `bool playing` whether the animation is currently playing
* `bool beginPlaying` whether the animation has started and not been reset. True even if ended or paused
* `AnimSequence animSequence` a reference to the `AnimSequence` that controls the animation. Null if animation is stand-alone

Duplicate the file `Anim_Template.cs` and rename it to make a new animation. Then simply fill in the life cycle functions.

### Reversibility
A note about design philosophy when it comes to animations: animations should ideally be deterministic and analytical. That is to say, the state of an animation should be a function of time, rather than a dynamic system in which the state in each frame depends on the state of the previous frame. The reasoning behind this is reversibility. AnimSequences can be "scrubbed" or moved back and forth through at will, so in dynamic systems, errors will accumulate with repeated reversal. 

### Creating Lectures
To create a lecture using `AnimSequence`, create a prefab and put an `AnimSequence` component on it. Also add an `AudioSource` component and ensure that `AnimSequence`'s reference to that audio source is set. Then, fill in the list of segments with lecture segments. Each segment includes:
* A reference to an audio clip which will form the instruction of the lecture
* A list of animations, each with a `timing` attribute.

You can reference an `AnimationBase` that exists in the scene (such as the Anim_SpinUp on the Structure GameObject) or you can create your own on a new GameObject as a child of the prefab and reference that. You can also choose a Unity Event to trigger instead of the AnimationBase. Or you can select a Unity Animator to trigger, although this feature has not yet been fully implemented.

Once you have chosen an AnimationBase or a Unity Event, you can set the timing of the action. The timing is a float that determines the time in seconds after the beginning of the segment (the audio clip) that the action should occur. Change the ActionType to tell the AnimSequence which type of action to perform.

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

In which x is a value between 0 and 1. These can be used to access the following easing types:
* Linear (no easing)
* Quadratic (best for simple movement, simulated constant acceleration/deceleration)
* Exponential
* Back (overshoots target slightly before settling)
* Elastic (simulates damped spring)
* Bounce
* Pointer (satisfying jump forward and settle back)

Feel free to add your own easing functions to `Easing` if you want to use them in your animations.