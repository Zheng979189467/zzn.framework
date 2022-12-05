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
        foreach (var item in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (item.IsSubclassOf(typeof(ViewModel)))
            {
                builder.Register(item, Lifetime.Singleton).As<ViewModel,IStartable>().AsSelf();
            }
        }
    }
    public Dictionary<string, ViewPanel> panels;
    public T GetUIView<T>() where T : ViewPanel
    {
        if (panels == null)
        {
            panels = new Dictionary<string, ViewPanel>();
            return null;
        }
        else if(panels.TryGetValue(typeof(T).Name, out var panel))
        {
            return panel as T;
        }
        return null;
    }
    public T OpenUIView<T>(UILevel uILevel=UILevel.Common) where T : ViewPanel
    {
        var panel = GetUIView<T>();
        if (panel!=null)
        {
            panel.Show();
            return panel;
        }
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
        res.name = typeof(T).Name;
       parent.Container.Inject(res);
        if (panels == null)
        {
            panels = new Dictionary<string, ViewPanel>();
        }
        panels.Add(res.name, res);
        return res;
    }

    internal void Close(ViewPanel viewPanel)
    {
        if (panels.ContainsValue(viewPanel))
            panels.Remove(viewPanel.name);
        UnityEngine.GameObject.Destroy(viewPanel.gameObject);
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
public abstract class ViewPanel: Panel
{
    [Inject]
    public UIServer uIServer;
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Discharge()
    {
        uIServer.Close(this);
    }
}

public abstract class Panel : MonoBehaviour, IPanel
{
   
}

public interface IPanel
{
}