using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Core.Load
{
    public class LoaderCtrl
    {
        private Dictionary<string, Object> assetsCaches = new Dictionary<string, Object>();
        private LoaderBase loader;
        private LoadType loadType;

        private bool useCache;


        public LoaderCtrl(LoadType loadType, bool useCache)
        {
            this.loadType = loadType;
            this.useCache = useCache;

            if (loadType == LoadType.Resources)
            {
                loader = new ResourcesLoader();
            }
        }

        public Object Load(string path)
        {
            if (CheckContainsAssets(path))
            {
                return assetsCaches[path];
            }

            Object ass = loader.LoadAssets(path);
            if (useCache)
            {
                assetsCaches.Add(path, ass);
            }

            return ass;
        }

        public T Load<T>(string path) where T : Object
        {
            if (CheckContainsAssets(path))
            {
                return (T) assetsCaches[path];
            }

            T ass = loader.LoadAssets<T>(path);
            if (useCache)
            {
                assetsCaches.Add(path, ass);
            }


            return ass;
        }

        private bool CheckContainsAssets(string path)
        {
            return assetsCaches.ContainsKey(path);
        }

        public IEnumerator LoadAssetsIEnumerator(string path, Type assetType, Action<Object> callback)
        {
            if (CheckContainsAssets(path))
            {
                callback(assetsCaches[path]);
            }

            loader.LoadAssetsIEnumerator(path, assetType, (ass) =>
            {
                if (useCache)
                {
                    assetsCaches.Add(path, ass);
                }

                callback?.Invoke(ass);
            });


            yield return 0;
        }
    }
}