Shader "Custom/CharacterMask"
{
    Properties
    {
        [IntRange] _StencilID("Stencil ID", Range(0, 255)) = 0
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque" 
            "RenderPipeline" = "Universal"
            "Queue"="Geometry-100" 
        }

        Pass
        {
            ColorMask 0
            ZWrite Off

            Stencil
            {
                Ref [_StencilID]
                Pass Replace
            }
        }
       
    }
}
