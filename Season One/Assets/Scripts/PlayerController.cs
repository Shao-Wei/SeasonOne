using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace weisquare.SeasonOne
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public float walkSpeed, runSpeed, rotateSpeed, jumpForce, walkAnimationSpeed, runAnimatonSpeed;

        public Animator animator;
        public Rigidbody rb;
        public GameObject camera;

        public float RideHeight, RideSpringStrength, RideSpringDamper;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            //Allow the player to move left and right
            float horizontal = Input.GetAxisRaw("Horizontal");
            //Allow the player to move forward and back
            float vertical = Input.GetAxisRaw("Vertical");
            
            // speed
            float speed = walkSpeed;
            float animSpeed = walkAnimationSpeed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                vertical *= 2f;
                speed = runSpeed;
                animSpeed = runAnimatonSpeed;
            }

            // Movement on the plane
            var translation = transform.forward * (vertical * Time.deltaTime);
            translation += transform.right * (horizontal * Time.deltaTime);
            translation *= speed;
            translation = GetComponent<Rigidbody>().position + translation;
            GetComponent<Rigidbody>().MovePosition(translation);

            animator.SetFloat("Vertical", vertical, 0.1f, Time.deltaTime);
            animator.SetFloat("Horizontal", horizontal, 0.1f, Time.deltaTime);
            animator.SetFloat("WalkSpeed", animSpeed);

            // Float on the plane
            Vector3 groundRayDir = -Vector3.up;
            RaycastHit groundRayHit;
            if( Physics.Raycast(transform.position, groundRayDir, out groundRayHit) )
            {
                Vector3 vel = rb.velocity;
                Vector3 vel2 = Vector3.zero;
                Rigidbody hitBody = groundRayHit.rigidbody;
                if( hitBody )
                    vel2 = hitBody.velocity;

                float rayDirVel = Vector3.Dot(groundRayDir, vel);
                float rayDirVel2 = Vector3.Dot(groundRayDir, vel2);
                float relVel = rayDirVel - rayDirVel2;
                float x = groundRayHit.distance - RideHeight;
                float springForce = (x * RideSpringStrength) - (relVel * RideSpringDamper);
                Debug.DrawLine(transform.position, transform.position + (groundRayDir * springForce), Color.yellow);

                rb.AddForce(groundRayDir * springForce);
                if( hitBody )
                    hitBody.AddForceAtPosition( -groundRayDir * springForce, groundRayHit.point);
            }
        }
    }
}
