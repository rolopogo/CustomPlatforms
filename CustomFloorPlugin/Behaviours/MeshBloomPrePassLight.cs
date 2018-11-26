using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class MeshBloomPrePassLight : BloomPrePassLight
{
    // Token: 0x17000049 RID: 73
    // (get) Token: 0x060001B5 RID: 437 RVA: 0x0000B1F0 File Offset: 0x000093F0
    // (set) Token: 0x060001B4 RID: 436 RVA: 0x0000B1E0 File Offset: 0x000093E0
    public override Color color
    {
        get
        {
            return this._color;
        }
        set
        {
            this._color = value;
            this.UpdateMaterialPropertyBlock();
        }
    }

    // Token: 0x060001B6 RID: 438 RVA: 0x0000B1F8 File Offset: 0x000093F8
    public virtual void Awake()
    {
        this.Init();
    }

    // Token: 0x060001B7 RID: 439 RVA: 0x0000B200 File Offset: 0x00009400
    protected override void OnEnable()
    {
        base.OnEnable();
        
        this.UpdateMaterialPropertyBlock();
    }

    // Token: 0x060001B8 RID: 440 RVA: 0x0000B264 File Offset: 0x00009464
    public virtual void Init()
    {
        if (this._isInitialized)
        {
            return;
        }
        this._isInitialized = true;
        this._meshFilter = base.GetComponent<MeshFilter>();
        this._meshRenderer = base.GetComponent<MeshRenderer>();
        this._colorID = Shader.PropertyToID("_Color");
        this._sizeParamsID = Shader.PropertyToID("_SizeParams");
        this._transform = base.transform;
    }

    // Token: 0x060001B9 RID: 441 RVA: 0x0000B2C8 File Offset: 0x000094C8
    public virtual void UpdateMaterialPropertyBlock()
    {
        if (this._materialPropertyBlock == null)
        {
            this._materialPropertyBlock = new MaterialPropertyBlock();
        }
        this._materialPropertyBlock.SetColor(this._colorID, this._color);
        this._materialPropertyBlock.SetVector(this._sizeParamsID, new Vector3(1, 1, 0));
        this._meshRenderer.SetPropertyBlock(this._materialPropertyBlock);
    }

    // Token: 0x060001BA RID: 442 RVA: 0x0000B340 File Offset: 0x00009540
    public virtual Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Tube";
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1f, 0f, -1f),
            new Vector3(1f, 0f, -1f),
            new Vector3(1f, 1f, -1f),
            new Vector3(-1f, 1f, -1f),
            new Vector3(-1f, 1f, 1f),
            new Vector3(1f, 1f, 1f),
            new Vector3(1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f)
        };
        int[] triangles = new int[]
        {
            0,
            2,
            1,
            0,
            3,
            2,
            2,
            3,
            4,
            2,
            4,
            5,
            1,
            2,
            5,
            1,
            5,
            6,
            0,
            7,
            4,
            0,
            4,
            3,
            5,
            4,
            7,
            5,
            7,
            6,
            0,
            6,
            7,
            0,
            1,
            6
        };
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(new Vector3(0f, 0f, 0f), new Vector3(10f, 2000f, 10f));
        return mesh;
    }

    public override void FillMeshData(int lightNum, Vector3[] vertices, Color[] colors, Vector4[] viewPos, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, float lineWidth)
    {
        float y = -this._length * this._center;
        float y2 = this._length * (1f - this._center);
        Matrix4x4 localToWorldMatrix = this._transform.localToWorldMatrix;
        Vector3 point = localToWorldMatrix.MultiplyPoint3x4(new Vector4(0f, y, 0f));
        Vector3 point2 = localToWorldMatrix.MultiplyPoint3x4(new Vector4(0f, y2, 0f));
        Vector3 v = viewMatrix.MultiplyPoint3x4(point);
        Vector3 v2 = viewMatrix.MultiplyPoint3x4(point2);
        Vector4 a = projectionMatrix * new Vector4(v.x, v.y, v.z, 1f);
        Vector4 a2 = projectionMatrix * new Vector4(v2.x, v2.y, v2.z, 1f);
        bool flag = a.x >= -a.w;
        bool flag2 = a2.x >= -a2.w;
        if (!flag && !flag2)
        {
            for (int i = 0; i < 4; i++)
            {
                vertices[lightNum * 4 + i] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t = (-a.w - a.x) / (a2.x - a.x + a2.w - a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t);
        }
        flag = (a.x <= a.w);
        flag2 = (a2.x <= a2.w);
        if (!flag && !flag2)
        {
            for (int j = 0; j < 4; j++)
            {
                vertices[lightNum * 4 + j] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t2 = (a.w - a.x) / (a2.x - a.x - a2.w + a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t2);
        }
        flag = (a.y >= -a.w);
        flag2 = (a2.y >= -a2.w);
        if (!flag && !flag2)
        {
            for (int k = 0; k < 4; k++)
            {
                vertices[lightNum * 4 + k] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t3 = (-a.w - a.y) / (a2.y - a.y + a2.w - a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t3);
        }
        flag = (a.y <= a.w);
        flag2 = (a2.y <= a2.w);
        if (!flag && !flag2)
        {
            for (int l = 0; l < 4; l++)
            {
                vertices[lightNum * 4 + l] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t4 = (a.w - a.y) / (a2.y - a.y - a2.w + a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t4);
        }
        flag = (a.z <= a.w);
        flag2 = (a2.z <= a2.w);
        if (!flag && !flag2)
        {
            for (int m = 0; m < 4; m++)
            {
                vertices[lightNum * 4 + m] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t5 = (a.w - a.z) / (a2.z - a.z - a2.w + a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t5);
        }
        float num = 0.0001f;
        flag = (a.z >= -a.w - num);
        flag2 = (a2.z >= -a2.w - num);
        if (!flag && !flag2)
        {
            for (int n = 0; n < 4; n++)
            {
                vertices[lightNum * 4 + n] = Vector3.zero;
            }
            return;
        }
        if (flag != flag2)
        {
            float t6 = (-a.w - a.z) / (a2.z - a.z + a2.w - a.w);
            this.ClipPoints(ref a, ref a2, ref v, ref v2, flag, t6);
        }
        Vector3 vector = a / a.w;
        Vector3 a3 = a2 / a2.w;
        vector.x = vector.x * 0.5f + 0.5f;
        vector.y = vector.y * 0.5f + 0.5f;
        vector.z = 0f;
        a3.x = a3.x * 0.5f + 0.5f;
        a3.y = a3.y * 0.5f + 0.5f;
        a3.z = 0f;
        Vector3 vector2 = a3 - vector;
        Vector3 vector3 = new Vector3(-vector2.y, vector2.x, 0f);
        vector3.Normalize();
        vector3 *= lineWidth;
        int num2 = lightNum * 4;
        vertices[num2] = vector - vector3;
        vertices[num2 + 1] = vector + vector3;
        vertices[num2 + 2] = a3 + vector3;
        vertices[num2 + 3] = a3 - vector3;
        Color color = this._color * this._intensityMultiplier;
        colors[num2] = color;
        colors[num2 + 1] = color;
        colors[num2 + 2] = color;
        colors[num2 + 3] = color;
        viewPos[num2] = v;
        viewPos[num2 + 1] = v;
        viewPos[num2 + 2] = v2;
        viewPos[num2 + 3] = v2;
    }

    public virtual void ClipPoints(ref Vector4 fromPointClipPos, ref Vector4 toPointClipPos, ref Vector3 fromPointViewPos, ref Vector3 toPointViewPos, bool fromPointInside, float t)
    {
        Vector4 vector = fromPointClipPos + (toPointClipPos - fromPointClipPos) * t;
        Vector4 v = fromPointViewPos + (toPointViewPos - fromPointViewPos) * t;
        if (fromPointInside)
        {
            toPointClipPos = vector;
            toPointViewPos = v;
        }
        else
        {
            fromPointClipPos = vector;
            fromPointViewPos = v;
        }
    }   

    protected float _intensityMultiplier = 1f;

    // Token: 0x040001B6 RID: 438
    [SerializeField]
    protected float _width = 0.5f;

    // Token: 0x040001B7 RID: 439
    [SerializeField]
    protected float _length = 1f;

    // Token: 0x040001B8 RID: 440
    [SerializeField]
    [Range(0f, 1f)]
    protected float _center = 0.5f;

    // Token: 0x040001B9 RID: 441
    [SerializeField]
    protected Color _color;

    // Token: 0x040001BA RID: 442
    protected int _colorID;

    // Token: 0x040001BB RID: 443
    protected int _sizeParamsID;

    // Token: 0x040001BC RID: 444
    protected MeshRenderer _meshRenderer;

    // Token: 0x040001BD RID: 445
    protected MeshFilter _meshFilter;

    // Token: 0x040001BE RID: 446
    protected MaterialPropertyBlock _materialPropertyBlock;

    // Token: 0x040001BF RID: 447
    protected bool _isInitialized;

    // Token: 0x040001C0 RID: 448
    protected Transform _transform;
    
    // Token: 0x040001C2 RID: 450
    protected const float kMaxWidth = 10f;

    // Token: 0x040001C3 RID: 451
    protected const float kMaxLength = 1000f;
}
