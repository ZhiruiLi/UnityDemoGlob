using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class RbfMain : MonoBehaviour
{
    // 参考对象
    public Transform[] refObjects;
    // 计算对象
    public Transform[] calObjects;

    // RBF 参数矩阵
    private Matrix<float> mCoefficientMatrix;

    void Start()
    {
        var psiMatrix = CalculatePsiMatrix(refObjects, refObjects);
        var colorsMatrix = GetColorsMatrix(refObjects);
        mCoefficientMatrix = psiMatrix.LU().Solve(colorsMatrix);
    }

    void Update()
    {
        var psiMatrix = CalculatePsiMatrix(calObjects, refObjects);
        var colorsMatrix = psiMatrix * mCoefficientMatrix;
        for (var i = 0; i < calObjects.Length; i++)
        {
            var r = colorsMatrix[i, 0];
            var g = colorsMatrix[i, 1];
            var b = colorsMatrix[i, 2];
            var a = colorsMatrix[i, 3];
            var mat = calObjects[i].GetComponent<Renderer>().material;
            mat.color = new Color(r, g, b, a);
        }
    }

    Matrix<float> CalculatePsiMatrix(Transform[] src, Transform[] tar)
    {
        var builder = Matrix<float>.Build;
        Matrix<float> m = builder.Dense(src.Length, tar.Length);
        for (var i = 0; i < src.Length; i++)
        {
            for (var j = 0; j < tar.Length; j++)
            {
                m[i, j] = Vector3.Distance(src[i].position, tar[j].position);
            }
        }
        return m;
    }

    Matrix<float> GetColorsMatrix(Transform[] objs)
    {
        var builder = Matrix<float>.Build;
        Matrix<float> m = builder.Dense(objs.Length, 4);
        for (var i = 0; i < objs.Length; i++)
        {
            var color = objs[i].GetComponent<Renderer>().material.color;
            m[i, 0] = color.r;
            m[i, 1] = color.g;
            m[i, 2] = color.b;
            m[i, 3] = color.a;
        }
        return m;
    }
}
