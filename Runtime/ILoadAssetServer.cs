using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace Framework
{
    public interface ILoadAssetServer {
         T Load<T>(string path) where T : Object;
         T[] LoadAll<T>(string path) where T : Object;
        UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
       }
}