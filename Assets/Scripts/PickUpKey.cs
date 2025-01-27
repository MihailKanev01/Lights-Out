using UnityEngine;

public class PickUpKey : MonoBehaviour
{
    public GameObject keyObject;
    public GameObject inventoryObject;
    public GameObject pickUpText;
    public AudioSource keySound;

    private bool isInReach = false;

    void Start()
    {
        pickUpText.SetActive(false);
        inventoryObject.SetActive(false);
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            isInReach = true;
            pickUpText.SetActive(true);
        }
    }
    private void onTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            isInReach = false;
            pickUpText.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isInReach && Input.GetButtonDown("Interact"))
            PickUpKeyItem();
    }

    void PickUpKeyItem()
    {
        if (keySound != null)
        {
            keySound.Play();
        }
        if (keyObject != null)
        {
            keyObject.SetActive(false);
        }
        if (inventoryObject != null)
        {
            inventoryObject.SetActive(true);
        }
        pickUpText.SetActive(false);
        isInReach = false;
    }

}
