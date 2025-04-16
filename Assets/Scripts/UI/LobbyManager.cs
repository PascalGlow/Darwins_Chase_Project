
/*using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject lobbyCreateTextField;
    [SerializeField] GameObject lobbyJoinTextField;
    [SerializeField] TMP_Dropdown lobbyDropDown;
    [SerializeField] TMP_Text nickname;
    [SerializeField] TMP_Text nameDisplay;

    // Deine Serveradresse und Port
    public string serverAddress = "ec2-3-72-105-206.eu-central-1.compute.amazonaws.com"; // Ersetze mit deiner Server-IP oder DNS
    public int serverPort = 5055; // Ersetze mit deinem Server-Port

    public void ConnectToCustomServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectToMaster(serverAddress, serverPort, ""); // AppID ist leer für eigenen Server
            Debug.Log("Verbinde mit eigenem Server...");
        }
        else
        {
            Debug.Log("Bereits mit dem Server verbunden.");
        }
    }

    public void CreateLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string id = lobbyCreateTextField.GetComponent<TMP_InputField>().text;
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = true;
            roomOptions.PublishUserId = true;
            if (PhotonNetwork.InRoom || !PhotonNetwork.JoinOrCreateRoom(id, roomOptions, TypedLobby.Default))
            {
                Debug.Log("Erstellen des Raumes fehlgeschlagen, möglicherweise bereits in einem Raum oder Verbindungsfehler.");
            }
        }
        else
        {
            Debug.Log("Nicht verbunden oder nicht bereit, versuche jetzt zu verbinden!");
            ConnectToCustomServer();
            // Der eigentliche Raumerstellungsversuch erfolgt nicht direkt hier,
            // sondern idealerweise nach dem OnJoinedLobby Callback, wenn man sicher in der Lobby ist.
        }
    }

    public void JoinLobby()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            string id = lobbyJoinTextField.GetComponent<TMP_InputField>().text;
            if (PhotonNetwork.InRoom || !PhotonNetwork.JoinRoom(id))
            {
                Debug.Log("Konnte Raum nicht finden oder bereits im Raum.");
            }
        }
        else
        {
            Debug.Log("Nicht verbunden oder nicht bereit, versuche jetzt zu verbinden!");
            ConnectToCustomServer();
            // Der eigentliche Raumbeitrittsversuch erfolgt im OnJoinedLobby Callback.
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Erfolgreich Raum beigetreten!");
        PhotonNetwork.LoadLevel("Room");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        List<string> roomNames = roomList.Select(room => room.Name).ToList();
        lobbyDropDown.ClearOptions();
        lobbyDropDown.AddOptions(roomNames);
    }

    public void UpdateNickName()
    {
        PhotonNetwork.LocalPlayer.NickName = nickname.text;
        nameDisplay.text = "Nickname: " + nickname.text;
    }

    public void BackToMainMenu()
    {
        PhotonNetwork.LoadLevel("Tutorial");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Verbunden mit eigenem Master Server!");
        PhotonNetwork.JoinLobby(); // Automatisch der Standard-Lobby beitreten, sobald verbunden
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Erfolgreich der Lobby beigetreten!");
        // Hier könntest du optional versuchen, direkt einem Raum beizutreten,
        // falls der Spieler bereits einen Namen im Join-Feld eingegeben hat.
        string joinId = lobbyJoinTextField.GetComponent<TMP_InputField>().text;
        if (!string.IsNullOrEmpty(joinId) && !PhotonNetwork.InRoom)
        {
            PhotonNetwork.JoinRoom(joinId);
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogError($"Verbindung zum Server getrennt: {cause}");
    }
}*/
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.Collections.Generic;
using Photon.Realtime;
using System.Linq;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject lobbyCreateTextField;
    [SerializeField] GameObject lobbyJoinTextField;
    [SerializeField] TMP_Dropdown lobbyDropDown;
    [SerializeField] TMP_Text nickname;
    [SerializeField] TMP_Text nameDisplay;

   // Deine Serveradresse und Port
    public string serverAddress = "ec2-3-72-105-206.eu-central-1.compute.amazonaws.com"; // 
    public int serverPort = 5055; // Server-Port

    public void ConnectToCustomServer()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = "1";
            PhotonNetwork.ConnectToMaster(serverAddress, serverPort, "");//AppID ist leer da eigener Server.
            Debug.Log("Verbinde mit eigenem Server...");
        }
    }

	public void CreateLobby()
	{
		if (PhotonNetwork.IsConnected)
		{
			string id = lobbyCreateTextField.GetComponent<TMP_InputField>().text;
			RoomOptions roomOptions = new RoomOptions();
			roomOptions.IsVisible = true;
			roomOptions.PublishUserId = true;
			if (PhotonNetwork.InRoom || !PhotonNetwork.JoinOrCreateRoom(id, roomOptions, TypedLobby.Default)) ;
			{
				Debug.Log("Erstellen des Raumes fehlgeschlagen, m�glicherweise bereits in einem Raum");
			}
		}
		else
		{
			Debug.Log("Not connected, trying again now!");
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = "1";
			PhotonNetwork.JoinLobby();
		}
	}

	public void JoinLobby()
	{
		if (PhotonNetwork.IsConnected)
		{
			string id = lobbyJoinTextField.GetComponent<TMP_InputField>().text;
			if (PhotonNetwork.InRoom || !PhotonNetwork.JoinRoom(id))
			{
				Debug.Log("Konnte Raum nicht finden oder bereits im Raum");
			}
		}
		else
		{
			Debug.Log("Not connected, trying again now!");
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = "1";
			PhotonNetwork.JoinLobby();
		}
	}

	public override void OnJoinedRoom()
	{
		PhotonNetwork.LoadLevel("Room");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		base.OnRoomListUpdate(roomList);
		List<string> roomNames = roomList.Select(room => room.Name).ToList();
		lobbyDropDown.ClearOptions();
		lobbyDropDown.AddOptions(roomNames);
	}

	public void UpdateNickName()
	{
		PhotonNetwork.LocalPlayer.NickName = nickname.text;
		nameDisplay.text = "Nickname: " + nickname.text;
	}

	public void BackToMainMenu()
	{
		PhotonNetwork.LoadLevel("Tutorial");
	}
}
