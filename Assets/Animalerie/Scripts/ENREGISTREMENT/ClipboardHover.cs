using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClipboardHover : MonoBehaviour
{
    public GameObject pressFText;
    public GameObject registerPanel;

    bool isHovering = false;

    void Update()
    {
        if (isHovering && Input.GetKeyDown(KeyCode.F))
        {
            OpenRegister();
        }
    }

    public void OnHoverEnter(HoverEnterEventArgs args)
    {
        Debug.Log("Hover clipboard");
        isHovering = true;
        pressFText.SetActive(true);
    }

    public void OnHoverExit(HoverExitEventArgs args)
    {
        isHovering = false;
        pressFText.SetActive(false);
    }

    void OpenRegister()
    {
        pressFText.SetActive(false);
        registerPanel.SetActive(true);
    }
}