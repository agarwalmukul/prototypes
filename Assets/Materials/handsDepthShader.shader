Shader "Custom/handsDepthShader" {
	Properties {
		//_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_handTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		//LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _handTex;

		struct Input {
			float2 uv_MainTex;
		};

		//fixed4 _Color;

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 c;

			//if((tex2D (_handTex, IN.uv_MainTex)).r != 0){
			//	c = tex2D (_handTex, IN.uv_MainTex);
			//}else {
			//	c = tex2D (_MainTex, IN.uv_MainTex);
			//}
			c = tex2D (_MainTex, IN.uv_MainTex);
			c = fixed4(0.0f,1.0f,0.0f,1.0f);
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
