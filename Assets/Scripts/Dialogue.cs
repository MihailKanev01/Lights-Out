using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private Text textOB;
    [SerializeField] private GameObject activator;
    [SerializeField] private string dialogue = "Dialogue";
    [SerializeField] private float timer = 2f;

    private void Start()
    {
        textOB.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textOB.enabled = true;
            textOB.text = dialogue;
            StartCoroutine(DisableText());
        }
    }

    private IEnumerator DisableText()
    {
        yield return new WaitForSeconds(timer);
        textOB.enabled = false;
        if (activator != null)
        {
            Destroy(activator);
        }
    }
}
