#================= SETTINGS =================#
#Number of Dummy Files per Directory (Default:10)
$NumberOfFiles = 10;
#Dummy Files Content (Default:**Some Sensitive Random Data) --> Generated with https://fauxid.com/fake-name-generator
$FilesContent = 
"
Name: Cira Damico
Address: Rotonda Morgana 7
Sesto Miriam nell'emilia, 64093 Vercelli (LI) Italy
Latitude & longitude: 47.7632, -110.582772
Phone: 095 265 6759
Social Security Number: 123-34-45678

Codice Fiscale:
PUMEXH85R96Y622H
Partita Iva:
IT27003540120


Financial & Banking Numbers
Credit Card Number: 5245-1044-5105-2967 Visa
Exp Date: 12/22 CVV: 318

Bank: Unicredit
Bank Account Number: IT08W5065519281GIH446OM01DA
Routing Number: 101100964
IBAN: IT16G1590405827KI869NDJ0AO2 Italy

Cryptocurrency Addresses
Bitcoin Address: Bitcoin 14kuu513A6QBn8KaL7QZtqoxXPXjAk32zo
Ethereum Address: Ethereum 0xbd6F73738f848b51D0d5EC2E8bD3792EE01d7b0e
Ripple Address: Ripple rPijhF5rwdUTV2G33hXWFRmXHjabiQukd1
Monero Address: Monero 49ckEdfJSD6U8S4gQaLm6r2nzJJdo3Ba3VDaWrNFmDhN1k8FCjB5PRg6T6jHSdGt3v9K6kddzXAcggZpwT3yr1xXQWogYqd

#Have fun my firend :)
";
#================= END SETTINGS =================#

#Get RootDrive
$RootDrive = Get-PSDrive | select -Property Root;
if($RootDrive -like "*C:\*")
{
 $RootDrive = "C:\";
} 
#Get UserSpace & Appdata
$UserSpace = -join($env:USERPROFILE + "\");
$UserAppdata = -join($env:APPDATA + "\");

#================= DRIVE SECTION ==============#
#Create "A" n "Z" baseDirs in "C:\" drive
$A_TopDir = ( -join($RootDrive + "A_Symlink")); 
$Z_LowDir = ( -join($RootDrive + "Z_Symlink"));

New-Item -Path $RootDrive -Name "A_Symlink" -ItemType Directory;
New-Item -Path $RootDrive -Name "Z_Symlink" -ItemType Directory;

#Create Relative Symlinks
$A_TopDir_SymLink = ( -join($A_TopDir + "\A_Symlink_SymDecoy")); 
$Z_LowDir_Symlink = ( -join($Z_LowDir + "\Z_Symlink_SymDecoy"));

New-Item -Path $A_TopDir_SymLink -ItemType SymbolicLink -Value $Z_LowDir;
New-Item -Path $Z_LowDir_Symlink -ItemType SymbolicLink -Value $A_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($A_TopDir + "\BankData.txt")
    $file2 = -join($Z_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}

#Create "0" n "9" baseDirs in "C:\" drive
$0_TopDir = ( -join($RootDrive + "0_Symlink")); 
$9_LowDir = ( -join($RootDrive + "9_Symlink"));

New-Item -Path $RootDrive -Name "0_Symlink" -ItemType Directory;
New-Item -Path $RootDrive -Name "9_Symlink" -ItemType Directory;

#Create Relative Symlinks
$0_TopDir_SymLink = ( -join($0_TopDir + "\0_Symlink_SymDecoy")); 
$9_LowDir_Symlink = ( -join($9_LowDir + "\9_Symlink_SymDecoy"));

New-Item -Path $0_TopDir_SymLink -ItemType SymbolicLink -Value $9_LowDir;
New-Item -Path $9_LowDir_Symlink -ItemType SymbolicLink -Value $0_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($0_TopDir + "\BankData.txt")
    $file2 = -join($9_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}
#================= USERSPACE SECTION ==============#
#Create "A" n "Z" baseDirs in "%userprofile%"
$A_TopDir = ( -join($UserSpace + "A_Symlink")); 
$Z_LowDir = ( -join($UserSpace + "Z_Symlink"));

New-Item -Path $UserSpace -Name "A_Symlink" -ItemType Directory;
New-Item -Path $UserSpace -Name "Z_Symlink" -ItemType Directory;

#Create Relative Symlinks
$A_TopDir_SymLink = ( -join($A_TopDir + "\A_Symlink_SymDecoy")); 
$Z_LowDir_Symlink = ( -join($Z_LowDir + "\Z_Symlink_SymDecoy"));

New-Item -Path $A_TopDir_SymLink -ItemType SymbolicLink -Value $Z_LowDir;
New-Item -Path $Z_LowDir_Symlink -ItemType SymbolicLink -Value $A_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($A_TopDir + "\BankData.txt")
    $file2 = -join($Z_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}

#Create "0" n "9" baseDirs in "%userprofile%" drive
$0_TopDir = ( -join($UserSpace + "0_Symlink")); 
$9_LowDir = ( -join($UserSpace + "9_Symlink"));

New-Item -Path $UserSpace -Name "0_Symlink" -ItemType Directory;
New-Item -Path $UserSpace -Name "9_Symlink" -ItemType Directory;

#Create Relative Symlinks
$0_TopDir_SymLink = ( -join($0_TopDir + "\0_Symlink_SymDecoy")); 
$9_LowDir_Symlink = ( -join($9_LowDir + "\9_Symlink_SymDecoy"));

New-Item -Path $0_TopDir_SymLink -ItemType SymbolicLink -Value $9_LowDir;
New-Item -Path $9_LowDir_Symlink -ItemType SymbolicLink -Value $0_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($0_TopDir + "\BankData.txt")
    $file2 = -join($9_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}

#================= USERSPACE SECTION ==============#
#Create "A" n "Z" baseDirs in "%appdata%"
$A_TopDir = ( -join($UserAppdata + "A_Symlink")); 
$Z_LowDir = ( -join($UserAppdata + "Z_Symlink"));

New-Item -Path $UserAppdata -Name "A_Symlink" -ItemType Directory;
New-Item -Path $UserAppdata -Name "Z_Symlink" -ItemType Directory;

#Create Relative Symlinks
$A_TopDir_SymLink = ( -join($A_TopDir + "\A_Symlink_SymDecoy")); 
$Z_LowDir_Symlink = ( -join($Z_LowDir + "\Z_Symlink_SymDecoy"));

New-Item -Path $A_TopDir_SymLink -ItemType SymbolicLink -Value $Z_LowDir;
New-Item -Path $Z_LowDir_Symlink -ItemType SymbolicLink -Value $A_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($A_TopDir + "\BankData.txt")
    $file2 = -join($Z_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}

#Create "0" n "9" baseDirs in "%appdata%" drive
$0_TopDir = ( -join($UserAppdata + "0_Symlink")); 
$9_LowDir = ( -join($UserAppdata + "9_Symlink"));

New-Item -Path $UserAppdata -Name "0_Symlink" -ItemType Directory;
New-Item -Path $UserAppdata -Name "9_Symlink" -ItemType Directory;

#Create Relative Symlinks
$0_TopDir_SymLink = ( -join($0_TopDir + "\0_Symlink_SymDecoy")); 
$9_LowDir_Symlink = ( -join($9_LowDir + "\9_Symlink_SymDecoy"));

New-Item -Path $0_TopDir_SymLink -ItemType SymbolicLink -Value $9_LowDir;
New-Item -Path $9_LowDir_Symlink -ItemType SymbolicLink -Value $0_TopDir;

#Create Dummy Files to let the Malware start to enumerate something (Directory Skip Prevention)
For ($i=0; $i -le $NumberOfFiles; $i++) 
{
    $file1 = -join($0_TopDir + "\BankData.txt")
    $file2 = -join($9_TopDir + "\BankData.txt")
    New-Item $file1 -ItemType File -Value $FilesContent;
    New-Item $file2 -ItemType File -Value $FilesContent;
}

#TODO: Create Progammatically Some Canary Files around the system

#TODO: Anything that inspires you 