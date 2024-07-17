using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CircleImage : Image
{
    [SerializeField]
    public int segements = 100;  // Բ���ɶ��ٿ�������ƴ�ɣ��ֶ���Խ��Բ��Խƽ��

    [SerializeField]
    public float showPercent = 1;  // ��ʾ����ռԲ�εİٷֱȣ�����0.5��ʾ��Բ

    public readonly Color32 GRAY_COLOR = new Color32(60, 60, 60, 255);  // ��ɫ������δ��ʾ����

    public List<Vector3> _vertexList;  // ���ڴ洢������б�

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();  // ���֮ǰ�Ķ���

        _vertexList = new List<Vector3>();

        AddVertex(vh, segements);  // ��Ӷ���

        AddTriangle(vh, segements);  // ���������
    }

    private void AddVertex(VertexHelper vh, int segements)
    {
        float width = rectTransform.rect.width;  // ��ȡ���εĿ��
        float heigth = rectTransform.rect.height;  // ��ȡ���εĸ߶�
        int realSegments = (int)(segements * showPercent);  // ����ʵ�ʵķֶ���

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;  // ��ȡͼ���UV����

        float uvWidth = uv.z - uv.x;  // ����UV���
        float uvHeight = uv.w - uv.y;  // ����UV�߶�
        Vector2 uvCenter = new Vector2(uvWidth * 0.5f, uvHeight * 0.5f);  // ����UV�����ĵ�
        Vector2 convertRatio = new Vector2(uvWidth / width, uvHeight / heigth);  // ����UV���ű���

        float radian = (2 * Mathf.PI) / segements;  // ÿ���ֶεĻ���
        float radius = width * 0.5f;  // �뾶

        Vector2 originPos = new Vector2((0.5f - rectTransform.pivot.x) * width, (0.5f - rectTransform.pivot.y) * heigth);  // ����ԭ��λ��
        Vector2 vertPos = Vector2.zero;  // ��ʼ������λ��Ϊ��

        Color32 colorTemp = GetOriginColor();  // ��ȡ��ɫ
        UIVertex origin = GetUIVertex(colorTemp, originPos, vertPos, uvCenter, convertRatio);  // �������Ķ���
        vh.AddVert(origin);  // ������Ķ���

        int vertexCount = realSegments + 1;  // ��������
        float curRadian = 0;  // ��ǰ����
        Vector2 posTermp = Vector2.zero;  // ��ʱ����λ��
        for (int i = 0; i < segements + 1; i++)  // ѭ����Ӷ���
        {
            float x = Mathf.Cos(curRadian) * radius;  // ���㶥���x����
            float y = Mathf.Sin(curRadian) * radius;  // ���㶥���y����
            curRadian += radian;  // ��ǰ��������

            if (i < vertexCount)
            {
                colorTemp = color;  // �׶ε���ɫ
            }
            else
            {
                colorTemp = GRAY_COLOR;  // û��¶�Ĳ����û�ɫ
            }
            posTermp = new Vector2(x, y);  // ��������λ��
            UIVertex vertexTemp = GetUIVertex(colorTemp, posTermp + originPos, posTermp, uvCenter, convertRatio);  // ��������
            vh.AddVert(vertexTemp);  // ��Ӷ���
            _vertexList.Add(posTermp + originPos);  // �洢����λ��
        }
    }

    private Color32 GetOriginColor()
    {
        Color32 colorTemp = (Color.white - GRAY_COLOR) * showPercent;  // ������ɫ��ֵ
        return new Color32(
            (byte)(GRAY_COLOR.r + colorTemp.r),  // �����ɫ����
            (byte)(GRAY_COLOR.g + colorTemp.g),  // ������ɫ����
            (byte)(GRAY_COLOR.b + colorTemp.b),  // ������ɫ����
            255);  // ��͸����
    }

    private void AddTriangle(VertexHelper vh, int realSegements)
    {
        int id = 1;  // ��һ�������ID��1��0�����ĵ�
        for (int i = 0; i < realSegements; i++)  // ѭ�����������
        {
            vh.AddTriangle(id, 0, id + 1);  // ���һ��������
            id++;
        }
    }

    private UIVertex GetUIVertex(Color32 col, Vector3 pos, Vector2 uvPos, Vector2 uvCenter, Vector2 uvScale)
    {
        UIVertex vertexTemp = new UIVertex();  // ����һ��UI����
        vertexTemp.color = col;  // ���ö�����ɫ
        vertexTemp.position = pos;  // ���ö���λ��
        vertexTemp.uv0 = new Vector2(uvPos.x * uvScale.x + uvCenter.x, uvPos.y * uvScale.y + uvCenter.y);  // ����UV����
        return vertexTemp;
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector2 localPoint;  // �ֲ���
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint);  // ת����Ļ��Ϊ�ֲ���
        return IsValid(localPoint);  // �жϾֲ����Ƿ���Ч
    }

    private bool IsValid(Vector2 localPoint)
    {
        return GetCrossPointNum(localPoint, _vertexList) % 2 == 1;  // ��齻�����
    }

    private int GetCrossPointNum(Vector2 localPoint, List<Vector3> vertexList)
    {
        int count = 0;  // ��������
        Vector3 vert1 = Vector3.zero;  // ����1
        Vector3 vert2 = Vector3.zero;  // ����2
        int vertCount = vertexList.Count;  // ��������

        for (int i = 0; i < vertCount; i++)  // ѭ�����ж���
        {
            vert1 = vertexList[i];  // ��ǰ����
            vert2 = vertexList[(i + 1) % vertCount];  // ��һ���㣬��״

            if (IsYInRang(localPoint, vert1, vert2))
            {
                if (localPoint.x < GetX(vert1, vert2, localPoint.y))  // �ж�Xλ��
                {
                    count++;  // ��������
                }
            }
        }

        return count;
    }

    private bool IsYInRang(Vector2 localPoint, Vector3 vert1, Vector3 vert2)
    {
        if (vert1.y > vert2.y)  // �ж�Y����Χ
        {
            return localPoint.y < vert1.y && localPoint.y > vert2.y;  // �ж��Ƿ��ڷ�Χ��
        }
        else
        {
            return localPoint.y < vert2.y && localPoint.y > vert1.y;  // �ж��Ƿ��ڷ�Χ��
        }
    }

    private float GetX(Vector3 vert1, Vector3 vert2, float y)
    {
        float k = (vert1.y - vert2.y) / (vert1.x - vert2.x);  // ����б��
        return vert1.x + (y - vert1.y) / k;  // ��б�ʹ�ʽ��x����
    }
}
