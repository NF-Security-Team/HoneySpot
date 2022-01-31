###########################################################################################################################
#!/bin/bash
#
# Check MK Unix Local Script
# Honeypot state Checker
# Via Bash it retrives the HoneySpotter_HONEYPOTNAME state reading a txt file
# Checks if any honeypot triggered applies a $Warning or $Critical state status on CheckMK
# Author: Nicolas Fasolo
#
##########################################################################################################################

exec 2>>/usr/lib/check_mk_agent/HoneySpot/HoneySpotter_HONEYPOTNAME.ErrorLog

value=$(</usr/lib/check_mk_agent/HoneySpot/HoneySpotter_HONEYPOTNAME.CurrState)


if [[ $string == *"OK"* ]]; then
  echo "0 \"HoneySpotter-HONEYPOTNAME\" - Nothing is happening... you can sleep well :) OK"
fi

if [[ $string == *"WARNING"* ]]; then
  echo "1 \"HoneySpotter-HONEYPOTNAME\" -  Anomalous traffic detected! WARNING"
fi

if [[ $string == *"CRIT"* ]]; then
  echo "2 \"HoneySpotter-HONEYPOTNAME\" - INCIDENT TRIGGERED! CRITICAL!"
fi
