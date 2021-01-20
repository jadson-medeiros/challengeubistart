using System.Collections.Generic;
using ChallengeUbistart.Business.Notifications;

namespace ChallengeUbistart.Business.Intefaces
{
    public interface INotify
    {
        bool HasNotification();
        List<Notification> GetNotifications();
        void Handle(Notification notification);
    }
}