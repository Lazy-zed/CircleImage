using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleImageShader : MonoBehaviour
{
    public Image image; // 引用UI Image组件
    [Range(0, 1)]
    public float radius = 0.5f; // 半径范围0到1，默认值为0.5

    private Material material; // 私有材质变量

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>(); // 获取Image组件
        }

        material = new Material(Shader.Find("Custom/CircleImage")); // 创建新的Material并使用我们定义的Shader
        image.material = material; // 将材质赋值给Image组件
        UpdateShaderProperties(); // 更新Shader属性
    }

    void Update()
    {
        UpdateShaderProperties(); // 每帧更新Shader属性
    }

    private void UpdateShaderProperties()
    {
        if (material != null)
        {
            material.SetFloat("_Radius", radius); // 设置Shader中Radius属性的值
        }
    }
}
