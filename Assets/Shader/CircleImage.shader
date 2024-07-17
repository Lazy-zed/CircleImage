Shader "Custom/CircleImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // ����һ��2D������������Ϊ _MainTex, ��Ĭ��ֵΪ��ɫ����
        _Color ("Color", Color) = (1,1,1,1) // ����һ����ɫ��������Ϊ _Color, Ĭ��ֵΪ��ɫ
        _Radius ("Radius", Range(0, 1)) = 0.5 // ����һ������������Χ��0��1֮�䣩����������Ϊ _Radius, Ĭ��ֵΪ0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" } // ������Ⱦ����ΪOverlay�����Ƕ��У���RenderTypeΪ͸��
        LOD 200 // ���ø�SubShader��LOD����Ϊ200

        CGPROGRAM // ��ʼ��дCG����
        #pragma surface surf Standard alpha:blend // ʹ��surface ��ɫ������ȾģʽΪStandard��alpha���
        #pragma target 3.0 // Ŀ����ɫ��ģ�Ͱ汾Ϊ3.0

        sampler2D _MainTex; // ��������֮ǰ�����2D����
        half4 _Color; // ��������֮ǰ�������ɫ
        float _Radius; // ��������֮ǰ����İ뾶

        struct Input
        {
            float2 uv_MainTex; // �����UV���꣬���ڲ�������
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // ��ȡ������ɫ������Color���ԣ�������ɫ����
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            // �������ĵ�����
            float2 center = float2(0.5, 0.5);

            // ��������UV���������ĵ�ľ���
            float dist = distance(IN.uv_MainTex, center);

            // ���������������趨�İ뾶����alphaֵ����Ϊ0��ʹ֮͸��
            if(dist > _Radius)
            {
                c.a = 0;
            }

            // ����SurfaceOutputStandard��Albedo������⣩Ϊ��ɫ��RGBֵ
            o.Albedo = c.rgb;

            // ����SurfaceOutputStandard��Alpha��͸���ȣ�Ϊ��ɫ��alphaֵ
            o.Alpha = c.a;
        }
        ENDCG // ����CG����
    }
    FallBack "Diffuse" // ʹ��Diffuse��Ϊ����Ⱦģʽ������ǰģʽ��֧��
}
