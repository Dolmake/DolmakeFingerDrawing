Shader "Custom/TexCoordinate" {
	Properties
	{		
		_MainTex ("Main (RGBA)", 2D) = "white" { }			
	}
	
				

	SubShader
	{
		//Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }	
		Tags { "Queue"="Geometry"}			
		// note that a vertex shader is specified here but its using the one above
		Pass
		{
			Name "SpecialZ"
			//Tags { "LightMode" = "Always" }
			Cull Back
			ZWrite On
			ZTest LEqual 		
			ColorMask RGB			
			// you can choose what kind of blending mode you want for the outline
			// Normal
			Blend SrcAlpha OneMinusSrcAlpha 
			//Blend Zero SrcColor
			

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0

			struct vertOut {
                float4 pos:SV_POSITION;
                float4 scrPos:TEXCOORD;
            };

            vertOut vert(appdata_base v) {
                vertOut o;
                o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                o.scrPos = ComputeScreenPos(o.pos);
                return o;
            }

			uniform sampler2D _MainTex;			
            fixed4 frag(vertOut i) : COLOR0 {
               float2 wcoord = (i.scrPos.xy/i.scrPos.w);
               fixed4 tint =  tex2D( _MainTex, wcoord );
			   tint.a = 1.0f;
			   return tint;
            }
				
				
			ENDCG
		}
		
	
	}
	
	
	
	//FallBack "Unlit/Transparent"
}
