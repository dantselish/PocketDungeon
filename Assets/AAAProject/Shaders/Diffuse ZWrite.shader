Shader "Custom/DiffuseZWrite"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            // I HAD TO REMOVE THE LINE "Queue"="Transparent"
            "IgnoreProjector" = "True"
            "RenderType"      = "Opaque" // and change the rendertype to "Opaque"
        }

        LOD 200

        // extra pass that renders to depth buffer only
        Pass
        {
            ZWrite On
            ColorMask 0
        }

        UsePass "Transparent/Diffuse/FORWARD"
    }
    FallBack "Diffuse"
}