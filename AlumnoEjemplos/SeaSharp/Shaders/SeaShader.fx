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


float3 fvLightPosition = float3(-100.00, 100.00, -100.00);
float3 fvEyePosition = float3(0.00, 0.00, -100.00);

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
	float2 Texcoord : TEXCOORD0;
};

//Output del Vertex Shader
struct VS_OUTPUT
{
	float4 Position :        POSITION0;
	float2 Texcoord :        TEXCOORD0;
	float4 Color :			COLOR0;
};



//Vertex Shader
VS_OUTPUT vs_main(VS_INPUT Input)
{
	VS_OUTPUT Output;
	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	Output.Color = Input.Color;

	return(Output);

}


// Ejemplo de un vertex shader que anima la posicion de los vertices 
// ------------------------------------------------------------------
VS_OUTPUT vs_main2(VS_INPUT Input)
{
	VS_OUTPUT Output;

	float x = Input.Position.x;
	float z = Input.Position.z;
	// calculo coordenadas de textura
	float u = (x / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);
	float v = (z / 100 + 4000 / 100) / (2 * (4000 / 100) + 1);

	// calculo de la onda (movimiento grande)
	float ola = sin(u * 2 * 3.14159 * 2 + time) * cos(v * 2 * 3.14159 * 2 + time);
	Input.Position.y = 1 * ola * 160; //se lo aplicamos al eje y

	//Proyectar posicion
	Output.Position = mul(Input.Position, matWorldViewProj);

	//Propago las coordenadas de textura
	Output.Texcoord = Input.Texcoord;

	//Propago el color x vertice
	float4 ColorOut = Input.Color;
		ColorOut.a = 0.1;
	Output.Color = ColorOut;

	return(Output);

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
		ld += saturate(dot(N, LD))*k_ld;

	// 2- calcula la reflexion specular
	float3 D = normalize(float3(Pos.x, Pos.y, Pos.z) - fvEyePosition);
		//float ks = saturate(dot(reflect(LD, N), D));
		float ks = saturate(dot(reflect(LD, N), D));
	ks = pow(ks, fSpecularPower);
	le += ks*k_ls;

	//Obtener el texel de textura
	//float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	float4 fvBaseColor = float4(0, 0.1, 0.2, 0);

		// suma luz diffusa, ambiente y especular
		float4 RGBColor = float4(0, 0, 0, 0.7);
		RGBColor.rgb = saturate(fvBaseColor*(saturate(k_la + ld)) + le);

	// saturate deja los valores entre [0,1]. Una tecnica muy usada en motores modernos
	// es usar floating point textures auxialres, para almacenar mucho mas que 256 valores posibles 
	// de iluminiacion. En esos casos, el valor del rgb podria ser mucho mas que 1. 
	// Imaginen una excena outdoor, a la luz de sol, hay mucha diferencia de iluminacion
	// entre los distintos puntos, que no se pueden almacenar usando solo 8bits por canal.
	// Estas tecnicas se llaman HDRLighting (High Dynamic Range Lighting). 
	// Muchas inclusive simulan el efecto de la pupila que se contrae o dilata para 
	// adaptarse a la nueva cantidad de luz ambiente. 

	return RGBColor;
}



// ------------------------------------------------------------------
technique RenderScene
{
	pass Pass_0
	{
		VertexShader = compile vs_2_0 vs_main2();
		PixelShader = compile ps_2_0 ps_main();
	}

}