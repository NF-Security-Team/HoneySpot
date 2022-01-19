# HoneySpot
A complete system to deploy functional Honeypots to all infrastructures that needs to be notified when something anomalous occur.

TODO:
++ Create C# / C++ Windows & Linux Services that listen to a specific port or port range giving in output a file containing information about detected network traffic <br>
++ Create Check_MK Plugin to read files that will be outputted by Honeyport services <br>
++ Combine Services + Check_MK Plugins Variables to have a "Ready to deploy" system <br>
++ Add your ideas <br>

# Tools Decription
1) SYMTOOL.ps1 --> Creates some smart defenses against ransomware attacks with Symlinks and Dummy Files (Filled with fake private data) <br>
2) SR_HoneySpotter.ps1 --> Main Plugin that enables your Check_MK console to monitor the deployed honeypot state <br>
3) Windows-Service --> Main HoneyPot Service, on the configuration file you can setup whitelisted IPs, ports you want it to listen to and the number of packets required to trigger an alert; in the near future it will be possible to parse all the packets received with a custom Signature list to identify which type of datastream it is detecting.
