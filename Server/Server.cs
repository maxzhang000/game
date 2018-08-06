using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server{
    
    private TcpListener myListener;
    private Socket[] sockets;
    private Thread[] threads;//NEED TO REMOVE FINISHED THREADS FROM ARRAY
    private MessageBoard messages;

    public Server(String ip, int port){
        Console.WriteLine("Server Start");
        IPAddress ipAd = IPAddress.Parse("127.0.0.1");
        sockets = new Socket[0];
        threads = new Thread[0];
        messages = new MessageBoard();
        try{
            myListener = new TcpListener(ipAd,8001);
            
            myListener.Start();

            Console.WriteLine("The server is running at port 8001...");    
            Console.WriteLine("The local End point is  :" + myListener.LocalEndpoint );
            Console.WriteLine("Waiting for a connection.....");
        }catch(Exception e) {
            Console.WriteLine("Server Error..... " + e.StackTrace);
        }
    }

    public void Listen(){
        while(true){
            Socket newSocket = myListener.AcceptSocket();
            Console.WriteLine("Connection accepted from " + newSocket.RemoteEndPoint);
            Thread newThread = new Thread(() => this.Start(newSocket));
            Array.Resize(ref sockets, sockets.Length + 1);
            sockets[sockets.Length - 1] = newSocket;
            Array.Resize(ref threads, threads.Length + 1);
            threads[threads.Length - 1] = newThread;
            try {
                newThread.Start();
            }
            catch (ThreadStateException te) {
                Console.WriteLine(te.ToString() );
            }                
        }

    }


//1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij1234567890abcdefghij
    public String Read(Socket sock){
        String fullReadInString = "";
        try {
            
            byte[] header = new byte[4];
            sock.Receive(header);
            String leadingBytes = "";
            for (int i=0;i<header.Length;i++){
                leadingBytes+= Convert.ToChar(header[i]);
            }
            int readSize = int.Parse(leadingBytes);
            Console.WriteLine("Incoming " + readSize + " bytes ...");
            
            byte[] b = new byte[100];
            int k;
            int blocks = 0;
            do{
                k=sock.Receive(b);
                Console.WriteLine("Recieved" + k + " bytes ...");
                String recievedStr = "";
                for (int i=0;i<k;i++)
                    recievedStr = recievedStr + Convert.ToChar(b[i]);
                Console.WriteLine(recievedStr);
                fullReadInString = fullReadInString + recievedStr;
                blocks++;
            }while(blocks*b.Length < readSize);
            Console.WriteLine("Complete message: "+fullReadInString);
            return fullReadInString;
        }catch(Exception e){
            Console.WriteLine("Read Error..... " + e.StackTrace);
            return "READERROR";
        }

    }

    public String Write(Socket sock, String s){
        try{
            ASCIIEncoding asen=new ASCIIEncoding();
            sock.Send(asen.GetBytes(s));
            Console.WriteLine("\nSent Acknowledgement");
            return s;
        }catch(Exception e){
            Console.WriteLine("Read Error..... " + e.StackTrace);
            return "WRITEERROR";
        }
    }

    public void Close(Socket sock){
        //Array.remove(sockets,sock);
        sock.Close();
    }

    public void Start(Socket sock){
        bool reading = true;
        while(reading){
            String a = Read(sock);
            if (a.Equals("STOP") || a.Equals("READERROR")){
                reading = false;
            }else{
                messages.update(a);
                Write(sock,messages.output());
            }
            Console.WriteLine("Current Messages on Board:" + messages.output());
        }
        Close(sock);
    }

    public static void Main(){
        Server s = new Server("127.0.0.1",8001);
        Thread listenThread = new Thread(new ThreadStart(s.Listen));
        listenThread.Start();
    }
}
