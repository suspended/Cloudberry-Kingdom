﻿#include"RootEffect.fx"
Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = clamp; AddressV = clamp;};

float Fade;

PixelToFrame LightSourcePixelShader(VertexToPixel PSIn)
{
    PixelToFrame Output = (PixelToFrame)0;
    
    float d = (PSIn.TexCoords.x - .5) * (PSIn.TexCoords.x - .5) + (PSIn.TexCoords.y - .5) * (PSIn.TexCoords.y - .5);
    float4 baseColor = tex2D(TextureSampler, PSIn.TexCoords);
    
    Output.Color = PSIn.Color;
	
	Output.Color.a *= saturate(Fade*(.25 - d));

	// Premultiply the alpha
	Output.Color.rgb *= Output.Color.a;
	        
    return Output;
}

technique Simplest
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 SimplestVertexShader();
        PixelShader = compile ps_3_0 LightSourcePixelShader();
    }
}