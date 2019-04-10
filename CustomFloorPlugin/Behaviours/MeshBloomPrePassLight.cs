using UnityEngine;

// Token: 0x02000048 RID: 72
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
public class MeshBloomPrePassLight : TubeBloomPrePassLight
{
    public Renderer renderer;

    public void Init(Renderer renderer)
    {
        this.renderer = renderer;
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
            if(renderer.material!=null) renderer.material.color = value;
        }
    }

    public override void Refresh()
    {
        base.Refresh();
        renderer.material.color = color;
        _parametricBoxController.enabled = false;
    }
}