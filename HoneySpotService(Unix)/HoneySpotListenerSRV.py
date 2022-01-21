import socket
import sys
from datetime import datetime
import time
from threading import Thread
import multiprocessing

def on_new_client(connection, client_address, rcvd_data, all_data):
	while True:
		try:
			BUFFER_SIZE = 1024 * 128
			client_ip = client_address[0]
			client_src_port = str(client_address[1])
			#print('Connection from ' + client_ip + ':' + client_src_port + " received...") # debug
			# Creating our empty buffer stream
			rcvd_line = ""
			# Receive the data in small chunks
			while "\n" and "\r" not in rcvd_line:
				# Check if the session timeout has been reached
				#Set a timeout for socket connection
				timeout = time.time() + 5
				if time.time() > timeout:
				#if 1 == 2:
					# receive the chars from the server
					command = connection.recv(BUFFER_SIZE).decode()
					if not command:
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
			# Save received data to rcvd_data array
			all_data.append(curr_date + "," + client_ip + "," + rcvd_line)
			#print(all_data[-1]) # debug
			#print('Waiting for connections...') #debug
			if connection:
				connection.close()
			break
		except socket.error as err:
			#print('Probably the socket has timed out...' + str(err)) # debug
			break
		finally:
			# Clean up the connection
			if connection:
				connection.close()
			break
	if connection:
		connection.close()

def HoneySpotListener(all_data):
	# Creating our rcvd_data array to temporary store all our clients lines of commands
	rcvd_data = []

	# Create a TCP/IP socket
	sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

	# Bind the socket to the port
	server_address = ('0.0.0.0', 6857)
	#print('Starting up HoneySpotService on %s port %s' % server_address) # debug
	sock.bind(server_address)

	# Listen for incoming connections
	sock.listen(1)
	sock.settimeout(5)

	# Listen for incoming connection for 60 seconds
	reset_time = time.time() + 20

	while time.time()<reset_time:
		try:
			# Wait for connections
			connection, client_address = sock.accept()
			t = Thread(target=on_new_client,args=(connection,client_address,rcvd_data,all_data))
			t.start()
		except socket.error as e:
			# If there's a client connected timed out, close the connection. Otherwise continue
			try:
				connection.close()
			except:
				continue
			continue


# Main exectution starting...
# Our collected data as "archive"
global all_data
all_data = []

# Starting our Listener, it will execute for 60 secs and then it will stop by itself. So on... 
while True:
	#print("Listener Starting...") # debug
	#print('Waiting for connections...') # debug
	HoneySpotListener(all_data)
	#print("Listener Stopping...") # debug
	#print(all_data) # debug
