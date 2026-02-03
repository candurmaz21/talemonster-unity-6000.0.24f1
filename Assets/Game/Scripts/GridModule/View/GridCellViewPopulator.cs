using UnityEngine;
using VContainer;
using VContainer.Unity;
using Game.GridModule.Controller;
using Game.GridModule.Model;
using Game.GridModule.Factory;

namespace Game.GridModule.View
{
    public sealed class GridCellViewPopulator : IStartable
    {
        [Inject] readonly IGridProvider gridProvider;
        [Inject] readonly GridCellViewFactory factory;

        void IStartable.Start()
        {
            foreach (Cell cell in gridProvider.Cells)
            {
                factory.Create(cell);
            }
        }
    }
}