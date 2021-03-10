using UnityEngine;

public class PlayerInput : MonoBehaviour
{

    public CarController carController;

    //Pas d'input dans la fixedUpdate;
    // Update is called once per frame
    void Update()
    {
        carController.horizontalInput = Input.GetAxis("Horizontal");
        carController.verticalInput = Input.GetAxis("Vertical");
    }
}
