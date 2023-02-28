using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class PortalCamera : MonoBehaviour
{
    [SerializeField]
    private Portal[] portals = new Portal[2];

    [SerializeField]
    private Camera portalCamera;

    [SerializeField]
    private int iterations = 7;

    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    private Camera MainCamera;

    private void Awake()
    {
        MainCamera = GetComponent<Camera>();

        tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        portals[0].Renderer.material.maintexture = tempTexture1;
        portals[1].Renderer.material.maintexture = tempTexture2;
    }
    
    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (!portals[0].IsPlaced || !portals[1].IsPlaced)
        {
            return;
        }

        if (!portals[0].Renderer.IsVisible)
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i, SRC);
            }
        }

        if (!portals[0].Renderer.IsVisible)
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i, SRC);
            }
        }
    }

    private void RenderCamera(Portal inPortal, Portal outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        inTransform cameraTransform = portalCamera.transform;
        cameraTransform.position = cameraTransform.position;
        cameraTransform.rotation = cameraTransform.rotation;

        for(int i = 0; i <= iterationID; ++i)
        {
            //Position the camera behind the other portal
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            //Rotate the camera to look through the other portal
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        //Set the camera's oblique view frustum
        plane p = new plane(-outTransform.forward, outTransform.position);
        Vector4 clipPlaneworldSpace = new Vector(p.normal.x, p.normal.y, p.normal.z, p.distance);
        Vector4 clipPlaneCameraSpace =
            Matrix4x4.Transpose(Matrix4x4.inverse(portalCamera.worldToCameraMatrix)) * clipPlaneworldSpace;

        var newMatrix = MainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        portalCamera.ProjectionMatrix = newMatrix;

        // Render the camera to its render target
        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
}
