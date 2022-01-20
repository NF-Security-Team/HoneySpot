# HoneySpot
A complete system to deploy functional Honeypots to all infrastructures that needs to be notified when something anomalous occur.

TODO:
++ Create C# / C++ Windows & Linux Services that listen to a specific port or port range giving in output a file containing information about detected network traffic <br>
++ Create Check_MK Plugin to read files that will be outputted by Honeyport services <br>
++ Combine Services + Check_MK Plugins Variables to have a "Ready to deploy" system <br>
++ Add your ideas <br>

# Tools Decription <br>
1) SYMTOOL.ps1 --> Creates some smart defenses against ransomware attacks with Symlinks and Dummy Files (Filled with fake private data) <br>
2) SR_HoneySpotter.ps1 --> Main Plugin that enables your Check_MK console to monitor the deployed honeypot state <br>
3) HoneySpotService --> Main HoneyPot Service, on the configuration file you can setup whitelisted IPs, ports you want it to listen to and the number of packets required to trigger an alert; in the near future it will be possible to parse all the packets received with a custom Signature list to identify which type of datastream it is detecting. <br>

# How it works? <br>
The concept is fairly simple. <br>

1) Install a Windows or Linux "HoneySpotListenerSRV" Service via command line args <br>
Windows <br>

```
HoneySpotListenerSRV.exe /install -uninstall (shorten /i and /u)
```

Linux (TODO) <br>

```
chmod +x HoneySpotListenerSRV
./HoneySpotListenerSRV -install or -uninstall (shorten /i and /u)
```

2) Install most recent Check_Mk Agent <br>
Windows <br>

```
check_mk_agent.msi
```

Linux (DEB) <br>

```
root@linux# dpkg -i check-mk-agent_X.X.XpXX-X_all.deb
```

3) Add Firewall Exclusion for Executable path (Inbound Traffic) <br>

```
netsh advfirewall firewall add rule name="HoneySpot_PORTNUMBER" dir=in action=allow program="C:\HoneySpot\HoneySpoRt_Service.exe" enable=yes
```

4) Place the Check_Mk plugins inside "Plugins" local directory 
Windows <br>

```
C:\ProgramData\checkmk\agent\plugins\
```
Linux <br>

```
root@linux# chmod +x /usr/lib/check_mk_agent/local/HoneySpoRt_%portNumber%.sh
```

5) Do a "Service Discovery" and add your New HoneySpoRt Local Services <br>
<br>

![](https://i.imgur.com/QeO7uTh.png)

<br>

6) Set Notifications to know when something falls in your TRAP!<br>
<br>

![](https://i.imgur.com/c2XMJRy.png)

<br> 
TODO REST
