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
	MIPFILTER = Point;
	MINFILTER = Point;
	MAGFILTER = Point;
	ADDRESSU = Clamp;
	ADDRESSV = Clamp;
};



float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);
float3 fvEyeLookAt = float3(0.00, 0.00, -100.00);

float k_la = 0.7;							// luz ambiente global
float k_ld = 0.4;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;

float kx = 0.7;							// coef. de reflexion
float kc = 0;

float time = 0;


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


float calculate_Position(float x, float z)
{

	float y = 250;

	float u = (x / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);
	float v = (z / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);

	// calculo de la onda (movimiento grande)
	float ola = sin(u * 2 * 3.14159 * 2 + time) * cos(v * 2 * 3.14159 * 2 + time);
	float ola2 = clamp(sin(u * 20 * 3.14159 * 2 + time/2) * cos(v * 20 * 3.14159 * 2 + time/2),0.5,1);

	y =  ola * 150;

	float height = tex2Dlod(heightmap, float4(u, v, 0, 0)).r;

	y = y - height * 250;
	return y;
	//return 100;
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
	out float3 wPos : TEXCOORD6
	//out float  Fresnel : COLOR
	)
{
	//VS_OUTPUT Output;

	float3 normal;

	// Actualizo el output
	oPos = Pos;
	oPos.y = calculate_Position(Pos.x, Pos.z); //se lo aplicamos al eje y

	float dr = 10;

	//Proyectar posicion
	float4 PosAux = oPos;
		float heightx = calculate_Position(Pos.x + dr, Pos.z);
	float heightz = calculate_Position(Pos.x, Pos.z + dr);
	float3 dx = normalize(float3(dr, heightx - oPos.y, 0));
		float3 dz = normalize(float3(0, heightz - oPos.y, dr));

		normal = cross(dz, dx);

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
	Tex = Texcoord;

	//Propago la normal
	N = vN;

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
	//N = normalize(normal);

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
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	//float4 fvBaseColor = texCUBE(g_samCubeMap, Texcoord);
	//float4 fvBaseColor = float4(0.20, 0.35, 0.40, 0);
	float a = acos(dot(D, N));
	a = saturate(sin(a) + 0.2);
	float b = clamp(a, 0.8, 1);
	
		// suma luz diffusa, ambiente y especular
		float4 RGBColor = float4(0, 0, 0, b);
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

/**************************************************************************************/
/* RenderCubeMap */
/**************************************************************************************/
void VSCubeMap(float4 Pos : POSITION,
	float3 Normal : NORMAL,
	float2 Texcoord : TEXCOORD0,
	out float4 oPos : POSITION,
	out float3 EnvTex : TEXCOORD0,
	out float2 Tex : TEXCOORD2,
	out float3 N : TEXCOORD1,
	out float3 EnvTex1 : TEXCOORD3,
	out float3 EnvTex2 : TEXCOORD4,
	out float3 EnvTex3 : TEXCOORD5,
	out float3 wPos : TEXCOORD6
	//out float  Fresnel : COLOR
	)
{
	/*float y = calculate_Position(Pos.x, Pos.z);

	float dr = 100;


		float heightx = calculate_Position(Pos.x + dr, Pos.z);
		float heightz = calculate_Position(Pos.x, Pos.z + dr);
	float3 dx = normalize(float3(dr, heightx - y, 0));
		float3 dz = normalize(float3(0, heightz - y, dr));*/

	float3 dx = float3(1, 0, 0);
		float3 dz = float3(0, 0, 1);

		Normal = cross(dz, dx);

	wPos = mul(Pos, matWorld);
	float3 vEyeR = normalize(wPos - fvEyePosition);

		// corrijo la normal (depende de la malla)
		// ej. el tanque esta ok, la esfera esta invertida.
		//Normal*= -1;
		float3 vN = mul(Normal, (float3x3)matWorld);
		vN = normalize(vN);
	EnvTex = reflect(vEyeR, vN);

	// Refraccion de la luz
	EnvTex1 = refract(vEyeR, vN, 1.001);
	EnvTex2 = refract(vEyeR, vN, 1.009);
	EnvTex3 = refract(vEyeR, vN, 1.02);
	//Fresnel = FBias + FEscala*pow(1 + dot(vEyeR, vN), FPower);

	// proyecto
	oPos = mul(Pos, matWorldViewProj);

	//Propago la textura
	Tex = Texcoord;

	//Propago la normal
	N = vN;

}


float4 PSCubeMap(float3 EnvTex: TEXCOORD0,
	float3 N : TEXCOORD1,
	float3 Texcoord : TEXCOORD2,
	float3 Tex1 : TEXCOORD3,
	float3 Tex2 : TEXCOORD4,
	float3 Tex3 : TEXCOORD5,
	//float Fresnel : COLOR,
	float3 wPos : TEXCOORD6
	) : COLOR0
{
	float ld = 0;		// luz difusa
	float le = 0;		// luz specular

	N = normalize(N);

	// 1- calculo la luz diffusa
	float3 LD = normalize(fvLightPosition - wPos);
		ld += saturate(dot(N, LD))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = normalize(wPos - fvEyePosition);
		float ks = saturate(dot(reflect(LD, N), D));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	float k = 0.60;
	float4 fvBaseColor = k*texCUBE(g_samCubeMap, EnvTex) +
		(1 - k)*float4(0.5,0.5,0.5,1);
		//(1 - k)*tex2D(diffuseMap, Texcoord);

	// suma luz diffusa, ambiente y especular
	fvBaseColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);
	fvBaseColor.a = 0.8;
	float4 color_reflejado = fvBaseColor;

		float4 color_refractado = float4(
		texCUBE(g_samCubeMap, Tex1).x,
		texCUBE(g_samCubeMap, Tex2).y,
		texCUBE(g_samCubeMap, Tex3).z,
		1);
	//float4 color_refractado = texCUBE( g_samCubeMap, Tex1);

		return color_reflejado*kx + color_refractado*kc;
	return float4(N, 1);
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

