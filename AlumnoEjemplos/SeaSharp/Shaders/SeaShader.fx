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
texture texDiffuseMap;
sampler2D diffuseMap = sampler_state
{
	Texture = (texDiffuseMap);
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
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



float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);
float3 fvEyeLookAt = float3(0.00, 0.00, -100.00);

float k_la = 0.7;							// luz ambiente global
float k_ld = 0.4;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;

float kx = 1;							// coef. de reflexion
float kc = 0;

float time = 0;

float4 colorAgua;

float amplitud;

float frecuencia;

float rand;

float3 shipPos;



float3 g_LightDir = float3(0, -1, 0);

float g_fSpecularExponent = 3;

bool phong_lighting = true;

float min_cant_samples = 10;
float max_cant_samples = 50;


float fHeightMapScale = 0.1;
float fTexScale = 10;



/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float3 Tex : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float4 Color :			COLOR0;
	float3 Tex: TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 PosProp : TEXCOORD2;

};


float calculate_Position(float x, float z, float aux)
{

	float y = -150;

	float u = (x / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);
	float v = (z / 75 + 8000 / 75) / (2 * (8000 / 75) + 1);

	// calculo de la onda (movimiento grande)
	float ola = sin(u * frecuencia * 3.14159 * 2 + time) * cos(v * frecuencia * 3.14159 * 2 + time);

	float A = 10;
	float f = 120 + ((x*z) / 100000);
	float Speed = 0.5f;
	float L = 10;
	float phi = Speed * 2 * 3.14159f * 2 / L;

	// Aleatoria
	// Aleatoria
	
	float ola2 = //sin(v*4 + u * 40 * frecuencia * 2 + time) * cos((u+62) * 5 * frecuencia * 3.14159 * 2 - 2 * time) *
	//	-sin(u+v * 10 * frecuencia * 3.14159 * 2 )  //cos(v * 40 * frecuencia * 3.14159 * 2)
	sin(1 * u * 2 * 3.14159 * frecuencia + time) * amplitud + cos(3 * v * 2 * 3.14159 * frecuencia + time) * amplitud +
		 (A/10) * sin(f*z * 4 + phi*time) * cos(f*x / 4 + phi*time)
		 +(A / 20) * sin(f*x / 5 + phi*time) * cos(f*z / 2 + phi * time)
		 +(A / 30) * sin(f*(x+13) / 5 + phi*time) * cos(f*(z+28) / 10 + phi * time)
		
		;

	//y = y + ola * 150 + ola2 * 10;

	//float height = tex2Dlod(heightmap, float4(u, v, 0, 0)).r;

	if (aux == 1) y = y + ola * amplitud;
	if (aux == -1) y = ola2;
	
	return y;
	//return 0;
	
}

//float3 calculate_Normal(float3 posInit, float time){
//
//	// Calculate first vector
//	float u = posInit.x + 0.1f;
//	float y1 = calculate_Position(u, posInit.z, time);
//
//	float3 newVec1 = (u, y1, posInit.z) - posInit;
//
//	// Calculate second vector
//	float i = posInit.z + 0.1f;
//	float y2 = calculate_Position(posInit.x, i, time);
//
//	float3 newVec2 = (posInit.x, y2, i) - posInit;
//
//		return cross(newVec1, newVec2);

//}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------
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
	//out float  Fresnel : COLOR
	)
{
	//VS_OUTPUT Output;

	float3 normal;

	// Actualizo el output
	oPos = Pos;
	oPos.y = calculate_Position(Pos.x, Pos.z, -1); //se lo aplicamos al eje y

	float dr = 15;

	//Proyectar posicion
		float heightx = calculate_Position(Pos.x + dr, Pos.z, -1);
	float heightz = calculate_Position(Pos.x, Pos.z + dr, -1);
	float3 dx = normalize(float3(dr, heightx - oPos.y, 0));
		float3 dz = normalize(float3(0, heightz - oPos.y, dr));

		normal = cross(dz, dx);

	PosPix = oPos;
	oPos = mul(oPos, matWorldViewProj);
	//PosAux = mul(PosAux, matWorld);
	wPos = mul(Pos, matWorld);
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

	float hei = calculate_Position(Pos.x, Pos.z, -1);
	
	float textx = smoothstep(-8000, 8000, Pos.x);
	float textz = smoothstep(-8000, 8000, Pos.z);
	float2 text = float2(textx, textz);

	float3 posmin = shipPos - float3(80, 0, 20);
		float3 posmax = shipPos + float3(80, 0, 20);

		float4 aux = tex2Dgrad(heightmap, text * 20, ddx(text), ddy(text));
		//float4 aux = float4(hei, 0, 0, 1);
		N = normalize(N + aux.xyz);

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
	float k = 0.60;
	float4 fvBaseColor = k*texCUBE(g_samCubeMap, EnvTex) +
		(1 - k)*colorAgua;
	//(1 - k)*tex2D(diffuseMap, Texcoord);

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




/**************************************************************************************/
/* RenderCubeMap */
/**************************************************************************************/
void VSCubeMap(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	float2 Texcoord : TEXCOORD0,
	float4 Color : COLOR0,
	out float3 N : NORMAL,
	out float2 Tex : TEXCOORD0,
	out float3 oPos : TEXCOORD2
	)
{
	/*float y = calculate_Position(Pos.x, Pos.z);

	float dr = 100;


		float heightx = calculate_Position(Pos.x + dr, Pos.z);
		float heightz = calculate_Position(Pos.x, Pos.z + dr);
	float3 dx = normalize(float3(dr, heightx - y, 0));
		float3 dz = normalize(float3(0, heightz - y, dr));*/
	oPos = Pos;

	N = Normal;

	Tex = Texcoord;

}




// (*) Usar las coordenadas de texturas 2, 3 y demas es un "hack" habitual,
// que permite pasarle al pixel shader distintas variables que se calculan por vertice
// El rasterizer se ocupa de que al PS le lleguen los valores interpolados. 
// El hardware no tiene idea que son todos esos valores, es lo mismo si fuesen coordenadas
// de textura reales, o factores de iluminacion, o la velocidad de un punto. 
// 

//Pixel Shader
float4 ps_main(float3 Texcoord: TEXCOORD0, float3 N : TEXCOORD1,
	float3 Pos : TEXCOORD2) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	//float3 normal = tex2D(heightmap, Pos).xyz;
	N = normalize(N);

	// si hubiera varias luces, se podria iterar por c/u. 
	// Pero hay que tener en cuenta que este algoritmo es bastante pesado
	// ya que todas estas formulas se calculan x cada pixel. 
	// En la practica no es usual tomar mas de 2 o 3 luces. Generalmente 
	// se determina las luces que mas contribucion a la escena tienen, y 
	// el resto se aproxima con luz ambiente. 
	// for(int =0;i<cant_ligths;++i)
	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition - float3(Pos.x, Pos.y, Pos.z));
		ld += saturate(abs(dot(N, LD)))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x, Pos.y, Pos.z) - fvEyePosition);
		float ks = saturate(dot(reflect(LD, N), D));
		//float ks = saturate(dot(normalize(LD + D), N));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	//float4 fvBaseColor = col;
	//float4 fvBaseColor = texCUBE(g_samCubeMap, Texcoord);
	float4 fvBaseColor = float4(0.20, 0.35, 0.40, 0);
	/*float a = acos(dot(D, N));
	a = saturate(sin(a) + 0.2);
	float b = clamp(a, 0.8, 1);*/
	
		// suma luz diffusa, ambiente y especular
		float4 RGBColor = float4(0, 0, 0, 1);
		RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);

	// saturate deja los valores entre [0,1]. Una tecnica muy usada en motores modernos
	// es usar floating point textures auxialres, para almacenar mucho mas que 256 valores posibles 
	// de iluminiacion. En esos casos, el valor del rgb podria ser mucho mas que 1. 
	// Imaginen una excena outdoor, a la luz de sol, hay mucha diferencia de iluminacion
	// entre los distintos puntos, que no se pueden almacenar usando solo 8bits por canal.
	// Estas tecnicas se llaman HDRLighting (High Dynamic Range Lighting). 
	// Muchas inclusive simulan el efecto de la pupila que se contrae o dilata para 
	// adaptarse a la nueva cantidad de luz ambiente. 

	//return RGBColor;
	//return float4 (N, 1);
	return RGBColor;
}

technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 VSCubeMap2();
		PixelShader = compile ps_3_0 ps_main();
	}

}



// calcula la iluminaciond dinamica
float4 Phong(float2 texCoord, float3 vLightTS, float3 vViewTS, float dx, float dy)
{
	// Color Basico
	//float4 cBaseColor = tex2Dgrad(auxMap, texCoord, dx, dy);
		float4 cBaseColor = colorAgua;
		if (phong_lighting)
		{
		// Busco el vector normal en la textura Normal-Height Map  (esta en Tangent Space)
		float3 vNormalTS = normalize(tex2Dgrad(heightmap, texCoord, dx, dy) * 2 - 1);


			// Color difuso
			float3 vLightTSAdj = float3(vLightTS.x, -vLightTS.y, vLightTS.z);
			float cDiffuse = saturate(dot(vNormalTS, vLightTSAdj)) * k_ld;

		// Color specular
		float4 cSpecular = 0;
			float3 vReflectionTS = normalize(2 * dot(vViewTS, vNormalTS) * vNormalTS - vViewTS);
			float fRdotL = saturate(dot(vReflectionTS, vLightTSAdj));
		cSpecular = saturate(pow(fRdotL, g_fSpecularExponent))*k_ls;

		// Retorno color difuso	+ luz especular
		cBaseColor = (k_la + cDiffuse)*cBaseColor + cSpecular;
		}
	return cBaseColor;
}


void RenderSceneVS(float4 Pos : POSITION,
	float2 Texcoord : TEXCOORD0,
	float3 normal : NORMAL,
	out float4 oPos : POSITION,
	out float2 Tex : TEXCOORD0,
	out float3 tsView : TEXCOORD1,
	out float3 tsLight : TEXCOORD3,
	out float3 wsNormal : TEXCOORD2,
	out float3 wsView : TEXCOORD4
	)
{

	// Vector View = desde el ojo a la pos del vertice
	float4 VertexPositionWS = mul(Pos, matWorld);
		wsView = fvEyePosition.xyz - VertexPositionWS.xyz;


	// calculo la tg y la binormal ?
	float3 up = float3(0, 0, 1);
		if (abs(normal.z - 1) <= 0.0001)
			up = float3(0, 1, 0);
	float3 tangent = cross(normal, up);
		float3 binormal = cross(normal, tangent);

		/*
		// o la dejo fija?
		normal = float3(0,1,0);
		float3 tangent = float3(1,0,0);
		float3 binormal = float3(0,0,1);
		*/

		float3x3 tangentToWorldSpace;
	tangentToWorldSpace[0] = mul(tangent, matWorld);
	tangentToWorldSpace[1] = mul(binormal, matWorld);
	tangentToWorldSpace[2] = mul(normal, matWorld);

	// tangentToWorldSpace es una matriz ortogonal, su inversa = a su transpuesta
	// A es OrtoNorm <=> A-1 == At
	float3x3 worldToTangentSpace = transpose(tangentToWorldSpace);

		// proyecto
		oPos = mul(Pos, matWorldViewProj);
	//Propago la textura
	Tex = float2(Pos.x, Pos.z);

	tsView = mul(wsView, worldToTangentSpace);		// Vector View en TangentSpace
	tsLight = mul(g_LightDir, worldToTangentSpace);	// Vector Light en TangentSpace

	// propago el vector normal en Worldspace
	wsNormal = normal;
	// tambien devuelve el vector view en worldspace wsView

}


// ws = worldspace    ts = tangentspace
float4 PSParallaxOcclusion(float2 Texcoord: TEXCOORD0,
	float3 Pos : POSITION,
	float3 tsView : TEXCOORD1,
	float3 wsNormal : TEXCOORD2,
	float3 tsLight : TEXCOORD3,
	float3 wsView : TEXCOORD4
	) : COLOR0
{
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

	while (nCurrSample < nNumSamples)
	{
		vCurrSample = tex2Dgrad(heightmap, Texcoord + vCurrOffset, dx, dy);
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

	return Phong(Texcoord + vCurrOffset, tsLight, -tsView, dx, dy);
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