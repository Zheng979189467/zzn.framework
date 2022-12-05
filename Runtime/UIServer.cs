using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Framework;
using VContainer;
using VContainer.Unity;
using System.ComponentModel;
using System.Reflection;
using System;
using UniRx;
public class UIServer 
{
    [Inject]
    ResourceLoadServer ResourceLoadServer;
    [Inject]
    IObjectResolver objectResolver;
    [Inject]
    LifetimeScope parent;
    static  UIRoot uIRoot;
   public static void Init(IContainerBuilder builder)
   {
        builder.Register<UIServer>(Lifetime.Singleton);
        uIRoot =GameObject.Instantiate<UIRoot>( Resources.Load<UIRoot>("UIRoot"));
        
    }
    public T OpenUIView<T>(UILevel uILevel=UILevel.Common) where T : Panel
    {
        Transform tf;
        switch (uILevel)
        {
            case UILevel.Bg:
                tf = uIRoot.BG;
                break;
            case UILevel.Common:
                tf = uIRoot.Common;
                break;
            case UILevel.Top:
                tf = uIRoot.Top;
                break;
            case UILevel.Pop:
                tf = uIRoot.Pop;
                break;
            case UILevel.Cover:
                tf = uIRoot.Cover;
                break;
            default:
                tf = uIRoot.Common;
                break;
        }
       var res=GameObject.Instantiate<T>(ResourceLoadServer.Load<T>(typeof(T).Name),tf);
       parent.Container.Inject(res); 
       return res;
    }
}
public enum UILevel
{
    Bg,
    Common,
    Top,
    Pop,
    Cover
}
public static class UIServerTool
{
   
}
public interface IViewModel
{
    
}
public abstract class SubViewModel: IViewModel
{
   
}
public abstract class ViewModel : IViewModel,IStartable
{     
   
    [Inject]
    public abstract void Start();
    
}
public abstract class ViewPanel : Panel 
{
   
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Discharge()
    {
        Destroy(gameObject);
    }
}

public abstract class Panel : MonoBehaviour, IPanel
{
  
}

public interface IPanel
{
}