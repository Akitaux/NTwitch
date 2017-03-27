﻿using Model = NTwitch.Rest.API.UserNotifications;

namespace NTwitch.Rest
{
    public class RestUserNotifications
    {
        /// <summary> True if this user receives push notifications </summary>
        public bool IsPushEnabled { get; private set; }
        /// <summary> True if this user receives email notifications </summary>
        public bool IsEmailEnabled { get; private set; }

        internal RestUserNotifications() { }

        internal static RestUserNotifications Create(Model model)
        {
            var entity = new RestUserNotifications();
            entity.Update(model);
            return entity;
        }

        internal virtual void Update(Model model)
        {
            IsPushEnabled = model.IsPushEnabled;
            IsEmailEnabled = model.IsEmailEnabled;
        }
    }
}
