using Game.GridModule.Model;
using Game.GridModule.View;
using UnityEngine;
using VContainer;

namespace Game.GridModule.Factory
{
    public sealed class GridCellViewFactory
    {
        readonly IObjectResolver container;
        readonly GameObject prefab;
        readonly Transform cellPoolRoot;

        public GridCellViewFactory(IObjectResolver rContainer, GameObject rPrefab, Transform rCellPoolRoot)
        {
            container = rContainer;
            prefab = rPrefab;
            cellPoolRoot = rCellPoolRoot;
        }

        public GridCellView Create(Cell cell)
        {
            GameObject go = Object.Instantiate(prefab, cell.WorldPosition, Quaternion.identity, cellPoolRoot);
            GridCellView view = go.GetComponent<GridCellView>();
            container.Inject(view);
            view.Init(cell);
            return view;
        }
    }
}
