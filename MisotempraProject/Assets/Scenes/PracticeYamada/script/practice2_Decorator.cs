using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using AI.BehaviorTree;
using UnityEngine;
[System.Serializable]//必須


public class practice2_Decorator : AI.BehaviorTree.BaseDecorator
{
    [SerializeField]
    float m_trueDistance = 1.0f;

    public override bool IsPredicate(AIAgent agent, Blackboard blackboard)
    {
        return (blackboard.GetValue<Transform>("PlayerTransform").position 
			- agent.transform.position).sqrMagnitude < (m_trueDistance * m_trueDistance);
    }
}
