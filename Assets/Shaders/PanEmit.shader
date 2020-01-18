//Referenced: https://answers.unity.com/questions/1166659/how-to-scroll-a-texture-in-a-shader.html

Shader "Custom/PanShader" {
	Properties{
	_MainTex("Texture", 2D) = "white" {}
	_xScroll("Scroll X", Range(-100,100)) = 0
	_yScroll("Scroll Y", Range(-100,100)) = 0
	_EmitPower("Emission Strength", Range(0,100)) = 0
	_AlphaCutoff("Alpha Cutoff", Range(0,1)) = 0
	}
		SubShader{
			Tags{ "RenderType" = "Cutout" }
			CGPROGRAM
			#pragma surface surf Lambert alphatest:_AlphaCutoff
			struct Input {
				  float2 uv_MainTex;
				  float2 uv_SubTex;
				  float3 viewDir;
			};
			sampler2D _MainTex;
			float _xScroll;
			float _yScroll;
			float _EmitPower;

			void surf(Input IN, inout SurfaceOutput o) {
				fixed2 scrolledUV = IN.uv_MainTex;

				float newX = _xScroll * _Time;
				float newY = _yScroll * _Time;

				scrolledUV += fixed2(newX, newY);
				half4 newColor = tex2D(_MainTex, scrolledUV);

				o.Albedo += newColor.rbg;

				o.Emission = newColor.rgb * pow(newColor.rgb, _EmitPower);

				o.Alpha = tex2D(_MainTex, scrolledUV).a;
			}
			ENDCG
	}
		Fallback "Cutout"
}