// ---------------------------------------------------------
// Ejemplo shader Minimo:
// ---------------------------------------------------------

/**************************************************************************************/
/* Variables comunes */
/**************************************************************************************/

//Matrices de transformacion
float4x4 matWorld; //Matriz de transformacion World
float4x4 matWorldView; //Matriz World * View
float4x4 matWorldViewProj; //Matriz World * View * Projection
float4x4 matInverseTransposeWorld; //Matriz Transpose(Invert(World))


//Textura para DiffuseMap
texture superficieAgua2;
sampler2D heightmap2 = sampler_state
{
	Texture = <superficieAgua2>;
	MIPFILTER = LINEAR;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
};

texture  g_txCubeMap;
samplerCUBE g_samCubeMap =
sampler_state
{
	Texture = <g_txCubeMap>;
	MinFilter = Linear;
	MagFilter = Linear;
	MipFilter = Linear;
};


texture superficieAgua;
sampler heightmap = sampler_state
{
	Texture = <superficieAgua>;
	MIPFILTER = LINEAR;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
};

float3 fvLightPosition = float3(-100.00, 100.00, -100.00);	// posicion de la luz
float3 fvEyePosition = float3(0.00, 0.00, -100.00);	// posicion de la camara
float3 fvEyeLookAt = float3(0.00, 0.00, -100.00);	// posicion del objetivo

float k_la = 0.7;							// luz ambiente global
float k_ld = 0.4;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;				// exponente luz specular
float4 specularColor;						// color luz specular
float4 diffuseColor;						// color luz difusa

float kx = 1;							// coef. de reflexion
float kc = 0;							// coef. de refraccion

float time = 0;

float4 colorAgua;

float amplitudx;							// amplitud de la ola en eje x
float amplitudz;							// amplitud de la ola en eje z
float frecuenciax;							// frecuencia de la ola en eje x
float frecuenciaz;							// frecuencia de la ola en eje z

float3 shipPos;								// posicion del barco

float coeficiente = 0;						// coeficiente para interpolar heightmaps

float min_cant_samples = 10;
float max_cant_samples = 200;

float fHeightMapScale = 1;
float fTexScale = 10;



/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

float calculate_Position(float x, float z)
{
	float u = (x / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);
	float v = (z / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);
	
	float ola2 = 
	sin(1 * u * 2 * 3.14159 * frecuenciax + time) * amplitudx + cos(1 * v * 2 * 3.14159 * frecuenciaz + time) * amplitudz;
	
	return ola2;
}


void VSCubeMap2(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	float2 Texcoord : TEXCOORD0,
	out float4 oPos : POSITION,
	out float3 EnvTex : TEXCOORD0,
	out float2 Tex : TEXCOORD2,
	out float3 N : TEXCOORD1,
	out float3 EnvTex1 : TEXCOORD3,
	out float3 EnvTex2 : TEXCOORD4,
	out float3 EnvTex3 : TEXCOORD5,
	out float3 wPos : TEXCOORD6,
	out float3 PosPix : TEXCOORD7
	)
{
	float3 normal;

	// Actualizo el output
	oPos = Pos;
	oPos.y = calculate_Position(Pos.x, Pos.z); //se lo aplicamos al eje y

	float dr = 15;

	//Proyectar posicion
		float heightx = calculate_Position(Pos.x + dr, Pos.z);
	float heightz = calculate_Position(Pos.x, Pos.z + dr);
	float3 dx = normalize(float3(dr, heightx - oPos.y, 0));
		float3 dz = normalize(float3(0, heightz - oPos.y, dr));

		normal = cross(dz, dx);

	PosPix = oPos;
	wPos = mul(oPos, matWorld);
	oPos = mul(oPos, matWorldViewProj);
	//PosAux = mul(PosAux, matWorld);
	float3 vEyeR = normalize(wPos - fvEyePosition);

		// corrijo la normal (depende de la malla)
		// ej. el tanque esta ok, la esfera esta invertida.
		//Normal*= -1;
		float3 vN = mul(normal, (float3x3)matWorld);
		vN = normalize(vN);
	EnvTex = reflect(vEyeR, vN);

	// Refraccion de la luz
	EnvTex1 = refract(vEyeR, vN, 1.001);
	EnvTex2 = refract(vEyeR, vN, 1.009);
	EnvTex3 = refract(vEyeR, vN, 1.02);
	//Fresnel = FBias + FEscala*pow(1 + dot(vEyeR, vN), FPower);

	//Propago la textura
	Tex = (Pos.x, Pos.z);

	//Propago la normal
	N = normal;

}


float4 PSCubeMap(float3 EnvTex: TEXCOORD0,
	float3 N : TEXCOORD1,
	float2 Texcoord : TEXCOORD2,
	float3 Tex1 : TEXCOORD3,
	float3 Tex2 : TEXCOORD4,
	float3 Tex3 : TEXCOORD5,
	//float Fresnel : COLOR,
	float3 wPos : TEXCOORD6,
	float3 Pos : TEXCOORD7
	) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	/*if (Pos.x < 0) N = float3(1, 0, 0);
	if (Pos.x >= 0) N = float3(0, 1, 0);*/

	float hei = calculate_Position(Pos.x, Pos.z);
	
	float textx = smoothstep(-8000, 8000, Pos.x);
	float textz = smoothstep(-8000, 8000, Pos.z);
	float2 text = float2(textx, textz);

	float3 posmin = shipPos - float3(80, 0, 20);
		float3 posmax = shipPos + float3(80, 0, 20);

		float4 aux = tex2Dgrad(heightmap, text * 20, ddx(text), ddy(text));
		//float4 aux = float4(hei, 0, 0, 1);
		N = normalize(N + 0.2*aux.xyz);

	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition - wPos);
		ld += saturate(dot(N, LD))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = normalize(wPos - fvEyePosition);
		float ks = saturate(dot(reflect(LD, N), D));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

		if (posmin.z < Pos.z && posmax.z > Pos.z && posmin.x < Pos.x && posmax.x > Pos.x){

		colorAgua = float4(0, 0, 0, 1);
		}
	//Obtener el texel de textura
	float k = 0.30;
	float4 fvBaseColor = k*texCUBE(g_samCubeMap, EnvTex) +
		(1 - k)*colorAgua;
	//(1 - k)*tex2D(diffuseMap, text*20);

	// suma luz diffusa, ambiente y especular
	fvBaseColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);
	fvBaseColor.a = 1;
	float4 color_reflejado = fvBaseColor;

		float4 color_refractado = float4(
		texCUBE(g_samCubeMap, Tex1).x,
		texCUBE(g_samCubeMap, Tex2).y,
		texCUBE(g_samCubeMap, Tex3).z,
		1);
	//float4 color_refractado = texCUBE( g_samCubeMap, Tex1);

	float4 col = color_reflejado*kx + color_refractado*kc;
		col.a = 1;
	return col;
	//return float4(N, 1);
	//return aux;
	//return color_refractado;
	//return color_reflejado;
}


technique RenderCubeMap
{
	pass p0
	{
		VertexShader = compile vs_3_0 VSCubeMap2();
		PixelShader = compile ps_3_0 PSCubeMap();
	}
}


// calcula la iluminaciond dinamica
float4 Phong(float2 texCoord, float3 vLightTS, float3 vViewTS, float dx, float dy, float3 vEyeR, float3 normal)
{
	// Color Basico
	float a = coeficiente;
	float4 cBaseColor = colorAgua;

		// Busco el vector normal en las texturas Normal-Height Map  (esta en Tangent Space)
		float3 samp1 = normalize(tex2Dgrad(heightmap, texCoord, dx, dy) * 2 - 1);
		float3 samp2 = normalize(tex2Dgrad(heightmap2, texCoord, dx, dy) * 2 - 1);
		// Interpolo las dos texturas
		float3 vNormalTS = normalize(lerp(samp1, samp2, coeficiente) + 0.5*normal);
		// Calculo el reflejo de la normal tomando la normal nueva
		float3 EnvTex = reflect(vEyeR, vNormalTS);


			// Color difuso
			float3 vLightTSAdj = float3(vLightTS.x, -vLightTS.y, vLightTS.z);
			float cDiffuse = saturate(dot(vNormalTS, vLightTSAdj)) * k_ld * diffuseColor;

		// Color specular
		float4 cSpecular = 0;
			float3 vReflectionTS = normalize(2 * dot(vViewTS, vNormalTS) * vNormalTS - vViewTS);
			float fRdotL = saturate(dot(vReflectionTS, vLightTSAdj));
		cSpecular = saturate(pow(fRdotL, fSpecularPower))*k_ls*specularColor;

		// Calculo en color en base al envMap y al color del agua
		float k = 0.60;
		cBaseColor = k*texCUBE(g_samCubeMap, EnvTex) +
			(1 - k)*colorAgua;

		// suma luz diffusa, ambiente y especular
		cBaseColor = (k_la + cDiffuse)*cBaseColor + cSpecular;
		float4 color_reflejado = cBaseColor;

		float4 col = color_reflejado*kx;
			col.a = 1;
		return col;
}


void RenderSceneVS(float4 Pos : POSITION,
	float2 Texcoord : TEXCOORD0,
	float3 normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Tex : TEXCOORD0,
	out float3 tsView : TEXCOORD1,
	out float3 tsLight : TEXCOORD3,
	out float3 wsNormal : TEXCOORD2,
	out float3 wsView : TEXCOORD4,
	out float3 EnvTex : TEXCOORD5,
	out float3 vEyeR : TEXCOORD6
	)
{
	// Actualizo posicion
	oPos = Pos;
	oPos.y = calculate_Position(Pos.x, Pos.z); //se lo aplicamos al eje y
	// Proyecto posicion
	float3 wPos = mul(oPos, matWorld);

	// Calculo la normal
	float dr = 15;
	float heightx = calculate_Position(Pos.x + dr, Pos.z);
	float heightz = calculate_Position(Pos.x, Pos.z + dr);
	float3 dx = normalize(float3(dr, heightx - oPos.y, 0));
		float3 dz = normalize(float3(0, heightz - oPos.y, dr));
		normal = cross(dz, dx);

	// Vector View = desde el ojo a la pos del vertice
	float4 VertexPositionWS = mul(oPos, matWorld);
		wsView = fvEyePosition.xyz - VertexPositionWS.xyz;


	// calculo la tg y la binormal
	float3 up = float3(0, 0, 1);
		if (abs(normal.z - 1) <= 0.0001)
			up = float3(0, 1, 0);
	float3 tangent = cross(normal, up);
		float3 binormal = cross(normal, tangent);

		float3x3 tangentToWorldSpace;
	tangentToWorldSpace[0] = mul(tangent, matWorld);
	tangentToWorldSpace[1] = mul(binormal, matWorld);
	tangentToWorldSpace[2] = mul(normal, matWorld);

	// tangentToWorldSpace es una matriz ortogonal, su inversa = a su transpuesta
	// A es OrtoNorm <=> A-1 == At
	float3x3 worldToTangentSpace = transpose(tangentToWorldSpace);

		// proyecto
		oPos = mul(oPos, matWorldViewProj);
	//Propago la textura
	Tex = float2(Pos.x, Pos.z);

	tsView = mul(wsView, worldToTangentSpace);		// Vector View en TangentSpace
	tsLight = mul(fvLightPosition, worldToTangentSpace);	// Vector Light en TangentSpace

	// propago el vector normal en Worldspace
	wsNormal = normal;
	// tambien devuelve el vector view en worldspace wsView
	
	vEyeR = normalize(wPos - fvEyePosition);

		float3 vN = mul(normal, (float3x3)matWorld);
		vN = normalize(vN);
	EnvTex = reflect(vEyeR, vN);

}


// ws = worldspace    ts = tangentspace
float4 PSParallaxOcclusion(float2 Texcoord: TEXCOORD0,
	float3 Pos : POSITION,
	float3 tsView : TEXCOORD1,
	float3 wsNormal : TEXCOORD2,
	float3 tsLight : TEXCOORD3,
	float3 wsView : TEXCOORD4,
	float3 EnvTex : TEXCOORD5,
	float3 vEyeR : TEXCOORD6
	) : COLOR0
{
	float textx = smoothstep(-16000, 16000, Texcoord.x);
	float textz = smoothstep(-16000, 16000, Texcoord.y);
	Texcoord = float2(textx, textz)*fTexScale;

	// normalizo todo lo que interpola el PS
	wsView = normalize(wsView);
	tsView = normalize(tsView);
	tsLight = normalize(tsLight);
	wsNormal = normalize(wsNormal);

	// POM Algoritmo Standard 
	float fParallaxLimit = length(tsView.xy) / tsView.z;
	fParallaxLimit *= fHeightMapScale;
	float2 vOffset = normalize(-tsView.xy);
		vOffset = vOffset * fParallaxLimit;
	// interpola entre un min y un max, proporcionalmente al angulo de vision
	int nNumSamples = (int)lerp(min_cant_samples, max_cant_samples,
		abs(dot(wsView, wsNormal)));
	float fStepSize = 1.0 / (float)nNumSamples;

	float2 dx, dy;
	dx = ddx(Texcoord);
	dy = ddy(Texcoord);

	// Ray casting: 
	float2 vOffsetStep = fStepSize * vOffset;
		float2 vCurrOffset = float2(0, 0);
		float2 vLastOffset = float2(0, 0);

		float fCurrH = 0;
	float fLastH = 0;

	float stepHeight = 1.0;
	int nCurrSample = 0;
	float4 vCurrSample;
	float4 vLastSample;

	float4 samp1;
	float4 samp2;

	while (nCurrSample < nNumSamples)
	{
		samp1 = tex2Dgrad(heightmap, Texcoord + vCurrOffset, dx, dy);
		samp2 = tex2Dgrad(heightmap2, Texcoord + vCurrOffset, dx, dy);
		vCurrSample = lerp(samp1, samp2, coeficiente);
		fCurrH = vCurrSample.a;
		if (fCurrH > stepHeight)
		{
			float Ua = 0;
			float X = (fStepSize + (fCurrH - fLastH));
			if (X != 0.0f)
				Ua = ((stepHeight + fStepSize) - fLastH) / X;
			vCurrOffset = vLastOffset + Ua* vOffsetStep;
			nCurrSample = nNumSamples + 1;
		}
		else
		{
			nCurrSample++;
			stepHeight -= fStepSize;
			vLastOffset = vCurrOffset;
			vCurrOffset += vOffsetStep;
			vLastSample = vCurrSample;
			fLastH = fCurrH;
		}
	}

	return Phong(Texcoord + vCurrOffset, tsLight, -tsView, dx, dy, vEyeR, wsNormal);
}

// Parallax oclussion
technique ParallaxOcclusion
{
	pass p0
	{
		VertexShader = compile vs_3_0 RenderSceneVS();
		PixelShader = compile ps_3_0 PSParallaxOcclusion();
	}

}