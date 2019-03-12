Shader "KoLab/StencilMask"
{
    Properties {
        [IntRange] _StencilRef ("Stencil Reference Value", Range(0,255)) = 0
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}             
        Pass {
            Colormask 0           
            ZWrite Off
            Stencil {
                Ref 1
                Comp always
                Pass replace
            }
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            half4 frag(v2f i) : SV_Target {
                return half4(0,0,0,0);
            }
            ENDCG
        }
        
        Pass {                       
            ColorMask 0
            ZWrite on
            ZTest Always
            Stencil
            {
                Ref [_StencilRef]
                Comp equal
            }
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
             
            int _EyeIndex;
            
            struct appdata
            {
                float4 vertex : POSITION;
            };
 
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
 
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                //clear the depth buffer by setting Z to the far plane
//                #if defined(UNITY_REVERSED_Z)
//                o.pos.z = 0;
//                #else
//                o.pos.z = 1;
//                #endif
                return o;
            }
 
            half4 frag(v2f i) : COLOR
            {
                return half4(1,1,0,1);
            }
            ENDCG
        }
    }
}