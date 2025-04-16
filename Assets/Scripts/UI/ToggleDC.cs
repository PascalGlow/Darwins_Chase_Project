using UnityEngine;

public class ToggleDC : MonoBehaviour
{
    [SerializeField] GameObject Canvas;
    public bool hidden = true;
    public void toggleDC()
    {
        Animator anim = Canvas.GetComponent<Animator>();
        bool active = anim.GetBool("open");
        anim.SetBool("open", !active);
        hidden = !hidden;
    }
}
