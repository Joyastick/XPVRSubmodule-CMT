using UnityEngine;
using UnityEngine.UI;

public class ShadowCopy : MonoBehaviour
{
    public Text source;
    private Text target;

    private void Start()
    {
        target = GetComponent<Text>();
    }

    private void Update()
    {
        if (target && source)
            target.text = source.text;
    }
}