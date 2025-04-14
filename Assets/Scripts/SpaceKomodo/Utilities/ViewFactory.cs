using UnityEngine;
using VContainer;

namespace SpaceKomodo.Utilities
{
    public class ViewFactory<T1, TView> : IViewFactory<T1, TView> where TView : MonoBehaviour
    {
        private readonly TView _prefab;
        private readonly IObjectResolver _resolver;
        
        public ViewFactory(TView prefab, IObjectResolver resolver)
        {
            _prefab = prefab;
            _resolver = resolver;
        }

        public TView Create(T1 t1, Transform parent = null)
        {
            var instance = Object.Instantiate(_prefab, parent);
            _resolver.Inject(instance);
            
            if (instance is IInitializable<T1> initializable)
            {
                initializable.Initialize(t1);
            }
            
            return instance;
        }
    }
}