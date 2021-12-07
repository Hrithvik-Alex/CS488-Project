float SAT(float v)
{
    return clamp(v, 0, 1);
}

float Remap(float v, float lo, float ho, float ln, float hn) {
    return ln + (v-lo)*(hn-ln)/(ho-lo);
}

float L_i(float v0, float v1, float i_val) {
    return (1-i_val)*v0 + i_val*v1;
}

float inverseExp(float v) {
    return exp(-v);
}

float3 inverseExp(float3 v) {
    return exp(-v);
}

float2 rayBoxInt(float3 rayO, float3 RayD, float3 minB, float3 maxB ) {
    // modified slab test

    float3 t0 = (minB - rayO) / RayD;
    float3 t1 = (maxB - rayO) / RayD;
    float3 tmin = min(t0, t1);
    float3 tmax = max(t0, t1);
                
    float dstA = max(max(tmin.x, tmin.y), tmin.z);
    float dstB = min(tmax.x, min(tmax.y, tmax.z));

    float dstToBox = max(0, dstA);
    float boxTravelDist = max(0, dstB - dstToBox);
    return float2(dstToBox, boxTravelDist);
}

float2 quadraticRoots( float A, float B, float C)
{
	// from A4 polyroots.cpp

	float D;
	float q;

	if( A == 0 ) {
		if( B == 0 ) {
			return float2(-1,-1);
		} else {	
			return float2(-C/B,-1);
		}
	} else {
		float2 roots;
		/*  Compute the discrimanant D=b^2 - 4ac */
		D = B*B - 4*A*C;
		if( D < 0 ) {
			return float2(-1,-1);
		} else {
			/* Two real roots */
			q = -( B + sign(B)*sqrt(D) ) / 2.0;
			roots.x = q / A;
			if( q != 0 ) {
				roots.y = C / q;
			} else {
				roots.y = roots.x;
			}
			return roots;
		}
	}
}

float2 ray_sphere_intersect(float3 rayO, float3 rayD, float3 spherePos, float sphereRadius) {
	// modified ray sphere intersect from A4

	float2 roots = quadraticRoots(dot(rayD,rayD), dot(2*rayD,rayO-spherePos),
	dot(rayO-spherePos,rayO-spherePos) - sphereRadius*sphereRadius);
	          
	if(roots[0] > 0 && roots[1] > 0) {
		float close = min(roots[0],roots[1]);
		float far = max(roots[0], roots[1]);
		return float2(close, far-close);
	}   else if(roots[0] > 0 || roots[1] > 0) { // inside sphere
		float far = max(roots[0], roots[1]);
		return float2(0, far);
    }   else {
		return float2(999999,0);
	}

}