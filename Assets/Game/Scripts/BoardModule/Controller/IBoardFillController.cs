using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.BoardModule.Controller
{
    public interface IBoardFillController
    {
        UniTask<bool> RefillAsync(CancellationToken ct);
    }
}