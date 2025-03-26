using UnityEngine;
using System;

public class PlantCollision : MonoBehaviour
{
    public string plantId = "0";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlantCollision plant = collision.gameObject.GetComponentInParent<PlantCollision>();
        if (plant.plantId != plantId)
        {
            GameManager.instance.plantsCollision(plantId, plant.plantId);
        }
    }
}