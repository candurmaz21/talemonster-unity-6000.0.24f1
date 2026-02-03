using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.GridModule.Service
{
    public interface IGridGravityService
    {
        UniTask<bool> ApplyGravityAsync(CancellationToken ct);
    }
}