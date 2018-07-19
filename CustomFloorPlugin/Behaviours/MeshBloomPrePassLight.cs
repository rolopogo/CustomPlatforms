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

    // Token: 0x060001BB RID: 443 RVA: 0x0000B4BC File Offset: 0x000096BC
    public override void FillMeshData(int lightNum, Vector3[] vertices, Color32[] colors32, Vector2[] uv2, Vector2[] uv3, Matrix4x4 viewMatrix, Matrix4x4 projectionMatrix, float lineWidth)
    {
        float y = -this._length * this._center;
        float y2 = this._length * (1f - this._center);
        Matrix4x4 localToWorldMatrix = this._transform.localToWorldMatrix;
        Vector3 point = localToWorldMatrix.MultiplyPoint3x4(new Vector4(0f, y, 0f));
        Vector3 point2 = localToWorldMatrix.MultiplyPoint3x4(new Vector4(0f, y2, 0f));
        Vector3 vector = viewMatrix.MultiplyPoint3x4(point);
        Vector3 vector2 = viewMatrix.MultiplyPoint3x4(point2);
        Vector4 vector3 = projectionMatrix * new Vector4(vector.x, vector.y, vector.z, 1f);
        Vector4 vector4 = projectionMatrix * new Vector4(vector2.x, vector2.y, vector2.z, 1f);
        bool flag = vector3.x >= -vector3.w;
        bool flag2 = vector4.x >= -vector4.w;
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
            float d = (-vector3.w - vector3.x) / (vector4.x - vector3.x + vector4.w - vector3.w);
            Vector4 vector5 = vector3 + (vector4 - vector3) * d;
            if (flag)
            {
                vector4 = vector5;
            }
            else
            {
                vector3 = vector5;
            }
        }
        flag = (vector3.x <= vector3.w);
        flag2 = (vector4.x <= vector4.w);
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
            float d2 = (vector3.w - vector3.x) / (vector4.x - vector3.x - vector4.w + vector3.w);
            Vector4 vector6 = vector3 + (vector4 - vector3) * d2;
            if (flag)
            {
                vector4 = vector6;
            }
            else
            {
                vector3 = vector6;
            }
        }
        flag = (vector3.y >= -vector3.w);
        flag2 = (vector4.y >= -vector4.w);
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
            float d3 = (-vector3.w - vector3.y) / (vector4.y - vector3.y + vector4.w - vector3.w);
            Vector4 vector7 = vector3 + (vector4 - vector3) * d3;
            if (flag)
            {
                vector4 = vector7;
            }
            else
            {
                vector3 = vector7;
            }
        }
        flag = (vector3.y <= vector3.w);
        flag2 = (vector4.y <= vector4.w);
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
            float d4 = (vector3.w - vector3.y) / (vector4.y - vector3.y - vector4.w + vector3.w);
            Vector4 vector8 = vector3 + (vector4 - vector3) * d4;
            if (flag)
            {
                vector4 = vector8;
            }
            else
            {
                vector3 = vector8;
            }
        }
        flag = (vector3.z <= vector3.w);
        flag2 = (vector4.z <= vector4.w);
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
            float d5 = (vector3.w - vector3.z) / (vector4.z - vector3.z - vector4.w + vector3.w);
            Vector4 vector9 = vector3 + (vector4 - vector3) * d5;
            if (flag)
            {
                vector4 = vector9;
            }
            else
            {
                vector3 = vector9;
            }
        }
        float num = 0.0001f;
        flag = (vector3.z >= -vector3.w - num);
        flag2 = (vector4.z >= -vector4.w - num);
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
            float d6 = (-vector3.w - vector3.z) / (vector4.z - vector3.z + vector4.w - vector3.w);
            Vector4 vector10 = vector3 + (vector4 - vector3) * d6;
            if (flag)
            {
                vector4 = vector10;
            }
            else
            {
                vector3 = vector10;
            }
        }
        Matrix4x4 inverse = projectionMatrix.inverse;
        vector = inverse * vector3;
        vector2 = inverse * vector4;
        Vector3 vector11 = vector3 / vector3.w;
        Vector3 a = vector4 / vector4.w;
        vector11.x = vector11.x * 0.5f + 0.5f;
        vector11.y = vector11.y * 0.5f + 0.5f;
        vector11.z = 0f;
        a.x = a.x * 0.5f + 0.5f;
        a.y = a.y * 0.5f + 0.5f;
        a.z = 0f;
        Vector3 vector12 = a - vector11;
        Vector3 vector13 = new Vector3(-vector12.y, vector12.x, 0f);
        vector13.Normalize();
        vector13 *= lineWidth;
        Vector3 v = new Vector3(vector.x / vector3.w, vector.y / vector3.w, vector.z / vector3.w);
        Vector3 v2 = new Vector3(1f / vector3.w, 0f, 0f);
        Vector3 v3 = new Vector3(vector2.x / vector4.w, vector2.y / vector4.w, vector2.z / vector4.w);
        Vector3 v4 = new Vector3(1f / vector4.w, 0f, 0f);
        int num2 = lightNum * 4;
        vertices[num2] = vector11 - vector13;
        vertices[num2 + 1] = vector11 + vector13;
        vertices[num2 + 2] = a + vector13;
        vertices[num2 + 3] = a - vector13;
        Color32 color = this._color;
        colors32[num2] = color;
        colors32[num2 + 1] = color;
        colors32[num2 + 2] = color;
        colors32[num2 + 3] = color;
        uv2[num2] = v;
        uv2[num2 + 1] = v;
        uv2[num2 + 2] = v3;
        uv2[num2 + 3] = v3;
        uv3[num2] = v2;
        uv3[num2 + 1] = v2;
        uv3[num2 + 2] = v4;
        uv3[num2 + 3] = v4;
    }

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
