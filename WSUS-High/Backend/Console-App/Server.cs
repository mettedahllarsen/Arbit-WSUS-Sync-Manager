public class Server
{
	public string Name { get; }
	public string IPAddress { get; }
	public int Port { get; }
	public string Username { get; }
	public string Password { get; }

	public Server(string name, string ipAddress, int port, string username, string password)
	{
		Name = name;
		IPAddress = ipAddress;
		Port = port;
		Username = username;
		Password = password;
	}


}