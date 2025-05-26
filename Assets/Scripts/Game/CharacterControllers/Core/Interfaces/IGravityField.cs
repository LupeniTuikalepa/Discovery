using UnityEngine;

namespace Discovery.Game.CharacterControllers.Gravity
{
    public interface IGravityField
    {
        Vector3 GetGravityDirection(Vector3 from);
    }
}