using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RebindSFX : MonoBehaviour
{
    public void PlaySFX()
    {
        AudioController.Instance.PlaySFX(SFX.Menu, (int)MenuSFX.Click);
    }
}