Shader "Custom/CircleImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 定义一个2D纹理属性命名为 _MainTex, 并默认值为白色纹理
        _Color ("Color", Color) = (1,1,1,1) // 定义一个颜色属性命名为 _Color, 默认值为白色
        _Radius ("Radius", Range(0, 1)) = 0.5 // 定义一个浮点数（范围在0到1之间）的属性命名为 _Radius, 默认值为0.5
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" } // 设置渲染队列为Overlay（覆盖队列），RenderType为透明
        LOD 200 // 设置该SubShader的LOD级别为200

        CGPROGRAM // 开始编写CG程序
        #pragma surface surf Standard alpha:blend // 使用surface 着色器，渲染模式为Standard，alpha混合
        #pragma target 3.0 // 目标着色器模型版本为3.0

        sampler2D _MainTex; // 申明我们之前定义的2D纹理
        half4 _Color; // 申明我们之前定义的颜色
        float _Radius; // 申明我们之前定义的半径

        struct Input
        {
            float2 uv_MainTex; // 输入的UV坐标，用于采样纹理
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // 获取纹理颜色并乘以Color属性，用于颜色调整
            half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            
            // 定义中心点坐标
            float2 center = float2(0.5, 0.5);

            // 计算输入UV坐标与中心点的距离
            float dist = distance(IN.uv_MainTex, center);

            // 如果距离大于我们设定的半径，将alpha值设置为0，使之透明
            if(dist > _Radius)
            {
                c.a = 0;
            }

            // 设置SurfaceOutputStandard的Albedo（反射光）为颜色的RGB值
            o.Albedo = c.rgb;

            // 设置SurfaceOutputStandard的Alpha（透明度）为颜色的alpha值
            o.Alpha = c.a;
        }
        ENDCG // 结束CG程序
    }
    FallBack "Diffuse" // 使用Diffuse作为后备渲染模式，若当前模式不支持
}
