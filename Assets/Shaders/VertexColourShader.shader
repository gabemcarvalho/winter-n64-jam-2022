Shader "VertexColoredShader"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
    }
        SubShader
    {
        /*Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct vertexInput
            {
                float4 vertex : POSITION;
            };
            struct vertexOutput
            {
                float4 pos : SV_POSITION;
            };
            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.pos = UnityObjectToClipPos(input.vertex);
                return output;
            }
            float4 _Color;
            fixed4 frag(vertexOutput input) : SV_TARGET
            {
                return _Color;
            }
            ENDCG
        }*/

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            struct vertexInput
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };
            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float4 col : COLOR;
            };
            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                input.vertex.y += 0.01;
                output.pos = UnityObjectToClipPos(input.vertex);
                output.col = input.color;
                return output;
            }
            fixed4 frag(vertexOutput input) : SV_TARGET
            {
                // Discard the fragment if the alpha is less than half
                if (input.col.a < 0.5)
                    discard;
                return input.col;
            }
            ENDCG
        }
    }
}
