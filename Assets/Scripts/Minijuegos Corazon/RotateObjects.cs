using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObjects : MonoBehaviour
{
    public void rotateObjectsMinigame1(SelectObject objeto)  // Recibe el objeto correcto
    {

        if (objeto != null)
        {
            objeto.transform.Rotate(0, 180f, 0, Space.Self);
            AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Rotate);
        }
    }

    public void rotateObjectsMinigame2(SelectObject objeto)
    {
        if (objeto != null)
        {
            objeto.transform.Rotate(0, 30f, 0, Space.Self);
            AudioController.Instance.PlaySFX(SFX.HeartMinigames, (int)HeartMinigamesSFX.Rotate);
        }
    }
}
