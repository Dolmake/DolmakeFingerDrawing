Shader "Custom/Trans-Unlit-Multiply-Colored-Simple" {
	Properties
	{		
		_MainTex ("Base (RGB)", 2D) = "white" { }			
		_Color ("Color", COLOR) = (1,1,1,1)
	}
	
				

	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

		// note that a vertex shader is specified here but its using the one above
		Pass
		{
			Name "Trans-Unlit-Multiply-Colored-Simple"			
			Cull Back
			ZWrite Off				
			ColorMask RGB			
			// you can choose what kind of blending mode you want for the outline
			Blend SrcAlpha OneMinusSrcAlpha // Normal		
			//Blend Zero SrcColor
			

			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

				struct appdata
				{
					float4 vertex : POSITION;
					float2 texcoord: TEXCOORD0;					
				};

				struct v2f
				{
					float4 pos : POSITION;					
					float2 uvmain : TEXCOORD0; 					
				};				
				
				float4 _MainTex_ST;				
				uniform sampler2D _MainTex;	
				uniform fixed4 _Color;	
				
				v2f vert(appdata v)
				{
					// just make a copy of incoming vertex data but scaled according to normal direction
					v2f o;
					o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );	 				
					return o;
				}			
				
				half4 frag(v2f i) :COLOR
				{						
					return tex2D( _MainTex, i.uvmain ) * _Color; 					
				}
				
				
			ENDCG
		}
	}
	
	
	
	FallBack "Unlit/Transparent"
}
