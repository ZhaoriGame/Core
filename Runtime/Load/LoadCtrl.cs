using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Load
{
    public class LoadCtrl : MonoBehaviour
    {
        private LoaderCtrl loaderCtrl;


        /// <summary>
        /// 设置加载方法
        /// </summary>
        /// <param name="loaderCtrl"></param>
        public void SetLoader(LoaderCtrl loaderCtrl)
        {
            this.loaderCtrl = loaderCtrl;
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Object Load(string path)
        {
            return loaderCtrl.Load(path);
        }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="path"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Load<T>(string path) where T : Object
        {
            return loaderCtrl.Load<T>(path);
        }

        /// <summary>
        /// 异步加载
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(string path, Action<Object> callback) where T: Object
        {
            StartCoroutine(loaderCtrl.LoadAssetsIEnumerator(path, typeof(T), callback));
        }
        
        
    }
}