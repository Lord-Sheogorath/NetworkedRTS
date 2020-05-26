﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unit_System
{
    [CreateAssetMenu(fileName = "New MoveToAction", menuName = "ScriptableObject/RTS/AI/MoveToPoint")]
    public class MoveToPointAction : AIAction
    {
        [Header("Movement Data")]
        [Tooltip("Placement layers")] public LayerMask MovementLayers;
        [Tooltip("How far away from the target position the unit will stop")] public float StoppingAccuracy = 1;
        public float EvaluationValue = 0;

        public Vector3 CurrentTarget;

        public override float UpdateAction(AIAgent agent)
        {
            return 0.0f;
        }

        public override bool HasActionCompleted(AIAgent agent)
        {
            if (!agent) return true;

            return Vector3.Distance(agent.transform.position, CurrentTarget) <= StoppingAccuracy + 1;
        }

        public override bool ExecuteAction(AIAgent agent)
        {
            if (!base.ExecuteAction(agent)) return false;

            if (agent.NavAgent)
            {
                agent.NavAgent.SetDestination(CurrentTarget);
            }

            return true;
        }

        public override float EvaluateAction(AIAgent agent)
        {
            if (!agent) return 0.0f;

            return EvaluationValue;
        }

        public override void EnterAction(AIAgent agent)
        {
            if (!agent || !agent.NavAgent) return;

            if (agent.NavAgent.enabled)
            {
                agent.NavAgent.ResetPath();
            }
        }
        public override void ExitAction(AIAgent agent)
        {
            if (!agent || !agent.NavAgent) return;

            if (agent.NavAgent.enabled)
            {
                agent.NavAgent.ResetPath();
            }
        }

        public override bool SelectionAction(AIAgent agent)
        {
            RaycastHit rayHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit))
            {
                CurrentTarget = rayHit.point;

                // Checks if the hit object is on the right layer
                return Helper.IsInLayerMask(MovementLayers, rayHit.collider.gameObject.layer);
            }

            return false;
        }
    }
}