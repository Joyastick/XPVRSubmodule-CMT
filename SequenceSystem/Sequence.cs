using MyBox;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Conversation Matrix/Sequence", order = 2)]
public class Sequence : ScriptableObject
{
    private bool gotUID;
    [ReadOnly()] public string UID;

    private void OnValidate()
    {
        if (!gotUID)
        {
            gotUID = true;
            string st = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            for (int i = 0; i < 6; i++)
                UID += st[Random.Range(0, st.Length)];
        }
    }
}