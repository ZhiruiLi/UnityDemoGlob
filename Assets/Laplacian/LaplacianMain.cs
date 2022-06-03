using System;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class LaplacianMain : MonoBehaviour
{
    public Transform root;
    public GameObject handlerPrefab;
    public GameObject unconstrainPrefab;

    private readonly int mRows = 5;
    private readonly int mCols = 10;
    private readonly (int, int)[] mHandlersRowCol = new (int, int)[]{
        (0,0),(2,0),(4,0),
        (0,3),(2,3),(4,3),
        (0,6),(2,6),(4,6),
        (0,9),(2,9),(4,9),
    };

    List<List<Transform>> mVertexTransforms;

    Matrix<float> mLMatrix;
    Matrix<float> mDMatrix;
    Matrix<float> mAMatrix;
    Matrix<float> mBMatrix;

    public const float stepR = 2.5f;
    public const float stepC = 2.5f;

    private float Height
    {
        get => stepR * mRows - 1;
    }

    private float Width
    {
        get => stepC * mCols - 1;
    }

    private Vector3 GetOriginalPos(int r, int c)
    {
        var startX = root.position.x - Width / 2;
        var startY = root.position.y - Height / 2;
        var x = startX + c * stepC;
        var y = startY + r * stepR;
        return new Vector3(x, y, 0);
    }

    private Vector3 GetCurrentPos(int r, int c)
    {
        return mVertexTransforms[r][c].position;
    }

    private int GetVertexDegree(int r, int c)
    {
        if (r == 0 && c == 0) { return 2; }
        if (r == mRows - 1 && c == mCols - 1) { return 2; }
        if (r == 0 && c == mCols - 1) { return 3; }
        if (r == mRows - 1 && c == 0) { return 3; }
        if (r == 0 || r == mRows - 1 || c == 0 || c == mCols - 1) { return 4; }
        return 6;
    }

    private bool IsEdge((int, int) p1, (int, int) p2)
    {
        if (p1 == p2)
        {
            return false;
        }

        if (Mathf.Abs(p1.Item1 - p2.Item1) > 1)
        {
            return false;
        }

        if (Mathf.Abs(p1.Item2 - p2.Item2) > 1)
        {
            return false;
        }

        if (p1.Item1 + 1 == p2.Item1 && p1.Item2 + 1 == p2.Item2)
        {
            return false;
        }

        if (p2.Item1 + 1 == p1.Item1 && p2.Item2 + 1 == p1.Item2)
        {
            return false;
        }

        return true;
    }

    void Start()
    {
        mVertexTransforms = GenerateVertices();
        LaplacianSetup();
    }

    private void LaplacianSetup()
    {
        var vertexCount = mRows * mCols;
        var handlerCount = mHandlersRowCol.Length;
        var unconstraintCount = mRows * mCols - handlerCount;
        var M = Matrix<float>.Build;
        var identity = M.DenseIdentity(unconstraintCount);

    }

    private void LaplacianDeform()
    {

    }

    private List<List<Transform>> GenerateVertices()
    {
        var vertexTransforms = new List<List<Transform>>();

        var startX = root.position.x - Width / 2;
        var startY = root.position.y - Height / 2;
        for (int r = 0; r < mRows; r++)
        {
            var currentRow = new List<Transform>();
            vertexTransforms.Add(currentRow);
            for (int c = 0; c < mCols; c++)
            {
                var pos = GetOriginalPos(r, c);
                var rot = Quaternion.identity;
                bool isHandler = Array.Exists(mHandlersRowCol, rc => rc == (r, c));
                GameObject obj;
                if (isHandler)
                {
                    obj = Instantiate(handlerPrefab, pos, rot, root);
                    obj.AddComponent<ObjCtrl>();
                }
                else
                {
                    obj = Instantiate(unconstrainPrefab, pos, rot, root);
                }
                currentRow.Add(obj.transform);
            }
        }

        return vertexTransforms;
    }

    private void DrawMeshEdges()
    {
        for (int r1 = 0; r1 < mRows; r1++)
        {
            for (int c1 = 0; c1 < mCols; c1++)
            {
                for (int r2 = r1; r2 < mRows; r2++)
                {
                    for (int c2 = c1; c2 < mCols; c2++)
                    {
                        if (!IsEdge((r1, c1), (r2, c2)))
                        {
                            continue;
                        }

                        var p1 = GetCurrentPos(r1, c1);
                        var p2 = GetCurrentPos(r2, c2);
                        Debug.DrawLine(p1, p2);
                    }
                }
            }
        }
    }

    private void FixedUpdate()
    {
        DrawMeshEdges();
    }

    void Update()
    {
    }

}
