using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagFollowVehicle : MonoBehaviour
{
    public GameObject targetVehicleBody;
    float height;
    // bool heightAdjusted;

    private void Start()
    {
        // heightAdjusted = false;
        height = this.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetVehicleBody == null)
        {
            Debug.Log("TagFollowVehicle targetVehicleBody not available.");
            return;
        }

        // if (!heightAdjusted)
        // {
        //     height = this.transform.position.y;
        //     if (targetVehicleBody.GetComponentInParent<PlayerController>() != null)
        //     {
        //         height += 2.0f;
        //     }
        // 
        //     heightAdjusted = true;
        // }

        float positionY = this.transform.position.y;

        this.gameObject.transform.position = new Vector3(
            targetVehicleBody.transform.position.x,
            height,
            targetVehicleBody.transform.position.z);

        Vector3 newRotation = new Vector3(0.0f, targetVehicleBody.transform.eulerAngles.y, 0.0f);
        this.gameObject.transform.eulerAngles = newRotation;

    }
}
