using Cysharp.Threading.Tasks;
using Game.GridModule.Model;
using System.Collections.Generic;
using System.Threading;

namespace Game.MatchModule.Controller
{
    public interface IMatchController
    {
        UniTask<bool> ProcessMatchesAsync(CancellationToken ct);
        UniTask<bool> ProcessMatchesAsync(List<Cell> swappedCells, CancellationToken ct);
    }
}