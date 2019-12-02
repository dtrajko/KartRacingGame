using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun;

public class NetworkedPlayer : MonoBehaviourPunCallbacks
{
    public static GameObject LocalPlayerInstance;
    public GameObject playerNamePrefab;
    public Rigidbody rigidBody;
    public Renderer carMesh;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        else
        {
            GameObject playerName = Instantiate(playerNamePrefab);
            playerName.GetComponent<NameUIController>().target = rigidBody.gameObject.transform;
            playerName.GetComponent<Text>().text = photonView.Owner.NickName;
            playerName.GetComponent<NameUIController>().carRenderer = carMesh;
        }
    }
}
