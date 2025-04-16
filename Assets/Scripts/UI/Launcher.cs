using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject loading;
    string Version = "1";
    
    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
#if UNITY_EDITOR
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif
    }

    void Start()
    {
        loading.SetActive(true);
        PhotonNetwork.GameVersion = Version;
        if (PhotonNetwork.IsConnected) PhotonNetwork.JoinLobby();
        else if (!PhotonNetwork.ConnectUsingSettings()) Debug.Log("Could not join master server, try again");
    }

    public void Connect()
    {
        loading.SetActive(true);
        if (!PhotonNetwork.ConnectUsingSettings()) Debug.Log("Could not join master server, try again");
        PhotonNetwork.GameVersion = Version;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.GameVersion = Version;
        if (!PhotonNetwork.JoinLobby()) Debug.Log("Could not join a lobby, try again");
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.GameVersion = Version;
        base.OnJoinedLobby();
        SceneManager.LoadScene("Lobby");
    }
}
