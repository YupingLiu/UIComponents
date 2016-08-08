using UnityEngine;
public class PushService
{
    public static void Init()
    {
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            CleanNotification();
            NotificationMessage("长官，补给时间到了！马上回到战场领取体力吧！", 12, true);
            NotificationMessage("长官，补给时间到了！马上回到战场领取体力吧！", 18, true);
        }
#endif
    }

#if UNITY_IPHONE
    private static void NotificationMessage(string message, int hour, bool isRepeatDay)
    {
        int year = System.DateTime.Now.Year;
        int month = System.DateTime.Now.Month;
        int day = System.DateTime.Now.Day;
        System.DateTime newDate = new System.DateTime(year, month, day, hour, 0, 0);
        NotificationMessage(message, newDate, isRepeatDay);
    }

    //本地推送 你可以传入一个固定的推送时间
    private static void NotificationMessage(string message, System.DateTime newDate, bool isRepeatDay)
    {
        //推送时间需要大于当前时间
        if (newDate > System.DateTime.Now)
        {
            LocalNotification localNotification = new LocalNotification();
            localNotification.fireDate = newDate;
            localNotification.alertBody = message;
            localNotification.applicationIconBadgeNumber = 1;
            localNotification.hasAction = true;
            if (isRepeatDay)
            {
                //是否每天定期循环
                localNotification.repeatCalendar = CalendarIdentifier.ChineseCalendar;
                localNotification.repeatInterval = CalendarUnit.Day;
            }
            localNotification.soundName = LocalNotification.defaultSoundName;
            NotificationServices.ScheduleLocalNotification(localNotification);
        }
    }

    //清空所有本地消息
    private static void CleanNotification()
    {
        LocalNotification l = new LocalNotification();
        l.applicationIconBadgeNumber = -1;
        NotificationServices.PresentLocalNotificationNow(l);
        NotificationServices.CancelAllLocalNotifications();
        NotificationServices.ClearLocalNotifications();
        NotificationServices.RegisterForLocalNotificationTypes(LocalNotificationType.Badge);
        NotificationServices.RegisterForLocalNotificationTypes(LocalNotificationType.Sound);
        NotificationServices.RegisterForLocalNotificationTypes(LocalNotificationType.Alert);
    }
#endif
}
