﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Threading;
using UnityEngine;

namespace Cysharp.Threading.Tasks.Triggers
{
    public static partial class AsyncTriggerExtensions
    {
        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this GameObject gameObject)
        {
            return GetOrAddComponent<AsyncDestroyTrigger>(gameObject);
        }

        public static AsyncDestroyTrigger GetAsyncDestroyTrigger(this Component component)
        {
            return component.gameObject.GetAsyncDestroyTrigger();
        }
    }

    [DisallowMultipleComponent]
    public sealed class AsyncDestroyTrigger : MonoBehaviour
    {
        bool awakeCalled = false;
        bool called = false;
        CancellationTokenSource cts;

        public CancellationToken CancellationToken
        {
            get
            {
                if (cts == null)
                {
                    cts = new CancellationTokenSource();
                    if (!awakeCalled)
                    {
                        PlayerLoopHelper.AddAction(PlayerLoopTiming.Update, new AwakeMonitor(this));
                    }
                }
                return cts.Token;
            }
        }

        void Awake()
        {
            awakeCalled = true;
        }

        void OnDestroy()
        {
            called = true;

            cts?.Cancel();
            cts?.Dispose();
        }

        public UniTask OnDestroyAsync()
        {
            if (called) return UniTask.CompletedTask;

            var tcs = new UniTaskCompletionSource();

            // OnDestroy = Called Cancel.
            CancellationToken.RegisterWithoutCaptureExecutionContext(state =>
            {
                var tcs2 = (UniTaskCompletionSource)state;
                tcs2.TrySetResult();
            }, tcs);

            return tcs.Task;
        }

        class AwakeMonitor : IPlayerLoopItem
        {
            readonly AsyncDestroyTrigger trigger;

            public AwakeMonitor(AsyncDestroyTrigger trigger)
            {
                this.trigger = trigger;
            }

            public bool MoveNext()
            {
                if (trigger.called || trigger.awakeCalled) return false;
                if (trigger == null)
                {
                    trigger.OnDestroy();
                    return false;
                }
                return true;
            }
        }
    }
}

