//Copyright 2022, Infima Games. All Rights Reserved.

using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    /// <summary> 
    /// Camera Look. Handles the rotation of the camera.
    /// Переписанная версия с исправленной (плавной) логикой вращения.
    /// </summary> 
    public class CameraLook : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2 sensitivity = new Vector2(1.5f, 1.5f);
        [SerializeField] private Vector2 yClamp = new Vector2(-80, 80);

        [Header("Smoothing")]
        [Range(0.01f, 0.2f)]
        [SerializeField] private float smoothTime = 0.05f; // Чем меньше, тем резче

        private float pitch;
        private float yaw;

        private float pitchVelocity;
        private float yawVelocity;

        private float currentPitch;
        private float currentYaw;

        private CharacterBehaviour playerCharacter;

        private void Start()
        {
            playerCharacter = ServiceLocator.Current.Get<IGameModeService>().GetPlayerCharacter();
            Vector3 euler = transform.eulerAngles;
            currentPitch = pitch = euler.x;
            currentYaw = yaw = playerCharacter.transform.eulerAngles.y;
        }

        private void LateUpdate()
        {
            if (!playerCharacter.IsCursorLocked()) return;

            Vector2 input = playerCharacter.GetInputLook() * sensitivity;

            yaw += input.x;
            pitch -= input.y;
            pitch = Mathf.Clamp(pitch, yClamp.x, yClamp.y);

            currentYaw = Mathf.SmoothDampAngle(currentYaw, yaw, ref yawVelocity, smoothTime);
            currentPitch = Mathf.SmoothDampAngle(currentPitch, pitch, ref pitchVelocity, smoothTime);

            transform.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
            playerCharacter.transform.rotation = Quaternion.Euler(0f, currentYaw, 0f);
        }
    }
}