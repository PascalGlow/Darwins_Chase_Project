using System.Collections;
using TMPro;
using Photon.Pun;
using UnityEngine;
using System;

public class SaveToPicture : MonoBehaviour
{
    public void OnClickedExport()
    {
        Cursor.visible = false;
        StartCoroutine(SaveDC());
    }

    IEnumerator SaveBM()
    {
        Canvas gmCanvas = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().GenerateGMView(1f);
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;

        // Fill GM text fields with correct player bm
        GameObject gmContent = GameObject.Find("BusinessModelContent");
        for (int i = 0; i < gmContent.transform.childCount; i++)
        {
            TMP_Text textField = gmContent.transform.GetChild(i).GetChild(0).GetComponentInChildren<TMP_Text>();
            if (props.ContainsKey("GM" + i)) textField.text = props["GM" + i].ToString();
        }

        String path = "";
        try {
            path = Application.dataPath + "/Resources/output_gm.png";
            ScreenCapture.CaptureScreenshot(path);
        }
        catch (Exception e)
        {
            path = Application.dataPath + "output_gm.png";
            ScreenCapture.CaptureScreenshot(path);
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("Bild gespeichert in " + path);
        Destroy(gmCanvas);
        transform.GetChild(0).gameObject.SetActive(true);
        Cursor.visible = true;
    }

    IEnumerator SaveDC()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("DCPfeil").GetComponent<ToggleDC>().toggleDC();
        yield return new WaitForSeconds(.5f);
        String path = "";
        try {
            path = Application.dataPath + "/Resources/output_dc.png";
            ScreenCapture.CaptureScreenshot(path);
        } catch(Exception e) {
            path = Application.dataPath + "output_dc.png";
            ScreenCapture.CaptureScreenshot(path);
        }
        yield return new WaitForSeconds(1f);
        Debug.Log("Bild gespeichert in " + path);
        GameObject.FindGameObjectWithTag("DCPfeil").GetComponent<ToggleDC>().toggleDC();
        StartCoroutine(SaveBM());
    }
}
