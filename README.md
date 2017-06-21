# WebPush.NetCore
[![Travis](https://travis-ci.org/vip30/WebPush-NetCore.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/v/WebPush-NetCore.svg)](https://www.nuget.org/packages/WebPush-NetCore/)

A Brief Intro
-------------------

WebPush .NetCore version

[Original repo](https://github.com/web-push-libs/web-push-csharp)

[Visit Nuget](https://www.nuget.org/packages/WebPush-NetCore)


Usage
-------------------
Same as the originial version of web-push-csharp
```cs
using WebPush;

var pushEndpoint = @"https://fcm.googleapis.com/fcm/send/efz_TLX_rLU:APA91bE6U0iybLYvv0F3mf6uDLB6....";
var p256dh = @"BKK18ZjtENC4jdhAAg9OfJacySQiDVcXMamy3SKKy7FwJcI5E0DKO9v4V2Pb8NnAPN4EVdmhO............";
var auth = @"fkJatBBEl...............";

var subject = @"mailto:example@example.com";
var publicKey = @"BDjASz8kkVBQJgWcD05uX3VxIs_gSHyuS023jnBoHBgUbg8zIJvTSQytR8MP4Z3-kzcGNVnM...............";
var privateKey = @"mryM-krWj_6IsIMGsd8wNFXGBxnx...............";

var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
//var gcmAPIKey = @"[your key here]";

var webPushClient = new WebPushClient();
try
{
	webPushClient.SendNotification(subscription, "payload", vapidDetails);
    //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
}
catch (WebPushException exception)
{
	Console.WriteLine("Http STATUS code" + exception.StatusCode);
}
```

# Credits
- Ported from https://github.com/web-push-libs/web-push-csharp.
