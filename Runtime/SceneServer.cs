using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace Framework
{
   public class SceneServer
    {
        readonly LifetimeScope parent;
        [Inject]
        public SceneServer(LifetimeScope lifetimeScope)
        {
          parent = lifetimeScope;
        }
        internal static void Init(IContainerBuilder builder)
        {             
            builder.Register<SceneServer>(Lifetime.Singleton);
        }
            //IEnumerator LoadSceneAsync()
            //{
            //    // LifetimeScope generated in this block will be parented by `this.lifetimeScope`
            //    using (LifetimeScope.EnqueueParent(parent))
            //    {
            //        // If this scene has a LifetimeScope, its parent will be `parent`.
            //        var loading = SceneManager.LoadSceneAsync("...", LoadSceneMode.Additive);
            //        while (!loading.isDone)
            //        {
            //            yield return null;
            //        }
            //    }
            //}

            // UniTask
          public  async UniTask LoadSceneAsync(string sceneName,LoadType loadType , LoadSceneMode loadSceneMode)
        {
            if (sceneName != "_")
            using (LifetimeScope.EnqueueParent(parent))
            {
                if (loadType == LoadType.Builing)
                    await SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                else
                    await Addressables.LoadSceneAsync(sceneName, loadSceneMode);
            }
        }
    }
}

