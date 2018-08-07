using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Threading;

public class Client {

    private TcpClient tcpclient;

    public Client(String ip,int port){
        Console.WriteLine("Client Start");
        tcpclient = new TcpClient();
        try{
            Console.WriteLine("Connecting.....");
            tcpclient.Connect(ip,port);
            Console.WriteLine("Connected");
            Start();
        }catch (Exception e) {
            Console.WriteLine("Client Error..... " + e.StackTrace);
        }
    }

    public void Write(){
        try{
            while(true){
                Console.WriteLine("Enter the string to be transmitted : ");
                
                String str = Console.ReadLine();
                //adds 4 byte header to string
                String header = ""+str.Length;
                while(header.Length<4){
                    header = "0"+header;
                }
                str = header+str;

                Stream stm = tcpclient.GetStream();
                            
                ASCIIEncoding asen= new ASCIIEncoding();
                byte[] ba=asen.GetBytes(str);
                Console.WriteLine("Transmitting.....: " + str);
                
                stm.Write(ba,0,ba.Length);
            }
        }catch(Exception e) {
            Console.WriteLine("Write Error..... " + e.StackTrace);
        }
    }

    public void Read(){
        try{
            while(true){
                NetworkStream stm = tcpclient.GetStream();

                int x = 0;
                if(tcpclient.Available>0){
                    //Console.WriteLine("x:" + x );
                    byte[] bb=new byte[100];
                    int k=stm.Read(bb,0,100);
                    String read = "";
                    for (int i=0;i<k;i++)
                        read = read + Convert.ToChar(bb[i]);
                    Console.WriteLine("Server responded with....");
                    Console.Write(read+"\n"); 
                    x++;             
                }
            }
            //return 0;
        }catch(Exception e) {
            Console.WriteLine("Read Error..... " + e.StackTrace);
            //return -1;
        }
    }

    public void Start(){
        Thread readThread = new Thread(new ThreadStart(Read));
        Thread writeThread = new Thread(new ThreadStart(Write));
        readThread.Start();
        writeThread.Start();
        
    }

    public void Close(){
        tcpclient.Close();
    }


    public static void Main() {
        Client c1 = new Client("127.0.0.1",8001);
    }

}
