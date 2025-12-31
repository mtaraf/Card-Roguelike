using UnityEngine;
public class EnemyEnergy : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + 90f * Time.deltaTime);
    }
}