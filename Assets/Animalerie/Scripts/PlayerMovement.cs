using System.Collections; 
using System.Collections.Generic; 
using UnityEngine; 
public class PlayerMovement : MonoBehaviour { 
    public CharacterController controller; 
    public float moveSpeed = 12f; 
    public float gravity = -9.81f; 
    public Transform groundCheck; 
    public float groundDistance = 0.4f; 
    public LayerMask groundMask; 
    Vector3 velocity; 
    bool isGrounded; 
    void Update() { 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
         if(isGrounded && velocity.y < 0) { 
            velocity.y = -2f; 
        } 
         
         // Récupérer les déplacements horizontaux et verticaux 
         float x = Input.GetAxis("Horizontal"); 
         float z = Input.GetAxis("Vertical"); 
         
         // Déplacer la caméra en fonction des touches (W, A, S, D) 
         Vector3 move = transform.right * x + transform.forward * z; 
         transform.position += move * moveSpeed * Time.deltaTime; 
         controller.Move(move * moveSpeed * Time.deltaTime); velocity.y += gravity * Time.deltaTime; 
         controller.Move(velocity * Time.deltaTime); } 
         
}


//********Ajout du saut
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public CharacterController controller;
//     public float moveSpeed = 12f;
//     public float gravity = -9.81f;
//     public float jumpForce = 5f; // force du saut

//     Vector3 velocity;

//     void Update()
//     {
//         // Déplacements
//         float x = Input.GetAxis("Horizontal");
//         float z = Input.GetAxis("Vertical");

//         Vector3 move = transform.right * x + transform.forward * z;
//         controller.Move(move * moveSpeed * Time.deltaTime);

//         // SAUT (ESPACE)
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             velocity.y = jumpForce;
//         }

//         // Gravité
//         velocity.y += gravity * Time.deltaTime;
//         controller.Move(velocity * Time.deltaTime);
//     }
// }

