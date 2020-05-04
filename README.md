# MqttNotifier
Show notifications in Windows taskbar by MQTT topic subscription

This utility shows a notification in the Windows task bar for incoming MQTT messages. 
It can e.g. be used to get alerts from home automation installations.

Configuration is done via the standard .Net config file. Parameters:

|Category|Parameter|Description|Default|
|--------|---------|-----------|-------|
|Broker  |MqttBroker|MQTT broker|mqtt|
||MqttPort|MQTT broker port|  1883 for tcp, 8883 for ssl|
||ClientId|unique client ID| new GUID|
||Topic|Base topic to subscribe to|alert/#|
||QoS|MQTT QoS (0, 1, 2)|2|
|SSL|CaCertificateFile|file path of the CA certificate|null|
||ClientCertificateFile| client certificate|null|
|Credentials|CredentialsTarget|Target in the Credential Store|null|
|Notification|AlertDisplayTime|Time the alert stays visible|5000 (ms)|
||DefaultMessageType|Type of message: Info, Warning, Error|Warning|
||DefaultTitle|Title shown when the topic doesn't hold it|MQTT|
||IconFile|Icon shown in the taskbar|default.ico|

If you want to setup a plain tcp connection in port 1883, all you need to is specify the server name.
For SSL, specify the file containing the CA Certificate (crt format) in CaCertificateFile as well.
If you need to authenticate, setup a target in the Generic section of Credential Manager, and specify that target
in CredentialsTarget.

The default topic is alert/#. Two optional parameters can be used after this base: the message type and the title.
So for example, alert/error/Office will show the error icon with the notification, and use the title Office.
# Release Notes
|Date|Note|
|---|---|
|4-May-2020|Initial release|
