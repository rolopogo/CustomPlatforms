using System;
using UnityEngine;

// Token: 0x0200035C RID: 860
public class SpectrogramColumns : MonoBehaviour
{
    protected void Start()
    {
        this.CreateColums();
    }
    
    protected void Update()
    {
        float[] processedSamples;
        if (_spectrogramData == null) {
            processedSamples = new float[64];
            for (int i = 0; i < processedSamples.Length; i++)
            {
                processedSamples[i] = (Mathf.Sin((float)i / 64 * 9 * Mathf.PI + 1.4f* Mathf.PI) + 1.2f)/25;
            }
        }
        else
        {
            processedSamples = this._spectrogramData.ProcessedSamples;
        }
        
        for (int i = 0; i < processedSamples.Length; i++)
        {
            float num = processedSamples[i] * (5f + (float)i * 0.07f);
            if (num > 1f)
            {
                num = 1f;
            }
            num = Mathf.Pow(num, 2f);
            this._columnTransforms[i].localScale = new Vector3(this._columnWidth, Mathf.Lerp(this._minHeight, this._maxHeight, num) + (float)i * 0.1f, this._columnDepth);
            this._columnTransforms[i + 64].localScale = new Vector3(this._columnWidth, Mathf.Lerp(this._minHeight, this._maxHeight, num), this._columnDepth);
        }
        
        if (Input.GetKeyDown(KeyCode.Space)){

            for (int i = 0; i < processedSamples.Length; i++)
            {
                Console.Write(processedSamples[i] + " ");
            }
            Console.WriteLine(" ");
        }
    }
    
    private void CreateColums()
    {
        this._columnTransforms = new Transform[128];
        for (int i = 0; i < 64; i++)
        {
            this._columnTransforms[i] = this.CreateColumn(this._separator * (float)i);
            this._columnTransforms[i + 64] = this.CreateColumn(-this._separator * (float)(i + 1));
        }
    }
    
    private Transform CreateColumn(Vector3 pos)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this._columnPrefab, base.transform);
        gameObject.transform.localPosition = pos;
        gameObject.transform.localScale = new Vector3(this._columnWidth, this._minHeight, this._columnDepth);
        return gameObject.transform;
    }
    
    [SerializeField]
    private GameObject _columnPrefab;
    
    [SerializeField]
    private Vector3 _separator = new Vector3(0f, 0f, 1f);
    
    [SerializeField]
    private float _minHeight = 1f;

    [SerializeField]
    private float _maxHeight = 10f;
    
    [SerializeField]
    private float _columnWidth = 1f;
    
    [SerializeField]
    private float _columnDepth = 1f;
    
    private BasicSpectrogramData _spectrogramData;
    
    private Transform[] _columnTransforms;
}
