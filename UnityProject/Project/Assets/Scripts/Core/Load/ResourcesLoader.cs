using System;
using System.Collections;
using Tools;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Load
{
    public class ResourcesLoader : LoaderBase
    {
        public override IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<Object> callback)
        {
            ResourceRequest ass = null;
            string s = PathUtils.RemoveExtension(path);
            if (resType != null)
            {
                ass = Resources.LoadAsync(s, resType);
            }
            else
            {
                ass = Resources.LoadAsync(s);
            }

            yield return ass;

            if (ass.asset == null)
            {
                Debug.LogError("加载失败，Path：" + path);
            }

            if (callback != null)
            {
                callback(ass.asset);
            }

            yield return new WaitForEndOfFrame();
        }

        public override Object LoadAssets(string path)
        {
            string s = PathUtils.RemoveExtension(path);
            Object ass = Resources.Load(s);
            if (ass == null)
            {
                Debug.LogError("加载失败,Path:" + path);
            }

            return ass;
        }

        public override T LoadAssets<T>(string path)
        {
            string s = PathUtils.RemoveExtension(path);
            T ass = Resources.Load<T>(s);
            if (ass == null)
            {
                Debug.LogError("加载失败,Path:" + path);
            }

            return ass;
        }
    }
}