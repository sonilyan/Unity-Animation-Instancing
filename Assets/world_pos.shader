Shader "Unlit/world_pos"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = o.vertex; 
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				
				float4 tmp = mul(unity_CameraInvProjection,i.uv / i.uv.w);
				tmp.z = -tmp.z;
				float4 tmp2 = mul(unity_CameraToWorld,tmp/tmp.w);
				if(tmp2.y > 0) {
				    if(tmp2.x > 0) {
				        if(tmp2.z > 0){
				            return float4(1,0,0,0);
				        }
				        return float4(0,1,0,0);
				    }
				    return float4(1,1,1,1);
				}
				        
				return float4(0,0,0,0);
			}
			ENDCG
		}
	}
}
