1. ClipQuadProvider.cs is used for binding matrix to renderers with Material Property Block and achieving controls under effects by moving quad transform.

Parameters:
clipQuadRenderer - controlling quad renderers
targetRenderers - array of renderers wich we want to controlling
propertyName - shader matrix property name

2. Hex Metallic and Hex Specular shaders.
Lit shaders for Sci-Fi Dissolve effect.
MainTex, NormalMap, NormalScale, Specular Smoothness etc. is default PBR textures and parameters.
Hue Offset - offset for emission color hue.
Hex Pattern - 2 channel texture. R - is detail gradient for dissolve animation, G - is offset gradient to achieve some randomness. Using triplanar projection instead of UV.
Tiling - hex pattern tiling.
Falloff - a multiplier that controls the interpolated blend between the projected planes. Higher values means harder blends while lower means softer.
Hex Max Offset - randomness by green gradient.
Hex Color - main color of dissolve.
Distance Min, Max - min and max distance from clip quad (Z) for dissolve.
Hex Color 2 - additional color of dissolve emission.
Distance 2 Min, Max - min and max distance from clip quad (Z) for additional color.
Fresnel Color - color of fresnel gradient.
Fresnel Scale, Power - intensity and hardness of fresnel gradient.
Distance 3 Min, Max - min and max distance from clip quad (Z) for fresnel gradient.
Vertex Offset - how much vertices should be displaced when dissolved.
Levels Start - hardness of detail gradient of pattern at start of dissolving.
Levels End - hardness of detail gradient of pattern at end of dissolving.
Noise Influence - how much simplex 2D noise should affect to dissolve.
Noise Scale - size of noise.
Cull - Off, Front, Back face clipping.