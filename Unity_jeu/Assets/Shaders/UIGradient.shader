Shader "UI/Gradient"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (0.8, 0.65, 0.2, 1)
        _ColorBottom ("Bottom Color", Color) = (0, 0, 0, 1)
        _Angle ("Angle", Range(0, 360)) = 0
    }
    
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
        
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
            };
            
            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            float _Angle;
            
            v2f vert(appdata_t v)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(v.vertex);
                OUT.texcoord = v.texcoord;
                OUT.color = v.color;
                return OUT;
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                float angleRad = radians(_Angle);
                float cosAngle = cos(angleRad);
                float sinAngle = sin(angleRad);
                
                float2 rotatedUV = IN.texcoord - 0.5;
                rotatedUV = float2(
                    rotatedUV.x * cosAngle - rotatedUV.y * sinAngle,
                    rotatedUV.x * sinAngle + rotatedUV.y * cosAngle
                );
                rotatedUV += 0.5;
                
                fixed4 color = lerp(_ColorBottom, _ColorTop, rotatedUV.y);
                color.a *= IN.color.a;
                
                return color;
            }
            ENDCG
        }
    }
}