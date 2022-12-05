using System;
using System.Collections.Generic;
using System.Reflection;
using VContainer;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Framework;
using static Codice.Client.GameUI.Update.SelectedItemsCache;
using VContainer.Unity;

namespace Framework
{
    public class NavigationServer {
        static Assembly assembly;
        Dictionary<string, View> keyValuePairs;
        internal static void Init(IContainerBuilder builder)
        {
            assembly = Assembly.Load("Assembly-CSharp");
            builder.Register<NavigationServer>(Lifetime.Singleton);
            foreach (var item in assembly.GetTypes())
            {
                if (item.IsSubclassOf(typeof(ViewModel)))
                {
                    builder.Register(item, Lifetime.Singleton).As<ViewModel, IStartable>().AsSelf();
                }
            }
            builder.RegisterBuildCallback(container => container.Resolve<NavigationServer>().InjectView(container));
        }
        private void InjectView(IObjectResolver container)
        {
            keyValuePairs = new Dictionary<string, View>();
           
            foreach (var item in assembly.GetTypes())
            {
                 if (item.IsSubclassOf(typeof(View)))
                {
                    var t = item.GetCustomAttribute<RoutedAttribute>();
                    if (t != null)
                    {                       
                        var View = assembly.CreateInstance(item.FullName) as View;                   
                        keyValuePairs.Add(t.path, View);
                        container.Inject(View);
                    }
                }
            }
        }
        private Stack<View> _stackViews = new Stack<View>();
        public  void GoTo(string url)
        {
            if (keyValuePairs.TryGetValue(url, out var view))
            {
                Action go = async () =>
                {
                    View ExitView = _stackViews.Count > 0 ? _stackViews.Peek() : null;
                    if (ExitView != null)
                        ExitView.isLoad = false;
                    await view.Init();
                      ExitView?.Exit();
                    _stackViews.Push(view);
                };
                if (_stackViews.Count > 0&&_stackViews.Peek() != null)
                    _stackViews.Peek().PreExit(() => view.PreInit(go));
                else
                    view.PreInit(go);
            }
        }
        public void Back()
        {
            Action back = async () =>
            {

                if (_stackViews.Count > 1)
                {
                    var exit = _stackViews.Pop();
                    exit.isLoad = false;
                    await _stackViews.Peek().Init();
                    exit.Exit();
                }
            };
            _stackViews.Peek().PreExit(() =>
            {

                var curr = _stackViews.Pop();
                _stackViews.Peek().PreInit(()=> {
                    _stackViews.Push(curr);
                    back?.Invoke(); });
            });
           
        }
    }
    public abstract class View<T, F> :View where T : ViewModel where F : ViewPanel
    {
        [Inject]
        protected T ViewModel;
        [Inject]
        protected SceneServer SceneServer;
        [Inject]
        protected UIServer UIServer;
        [Inject]
        protected NavigationServer NavigationServer;
        protected F ViewPanel;
        public override async UniTask Init()
        {
            var route= GetType().GetCustomAttribute<RoutedAttribute>();
            await SceneServer.LoadSceneAsync(route.sceneName,route.LoadType, route.LoadSceneMode);
            ViewPanel = UIServer.OpenUIView<F>(UILevel.Common);
            ViewPanel.gameObject.SetActive(false);
            isLoad = true;
            OnEnable();
        }
        public override void Exit()
        {       
            ViewPanel.Discharge();
        }
    public async virtual UniTask<bool> m_PreInit()
    {
        await UniTask.Yield();
        return true;
    }
    public  override void  PreInit(Action action)
    {
        action?.Invoke();
    }
    public override void PreExit(Action action)
     {
        action?.Invoke();
     }
        public abstract void OnEnable();
    }
    public abstract class View : IView 
    {
        public bool isLoad { get ; set; }

        public abstract void Exit();

        public abstract UniTask Init();
        public abstract  void  PreInit(Action action);
            public abstract void PreExit(Action action);

}
    public interface IView
    {

        bool isLoad { get; set;}
        UniTask Init();
        void Exit();
    }
    [AttributeUsage(AttributeTargets.Class)]
    public class RoutedAttribute : Attribute
    {
        public string path;
        public string sceneName;
        public LoadType LoadType;
        public LoadSceneMode LoadSceneMode;
        public RoutedAttribute(string path="/",string sceneName="_",LoadType loadType= LoadType.Builing, LoadSceneMode loadSceneMode=LoadSceneMode.Single)
        {
            this.path = path;
            this.sceneName = sceneName;
            LoadType = loadType;
            LoadSceneMode = loadSceneMode;
        }
    }
    
    public struct SceneInfo
    {
        public LoadType loadType;      
        public string Name;
        public LoadSceneParameters LoadSceneParameters;
        public SceneInfo(string name,   LoadSceneParameters loadSceneParameters, LoadType loadType = LoadType.Builing)
        {
            this.loadType = loadType;
            Name = name;
            LoadSceneParameters = loadSceneParameters;
        }

        public SceneInfo(string name, LoadType loadType=LoadType.Builing)
        {
            this.loadType = loadType;
            Name = name;
            LoadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single, LocalPhysicsMode.Physics3D);            
        }
    }
    public enum LoadType
    {
        Builing,
        Addressable
    }
}