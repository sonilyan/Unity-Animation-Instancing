Shader "Unlit/anim_bones"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_AnimationTex ("Animation", 2D) = "black" {}
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
				float4 uv2 : TEXCOORD2;
				float4 color : COLOR;
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
			
			float4x4 GetMatrix(float boneIndex,float frame)
			{
			    float4 r1 = tex2Dlod(_AnimationTex,float4(boneIndex*4 / _AnimWidth,frame,0,0));
			    float4 r2 = tex2Dlod(_AnimationTex,float4((boneIndex*4+1) / _AnimWidth,frame,0,0));
			    float4 r3 = tex2Dlod(_AnimationTex,float4((boneIndex*4+2) / _AnimWidth,frame,0,0));
			    float4 r4 = tex2Dlod(_AnimationTex,float4((boneIndex*4+3) / _AnimWidth,frame,0,0));
			    
			    return float4x4(r1,r2,r3,r4);
			}
			
			v2f vert (appdata v)
			{
			    float4 boneIndex = v.color;
			    float4 boneWeight = v.uv2;
			    
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				
				float f = (_Time.y+UNITY_ACCESS_INSTANCED_PROP(Props, _TimeOffset)) * _Fps / _AnimHeight;
				fmod(f, 1.0);
				
				float4x4 Matrix = GetMatrix(boneIndex.x,f) * boneWeight.x; 
				
				if(boneWeight.y > 0)
				    Matrix += GetMatrix(boneIndex.y,f) * boneWeight.y;
				if(boneWeight.z > 0)
				    Matrix += GetMatrix(boneIndex.z,f) * boneWeight.z;
				if(boneWeight.w > 0)
				    Matrix += GetMatrix(boneIndex.w,f) * boneWeight.w;
				    
				o.vertex = mul(Matrix,v.vertex);
				
				o.vertex = UnityObjectToClipPos(o.vertex);
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
