using UnityEngine;
using System.Collections;
public class Tooltip : MonoBehaviour
{
    [SerializeField] GameObject TooltipObject;
    public void OnMouseDown()
    {
        StartCoroutine(ShowToolTip());

    }

    IEnumerator ShowToolTip()
    {
        TooltipObject.SetActive(true);
        yield return new WaitForSeconds(8f);
        TooltipObject.SetActive(false);
    }
}
