using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NameUIController : MonoBehaviour
{
    public Text playerName;
    public Transform target;

    // Start is called before the first frame update
    void Awake()
    {
        this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        playerName = this.GetComponent<Text>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Camera.main == null)
        {
            Debug.Log("Camera.main is empty");
            return;
        }

        this.transform.position = Camera.main.WorldToScreenPoint(target.position + Vector3.up * 1.5f);
    }
}
