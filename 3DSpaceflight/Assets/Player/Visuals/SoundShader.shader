//Highlights intersections with other objects
 
Shader "Custom/Sound"
{
    Properties
    {
        _RegularColor("Main Color", Color) = (1, 1, 1, .5) //Color when not intersecting
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
        _HighlightThresholdMax("Highlight Threshold Max", Float) = 1 //Max difference for intersections
		_Radius("Radius", Float) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }
        Pass
        {
            Blend One One
            ZWrite Off
			ZTest Off
            Cull Off
 
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            uniform sampler2D _CameraDepthTexture; //Depth Texture
            uniform float4 _RegularColor;
            uniform float4 _HighlightColor;
            uniform float _HighlightThresholdMax;
			uniform float _Radius;

			uniform float4x4 _InverseView; //Should be set via script
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 projPos : TEXCOORD0; //Screen position of pos
				float4 center : TEXCOORD1;
            };

			float2 toScreen(float2 notScreen) //converts from[-1,1] to [0,1] for a float2
			{
				return (notScreen + 1) / 2;
			}
 
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.projPos = toScreen(o.pos.xy / o.pos.w);
				o.center = mul(_Object2World, float4(0,0,0,1));
                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
                float4 finalColor = _RegularColor;
 
                //Get the distance to the camera from the depth buffer for this point
                float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.projPos));
 
				float2 p11_22 = float2(unity_CameraProjection._11, unity_CameraProjection._22);
				float3 vpos = float3((i.projPos * 2 - 1) / p11_22, -1) * sceneZ;
				float4 wpos = mul(_InverseView, float4(vpos, 1));

                float diff = distance(i.center, wpos); //distance in world space from center of sphere
				diff = _Radius - diff; //distance in world space from surface of sphere
				if(diff >= 0 && diff <= _HighlightThresholdMax)
				{
					diff = 1 - (diff / _HighlightThresholdMax); //now scaled to our max distance
					diff *= diff * diff;
                    finalColor = lerp(	_RegularColor,
										_HighlightColor,
										diff);
				}
 
                half4 c;
                c.r = finalColor.r;
                c.g = finalColor.g;
                c.b = finalColor.b;
                c.a = finalColor.a;
					
				//banding
				//c *= frac(5 * wpos.y);

                return c;
            }
 
            ENDCG
        }
    }
}