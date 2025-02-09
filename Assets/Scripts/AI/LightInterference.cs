using UnityEngine;

public class LightInterference : MonoBehaviour
{
    public Light[] lightsInHouse;
    private float timer;
    private float interval = 10f;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > interval)
        {
            TurnOffRandomLight();
            timer = 0;
        }
    }

    private void TurnOffRandomLight()
    {
        if (lightsInHouse.Length > 0)
        {
            int index = Random.Range(0, lightsInHouse.Length);
            lightsInHouse[index].enabled = false;
        }
    }
}
