using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IHackable
    {
        bool IsHacked();
        void Hacked();

        Transform GetHackPos();
    }
}
