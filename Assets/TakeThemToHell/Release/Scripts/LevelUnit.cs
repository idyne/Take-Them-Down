using UnityEngine;

public class LevelUnit : MonoBehaviour
{
    private bool isPainted = false;
    [SerializeField] private SpriteRenderer rend = null;
    [SerializeField] private Color paintColor;

    public void Paint()
    {
        if (!isPainted)
        {
            isPainted = true;
            rend.color = paintColor;
        }
    }
}
