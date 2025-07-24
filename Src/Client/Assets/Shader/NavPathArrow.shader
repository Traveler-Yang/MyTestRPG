Shader "Custom/NavPathArrow"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ScrollYSpeed("Y Scroll Speed", Range(-20, 20)) = 2
        _ScrollYMultiplier("Y Scroll Multiplier", Range(1, 10)) = 1
    }

        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            LOD 100
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _ScrollYSpeed;
                float _ScrollYMultiplier;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv * float2(1, _ScrollYMultiplier);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed2 uv = i.uv;
                    uv.y -= _ScrollYSpeed * _Time;
                    uv.y -= floor(uv.y) / _ScrollYMultiplier;
                    uv.y = frac(uv.y * _ScrollYMultiplier);

                    fixed4 col = tex2D(_MainTex, uv);
                    return col;
                }
                ENDCG
            }
        }
}

