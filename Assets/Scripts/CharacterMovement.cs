using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private Animator animator;
    private Rigidbody rb;

    public Transform handSocket;
    private GameObject appleInRange;

    private float verticalInput;
    private float horizontalInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Freeze rotation on X and Z so it doesn't tip over
        rb.freezeRotation = true;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");

        // Set animation parameters
        float speed = new Vector2(horizontalInput, verticalInput).magnitude;
        animator.SetFloat("Speed", speed);
        // 

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 moveDirection = transform.forward * verticalInput * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, rb.velocity.y, moveDirection.z);

        // Rotate character
        float turn = horizontalInput * rotationSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);


    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Apple"))
        {
            appleInRange = collision.gameObject;
            animator.SetTrigger("DoPickup");
        }
    }
    public void AttachAppleToHand()
    {
        if (appleInRange != null)
        {
            // Snap apple to hand
            appleInRange.transform.SetParent(handSocket);
            appleInRange.transform.localPosition = Vector3.zero;
            appleInRange.transform.localRotation = Quaternion.identity;

            // Disable physics so it stays in hand
            Rigidbody rb = appleInRange.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true;
        }
    }
}
