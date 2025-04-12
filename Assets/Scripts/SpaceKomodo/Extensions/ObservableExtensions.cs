using System;
using System.Collections.Generic;
using ObservableCollections;
using R3;

namespace SpaceKomodo.Extensions
{
    public static class ObservableExtensions
    {
        public static Observable<(T element, TProperty property)> ObserveElementObservableProperty<T, TProperty>(
            this ObservableList<T> source,
            Func<T, ReactiveProperty<TProperty>> propertySelector) where T : class
        {
            return Observable.Create<(T, TProperty)>(observer =>
            {
                var disposables = new CompositeDisposable();
                var currentSubscriptions = new Dictionary<T, IDisposable>();
                
                IDisposable SubscribeToElement(T element)
                {
                    return propertySelector(element)
                        .Subscribe(value => observer.OnNext((element, value)))
                        .AddTo(disposables);
                }
                
                foreach (var element in source)
                {
                    var subscription = SubscribeToElement(element);
                    currentSubscriptions.Add(element, subscription);
                }
                
                source.ObserveAdd()
                    .Subscribe(addEvent =>
                    {
                        var element = addEvent.Value;
                        var subscription = SubscribeToElement(element);
                        currentSubscriptions.Add(element, subscription);
                    })
                    .AddTo(disposables);
                
                source.ObserveRemove()
                    .Subscribe(removeEvent =>
                    {
                        var element = removeEvent.Value;
                        if (currentSubscriptions.TryGetValue(element, out var subscription))
                        {
                            subscription.Dispose();
                            currentSubscriptions.Remove(element);
                        }
                    })
                    .AddTo(disposables);
                
                return disposables;
            });
        }
        
        public static Observable<(T1, T2)> CombineLatest<T1, T2>(
            this Observable<T1> source1,
            Observable<T2> source2)
        {
            return source1.CombineLatest(source2, (x1, x2) => (x1, x2));
        }

        public static Observable<(T1, T2, T3)> CombineLatest<T1, T2, T3>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3)
        {
            return source1.CombineLatest(source2, source3, (x1, x2, x3) => (x1, x2, x3));
        }

        public static Observable<(T1, T2, T3, T4)> CombineLatest<T1, T2, T3, T4>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4)
        {
            return source1.CombineLatest(source2, source3, source4, (x1, x2, x3, x4) => (x1, x2, x3, x4));
        }

        public static Observable<(T1, T2, T3, T4, T5)> CombineLatest<T1, T2, T3, T4, T5>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5)
        {
            return source1.CombineLatest(source2, source3, source4, source5, (x1, x2, x3, x4, x5) => (x1, x2, x3, x4, x5));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6)> CombineLatest<T1, T2, T3, T4, T5, T6>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, (x1, x2, x3, x4, x5, x6) => (x1, x2, x3, x4, x5, x6));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7)> CombineLatest<T1, T2, T3, T4, T5, T6, T7>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, (x1, x2, x3, x4, x5, x6, x7) => (x1, x2, x3, x4, x5, x6, x7));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, (x1, x2, x3, x4, x5, x6, x7, x8) => (x1, x2, x3, x4, x5, x6, x7, x8));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, (x1, x2, x3, x4, x5, x6, x7, x8, x9) => (x1, x2, x3, x4, x5, x6, x7, x8, x9));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10,
            Observable<T11> source11)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10,
            Observable<T11> source11,
            Observable<T12> source12)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10,
            Observable<T11> source11,
            Observable<T12> source12,
            Observable<T13> source13)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10,
            Observable<T11> source11,
            Observable<T12> source12,
            Observable<T13> source13,
            Observable<T14> source14)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14));
        }

        public static Observable<(T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15)> CombineLatest<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            this Observable<T1> source1,
            Observable<T2> source2,
            Observable<T3> source3,
            Observable<T4> source4,
            Observable<T5> source5,
            Observable<T6> source6,
            Observable<T7> source7,
            Observable<T8> source8,
            Observable<T9> source9,
            Observable<T10> source10,
            Observable<T11> source11,
            Observable<T12> source12,
            Observable<T13> source13,
            Observable<T14> source14,
            Observable<T15> source15)
        {
            return source1.CombineLatest(source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12, source13, source14, source15, (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15) => (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15));
        }
    }
}