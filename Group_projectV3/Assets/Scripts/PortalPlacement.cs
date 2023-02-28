using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [RequireComponent(typeof(CameraMove))]
public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField]
    private PortalPair portals;

    [SerializeField]
    private LayerMask layermask;

    [SerializeField]
    private Crosshair crosshair;

    private CameraMove cameraMove;

    private void Awake()
    {
        cameraMove = GetComponent<cameraMove>();
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            FirePortal(0, transform.position, transform.forward, 250.0f);
        }
        else if (Input.GetButtonDown("Fire2"))
        {
            FirePortal(1, transform.position, transform.forward, 250.0f);
        }
    }

    private void FirePortal(int portalID, Vector3 pos, Vector3 dir, float distance)
    {
        RaycastHit hit;
        Physics.Raycast(pos, dir, out hit, distance, layermask);

        if(hit.collider !- null)
        {
            //If we shoot a portal, Recusively fire through the portal.
            if (hit.collider.tag == "Portal")
            {
                var inPortal = hit.collider.Getcomponent<Portal>();

                if(inPortal == null)
                {
                    return;
                }

                //Update position of raycast origin with small offset.
                Vector3 relativePos = inPortal.transform.InverseTransformPoint(hit.point + dir);
                relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
                pos = outPortal.transform.TransformPoint(relativePos);

                //Update direction of raycast.
                Vector3 relativeDir = inPortal.transform.InverseTransformDirection(dir);
                relativeDir = Quanternion.Euler(0.0f, 180.0f, 0.0f) * relativeDir;
                dir = outPortal.transform.TransformDirection(relativeDir);

                distance -= Vector3.Distance(pos, hit.point);

                FirePortal(portalID, pos, dir, distance);

                return;
            }

            //Orient the portal according to the camera look direction and surface direction.
            var cameraRotation = cameraMove.TargetRotation;
            var portalRight = cameraRotation * Vector3.right;

            if(Mathf.Abs(portalRight.x) >= Mathf.Abs(portalRight.z))
            {
                portalRight = (portalRight.x >= 0) ? Vector3.right : -Vector3.right;
            }
            else
            {
                portalRight = (portalRight.z >= 0) ? Vector3.forward : -Vector3.forward;
            }

            var portalForward = -hit.normal;
            var portalUp = -Vector3.Cross(portalRight, portalForward)

            var portalRotation = Quaternion.LookRotation(portalRight, portalForward);

            //attempt to place the portal
            bool wasPlaced = portals.Portals[portalID].PlacePortal(hit.collider, hit.point, portalRotation);

        }
    }
}