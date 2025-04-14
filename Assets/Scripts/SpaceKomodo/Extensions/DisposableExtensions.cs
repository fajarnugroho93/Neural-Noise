using System;
using R3;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SpaceKomodo.Extensions
{
    public static class DisposableExtensions
    {
        public static IDisposable ToDisposable(this GameObject gameObject)
        {
            return Disposable.Create(OnDisposed);

            void OnDisposed()
            {
                if (gameObject == null)
                {
                    return;
                }
                
                Object.Destroy(gameObject);
            }
        }
    }
}