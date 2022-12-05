using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;
using VContainer;

namespace Framework
{
    public class ResourceLoadServer : ILoadAssetServer
    {
       
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            
            return Resources.Load<T>(path);
        }

        public T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            return Resources.LoadAll<T>(path);
        }

        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
            var  ResourceRequest  = Resources.LoadAsync<T>(path);
            return (await ResourceRequest) as T;
        }    
    }
    public class AddressableLoadServer : ILoadAssetServer
    {
        [Obsolete("不允许Addressables同步加载")]
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            throw new System.NotImplementedException();
        }
        [Obsolete("不允许Addressables同步加载")]
        public T[] LoadAll<T>(string path) where T : UnityEngine.Object
        {
            throw new System.NotImplementedException();
        }
        public AddressableLoadServer()
        {

        }
        public async UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
          var Async=  Addressables.LoadAssetAsync<T>(path);
            await Async;
            return Async as T;
        }
    }
}

