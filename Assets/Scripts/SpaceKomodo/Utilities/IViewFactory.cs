using UnityEngine;

namespace SpaceKomodo.Utilities
{
    public interface IViewFactory<TModel, TView> where TView : MonoBehaviour
    {
        TView Create(TModel model, Transform parent = null);
    }
}