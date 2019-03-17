using UnityEngine;

// Token: 0x02000048 RID: 72
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class MeshBloomPrePassLight : TubeBloomPrePassLight
{
    Material material;

    public void Init(Renderer renderer)
    {
        material = renderer.material;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _parametricBoxController.enabled = false;
    }
    
    public override Color color
    {
        get
        {
            return this._color;
        }
        set
        {
            base.color = value;
            material.color = value;
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        material.color = color;
    }
}