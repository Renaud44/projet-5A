using UnityEngine;
using UnityEngine.Events;

public class SimpleButton : MonoBehaviour
{
    public UnityAction action;

    public void OnPress()
    {
        if (action != null)
        {
            Debug.Log("Clic sur : " + gameObject.name);

            // Petit effet visuel : le bouton s'enfonce
            StartCoroutine(AnimClick());

            action.Invoke();
        }
    }

    System.Collections.IEnumerator AnimClick()
    {
        Vector3 initialPos = transform.localPosition;
        transform.localPosition += new Vector3(0, 0, 0.02f); // S'enfonce (Z positif car on regarde de face arrière parfois)
        yield return new WaitForSeconds(0.1f);
        transform.localPosition = initialPos;
    }
}