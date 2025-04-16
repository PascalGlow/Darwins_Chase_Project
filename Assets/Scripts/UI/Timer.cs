using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TMP_Text CountDownText;
    [SerializeField] Image CircularCountdown;
    public float duration;
    float remaining;
    bool canStart;

	/* public void Begin(float seconds)
	 {
		 remaining = seconds;
		 canStart = true;

	 }
	 private void Update()
	 {

		 if (!FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf)
		 {
			if(!canStart)
			 canStart = true;

			 UpdateTimer();

		 }
		 if(FindObjectOfType<MessageUI_Setting>().ChatBox.activeSelf&&canStart==true)
		 {
			 canStart = false;
			Time.timeScale = 0;

		 }
	 }

	   private void UpdateTimer()
	 {
		 if (remaining > 0&&canStart)
		 {

			 CircularCountdown.fillAmount = Mathf.InverseLerp(0, duration, remaining);
			 CountDownText.text = ((int)remaining).ToString();
			 remaining-=Time.deltaTime;

		 }
		 if(canStart&&remaining<=0)
		 Destroy(this.GetComponentInParent<Canvas>().gameObject);
	 } */

	public void Begin(float seconds)
	{
		remaining = seconds;
		StartCoroutine(UpdateTimer());
	}

	private IEnumerator UpdateTimer()
	{
		while (remaining > 0)
		{
			CircularCountdown.fillAmount = Mathf.InverseLerp(0, duration, remaining);
			CountDownText.text = remaining.ToString();
			remaining--;
			yield return new WaitForSeconds(1f);
		}
		Destroy(this.GetComponentInParent<Canvas>().gameObject);
	}
}
