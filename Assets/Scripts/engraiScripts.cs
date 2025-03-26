using UnityEngine;
using System.Collections;

public class engraiScripts : MonoBehaviour
{
    public bool destroying = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destroying) return;
        if (collision.gameObject.transform.CompareTag("PlantTige"))
        {
            GameManager.instance.getPlantById(collision.gameObject.GetComponentInParent<PlantCollision>().plantId).pointsAvailable++;
            StartCoroutine(lerpToPoint(collision.ClosestPoint(transform.position)));
        }
    }

    IEnumerator lerpToPoint(Vector3 pos)
    {
        destroying = true;
        while (MathHelpers.distance(pos, gameObject.transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(gameObject.transform.position, pos, Time.deltaTime * 15);
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
        yield return null;
    }
}
