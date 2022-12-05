using MessagePipe;
using VContainer;
using VContainer.Unity;
using UnityEngine;
using System.Collections.Generic;
namespace Framework
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        public List<ScriptableObject> GameSetting;
        public string MainPath;
        protected override void Configure(IContainerBuilder builder)
        {
            var options = builder.RegisterMessagePipe();
            builder.RegisterBuildCallback(c => GlobalMessagePipe.SetProvider(c.AsServiceProvider()));
            foreach (var item in GameSetting)
            {
                builder.RegisterInstance(item).AsImplementedInterfaces().AsSelf();
            }
            builder.Register<AddressableLoadServer>(Lifetime.Singleton).As<ILoadAssetServer>();
            SceneServer.Init(builder);
            builder.Register<ResourceLoadServer>(Lifetime.Singleton);
            UIServer.Init(builder);
           
            NavigationServer.Init(builder);
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            Container.Resolve<NavigationServer>().GoTo(MainPath);
        }
    }
}