using UnityEngine;
using Photon.Pun;
using TMPro;
public class NoteSpawning : MonoBehaviourPun
{
    [SerializeField] GameObject Note;
    [SerializeField] ToggleDC ToggleDC;
    GameManager GameManager;
    public GameObject lastNote;
    PlayerDCNotes PlayerDCNotes;
    public bool editingDC = false;
    float lastClickTime;
    void Start()
    {
        editingDC = false;
        lastClickTime = Time.time;
        ToggleDC = ToggleDC.GetComponent<ToggleDC>();
        PlayerDCNotes = GetComponent<PlayerDCNotes>();
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && editingDC && !ToggleDC.hidden) // Primary mouse button clicked
        {
            if (Time.time < lastClickTime + .2f) // Double click in small amount of time?
            {
                GameObject newNote = Instantiate(Note, transform, true);
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = Camera.main.nearClipPlane; // Custom z to prevent - value
                newNote.transform.position = Camera.main.ScreenToWorldPoint(mousePos); // Set position to world coordinate of mouse
                newNote.GetComponentInChildren<TMP_InputField>().Select(); // Focus on field
                lastNote = newNote;
            } else if (lastNote != null) { // Single-Clicked somewhere after creating Note -> Save
                string text = lastNote.GetComponentInChildren<TMP_InputField>().text;
                if (text != "")
                {
                    int type = GameManager.lastRocket;
                    PlayerDCNotes.AddNote(text, type);
                    lastNote = null;
                }
                lastClickTime = Time.time;
            } else {
                lastClickTime = Time.time;
            }
        }
    }

    public void StopEditing()
    {
        if (lastNote != null) PlayerDCNotes.AddNote(lastNote.GetComponentInChildren<TMP_InputField>().text, GameManager.lastRocket);
        lastNote = null;
        editingDC = false;
    }
}
