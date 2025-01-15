using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "Are Target In Range", story: "Are Target In [Fov] Range", category: "Conditions", id: "8a7275cacbf8746a518bf95069324000")]
public partial class AreTargetInRangeCondition : Condition
{
    [SerializeReference] public BlackboardVariable<FieldOfView> Fov;

    public override bool IsTrue()
    {
        return Fov.Value.IsTargetInRange().isInRange;
    }
}
