using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class GMInputManager : MonoBehaviourPun
{
    [SerializeField] GameObject WaitingText;
    [SerializeField] TMP_Text nameField;
    Dictionary<string, bool> playerSubmittedGM = new Dictionary<string, bool>();
    
    void Start()
    {
        // Initialize player submitted dictionary
        photonView.ViewID = 9999999;
        new List<Photon.Realtime.Player>(PhotonNetwork.PlayerList).ForEach(player => playerSubmittedGM.Add(player.NickName, false));
    }

    public void GMReadyClicked()
    {
        WaitingText.SetActive(true); // Show waiting text

        Transform gmContent = transform.Find("BusinessModelContent");
        ExitGames.Client.Photon.Hashtable props = PhotonNetwork.LocalPlayer.CustomProperties;
        props.Add("companyName", gmContent.Find("Name").GetChild(0).GetComponentInChildren<TMP_InputField>().text);

        for(int i = 1; i < gmContent.childCount; i++)
        {
            string key = "GM" + (i-1);
            string value = gmContent.GetChild(i).GetChild(0).GetComponentInChildren<TMP_InputField>().text;
            props.Add(key, value);
        }

        // Push to others and Set Ready
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        photonView.RPC("SetReady", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void SetReady(string playerName)
    {
        playerSubmittedGM[playerName] = true;
        if (!playerSubmittedGM.ContainsValue(false)) StartGame();
	}

    void StartGame()
    {
        UIManager.DrawText(PhotonNetwork.LocalPlayer.NickName + ", CEO von " + PhotonNetwork.LocalPlayer.CustomProperties["companyName"], nameField);
        Vector3 c = (Vector3)PhotonNetwork.LocalPlayer.CustomProperties["color"];
        nameField.color = new Color(c.x, c.y, c.z);
        Destroy(gameObject);
        FindObjectOfType<DF2Client>().Introduction();
    }
}
