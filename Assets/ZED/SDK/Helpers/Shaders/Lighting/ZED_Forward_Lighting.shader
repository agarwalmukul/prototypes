//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============
 // Computes lighting and shadows and apply them to the real
	Shader "ZED/ZED Forward Lighting"
{

	Properties{
		[MaterialToggle] directionalLightEffect("Directional light affects image", Int) = 0
	}
		SubShader{

		ZWrite On
		Pass{
		Name "FORWARD"
		Tags{ "LightMode" = "Always" }

		Cull Off
		CGPROGRAM
		// compile directives
#pragma target 4.0

#pragma vertex vert_surf
#pragma fragment frag_surf


#pragma multi_compile_fwdbase
#pragma multi_compile_fwdadd_fullshadows

#pragma multi_compile __ NO_DEPTH_OCC

#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"

#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"

#include "AutoLight.cginc"
#include "../ZED_Utils.cginc"
#define ZED_SPOT_LIGHT_DECLARATION
#define ZED_POINT_LIGHT_DECLARATION
#include "ZED_Lighting.cginc"

		sampler2D _MainTex;
		sampler2D _handsDepthTex;

	struct Input {
		float2 uv_MainTex;
	};

	float4x4 _CameraMatrix;
	sampler2D _DirectionalShadowMap;


	struct v2f_surf {
		float4 pos : SV_POSITION;
		float4 pack0 : TEXCOORD0;
		SHADOW_COORDS(4)
			ZED_WORLD_DIR(1)

	};



		float4 _MainTex_ST;
		sampler2D _DepthXYZTex;
		float4 _DepthXYZTex_ST;
		sampler2D _MaskTex;
		int _HasShadows;
		float4 ZED_directionalLight[2];
		int directionalLightEffect;
		float _ZEDFactorAffectReal;

		// vertex shader
		v2f_surf vert_surf(appdata_full v) {



		v2f_surf o;
		UNITY_INITIALIZE_OUTPUT(v2f_surf,o);
		o.pos = UnityObjectToClipPos(v.vertex);

		ZED_TRANSFER_WORLD_DIR(o)

			o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
		o.pack0.zw = TRANSFORM_TEX(v.texcoord, _DepthXYZTex);

		o.pack0.y = 1 - o.pack0.y;
		o.pack0.w = 1 - o.pack0.w;

		TRANSFER_SHADOW(o);

		return o;
	}
	// fragment shader
	void frag_surf(v2f_surf IN, out fixed4 outColor : SV_Target, out float outDepth : SV_Depth) {
	    UNITY_INITIALIZE_OUTPUT(fixed4,outColor);
		float4 uv = IN.pack0;

		float3 zed_xyz = tex2D(_DepthXYZTex, uv.zw).xxx;

	


		fixed4 c = 0;
		float4 color = tex2D(_MainTex, uv.xy).bgra;
		float3 normals = tex2D(_NormalsTex, uv.zw).rgb;


		//Apply directional light
		color *=  _ZEDFactorAffectReal;

		//Compute world space
		float3 worldspace;
		GET_XYZ(IN, zed_xyz.x, worldspace)

		//Apply shadows
		// e(1) == 2.71828182846
		if (_HasShadows == 1) {
			//Depends on the ambient lighting
			float atten = saturate(tex2D(_DirectionalShadowMap, fixed2(uv.z, 1 - uv.w)) + log(1 + 1.72*length(UNITY_LIGHTMODEL_AMBIENT.rgb)/4.0));

			c = half4(color*atten);
		}
		else {
			c = half4(color);
		}


		//Add light
	    c += saturate(computeLighting(color.rgb, normals, worldspace, 1));

		c.a = 0;
		outColor.rgb = c;

	#ifdef NO_DEPTH_OCC
	    outDepth = 0;
	#else
		
		//if (tex2D(_handsDepthTex, fixed2( (0.8f* (uv.x-0.5f)) + 0.5f, 0.8f * (1- uv.y) + 0.5f ) ).r != 0.0f){
			//outDepth = computeDepthXYZ(zed_xyz.z);
			//outDepth = tex2D(_handsDepthTex, 0.8f * fixed2( uv.x - 0.1f, 1- uv.y) ).r;
			//outDepth = tex2D(_handsDepthTex, fixed2( (0.8f* (uv.x-0.5f)) + 0.5f, 0.8f * (1- uv.y) + 0.5f ) ).r;
		if(tex2D(_handsDepthTex, fixed2(uv.x, 1 - uv.y)).r != 0.0f){
			fixed4 col = c;
			float per = 0.1f;
			fixed4 t = fixed4(0.5078, 0.2343, 0.1289, 0);
			//if (((col.r - t.r)/t.r)<per && ((col.g - t.g)/t.g)<per && ((col.b - t.b)/t.b)<per){
			//if (((col.r - t.r))<per && ((col.g - t.g))<per && ((col.b - t.b))<per){
			//if (((col.r - 0.50))<per && ((col.g - 0.23))<per && ((col.b - 0.12))<per){
			//if (((col.r))<per && ((col.g))<per && ((col.b))<per){
			//if ( 0.45f<col.r<0.55f && 0.18f<col.g<0.28f && 0.10f<col.b<0.15f){
			//if ( col.r>0.49f && col.r<0.51f && col.g>0.22f && col.g<0.24f && col.b>0.11f && col.b<0.13f){
			if ( col.r>0.1f && col.r<0.2f && col.g<0.2f && col.b<0.13f){
				outDepth = tex2D(_handsDepthTex, fixed2(uv.x, 1 - uv.y)).r;
				//outDepth = 0.0f;
			}
			else {
				float3 depthxyz = tex2D(_DepthXYZTex, float2( uv.z, 0.0f)).xxx;
				outDepth = computeDepthXYZ(depthxyz.z);
				//outDepth = 0.0f;
			}
		} else {
			outDepth = computeDepthXYZ(zed_xyz.z);
		}

	#endif
			//130/256, 60/256, 33/256
			//if (abs((col.r - 0.5078f)/0.5078.0f)<per && abs((col.g - 0.2343f)/0.2343f)<per && abs((col.b - 0.1289f)/0.1289f)<per){
		
		}
	ENDCG

	}
	}
		Fallback Off
}
