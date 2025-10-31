using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OldElevator {
    public class ElevatorInteraction : MonoBehaviour {
        public LayerMask layerMaskButtons;
        public float maxInteractionDistance;
        public UnityEngine.UI.Image interactIcon;

        // New: optional transform to use as the raycast origin/direction. If null, falls back to this.transform
        public Transform rayOrigin;

        void Start() {
            if (layerMaskButtons == 0) layerMaskButtons = LayerMask.GetMask("ElevatorInteraction");
        }


        void Update() {
            Transform originT = (rayOrigin != null) ? rayOrigin : transform;

            // draw ray in Play mode for easier debugging
            Debug.DrawRay(originT.position, originT.forward * maxInteractionDistance, Color.blue);

            // Use the configurable originT for the raycast (fixed incorrect parameter list)
            if (Physics.Raycast(
                originT.position,
                originT.forward,
                out RaycastHit hit,
                maxInteractionDistance,
                layerMaskButtons
            )) {
                // show interact icon when something is hittable
               // InteractionIcon.inst.SetInteractable(true);

                //player can interact with an elevator interactable object
                if (
                    //Input.GetButtonDown("Fire1")
                    Input.GetButtonDown("Interact")
                    ) {
                    if (hit.collider.transform.TryGetComponent(out ElevatorButton btn)) {
                        //it is a button. Press it.
                        btn.Press();
                    } else if (hit.collider.transform.TryGetComponent(out DoorTrigger door)) {
                        //it is a door. Open/Close it.
                        door.Toggle();
                    }
                }
            } else {
                //can't interact
                //InteractionIcon.inst.SetInteractable(false);
            }
        }

        // Draw the interaction ray in the Scene view when the object is selected
        void OnDrawGizmosSelected() {
            DrawInteractionGizmos();
        }

        // Always draw gizmos in the Scene view for easier debugging
        void OnDrawGizmos() {
            DrawInteractionGizmos();
        }

        void DrawInteractionGizmos() {
            Transform originT = (rayOrigin != null) ? rayOrigin : transform;
            Vector3 origin = originT.position;
            Vector3 dir = originT.forward;
            float distance = Mathf.Max(0.01f, maxInteractionDistance);

            // Ensure we have a mask to use in the editor as well
            int mask = (layerMaskButtons == 0) ? LayerMask.GetMask("ElevatorInteraction") : layerMaskButtons;

            // Cast the same ray used for interaction to show if it would hit something
            if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, mask)) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(origin, hit.point);
                Gizmos.DrawSphere(hit.point, 0.05f);
            } else {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(origin, origin + dir * distance);
                Gizmos.DrawSphere(origin + dir * distance, 0.03f);
            }
        }
    }
}