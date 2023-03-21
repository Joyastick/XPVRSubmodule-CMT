using MyBox;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(BoxCollider))]
public class CMT_UI_Button : MonoBehaviour
{
    [Tooltip("use this to invoke what is in the button component's onClick event")]
    public bool useExistingButton = true; //if there is an existing button, it will be invoked on press

    [ConditionalField("useExistingButton",true)]
    [Tooltip("events under this will be triggered on button press if 'useExistingButton' is set to false")]
    public UnityEvent onButtonPressed;

    private BoxCollider _bC;
    
    //private GameObject indicator;
    private void OnValidate()
    {
        if (_bC == null)
        {
            _bC = GetComponent<BoxCollider>();
            _bC.isTrigger = true;
            var rect = GetComponent<RectTransform>().rect;
            _bC.size = new Vector3(rect.width, rect.height, 30);
        }
    }

    private void ButtonPressed()
    {
        var success = false;
        if (useExistingButton)
        {
            var temp = GetComponent<Button>();
            if (temp != null)
            {
                temp.onClick.Invoke();
                success = true;
            }
        }

        if (!success) //fallback if button fails
            onButtonPressed.Invoke();
    }
}