using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;
using Network;
using SkillBridge.Message;
using UnityEngine.Events;
using Models;
using Managers;

public class StatusService : Singleton<StatusService>, IDisposable
{
    /// <summary>
    /// 状态通知与处理委托
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public delegate bool StatusNotifyHandler(NStatus status);

    Dictionary<StatusType, StatusNotifyHandler> eventMap = new Dictionary<StatusType, StatusNotifyHandler>();
    /// <summary>
    /// 解决返回角色选择后重复进入，多次添加道具的bug，下面判断action是否重复，重复则不执行
    /// </summary>
    HashSet<StatusNotifyHandler> handlers = new HashSet<StatusNotifyHandler>();

    public void Init()
    {

    }


    /// <summary>
    /// 注册状态通知消息
    /// </summary>
    /// <param name="function"></param>
    /// <param name="action"></param>
    public void RegisterStatusNofity(StatusType function, StatusNotifyHandler action)
    {
        if (handlers.Contains(action))
        {
            return;
        }
        if (!eventMap.ContainsKey(function))
        {
            eventMap[function] = action;
        }
        else
        {
            eventMap[function] += action;
        }
        handlers.Add(action);
    }

    public StatusService()
    {
        MessageDistributer.Instance.Subscribe<StatusNotify>(this.OnStatusNotify);
    }

    public void Dispose()
    {
        MessageDistributer.Instance.Unsubscribe<StatusNotify>(this.OnStatusNotify);
    }

    private void OnStatusNotify(object sender, StatusNotify notify)
    {
        foreach (NStatus status in notify.Status)
        {
            Notify(status);
        }
    }

    private void Notify(NStatus status)
    {
        Debug.LogFormat("StatusNotify:[{0}][{1}]{2}:{3}", status.Type, status.Action, status.Id, status.Value);
        if (status.Type == StatusType.Money)
        {
            if (status.Action == StatusAction.Add)
            {
                User.Instance.AddGold(status.Value);
            }
            else if (status.Action == StatusAction.Delete)
            {
                User.Instance.AddGold(-status.Value);
            }
        }
        //StatusNotifyHandler的委托变量
        StatusNotifyHandler handler;
        if (eventMap.TryGetValue(status.Type, out handler))
        {
            handler(status);
        }
    }

}
