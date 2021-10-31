// Upgrade NOTE: upgraded instancing buffer 'KnifeHexSpecular' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Knife/Hex Specular"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.01
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("MainTex", 2D) = "white" {}
		_NormalMap("Normal Map", 2D) = "bump" {}
		_NormalScale("Normal Scale", Float) = 1
		_SpecularSmoothness("Specular Smoothness", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0.354
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		_AOStrength("AO Strength", Float) = 0
		_HueOffset("Hue Offset", Range( 0 , 1)) = 0
		_HexPattern("Hex Pattern", 2D) = "white" {}
		_Tiling("Tiling", Float) = 1
		_Falloff("Falloff", Float) = 1
		_HexMaxOffset("Hex Max Offset", Float) = 0
		[HDR]_HexColor("Hex Color", Color) = (1.490777,6.498019,1.134088,0)
		_DistanceMin("Distance Min", Float) = 0
		_DistanceMax("Distance Max", Float) = 0
		[HDR]_HexColor2("Hex Color 2", Color) = (1.490777,6.498019,1.134088,0)
		_Distance2Min("Distance 2 Min", Float) = 0
		_Distance2Max("Distance 2 Max", Float) = 0
		_FresnelColor("Fresnel Color", Color) = (1,1,1,1)
		_FresnelScale("Fresnel Scale", Float) = 1
		_FresnelPower("Fresnel Power", Float) = 5
		_Distance3Min("Distance 3 Min", Float) = 0
		_Distance3Max("Distance 3 Max", Float) = 0
		_VertexOffset("Vertex Offset", Float) = 0
		_LevelsStart("Levels Start", Vector) = (0.99,1,0,1)
		_LevelsEnd("Levels End", Vector) = (0,1,0,1)
		_NoiseInfluence("Noise Influence", Float) = 0
		_NoiseScale("Noise Scale", Float) = 1
		[Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull [_Cull]
		CGINCLUDE
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma multi_compile_instancing
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float3 vertexToFrag52;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform float _Cull;
		uniform float _VertexOffset;
		uniform float _DistanceMin;
		uniform float _DistanceMax;
		uniform float _NoiseScale;
		uniform float _NoiseInfluence;
		uniform sampler2D _NormalMap;
		uniform float _NormalScale;
		uniform float4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _HexColor;
		uniform float _HueOffset;
		sampler2D _HexPattern;
		uniform float _Tiling;
		uniform float _Falloff;
		uniform float _HexMaxOffset;
		uniform float4 _LevelsEnd;
		uniform float4 _LevelsStart;
		uniform float _Distance2Min;
		uniform float _Distance2Max;
		uniform float4 _HexColor2;
		uniform float _FresnelScale;
		uniform float _FresnelPower;
		uniform float _Distance3Min;
		uniform float _Distance3Max;
		uniform float4 _FresnelColor;
		uniform sampler2D _SpecularSmoothness;
		uniform float _Smoothness;
		uniform sampler2D _AmbientOcclusion;
		uniform float _AOStrength;
		uniform float _Cutoff = 0.01;

		UNITY_INSTANCING_BUFFER_START(KnifeHexSpecular)
			UNITY_DEFINE_INSTANCED_PROP(float4x4, _ClipQuadMatrix)
#define _ClipQuadMatrix_arr KnifeHexSpecular
			UNITY_DEFINE_INSTANCED_PROP(float4, _NormalMap_ST)
#define _NormalMap_ST_arr KnifeHexSpecular
			UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define _MainTex_ST_arr KnifeHexSpecular
			UNITY_DEFINE_INSTANCED_PROP(float4, _SpecularSmoothness_ST)
#define _SpecularSmoothness_ST_arr KnifeHexSpecular
			UNITY_DEFINE_INSTANCED_PROP(float4, _AmbientOcclusion_ST)
#define _AmbientOcclusion_ST_arr KnifeHexSpecular
		UNITY_INSTANCING_BUFFER_END(KnifeHexSpecular)


		float4x4 Inverse4x4(float4x4 input)
		{
			#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
			float4x4 cofactors = float4x4(
			minor( _22_23_24, _32_33_34, _42_43_44 ),
			-minor( _21_23_24, _31_33_34, _41_43_44 ),
			minor( _21_22_24, _31_32_34, _41_42_44 ),
			-minor( _21_22_23, _31_32_33, _41_42_43 ),
		
			-minor( _12_13_14, _32_33_34, _42_43_44 ),
			minor( _11_13_14, _31_33_34, _41_43_44 ),
			-minor( _11_12_14, _31_32_34, _41_42_44 ),
			minor( _11_12_13, _31_32_33, _41_42_43 ),
		
			minor( _12_13_14, _22_23_24, _42_43_44 ),
			-minor( _11_13_14, _21_23_24, _41_43_44 ),
			minor( _11_12_14, _21_22_24, _41_42_44 ),
			-minor( _11_12_13, _21_22_23, _41_42_43 ),
		
			-minor( _12_13_14, _22_23_24, _32_33_34 ),
			minor( _11_13_14, _21_23_24, _31_33_34 ),
			-minor( _11_12_14, _21_22_24, _31_32_34 ),
			minor( _11_12_13, _21_22_23, _31_32_33 ));
			#undef minor
			return transpose( cofactors ) / determinant( input );
		}


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 HSVToRGB( float3 c )
		{
			float4 K = float4( 1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0 );
			float3 p = abs( frac( c.xxx + K.xyz ) * 6.0 - K.www );
			return c.z * lerp( K.xxx, saturate( p - K.xxx ), c.y );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
		}

		inline float4 TriplanarSampling4( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4x4 _ClipQuadMatrix_Instance = UNITY_ACCESS_INSTANCED_PROP(_ClipQuadMatrix_arr, _ClipQuadMatrix);
			float4x4 invertVal36 = Inverse4x4( _ClipQuadMatrix_Instance );
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float3 vertexToFrag52 = ase_worldPos;
			float4 appendResult12 = (float4(vertexToFrag52 , 1.0));
			float4 break14 = mul( _ClipQuadMatrix_Instance, appendResult12 );
			float3 appendResult32 = (float3(break14.x , break14.y , 0.0));
			float3 worldToObjDir38 = mul( unity_WorldToObject, float4( mul( invertVal36, float4( appendResult32 , 0.0 ) ).xyz, 0 ) ).xyz;
			float3 normalizeResult39 = normalize( worldToObjDir38 );
			float3 offset_direction102 = normalizeResult39;
			float2 appendResult133 = (float2(break14.x , break14.y));
			float simplePerlin2D132 = snoise( appendResult133*_NoiseScale );
			simplePerlin2D132 = simplePerlin2D132*0.5 + 0.5;
			float temp_output_134_0 = ( break14.z + ( simplePerlin2D132 * _NoiseInfluence ) );
			float smoothstepResult225 = smoothstep( _DistanceMin , _DistanceMax , temp_output_134_0);
			float clip_1_no_offset227 = saturate( smoothstepResult225 );
			float3 final_v_offset195 = ( offset_direction102 * _VertexOffset * clip_1_no_offset227 );
			v.vertex.xyz += final_v_offset195;
			v.vertex.w = 1;
			o.vertexToFrag52 = ase_worldPos;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float4 _NormalMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_NormalMap_ST_arr, _NormalMap_ST);
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST_Instance.xy + _NormalMap_ST_Instance.zw;
			float3 final_normal205 = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ), _NormalScale );
			o.Normal = final_normal205;
			float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
			float4 final_color207 = ( _Color * tex2D( _MainTex, uv_MainTex ) );
			o.Albedo = final_color207.rgb;
			float3 hsvTorgb213 = RGBToHSV( _HexColor.rgb );
			float hue_offset211 = _HueOffset;
			float3 hsvTorgb214 = HSVToRGB( float3(( hsvTorgb213.x + hue_offset211 ),hsvTorgb213.y,hsvTorgb213.z) );
			float4x4 _ClipQuadMatrix_Instance = UNITY_ACCESS_INSTANCED_PROP(_ClipQuadMatrix_arr, _ClipQuadMatrix);
			float4 appendResult12 = (float4(i.vertexToFrag52 , 1.0));
			float4 break14 = mul( _ClipQuadMatrix_Instance, appendResult12 );
			float2 appendResult133 = (float2(break14.x , break14.y));
			float simplePerlin2D132 = snoise( appendResult133*_NoiseScale );
			simplePerlin2D132 = simplePerlin2D132*0.5 + 0.5;
			float temp_output_134_0 = ( break14.z + ( simplePerlin2D132 * _NoiseInfluence ) );
			float2 temp_cast_2 = (_Tiling).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_vertexNormal = mul( unity_WorldToObject, float4( ase_worldNormal, 0 ) );
			float4 triplanar4 = TriplanarSampling4( _HexPattern, ase_vertex3Pos, ase_vertexNormal, _Falloff, temp_cast_2, 1.0, 0 );
			float hex_offset175 = (0.0 + (triplanar4.y - 0.0) * (-1.0 - 0.0) / (1.0 - 0.0));
			float temp_output_179_0 = ( hex_offset175 * _HexMaxOffset );
			float smoothstepResult18 = smoothstep( _DistanceMin , _DistanceMax , ( temp_output_134_0 + temp_output_179_0 ));
			float clip_1106 = saturate( smoothstepResult18 );
			float hex98 = triplanar4.r;
			float4 lerpResult64 = lerp( _LevelsEnd , _LevelsStart , clip_1106);
			float4 break65 = lerpResult64;
			float temp_output_120_0 = ( saturate( (break65.z + (hex98 - break65.x) * (break65.w - break65.z) / (break65.y - break65.x)) ) * ( 1.0 - clip_1106 ) );
			float smoothstepResult114 = smoothstep( _Distance2Min , _Distance2Max , ( temp_output_134_0 + temp_output_179_0 ));
			float clip_2116 = saturate( smoothstepResult114 );
			float3 hsvTorgb217 = RGBToHSV( _HexColor2.rgb );
			float3 hsvTorgb218 = HSVToRGB( float3(( hsvTorgb217.x + hue_offset211 ),hsvTorgb217.y,hsvTorgb217.z) );
			float temp_output_123_0 = ( saturate( (0.0 + (( 1.0 - hex98 ) - 0.95) * (1.0 - 0.0) / (1.0 - 0.95)) ) * ( 1.0 - clip_2116 ) );
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNdotV138 = dot( ase_worldNormal, ase_worldViewDir );
			float fresnelNode138 = ( 0.0 + _FresnelScale * pow( 1.0 - fresnelNdotV138, _FresnelPower ) );
			float smoothstepResult145 = smoothstep( _Distance3Min , _Distance3Max , temp_output_134_0);
			float fresnel_clip142 = saturate( smoothstepResult145 );
			float3 hsvTorgb221 = RGBToHSV( _FresnelColor.rgb );
			float3 hsvTorgb222 = HSVToRGB( float3(( hsvTorgb221.x + hue_offset211 ),hsvTorgb221.y,hsvTorgb221.z) );
			float3 fresnel149 = ( fresnelNode138 * fresnel_clip142 * hsvTorgb222 );
			float3 clampResult151 = clamp( ( ( hsvTorgb214 * clip_1106 * temp_output_120_0 ) + ( clip_2116 * hsvTorgb218 * temp_output_123_0 ) + fresnel149 ) , float3( 0,0,0 ) , float4(999,999,999,999).xyz );
			float3 final_emission203 = clampResult151;
			o.Emission = final_emission203;
			float4 _SpecularSmoothness_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_SpecularSmoothness_ST_arr, _SpecularSmoothness_ST);
			float2 uv_SpecularSmoothness = i.uv_texcoord * _SpecularSmoothness_ST_Instance.xy + _SpecularSmoothness_ST_Instance.zw;
			float4 tex2DNode186 = tex2D( _SpecularSmoothness, uv_SpecularSmoothness );
			float4 final_specular201 = tex2DNode186;
			o.Specular = final_specular201.rgb;
			float final_smoothness199 = ( tex2DNode186.a * _Smoothness );
			o.Smoothness = final_smoothness199;
			float4 _AmbientOcclusion_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_AmbientOcclusion_ST_arr, _AmbientOcclusion_ST);
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST_Instance.xy + _AmbientOcclusion_ST_Instance.zw;
			float final_ao197 = ( tex2D( _AmbientOcclusion, uv_AmbientOcclusion ).r * _AOStrength );
			o.Occlusion = final_ao197;
			o.Alpha = 1;
			float total_hex117 = ( temp_output_120_0 + temp_output_123_0 );
			float lerpResult88 = lerp( 1.0 , total_hex117 , sign( clip_1106 ));
			float final_opacity193 = saturate( lerpResult88 );
			clip( final_opacity193 - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardSpecular keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 4.6
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 customPack2 : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.customPack2.xyz = customInputData.vertexToFrag52;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				surfIN.vertexToFrag52 = IN.customPack2.xyz;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputStandardSpecular o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandardSpecular, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
-1920;62;1920;997;2029.667;394.0912;2.752045;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;10;-4871.084,1474.225;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.VertexToFragmentNode;52;-4545.476,1484.77;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;12;-3826.362,1534.726;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Matrix4X4Node;11;-3940.773,937.1696;Inherit;False;InstancedProperty;_ClipQuadMatrix;Clip Quad Matrix;31;0;Create;True;0;0;0;False;0;False;1,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-3481.371,1211.639;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;165;-2292.72,313.6269;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;9;-1887.31,438.6961;Inherit;False;Property;_Falloff;Falloff;12;0;Create;True;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1899.962,361.8906;Inherit;False;Property;_Tiling;Tiling;11;0;Create;True;0;0;0;False;0;False;1;1.33;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TriplanarNode;4;-1711.404,249.5;Inherit;True;Spherical;Object;False;Hex Pattern;_HexPattern;white;10;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;14;-3337.85,1223.464;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TFHCRemapNode;184;-1311.822,373.3919;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;-1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-3702.506,1808.485;Inherit;False;Property;_NoiseScale;Noise Scale;29;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;133;-3587.77,1650.47;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-1108.932,454.6144;Inherit;False;hex_offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;135;-3420.506,1847.485;Inherit;False;Property;_NoiseInfluence;Noise Influence;28;0;Create;True;0;0;0;False;0;False;0;0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;132;-3427.216,1614.445;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;178;-3535.376,2086.486;Inherit;False;Property;_HexMaxOffset;Hex Max Offset;13;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;177;-3499.376,2003.486;Inherit;False;175;hex_offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;136;-3158.506,1711.485;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;134;-3045.506,1390.485;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;-3280.375,2001.486;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2849.2,1564.959;Inherit;False;Property;_DistanceMax;Distance Max;16;0;Create;True;0;0;0;False;0;False;0;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-2811.172,1362.236;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-2971.398,1512.959;Inherit;False;Property;_DistanceMin;Distance Min;15;0;Create;True;0;0;0;False;0;False;0;-0.19;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;18;-2305.198,1255.759;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;19;-2089.469,1223.069;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;112;-2928.201,1890.879;Inherit;False;Property;_Distance2Min;Distance 2 Min;18;0;Create;True;0;0;0;False;0;False;0;-0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-2781.302,1950.679;Inherit;False;Property;_Distance2Max;Distance 2 Max;19;0;Create;True;0;0;0;False;0;False;0;-0.02;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-2837.915,1673.606;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;106;-1833.278,1143.084;Inherit;False;clip_1;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;61;-2338.53,-92.84179;Inherit;False;Property;_LevelsStart;Levels Start;26;0;Create;True;0;0;0;False;0;False;0.99,1,0,1;0.99,1,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;98;-1229.494,283.7506;Inherit;False;hex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;63;-2326.685,-251.3267;Inherit;False;Property;_LevelsEnd;Levels End;27;0;Create;True;0;0;0;False;0;False;0,1,0,1;0,1,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;107;-2071.883,82.54239;Inherit;False;106;clip_1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;114;-2713.702,1728.379;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;144;-3052.058,2370.835;Inherit;False;Property;_Distance3Min;Distance 3 Min;23;0;Create;True;0;0;0;False;0;False;0;-0.47;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;210;-515.077,-727.0689;Inherit;False;Property;_HueOffset;Hue Offset;9;0;Create;True;0;0;0;False;0;False;0;0.181;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;64;-1817.708,-96.88942;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;143;-2905.159,2430.635;Inherit;False;Property;_Distance3Max;Distance 3 Max;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;115;-2543.677,1729.988;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;100;-1175.136,880.4961;Inherit;False;98;hex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;211;-164.1267,-727.2267;Inherit;False;hue_offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-2350.721,1668.981;Inherit;False;clip_2;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;139;-1910.493,1935.05;Inherit;False;Property;_FresnelColor;Fresnel Color;20;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.3254716,0.6488545,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;99;-947.094,144.7506;Inherit;False;98;hex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;7;-1004.642,856.1454;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;65;-1064.308,184.088;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SmoothstepOpNode;145;-2837.559,2215.313;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;60;-763.0596,223.488;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;92;-825.1544,891.9654;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0.95;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-1159.479,614.2109;Inherit;False;116;clip_2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;223;-1540.991,2251.225;Inherit;False;211;hue_offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;221;-1660.991,2097.225;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;146;-2667.534,2209.944;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;109;-1497.881,568.3901;Inherit;False;106;clip_1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;126;-146.3259,446.309;Inherit;False;Property;_HexColor2;Hex Color 2;17;1;[HDR];Create;True;0;0;0;False;0;False;1.490777,6.498019,1.134088,0;1.84147,5.410765,8.675366,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-643.2999,-59.10001;Inherit;False;Property;_HexColor;Hex Color;14;1;[HDR];Create;True;0;0;0;False;0;False;1.490777,6.498019,1.134088,0;5.111077,12.68957,33.66261,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;94;-647.3323,770.148;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;40;-1342.479,551.5402;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;224;-1294.991,2110.225;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-1709.493,1600.05;Inherit;False;Property;_FresnelPower;Fresnel Power;22;0;Create;True;0;0;0;False;0;False;5;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;141;-1802.493,1527.05;Inherit;False;Property;_FresnelScale;Fresnel Scale;21;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;62;-606.9664,399.6808;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.InverseOpNode;36;-3170.109,547.7379;Inherit;False;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4x4;0
Node;AmplifyShaderEditor.DynamicAppendNode;32;-2880.502,1035.767;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;124;-968.887,645.4107;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;142;-2474.578,2148.937;Inherit;False;fresnel_clip;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;120;-493.8793,458.2111;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;219;-127.4456,955.3665;Inherit;False;211;hue_offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RGBToHSVNode;213;-411.2012,-182.5007;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RGBToHSVNode;217;-247.4456,801.3665;Inherit;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-2728.772,596.0115;Inherit;False;2;2;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.HSVToRGBNode;222;-1078.991,2096.225;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FresnelNode;138;-1549.314,1419.336;Inherit;False;Standard;WorldNormal;ViewDir;False;False;5;0;FLOAT3;0,0,1;False;4;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;147;-1599.134,1726.964;Inherit;False;142;fresnel_clip;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;216;-291.2012,-28.50073;Inherit;False;211;hue_offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-460.114,640.862;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;215;-45.20117,-169.5007;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;220;118.5545,814.3665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;148;-1184.134,1431.964;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SmoothstepOpNode;225;-2496.78,1129.89;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TransformDirectionNode;38;-2488.703,535.645;Inherit;False;World;Object;False;Fast;False;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;91;-367.0924,535.6149;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;218;334.5544,800.3665;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;149;-1003.72,1485.454;Inherit;False;fresnel;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-212.1889,632.1171;Inherit;False;total_hex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.HSVToRGBNode;214;170.7988,-183.5007;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;226;-2230.28,1132.49;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;128;-131.226,332.6089;Inherit;False;116;clip_2;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;714.2279,955.2197;Inherit;False;106;clip_1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;108;-358.8964,277.6499;Inherit;False;106;clip_1;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;39;-2221.442,506.6024;Inherit;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;150;19.95581,93.96732;Inherit;False;149;fresnel;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SignOpNode;86;925.9911,880.1365;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;128.774,353.6089;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;695.9269,851.6497;Inherit;False;117;total_hex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;227;-2036.58,1099.99;Inherit;False;clip_1_no_offset;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-30.79999,230.5;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-2021.197,558.1201;Inherit;False;offset_direction;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;43;831.11,218.3007;Inherit;False;Property;_Smoothness;Smoothness;6;0;Create;True;0;0;0;False;0;False;0.354;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;1166.329,1031.601;Inherit;False;102;offset_direction;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;34;1203.706,1123.667;Inherit;False;Property;_VertexOffset;Vertex Offset;25;0;Create;True;0;0;0;False;0;False;0;0.24;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;88;1078.502,683.3259;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;129;223.774,228.6089;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;153;231.7218,481.5371;Inherit;False;Constant;_Vector0;Vector 0;27;0;Create;True;0;0;0;False;0;False;999,999,999,999;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;41;982.6724,-570.2574;Inherit;True;Property;_MainTex;MainTex;2;0;Create;True;0;0;0;False;0;False;-1;None;b8045c1a1b0e07f44bb77e5cf0697267;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;188;764.8257,409.0189;Inherit;True;Property;_AmbientOcclusion;Ambient Occlusion;7;0;Create;True;0;0;0;False;0;False;-1;None;6f3d0bd47c61218468fe04a8fa67f04e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;111;1087.784,1240.709;Inherit;False;227;clip_1_no_offset;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;186;740.6858,6.213074;Inherit;True;Property;_SpecularSmoothness;Specular Smoothness;5;0;Create;True;0;0;0;False;0;False;-1;None;9b35abb31a2f76049aa20996323d6e20;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;47;-292.4768,-450.1031;Inherit;False;Property;_NormalScale;Normal Scale;4;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;44;1065.639,-753.7665;Inherit;False;Property;_Color;Color;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;189;836.0777,627.6431;Inherit;False;Property;_AOStrength;AO Strength;8;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;151;407.6842,163.2034;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;1,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;190;1104.46,491.3551;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;119;1251.233,669.4751;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;187;1128.686,171.2131;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;1310.639,-610.7665;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;46;-15.47678,-460.1031;Inherit;True;Property;_NormalMap;Normal Map;3;0;Create;True;0;0;0;False;0;False;-1;None;4807bb77564f5ec4e8d191aee943e343;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;1414,1124.137;Inherit;False;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;199;1352.794,288.624;Inherit;False;final_smoothness;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;203;635.7942,216.624;Inherit;False;final_emission;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;205;373.7942,-448.376;Inherit;False;final_normal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;201;1163.794,32.62402;Inherit;False;final_specular;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;195;1683.709,1000.965;Inherit;False;final_v_offset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;197;1276.794,476.624;Inherit;False;final_ao;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;193;1320.146,823.2437;Inherit;False;final_opacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;207;1472.794,-481.376;Inherit;False;final_color;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;1864.794,478.624;Inherit;False;195;final_v_offset;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PosVertexDataNode;155;-4661.768,1123.387;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;204;1837.794,79.62402;Inherit;False;203;final_emission;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;208;1942.794,-80.37598;Inherit;False;207;final_color;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;-2338.771,219.6135;Inherit;False;156;vertex_local_position;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexToFragmentNode;157;-4102.768,1186.387;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;200;1833.794,233.624;Inherit;False;199;final_smoothness;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;159;-4334.817,1052.822;Inherit;False;FLOAT3;0;2;1;3;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;169;2365.058,480.2991;Inherit;False;Property;_Cull;Cull;30;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.CullMode;True;0;False;2;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;156;-3844.768,1173.387;Inherit;False;vertex_local_position;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NormalVertexDataNode;192;995.723,1056.914;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;202;1835.794,150.624;Inherit;False;201;final_specular;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;198;1873.794,308.624;Inherit;False;197;final_ao;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;104;-4173.584,1326.97;Inherit;False;world_position;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;1861.646,396.0438;Inherit;False;193;final_opacity;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;206;1873.794,0.6240234;Inherit;False;205;final_normal;1;0;OBJECT;;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2259.026,37.43348;Float;False;True;-1;6;ASEMaterialInspector;0;0;StandardSpecular;Knife/Hex Specular;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Off;1;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.01;True;True;0;False;TransparentCutout;;AlphaTest;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;True;169;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;52;0;10;0
WireConnection;12;0;52;0
WireConnection;13;0;11;0
WireConnection;13;1;12;0
WireConnection;4;9;165;0
WireConnection;4;3;5;0
WireConnection;4;4;9;0
WireConnection;14;0;13;0
WireConnection;184;0;4;2
WireConnection;133;0;14;0
WireConnection;133;1;14;1
WireConnection;175;0;184;0
WireConnection;132;0;133;0
WireConnection;132;1;137;0
WireConnection;136;0;132;0
WireConnection;136;1;135;0
WireConnection;134;0;14;2
WireConnection;134;1;136;0
WireConnection;179;0;177;0
WireConnection;179;1;178;0
WireConnection;185;0;134;0
WireConnection;185;1;179;0
WireConnection;18;0;185;0
WireConnection;18;1;17;0
WireConnection;18;2;16;0
WireConnection;19;0;18;0
WireConnection;180;0;134;0
WireConnection;180;1;179;0
WireConnection;106;0;19;0
WireConnection;98;0;4;1
WireConnection;114;0;180;0
WireConnection;114;1;112;0
WireConnection;114;2;113;0
WireConnection;64;0;63;0
WireConnection;64;1;61;0
WireConnection;64;2;107;0
WireConnection;115;0;114;0
WireConnection;211;0;210;0
WireConnection;116;0;115;0
WireConnection;7;0;100;0
WireConnection;65;0;64;0
WireConnection;145;0;134;0
WireConnection;145;1;144;0
WireConnection;145;2;143;0
WireConnection;60;0;99;0
WireConnection;60;1;65;0
WireConnection;60;2;65;1
WireConnection;60;3;65;2
WireConnection;60;4;65;3
WireConnection;92;0;7;0
WireConnection;221;0;139;0
WireConnection;146;0;145;0
WireConnection;94;0;92;0
WireConnection;40;0;109;0
WireConnection;224;0;221;1
WireConnection;224;1;223;0
WireConnection;62;0;60;0
WireConnection;36;0;11;0
WireConnection;32;0;14;0
WireConnection;32;1;14;1
WireConnection;124;0;121;0
WireConnection;142;0;146;0
WireConnection;120;0;62;0
WireConnection;120;1;40;0
WireConnection;213;0;6;0
WireConnection;217;0;126;0
WireConnection;37;0;36;0
WireConnection;37;1;32;0
WireConnection;222;0;224;0
WireConnection;222;1;221;2
WireConnection;222;2;221;3
WireConnection;138;2;141;0
WireConnection;138;3;140;0
WireConnection;123;0;94;0
WireConnection;123;1;124;0
WireConnection;215;0;213;1
WireConnection;215;1;216;0
WireConnection;220;0;217;1
WireConnection;220;1;219;0
WireConnection;148;0;138;0
WireConnection;148;1;147;0
WireConnection;148;2;222;0
WireConnection;225;0;134;0
WireConnection;225;1;17;0
WireConnection;225;2;16;0
WireConnection;38;0;37;0
WireConnection;91;0;120;0
WireConnection;91;1;123;0
WireConnection;218;0;220;0
WireConnection;218;1;217;2
WireConnection;218;2;217;3
WireConnection;149;0;148;0
WireConnection;117;0;91;0
WireConnection;214;0;215;0
WireConnection;214;1;213;2
WireConnection;214;2;213;3
WireConnection;226;0;225;0
WireConnection;39;0;38;0
WireConnection;86;0;110;0
WireConnection;127;0;128;0
WireConnection;127;1;218;0
WireConnection;127;2;123;0
WireConnection;227;0;226;0
WireConnection;8;0;214;0
WireConnection;8;1;108;0
WireConnection;8;2;120;0
WireConnection;102;0;39;0
WireConnection;88;1;118;0
WireConnection;88;2;86;0
WireConnection;129;0;8;0
WireConnection;129;1;127;0
WireConnection;129;2;150;0
WireConnection;151;0;129;0
WireConnection;151;2;153;0
WireConnection;190;0;188;1
WireConnection;190;1;189;0
WireConnection;119;0;88;0
WireConnection;187;0;186;4
WireConnection;187;1;43;0
WireConnection;45;0;44;0
WireConnection;45;1;41;0
WireConnection;46;5;47;0
WireConnection;35;0;103;0
WireConnection;35;1;34;0
WireConnection;35;2;111;0
WireConnection;199;0;187;0
WireConnection;203;0;151;0
WireConnection;205;0;46;0
WireConnection;201;0;186;0
WireConnection;195;0;35;0
WireConnection;197;0;190;0
WireConnection;193;0;119;0
WireConnection;207;0;45;0
WireConnection;157;0;155;0
WireConnection;156;0;157;0
WireConnection;104;0;52;0
WireConnection;0;0;208;0
WireConnection;0;1;206;0
WireConnection;0;2;204;0
WireConnection;0;3;202;0
WireConnection;0;4;200;0
WireConnection;0;5;198;0
WireConnection;0;10;194;0
WireConnection;0;11;196;0
ASEEND*/
//CHKSM=2F272717F57D2577635DBBE0D3A424BD3D4EE692