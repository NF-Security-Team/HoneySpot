import socket
import sys
from datetime import datetime
import time
from threading import Thread
import multiprocessing
import os
import logging

def on_new_client(connection, client_address, rcvd_data, all_data,whitelisted_ips,new_received_lines):
	while True:
		try:
			BUFFER_SIZE = 1024 * 128
			client_ip = client_address[0]
			client_src_port = str(client_address[1])
			logging.info("Connection from " + client_ip + ":" + client_src_port + " received..." + f"\n")
			# Creating our empty buffer stream
			rcvd_line = ""
			# Receive the data in small chunks
			while "\n" and "\r" not in rcvd_line:
				# Check if the session timeout has been reached
				# Set a timeout of 5 seconds for socket connection
				timeout = time.time() + 5
				if time.time() > timeout:
					# receive the chars from the server
					command = connection.recv(BUFFER_SIZE).decode()
					if not command:
						logging.warn("Timeout reached. Closing connection with client..." + f"\n")
						rcvd_line = "ERR - Connection Timeout" # debug
						break
				else:
					# receive the chars from the server
					command = connection.recv(BUFFER_SIZE).decode()
					splitted_command = command.split()

					# Save current time to variabile for later store in our rcvd_data array
					curr_date = datetime.now()
					curr_date = curr_date.strftime("%d/%m/%Y %H:%M:%S")

					# Appending each char to our received buffer stream (as String)
					rcvd_line = rcvd_line + command
			# Removing "\n" and "\r" linebreak from our received line
			rcvd_line = rcvd_line.strip("\n")
			rcvd_line = rcvd_line.strip("\r")
			# Save received data to rcvd_data array if src ip is not present in whitelist_db
			if client_ip not in whitelisted_ips:
				all_data.append(curr_date + "," + client_ip + "," + rcvd_line + f"\n")
				new_received_lines[0] = new_received_lines[0] + 1
				logging.info("Received line. It will be written to sniff file..." + f"\n")
				logging.debug(curr_date + "," + client_ip + "," + rcvd_line + f"\n")
			else:
				logging.debug("src_ip: " + client_ip + " is present in whitelist_db." + f"\n" + "It won't be written to sniff_file..." + f"\n")
			#print(all_data[-1]) # debug
			#print('Waiting for connections...') #debug
			if connection:
				connection.close()
			break
		except socket.error as err:
			logging.error("Probably the socket has timed out..." + str(err))
			#print('Probably the socket has timed out...' + str(err)) # debug
			break
		finally:
			# Clean up the connection
			if connection:
				connection.close()
			break
	if connection:
		connection.close()

def HoneySpotListener(all_data, new_received_lines, CheckState_path):
	# Creating our rcvd_data array to temporary store all our clients lines of commands
	rcvd_data = []

	# Create a TCP/IP socket
	sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

	# Bind the socket to the port
	server_address = ('0.0.0.0', 6857)
	listening_port = str(server_address[1])
	# Setup our CheckState and if we are at the 1st session set it as "OK - OK STATE"
	CheckState_path[0] = "./"
	CheckState_path[0] = CheckState_path[0] + "HoneySpotter_" + listening_port + ".CurrState"
	if counter == 0:
		try:
			f = open(CheckState_path[0],"w")
			f.write("OK - OK STATE")
			f.close()
		except:
			logging.error("Couldn't write CheckState file " + CheckState_path[0])
	#print('Starting up HoneySpotService on %s port %s' % server_address) # debug
	sock.bind(server_address)

	# Listen for incoming connections
	logging.info("Starting listener on port " + listening_port + "...")
	sock.listen(1)
	sock.settimeout(5)

	# Listen for incoming connection for 60 seconds
	reset_time = time.time() + 60

	new_received_lines[0]=0

	while time.time()<reset_time:
		try:
			# Read whitelisted_ips
			try:
				f = open(WL_path, "r")
				whitelisted_ips = f.readlines()
				f.close()
			except:
				logging.error("Couldn't read " + WL_path + " file")
				whitelisted_ips = []
			# Wait for connections
			connection, client_address = sock.accept()
			t = Thread(target=on_new_client,args=(connection,client_address,rcvd_data,all_data,whitelisted_ips,new_received_lines))
			t.start()
		except socket.error as e:
			# If there's a client connected timed out, close the connection. Otherwise continue
			try:
				connection.close()
			except:
				continue
			continue
	# After a listening session of N seconds, append new received data to sniff file
	

# Main execution starting...

# Setup a logger for our service
logging.basicConfig(filename='HoneySpotListenerSRV.log', format='%(levelname)s %(threadName)s %(message)s', level=logging.DEBUG)

# Defining some vars
#global CheckState_path
Logger_Folder="./Logger"
WL_path = "./Logger/WhiteList.db"
sniff_path = "./Logger/sniff"
global new_received_lines
new_received_lines = []
new_received_lines.append(0)
global CheckState_path
CheckState_path = []
CheckState_path.append("")

# Our collected data as "archive" and every listening loop received data array
global all_data
all_data = []
new_data = []

# Creating needed folders

if not os.path.exists(Logger_Folder):
	try:
		os.makedirs(Logger_Folder)
	except:
		logging.error("Error while creating " + Logger_Folder + " directory...")

if not os.path.exists(sniff_path):
	try:
		f = open(sniff_path, "w")
		f.write("")
		f.close()
	except:
		logging.error("Error while opening " + sniff_path)

if not os.path.exists(WL_path):
	try:
		f = open(WL_path, "w")
		f.write("")
		f.close()
	except:
		logging.error("Error while opening " + WL_path)



# Starting our Listener, it will execute for 60 secs and then it will stop by itself. So on... 
while True:
	# Using a counter to make our HoneySpot run 30 times (30 minutes), then resets CheckState
	counter = 0
	while counter < 30:
		HoneySpotListener(all_data, new_received_lines, CheckState_path)
		# Creating a new array with only newly received data
		if new_received_lines[0] != 0:
			new_data = []
			i = 1
			while i <= new_received_lines[0]:
				new_data.append(all_data[-i])
				i = i+1
			# append newly received data to sniff_file
			try:
				f = open(sniff_path, "a")
				for x in new_data:
					f.write(x)
				f.close()
			except:
				logging.error("Couldn't write " + sniff_path)
			# set CheckState as CRITICAL - CRIT STATE 
			try:
				f = open(CheckState_path[0], "w")
				content = "Sniff Path: " + sniff_path + " -- Received Traffic: " + new_data[0] + f"\n"
				f.write("CRITICAL - CRIT STATE -" + content)
				f.close()
			except:
				logging.error("Error while opening " + CheckState_path[0] + f"\n")
		counter = counter + 1
