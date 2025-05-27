using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

[RequireComponent(typeof(Rigidbody))]
public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 180f;

    private Animator animator;
    private Rigidbody rb;

    public GameObject holdPoint;
    public GameObject itemToPickup;
    public float throwForce = 500f;

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

        if (Input.GetKeyDown(KeyCode.T))
        {
            animator.SetTrigger("DoThrow");
        }

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
            itemToPickup = collision.gameObject;
 
            animator.SetTrigger("DoPickup");
        }
    }
    public void AttachAppleToHand()
    {
        Debug.Log("AttachAppleToHand event triggered");
        if (itemToPickup != null)

        {
            // Disable physics so it stays in hand nb do this before parenting it

            Rigidbody rb = itemToPickup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("disabling physics");
                rb.velocity = Vector3.zero;  // Stop motion
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;       // Disable physics
                rb.detectCollisions = false; // Optional: Avoid post-parenting collision issues
            }

            // Snap apple to hand
            itemToPickup.transform.SetParent(holdPoint.transform);
            itemToPickup.transform.localPosition = Vector3.zero;
            itemToPickup.transform.localRotation = Quaternion.identity;

        }
    }
    public void ThrowItem()
    {
        Debug.Log("Throw event triggered");
        if (itemToPickup != null)
        {
            // Detach from hand
            itemToPickup.transform.SetParent(null);

            Rigidbody rb = itemToPickup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true;

                // Apply force forward from hand
                Vector3 throwDirection = transform.forward;
                rb.AddForce(throwDirection * throwForce);
            }
            
        }

    }
}
