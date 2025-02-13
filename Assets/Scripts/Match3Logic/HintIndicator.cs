using System.Collections;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(SpriteRenderer))]
public class HintIndicator : Singleton<HintIndicator>
{
    private SpriteRenderer spriteRenderer;

    private Transform hintLocation;
    private Coroutine autoHintCR;

    [SerializeField]
    private float delayBeforeAutoHint;
    protected override void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void IndicateHint(Transform hintLocation)
    {
        CancelHint();
        transform.position = hintLocation.position;
        spriteRenderer.enabled = true;
    }

    public void CancelHint()
    {
        spriteRenderer.enabled = false;

        if (autoHintCR != null)
            StopCoroutine(autoHintCR);

        autoHintCR = null;
    }

    public void StartAutoHint(Transform hintLocation)
    {
        CancelHint();
        this.hintLocation = hintLocation;

        autoHintCR = StartCoroutine(WaitAndIndicateHint());
    }

    private IEnumerator WaitAndIndicateHint()
    {
        yield return new WaitForSeconds(delayBeforeAutoHint);
        IndicateHint(hintLocation);
    }
}