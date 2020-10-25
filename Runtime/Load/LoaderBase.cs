using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Load
{
    /// <summary>
    /// 加载基类
    /// </summary>
    public abstract class LoaderBase
    {
        public abstract IEnumerator LoadAssetsIEnumerator(string path, Type resType, Action<Object> callback);

        public abstract Object LoadAssets(string path);

        public abstract T LoadAssets<T>(string path) where T : Object;
        
        
    }
}