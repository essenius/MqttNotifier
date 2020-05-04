# MqttNotifier
Show notifications in Windows taskbar by MQTT topic subscription

This utility shows a notification in the Windows task bar for incoming MQTT messages. 
It can e.g. be used to get alerts from home automation installations.

Configuration is done via the standard .Net config file. Parameters:

|Category|Parameter|Description|Default|
+--------+---------+-----------+-------+
|Broker  |MqttBroker|MQTT broker URL|mqtt|

|MqttPort|MQTT broker port|  1883 for tcp, 8883 for ssl
|ClientId|unique client ID| new GUID
|Topic|Base topic to subscribe to|alert/#
|QoS|MQTT QoS (0, 1, 2)|2|
|SSL|CaCertificateFile|file path of the CA certificate|null
|ClientCertificateFile| client certificate|null
Credentials|CredentialsTarget|Target in the Credential Store|null
Notification|AlertDisplayTime|Time the alert stays visible|5000 (ms)
|DefaultMessageType|Type of message: Info, Warning, Error|Warning
|DefaultTitle|Title shown when the topic doesn't hold it|MQTT
|IconFile|Icon shown in the taskbar|default.ico
