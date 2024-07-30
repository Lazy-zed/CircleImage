using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

public class CircleImage : Image
{
    [SerializeField]
    public int segements = 100;  // 圆形由多少块三角形拼成，分段数越多圆形越平滑

    [SerializeField]
    public float showPercent = 1;  // 显示部分占圆形的百分比，比如0.5表示半圆

    public readonly Color32 GRAY_COLOR = new Color32(60, 60, 60, 255);  // 灰色，用于未显示部分

    public List<Vector3> _vertexList;  // 用于存储顶点的列表

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();  // 清除之前的顶点

        _vertexList = new List<Vector3>();

        AddVertex(vh, segements);  // 添加顶点

        AddTriangle(vh, segements);  // 添加三角形
    }

    private void AddVertex(VertexHelper vh, int segements)
    {
        float width = rectTransform.rect.width;  // 获取矩形的宽度
        float heigth = rectTransform.rect.height;  // 获取矩形的高度
        int realSegments = (int)(segements * showPercent);  // 计算实际的分段数

        Vector4 uv = overrideSprite != null ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;  // 获取图像的UV坐标

        float uvWidth = uv.z - uv.x;  // 计算UV宽度
        float uvHeight = uv.w - uv.y;  // 计算UV高度
        Vector2 uvCenter = new Vector2(uvWidth * 0.5f, uvHeight * 0.5f);  // 计算UV的中心点
        Vector2 convertRatio = new Vector2(uvWidth / width, uvHeight / heigth);  // 计算UV缩放比率

        float radian = (2 * Mathf.PI) / segements;  // 每个分段的弧度
        float radius = width * 0.5f;  // 半径

        Vector2 originPos = new Vector2((0.5f - rectTransform.pivot.x) * width, (0.5f - rectTransform.pivot.y) * heigth);  // 计算原点位置
        Vector2 vertPos = Vector2.zero;  // 初始化顶点位置为零

        Color32 colorTemp = GetOriginColor();  // 获取颜色
        UIVertex origin = GetUIVertex(colorTemp, originPos, vertPos, uvCenter, convertRatio);  // 创建中心顶点
        vh.AddVert(origin);  // 添加中心顶点

        int vertexCount = realSegments + 1;  // 顶点数量
        float curRadian = 0;  // 当前弧度
        Vector2 posTermp = Vector2.zero;  // 临时顶点位置
        for (int i = 0; i < segements + 1; i++)  // 循环添加顶点
        {
            float x = Mathf.Cos(curRadian) * radius;  // 计算顶点的x坐标
            float y = Mathf.Sin(curRadian) * radius;  // 计算顶点的y坐标
            curRadian += radian;  // 当前弧度增加

            if (i < vertexCount)
            {
                colorTemp = color;  // 首段的颜色
            }
            else
            {
                colorTemp = GRAY_COLOR;  // 没显露的部分用灰色
            }
            posTermp = new Vector2(x, y);  // 创建顶点位置
            UIVertex vertexTemp = GetUIVertex(colorTemp, posTermp + originPos, posTermp, uvCenter, convertRatio);  // 创建顶点
            vh.AddVert(vertexTemp);  // 添加顶点
            _vertexList.Add(posTermp + originPos);  // 存储顶点位置
        }
    }

    private Color32 GetOriginColor()
    {
        Color32 colorTemp = (Color.white - GRAY_COLOR) * showPercent;  // 计算颜色差值
        return new Color32(
            (byte)(GRAY_COLOR.r + colorTemp.r),  // 计算红色分量
            (byte)(GRAY_COLOR.g + colorTemp.g),  // 计算绿色分量
            (byte)(GRAY_COLOR.b + colorTemp.b),  // 计算蓝色分量
            255);  // 不透明度
    }

    private void AddTriangle(VertexHelper vh, int realSegements)
    {
        int id = 1;  // 第一个顶点的ID是1，0是中心点
        for (int i = 0; i < realSegements; i++)  // 循环添加三角形
        {
            vh.AddTriangle(id, 0, id + 1);  // 添加一个三角形
            id++;
        }
    }

    private UIVertex GetUIVertex(Color32 col, Vector3 pos, Vector2 uvPos, Vector2 uvCenter, Vector2 uvScale)
    {
        UIVertex vertexTemp = new UIVertex();  // 创建一个UI顶点
        vertexTemp.color = col;  // 设置顶点颜色
        vertexTemp.position = pos;  // 设置顶点位置
        vertexTemp.uv0 = new Vector2(uvPos.x * uvScale.x + uvCenter.x, uvPos.y * uvScale.y + uvCenter.y);  // 计算UV坐标
        return vertexTemp;
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Vector2 localPoint;  // 局部点
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out localPoint);  // 转换屏幕点为局部点
        return IsValid(localPoint);  // 判断局部点是否有效
    }

    private bool IsValid(Vector2 localPoint)
    {
        return GetCrossPointNum(localPoint, _vertexList) % 2 == 1;  // 检查交叉点数
    }
    //Test
    private int GetCrossPointNum(Vector2 localPoint, List<Vector3> vertexList)
    {
        int count = 0;  // 交叉点计数
        Vector3 vert1 = Vector3.zero;  // 顶点1
        Vector3 vert2 = Vector3.zero;  // 顶点2
        int vertCount = vertexList.Count;  // 顶点总数

        for (int i = 0; i < vertCount; i++)  // 循环所有顶点
        {
            vert1 = vertexList[i];  // 当前顶点
            vert2 = vertexList[(i + 1) % vertCount];  // 下一顶点，环状

            if (IsYInRang(localPoint, vert1, vert2))
            {
                if (localPoint.x < GetX(vert1, vert2, localPoint.y))  // 判断X位置
                {
                    count++;  // 交叉点计数
                }
            }
        }

        return count;
    }

    private bool IsYInRang(Vector2 localPoint, Vector3 vert1, Vector3 vert2)
    {
        if (vert1.y > vert2.y)  // 判断Y方向范围
        {
            return localPoint.y < vert1.y && localPoint.y > vert2.y;  // 判断是否在范围内
        }
        else
        {
            return localPoint.y < vert2.y && localPoint.y > vert1.y;  // 判断是否在范围内
        }
    }

    private float GetX(Vector3 vert1, Vector3 vert2, float y)
    {
        float k = (vert1.y - vert2.y) / (vert1.x - vert2.x);  // 计算斜率
        return vert1.x + (y - vert1.y) / k;  // 用斜率公式求x坐标
    }
}
