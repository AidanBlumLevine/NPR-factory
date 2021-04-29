void Softmax_float(float In1, float In2, float In3, float softness, out float Out1, out float Out2, out float Out3)
{
    // float In1Vs2 = smoothstep(In2 - softness, In2 + softness, In1) * In1;
    // float In2Vs1 = smoothstep(In1 - softness, In1 + softness, In2) * In2;
    // float In3Vs2 = smoothstep(In2 - softness, In2 + softness, In3) * In3;
    // Out1 = smoothstep(In3 - softness, In3 + softness, In1Vs2);
    // Out2 = smoothstep(In3 - softness, In3 + softness, In2Vs1);
    Out3 = In3 - (In2 + In1)/2;
    Out2 = In2 - (In3 + In1)/2;
    Out1 = In1 - (In2 + In3)/2;
    //Out3 = smoothstep(In1 - softness, In1 + softness, In3Vs2);
    Out1 = smoothstep(-softness,softness,In1-max(In3,In2));
    Out2 = smoothstep(-softness,softness,In2-max(In1,In3));
    Out3 = smoothstep(-softness,softness,In3-max(In1,In2));
}