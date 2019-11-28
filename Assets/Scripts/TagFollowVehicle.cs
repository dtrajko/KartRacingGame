using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagFollowVehicle : MonoBehaviour
{
    public GameObject targetVehicleBody;
    float height;

    private void Awake()
    {
        height = this.transform.position.y;
        if (targetVehicleBody.GetComponentInParent<PlayerController>() != null)
        {
            height += 2.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float positionY = this.transform.position.y;

        this.gameObject.transform.position = new Vector3(
            targetVehicleBody.transform.position.x,
            height,
            targetVehicleBody.transform.position.z);

        Vector3 newRotation = new Vector3(0.0f, targetVehicleBody.transform.eulerAngles.y, 0.0f);
        this.gameObject.transform.eulerAngles = newRotation;

    }
}
