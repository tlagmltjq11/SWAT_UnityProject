using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Hostage"))
        {
            GameManager.Instance.SetState(GameManager.eGameState.Success);
        }
    }
}
