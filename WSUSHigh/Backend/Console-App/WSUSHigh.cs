public class WSUSHigh
{
	private string designatedDirectory;
	private List<Server> savedServers;

	public WSUSHigh(string designatedDirectory)
	{
		this.designatedDirectory = designatedDirectory;
		this.savedServers = new List<Server>();
	}

	public void ReceiveUpdates()
	{
		if (AccessDirectory())
		{
			var updates = RetrieveUpdates();
			StoreUpdates(updates);
		}
	}

	private bool AccessDirectory()
	{
		if (Directory.Exists(designatedDirectory))
		{
			Console.WriteLine("WSUSHigh successfully accessed the designated directory.");
			return true;
		}
		else
		{
			Console.WriteLine("Failed to access the designated directory.");
			return false;
		}
	}

	private string[] RetrieveUpdates()
	{
		Console.WriteLine("WSUSHigh is retrieving updates and metadata from the designated directory.");

		// Get all files in the directory
		string[] updates = Directory.GetFiles(designatedDirectory);

		// Print out the names of the updates for debugging purposes
		foreach (string update in updates)
		{
			Console.WriteLine($"Retrieved update: {Path.GetFileName(update)}");
		}

		return updates;
	}

	private void StoreUpdates(string[] updates)
	{
		Console.WriteLine("WSUSHigh is storing updates and metadata for distribution.");

		// Define the directory where the updates will be stored
		string storageDirectory = @"C:\WSUSHigh\StoredUpdates";

		// Create the directory if it doesn't exist
		Directory.CreateDirectory(storageDirectory);

		// Copy each update to the storage directory
		foreach (string update in updates)
		{
			string destinationPath = Path.Combine(storageDirectory, Path.GetFileName(update));
			File.Copy(update, destinationPath, true);
			Console.WriteLine($"Stored update: {Path.GetFileName(update)}");
		}
	}

	public void DistributeUpdate(Server serverToUpdate)
	{
		Console.WriteLine($"Initiating update distribution to server: {serverToUpdate.Name}");

		// Opret forbindelse til serveren (IP/Port)
		if (ConnectToServer(serverToUpdate))
		{
			// Overfør opdateringerne til serveren
			TransferUpdates(serverToUpdate);

			// Udfør de nødvendige handlinger på serveren for at anvende opdateringerne (script)
			ApplyUpdates(serverToUpdate);

			Console.WriteLine("Update distribution completed successfully.");
		}
		else
		{
			Console.WriteLine("Failed to connect to the server. Update distribution aborted.");
		}
	}

	private bool ConnectToServer(Server server)
	{
		// Logik til at etablere forbindelse til serveren
		// Returner true hvis forbindelsen lykkes, ellers false
		return true; 
	}

	private void TransferUpdates(Server server)
	{
		// Logik til at overføre opdateringerne til serveren
		Console.WriteLine("Transferring updates to the server...");
		// Simulerer overførsel ved at sove i 2 sekunder
		System.Threading.Thread.Sleep(2000);
		Console.WriteLine("Updates transferred successfully.");
	}

	private void ApplyUpdates(Server server)
	{
		// Logik til at anvende opdateringerne på serveren (script)
		Console.WriteLine("Applying updates on the server...");
		// Simulerer anvendelse ved at sove i 2 sekunder
		System.Threading.Thread.Sleep(2000);
		Console.WriteLine("Updates applied successfully.");
	}
}
