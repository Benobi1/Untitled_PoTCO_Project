using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class NetworkPlayerController : NetworkBehaviour
{
  [Header("Movement Settings")]
  [SerializeField] private float moveSpeed = 5f;

  [FormerlySerializedAs("camera")]
  [Header("Camera Settings")]
  [SerializeField] private GameObject playerCameraObject;
  
  private CharacterController controller;
  private Vector3 velocity;
  
  public override void OnNetworkSpawn() {
    if (!IsOwner) {
      //Destroy(GetComponentInChildren<Camera>());
      enabled = false;

      if (playerCameraObject != null) {
        playerCameraObject.SetActive(false);
      }
      return;
    }

    if (playerCameraObject != null) {
      playerCameraObject.SetActive(true);
    }
    
    controller = GetComponent<CharacterController>();
  }
  
  void Update() {
    if (!IsOwner) return;
    if (Keyboard.current == null) return;

    float moveX = 0f;
    float moveZ = 0f;

    if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveZ += 1f;
    if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
    if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveZ -= 1f;
    if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
    
    Vector3 movement = (transform.right * moveX + transform.forward * moveZ).normalized;
    controller.Move(movement * moveSpeed * Time.deltaTime);
  }
}
