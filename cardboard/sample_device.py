import socket

def main():
    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    print socket.gethostname()
    s.bind((socket.gethostname(), 6612))
    s.listen(1)
    (client, address) = s.accept()
    while True:
        gesture = client.recv(1)
        print gesture

if __name__ == "__main__":
    main()
