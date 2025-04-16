using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MessageUI_Setting : MonoBehaviour
{
    public GameObject TextBackground, ContentHandler,ChatBox;
    public Scrollbar scrollbar;
   
    void Start()
    {
        FindObjectOfType<DF2Client>().UserTextEvent += SendUserText;
        FindObjectOfType<DF2Client>().AITextEvent += SendAIText;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SendUserText(string text)
    {
       
       GameObject newGameObject= Instantiate(TextBackground, ContentHandler.transform);
        TextMeshProUGUI[] textList = newGameObject.transform.GetComponentsInChildren<TextMeshProUGUI>();
       
        textList[0].text = PhotonNetwork.LocalPlayer.NickName;
        textList[1].text = text;
	}
    public void SendAIText(string text)
    {
        GameObject newGameObject = Instantiate(TextBackground, ContentHandler.transform);
        TextMeshProUGUI[] textList = newGameObject.transform.GetComponentsInChildren<TextMeshProUGUI>();
        textList[0].text = "Charles";
        textList[1].text = text;
		StartCoroutine(WaitForAIText());
	}

    IEnumerator WaitForAIText()
    {
        yield return new WaitForSeconds(0.5F);
        scrollbar.value = -0.1F;
    }

	public void Handle_Chatbox()
    {
        
        if (ChatBox.activeSelf)
        {
            ChatBox.SetActive(false);
            Time.timeScale=1;
        }
        else
        {
            ChatBox.SetActive(true);
        }
            
    }
}
