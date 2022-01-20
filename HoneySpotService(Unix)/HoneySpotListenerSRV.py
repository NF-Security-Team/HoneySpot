import socket
import sys
from datetime import datetime

# Creating our rcvd_data array to store all our clients lines of commands
rcvd_data = []

# Create a TCP/IP socket
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
BUFFER_SIZE = 1024 * 128

# Bind the socket to the port
server_address = ('0.0.0.0', 6857)
print('Starting up HoneySpotService on %s port %s' % server_address)
sock.bind(server_address)

# Listen for incoming connections
sock.listen(1)

while True:
	# Wait for a connection
	print('Waiting for a connection...')
	connection, client_address = sock.accept()
	# Creating our empty buffer stream
	rcvd_line = ""

	try:
		client_ip = client_address[0]
		client_src_port = str(client_address[1])
		print('Connection from ' + client_ip + ':' + client_src_port + " received...")

		# Receive the data in small chunks
		while "\n" not in rcvd_line:
			# receive the chars from the server
			command = connection.recv(BUFFER_SIZE).decode()
			splitted_command = command.split()

			# Save current time to variabile for later store in our rcvd_data array
			curr_date = datetime.now()
			curr_date = curr_date.strftime("%d/%m/%Y %H:%M:%S")

			# Appending each char to our received buffer stream (as String)
			rcvd_line = rcvd_line + command
			
		# Removing "\n" linebreak from our received line
		rcvd_line = rcvd_line.strip("\n") 
		# Save received data to rcvd_data array
		rcvd_data.append(curr_date + "," + client_ip + "," + rcvd_line)
		print(rcvd_data[-1])
		connection.close()
	finally:
		# Clean up the connection
		connection.close()