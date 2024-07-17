using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleImageShader : MonoBehaviour
{
    public Image image; // ����UI Image���
    [Range(0, 1)]
    public float radius = 0.5f; // �뾶��Χ0��1��Ĭ��ֵΪ0.5

    private Material material; // ˽�в��ʱ���

    void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>(); // ��ȡImage���
        }

        material = new Material(Shader.Find("Custom/CircleImage")); // �����µ�Material��ʹ�����Ƕ����Shader
        image.material = material; // �����ʸ�ֵ��Image���
        UpdateShaderProperties(); // ����Shader����
    }

    void Update()
    {
        UpdateShaderProperties(); // ÿ֡����Shader����
    }

    private void UpdateShaderProperties()
    {
        if (material != null)
        {
            material.SetFloat("_Radius", radius); // ����Shader��Radius���Ե�ֵ
        }
    }
}
