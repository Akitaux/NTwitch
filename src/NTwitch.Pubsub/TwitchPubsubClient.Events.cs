﻿using System;
using System.Threading.Tasks;

namespace NTwitch.Pubsub
{
    public partial class TwitchPubsubClient
    {
        private readonly AsyncEvent<Func<Task>> _readyEvent = new AsyncEvent<Func<Task>>();
        public event Func<Task> Ready
        {
            add { _readyEvent.Add(value); }
            remove { _readyEvent.Remove(value); }
        }

        private readonly AsyncEvent<Func<StreamStatusEventArgs, Task>> _streamOnlineEvent = new AsyncEvent<Func<StreamStatusEventArgs, Task>>();
        public event Func<StreamStatusEventArgs, Task> StreamOnline
        {
            add { _streamOnlineEvent.Add(value); }
            remove { _streamOnlineEvent.Remove(value); }
        }

        private readonly AsyncEvent<Func<StreamStatusEventArgs, Task>> _streamOfflineEvent = new AsyncEvent<Func<StreamStatusEventArgs, Task>>();
        public event Func<StreamStatusEventArgs, Task> StreamOffline
        {
            add { _streamOfflineEvent.Add(value); }
            remove { _streamOfflineEvent.Remove(value); }
        }

        private readonly AsyncEvent<Func<WhisperReceivedEventArgs, Task>> _whisperReceivedEvent = new AsyncEvent<Func<WhisperReceivedEventArgs, Task>>();
        public event Func<WhisperReceivedEventArgs, Task> WhisperReceived
        {
            add { _whisperReceivedEvent.Add(value); }
            remove { _whisperReceivedEvent.Remove(value); }
        }
    }
}
