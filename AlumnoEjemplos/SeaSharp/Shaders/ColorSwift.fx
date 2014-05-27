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

	// Animar posicion
	/*Input.Position.x += sin(time)*30*sign(Input.Position.x);
	Input.Position.y += cos(time)*30*sign(Input.Position.y-20);
	Input.Position.z += sin(time)*30*sign(Input.Position.z);
	*/

	// Animar posicion
	//float X = Input.Position.x;
	//Input.Position.y = -30 + 150 * cos(time) - 100 * sin(time);

	// calculo de la onda (movimiento grande)
	//float u = Input.Position.x;
	//float v = Input.Position.y;
	//float ola = sin(v * 2 * 3.14159 * 0.0001 * time) * cos(u * 2 * 3.14159 * 0.0001 * time);
	//Input.Position.y = ola * 100; //se lo aplicamos al eje y
	//float atenuacion = length(float2(x,z)-isla_pos.xz);
	//atenuacion = lerp(1,1-(1/atenuacion+1)+0.1,(1/atenuacion+1)*0.1);

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

	// Animar color
	//Input.Color.r = abs(sin(time));
	//Input.Color.g = abs(cos(time));

	//Propago el color x vertice
	float4 ColorOut = Input.Color;
		ColorOut.a = 0.1;
	Output.Color = ColorOut;

	return(Output);

}

//Pixel Shader
float4 ps_main(float2 Texcoord: TEXCOORD0, float4 Color : COLOR0) : COLOR0
{
	// Obtener el texel de textura
	// diffuseMap es el sampler, Texcoord son las coordenadas interpoladas
	float4 fvBaseColor = tex2D(diffuseMap, Texcoord);
	// combino color y textura
	// en este ejemplo combino un 80% el color de la textura y un 20%el del vertice
	//float4 Out = Color;
	//Out.a = 255;

	return Color;
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