Shader "Custom/PartlyLightUp" {
	Properties
	{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
	_SecondTex("SecondTex (RGB)", 2D) = "white"
		_ThirdTex("ThirdTex (RGB)", 2D) = "white"
		_BumpMap("Bumpmap", 2D) = "bump" {}
	_BlendAlpha("Blend Alpha", float) = 0
		//extra
		_Glow("Intensity", Range(0, 10)) = 1

	}
		SubShader
	{
		Tags{ "Queue" = "Geometry-9" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		Lighting Off
		Cull Off
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
#pragma surface surf Lambert

		fixed4 _Color;
	sampler2D _MainTex;
	sampler2D _SecondTex;
	sampler2D _ThirdTex;
	float _BlendAlpha;
	sampler2D _BumpMap;
	half _Glow;


	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float2 uv_SecondTex;

	};

	void surf(Input IN, inout SurfaceOutput o) {
		fixed4 c = _Glow *tex2D(_SecondTex, IN.uv_MainTex).a;
		c += tex2D(_MainTex, IN.uv_MainTex) * tex2D(_ThirdTex, IN.uv_MainTex);
		o.Albedo = c.rgb;
		o.Alpha = c.a;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));


	}
	ENDCG
	}

		Fallback "Transparent/VertexLit"
}
