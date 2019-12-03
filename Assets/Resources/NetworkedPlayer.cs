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
    public MeshRenderer carMesh;

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
            playerName.GetComponent<NameUIController>().carRenderer = carMesh;

            string sentName = null;
            if (photonView.InstantiationData != null)
            {
                sentName = (string)photonView.InstantiationData[0];
            }
            if (sentName != null)
            {
                playerName.GetComponent<Text>().text = sentName;
            }
            else
            {
                playerName.GetComponent<Text>().text = photonView.Owner.NickName;
            }

        }
    }
}
