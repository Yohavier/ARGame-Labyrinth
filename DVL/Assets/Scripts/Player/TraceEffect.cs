using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceEffect : MonoBehaviour
{
    void Start()
    {
        transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        StartCoroutine(Expand());
    }

    private IEnumerator Expand()
    {
        while (transform.localScale.x < 0.1f)
        {
            transform.localScale += new Vector3(0.005f, 0, 0.005f);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}