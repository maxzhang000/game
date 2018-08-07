using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class Server{
    
    private TcpListener myListener;
    //NEED TO REMOVE FINISHED SOCKETS FROM ARRAY
    private Socket[] sockets;
    //NEED TO REMOVE FINISHED THREADS FROM ARRAY
    private Thread[] threads;
    private MessageBoard messages;
    private bool update;

    public Server(String ip, int port){
        Console.WriteLine("Server Start");
        IPAddress ipAd = IPAddress.Parse("127.0.0.1");
        sockets = new Socket[0];
        threads = new Thread[0];
        messages = new MessageBoard();
        update = false;
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
            update = true;
            return fullReadInString;
        }catch(Exception e){
            Console.WriteLine("Read Error..... Connection Closed" + e.StackTrace);
            Close(sock);
            return "READERROR";
        }

    }

    public String Send(Socket sock, String s){
        try{
            ASCIIEncoding asen=new ASCIIEncoding();
            /*
            String header = ""+s.Length;
            while(header.Length<4){
                header = "0" + header;
            }
            s = header + s;
            */
            sock.Send(asen.GetBytes(s));
            if (!messages.output().Equals(s)){
                Console.WriteLine("WHATTHEFUCK");
            }
            Console.WriteLine("\nSent Acknowledgement with {0} bytes:"+s,s.Length);
            return s;
        }catch(Exception e){
            Console.WriteLine("Send Error, Connection Closed..... " + e.StackTrace);
            Close(sock);
            return "SendERROR";
        }
    }

    public void SendAll(){
        while (true) {
            Thread.Sleep(2);
            if (update == true){
                for(int i = 0; i < sockets.Length; i++){
                    Send(sockets[i],messages.output());
                }
                update = false;
            }
        }
    }

    public void Close(Socket sock){
        RemoveSocket(sock);
        sock.Close();
    }

    private void RemoveSocket(Socket sock){
        for (int i = 0; i < sockets.Length; i++){
            if (sockets[i].Equals(sock)){
                Socket[] temp = new Socket[sockets.Length-1];
                for (int j = 0; j<i; j++){
                    temp[j] = sockets[j];
                }
                for (int j = i; j < sockets.Length-1; j++){
                    temp[j] = sockets[j+1];
                }
                sockets = temp;
                break;
            }
        }
    }

    public void Start(Socket sock){
        update = true;
        bool reading = true;
        while(reading){
            String a = Read(sock);
            if (a.Equals("STOP") || a.Equals("READERROR")){
                reading = false;
            }else{
                messages.update(a);
                update = true;
            }
            Console.WriteLine("Current Messages on Board:\n" + messages.output());
        }
        Close(sock);
    }

    public static void Main(){
        Server s = new Server("127.0.0.1",8001);
        Thread listenThread = new Thread(new ThreadStart(s.Listen));
        listenThread.Start();
        Thread writeThread = new Thread(new ThreadStart(s.SendAll));
        writeThread.Start();
    }
}
