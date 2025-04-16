using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;
    const float spawnOffset = 1.05f;

    void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            Vector2 offset = new Vector2(Random.Range(1f, spawnOffset), Random.Range(1f, spawnOffset));
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(-1603, 108) * offset, Quaternion.identity); // Spawn player at starting field
        } else {
            Debug.Log("Spawne Spieler im Einzelspielermodus");
            Instantiate(playerPrefab, new Vector2(-8, 0.6f), Quaternion.identity); // Not in room, spawn online
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
