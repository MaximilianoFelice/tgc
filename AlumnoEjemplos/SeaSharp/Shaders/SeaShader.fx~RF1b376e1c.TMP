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

texture g_RenderTarget;
sampler RenderTarget =
sampler_state
{
	Texture = <g_RenderTarget>;
	MINFILTER = LINEAR;
	MAGFILTER = LINEAR;
	MIPFILTER = LINEAR;
};


texture superficieAgua;
sampler heightmap = sampler_state
{
	Texture = <superficieAgua>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = Clamp;
	AddressV = Clamp;
};



float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);
float3 fvEyeLookAt = float3(0.00, 0.00, -100.00);

float k_la = 0.7;							// luz ambiente global
float k_ld = 0.4;							// luz difusa
float k_ls = 1.0;							// luz specular
float fSpecularPower = 16.84;

float time = 0;


/**************************************************************************************/
/* RenderScene */
/**************************************************************************************/

//Input del Vertex Shader
struct VS_INPUT
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Tex : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float4 Color :			COLOR0;
	float2 Tex: TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 PosProp : TEXCOORD2;

};


float calculate_Position(float x, float z)
{

	float y = 1;

	float u = (x / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);
	float v = (z / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);

	// calculo de la onda (movimiento grande)
	float ola = sin(u * 2 * 3.14159 * 2 + time) * cos(v * 2 * 3.14159 * 2 + time);

	y = y * ola * 200;

	//float height = tex2Dlod(heightmap, float4(u, v, 0, 0)).r;

	//y = y + height * float(50);
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
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;

	float3 normal;

	// Actualizo el output
	Output.Position = Input.Position;
	Output.Position.y = calculate_Position(Input.Position.x, Input.Position.z); //se lo aplicamos al eje y

	float dr = 100;

	//Proyectar posicion
	float4 PosAux = Output.Position;
		float heightx = calculate_Position(Input.Position.x + dr, Input.Position.z);
	float heightz = calculate_Position(Input.Position.x, Input.Position.z + dr);
	float3 dx = normalize(float3(dr, heightx - Output.Position.y, 0));
		float3 dz = normalize(float3(0, heightz - Output.Position.y, dr));

	Output.Position = mul(Output.Position, matWorldViewProj);
	//PosAux = mul(PosAux, matWorld);

	Output.PosProp = float3(PosAux.x, PosAux.y, PosAux.z);

	//Propago el color x vertice
	float4 ColorOut = Input.Color;
	Output.Color = ColorOut;

	// Propago la normal
	//normal = calculate_Normal(Input.Position.xyz, time);


	normal = cross(dz, dx);
	Output.Normal = mul(normal, matWorld);

	Output.Tex = Input.Tex;

	return(Output);

}



// (*) Usar las coordenadas de texturas 2, 3 y demas es un "hack" habitual,
// que permite pasarle al pixel shader distintas variables que se calculan por vertice
// El rasterizer se ocupa de que al PS le lleguen los valores interpolados. 
// El hardware no tiene idea que son todos esos valores, es lo mismo si fuesen coordenadas
// de textura reales, o factores de iluminacion, o la velocidad de un punto. 
// 

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float3 N : TEXCOORD1,
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
	float4 fvBaseColor = tex2Dgrad(RenderTarget, Texcoord, ddx(Texcoord), ddy(Texcoord));
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
	return fvBaseColor;
}


void main_ps2(float3 position  : TEXCOORD2,

float3 normal : TEXCOORD1,



out float4 color : COLOR)

{

	float3 globalAmbient = (0.7, 0.7, 0.7);

		float3 lightColor = (1, 1, 1);

		float3 lightPosition = fvLightPosition;

		float3 eyePosition = fvEyePosition;

		float3 Ke = (0, 0, 0);

		float3 Ka = (0, 0, 0);

		float3 Kd = (0.5, 0.5, 0.5);

	 float3 Ks = (1, 1, 1);

	 float  shininess = fSpecularPower;



	float3 P = position;

		float3 N = normalize(normal);



		// Compute the emissive term

		float3 emissive = Ke;



		// Compute the ambient term

		float3 ambient = Ka * globalAmbient;



		// Compute the diffuse term

		float3 L = normalize(lightPosition - P);

		float diffuseLight = max(dot(N, L), 0);

	float3 diffuse = Kd * lightColor * diffuseLight;



		// Compute the specular term

		float3 V = normalize(eyePosition - P);

		float3 H = normalize(L + V);

		float specularLight = pow(max(dot(N, H), 0),

		shininess);

	if (diffuseLight <= 0) specularLight = 0;

	float3 specular = Ks * lightColor * specularLight;



		color.xyz = emissive + ambient + diffuse + specular;

	color.w = 1;

}


// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_3_0 vs_main();
		PixelShader = compile ps_3_0 ps_main();
	}

}