using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine;

public class Player_DIE : FSMSingleton<Player_DIE>, IFSMState<Player_StateManager>
{
    public void Enter(Player_StateManager e)
    {
        GameObject dyingModel = Resources.Load("Player/Player_Dying_Model") as GameObject;
        var model = Instantiate(dyingModel, new Vector3(e.gameObject.transform.position.x, e.gameObject.transform.position.y - 0.9f, e.gameObject.transform.position.z), e.gameObject.transform.rotation);
        SoundManager.Instance.Play2DSound(SoundManager.eAudioClip.PLAYER_DEATH, 1f);
        UIManager.Instance.PlayerDie();
        e.gameObject.SetActive(false);
    }

    public void Execute(Player_StateManager e)
    {

    }

    public void Exit(Player_StateManager e)
    {

    }
}
