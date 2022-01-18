###########################################################################################################################
#
# Check MK Local Script
# Honeypot state Checker
# Via Powershell it retrive the alyvix state reading a txt file
# Checks if any honeypot triggered applies a $Warning or $Critical state status on CheckMK
# Author: Nicolas Fasolo
#
##########################################################################################################################

#SET Threshold 
$Critical = "CRIT"
$Warning = "WARNING"
$OK = "OK"

#### Code Start

	#The file where it needs to be written current state be sure to replace "HONEYPOTNAME" with the actual monitored honeypot name
    $Body = & "C:\ProgramData\checkmk\agent\mrpe\HoneySpotter_HONEYPOTNAME.CurrState"
	$content = [IO.File]::ReadAllText($Body)

    if($content -contains $Critical){
        #check_mk output
        Write-Host "<<<local>>>"
        Write-Host "2 "HoneySpotter-HONEYPOTNAME" - CRITICAL: $content"
    }
    elseif ($content -contains $Warning){
        #check_mk output
        Write-Host "<<<local>>>"
        Write-Host "1 "HoneySpotter-HONEYPOTNAME" - WARNING: $content"
    }
	elseif ($content -contains $OK){
        #check_mk output
        Write-Host "<<<local>>>"
        Write-Host "0 "HoneySpotter-HONEYPOTNAME" - OK: Contained Check --> $content"
    }    
	else 
	{		
		#check_mk output
		Write-Host "<<<local>>>"
		Write-Host "0 "HoneySpotter-HONEYPOTNAME" - OK: $content"
	}
    