const float TAU = 6.28318530718;
const float MAX_ITER = 5;

	float time = iTime * .5+23.0;
    // uv should be the 0-1 uv of texture...
	float2 uv = fragCoord; //.xy / iResolution.xy;
    
#ifdef SHOW_TILING
	float2 p = (uv*TAU*2.0)% TAU-250.0;
#else
    float2 p = (uv*TAU, TAU)% -250.0;
#endif
	float2 i = float2(p);
	float c = 1.0;
	float inten = .005;

	for (int n = 0; n < MAX_ITER; n++) 
	{
		float t = time * (1.0 - (3.5 / float(n+1)));
		i = p + float2(cos(t - i.x) + sin(t + i.y), sin(t - i.y) + cos(t + i.x));
		c += 1.0/length(float2(p.x / (sin(i.x+t)/inten),p.y / (cos(i.y+t)/inten)));
	}
	c /= float(MAX_ITER);
	c = 1.17-pow(c, 1.4);
	float3 colour = float3(pow(abs(c), 8.0), 0, 0);
    colour = clamp(colour + float3(0.0, 0.35, 0.5), 0.0, 1.0);

	#ifdef SHOW_TILING
	// Flash tile borders...
	vec2 pixel = 2.0 / iResolution.xy;
	uv *= 2.0;
	float f = floor(mod(iTime*.5, 2.0)); 	// Flash value.
	vec2 first = step(pixel, uv) * f;		   	// Rule out first screen pixels and flash.
	uv  = step(fract(uv), pixel);				// Add one line of pixels per tile.
	colour = mix(colour, float3(1.0, 1.0, 0.0), (uv.x + uv.y) * first.x * first.y); // Yellow line
	#endif
	fragColor = float4(colour, 1.0);