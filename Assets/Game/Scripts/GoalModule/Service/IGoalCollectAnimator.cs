using System.Threading;
using Cysharp.Threading.Tasks;
using Game.HeroModule.Model;
using UnityEngine;


namespace Game.GoalModule.Service
{
    public interface IGoalCollectAnimator
    {
        UniTask Play(HeroType type, int level, Vector3 worldStartPos, CancellationToken ct);
    }
}