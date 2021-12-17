using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform viewPoint;
    [SerializeField] float mouseSensitive = 1f;
    [SerializeField] float moveSpeed = 5f, runSpeed = 8f;
    private float activeSpeed;
    [SerializeField] CharacterController charController;
    [SerializeField] float jumpForce = 6f;
    [SerializeField] float gravityMod = 2.5f;

    private float verticalRotationStored;
    private Vector2 mouseInput;
    private Vector3 moveDirection, movement;
    private Camera cam;

    [SerializeField] Transform groundCheckPoint;
    private bool isGrounded;
    [SerializeField] LayerMask groundLayers;

    [SerializeField] GameObject bulletImpact;
    [SerializeField] float timeBetweenShot = .1f;
    float shotCounter;

    [SerializeField] float maxHeat = 10f, heatPerShot = 1f, coolRate = 4f, overHeatCoolRate = 5f;
    float heatCounter;
    bool overHeated;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main;
        UIController.instance.weaponTempSlider.maxValue = maxHeat;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            PlayerMovement();
                if(Input.GetMouseButtonDown(0))
                {
                    Shoot();
                }
            CursorUnlockWhenESC();
        }
    }

    private void Shoot()
    {
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        ray.origin = cam.transform.position;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log("HIT" + hit.collider.gameObject.name);
            GameObject bulletImpactObject = Instantiate(bulletImpact, hit.point + (hit.normal * 0.002f), Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(bulletImpactObject, 10f);
        }
    }

    private void PlayerMovement()
    {
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitive;
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + mouseInput.x, transform.rotation.eulerAngles.z);

        verticalRotationStored += mouseInput.y;
        verticalRotationStored = Mathf.Clamp(verticalRotationStored, -60, 60);
        viewPoint.rotation = Quaternion.Euler(-verticalRotationStored, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
    }

    private static void CursorUnlockWhenESC()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Cursor.lockState == CursorLockMode.None)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void LateUpdate()
    {
        if (photonView.IsMine)
        {
            cam.transform.position = viewPoint.position;
            cam.transform.rotation = viewPoint.rotation;
        }

    }
}
