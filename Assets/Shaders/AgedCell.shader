Shader "Custom/AgedCell" {
	Properties {
		_Age ("Age", Float) = 0
		_YoungColor ("Young Color", Color) = (1,1,1,1)
		_OldColor ("Old Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows alpha:auto

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		float _Age;
		fixed4 _YoungColor;
		fixed4 _OldColor;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * float4(IN.worldPos.z, IN.worldPos.x, IN.worldPos.y, 1);
			o.Albedo = c.rgb * lerp( _YoungColor, _OldColor, clamp(_Age, 0, 100) / 100);
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = /*float3(IN.uv_MainTex,0) **/ _Glossiness * sin(_Time.x);
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
