Shader "Unlit/anim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AnimationTex ("Animation", 2D) = "white" {}
		_AnimHeight("Anim Height", Float) = 0
		_AnimWidth("Anim Width", Float) = 0
		_Fps("Fps",Float) = 30
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _TimeOffset)
            UNITY_INSTANCING_BUFFER_END(Props)

            float _Fps;
            float _AnimHeight;
            float _AnimWidth;
            
            sampler2D _AnimationTex;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v,uint vid:SV_VertexID)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				
				float f = (_Time.y+UNITY_ACCESS_INSTANCED_PROP(Props, _TimeOffset)) * _Fps / _AnimHeight;
				fmod(f, 1.0);
				
				float4 off = tex2Dlod(_AnimationTex,float4(vid / _AnimWidth,f,0,0));
				
				off.xyz = off.xyz * 2 - 1;
				off *= off.w;
				
				v.vertex += float4(off.xyz,0);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    UNITY_SETUP_INSTANCE_ID(i);
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
