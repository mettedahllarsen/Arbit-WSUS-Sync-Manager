using ICSharpCode.SharpZipLib.Tar;
using System.Formats.Tar;
using System.IO.Compression;

//Path to the local folder where the updates are stored
string tarFolderPath = @"C:\WSUSUpdates";
string tarFileName = "test.txt";

string tarFilePath = Path.Combine(tarFolderPath, tarFileName);

// RUN THIS <----------------------------------------


CheckForFolderAndTarFile(tarFolderPath, tarFileName);
string extractedFilesPath = ExtractTarContents(tarFolderPath, tarFilePath);
CheckExtractedFiles(extractedFilesPath);


//---------------------------------------------------




// Method to check if the folder and tar file exist, and extract the contents of the tar file
static void CheckForFolderAndTarFile(string tarFolderPath, string tarFilePath)
{
	if (Directory.Exists(tarFolderPath))
	{

		if (File.Exists(tarFilePath))
		{
			Console.WriteLine("Success: The folder and tar file both exist.");
		}
		else
		{
			Console.WriteLine("Error: Tar file not found in the local folder.");
		}
	}
	else
	{
		Console.WriteLine("Error: Local folder not found.");
	}
}

// Method to extract the contents of a TAR file
static string ExtractTarContents(string tarFolderPath, string tarFilePath)
{
	// Create a directory where the contents will be extracted
	string extractedPath = Path.Combine(tarFolderPath, "extracted");

	Directory.CreateDirectory(extractedPath);

	// Extract the TAR file
	ZipFile.ExtractToDirectory(tarFilePath, extractedPath);

	// Return the path to the directory containing the extracted contents
	return extractedPath;
}


static void CheckExtractedFiles(string extractedFilesPath)
{
	// Get files from folder
	string[] files = Directory.GetFiles(extractedFilesPath);

	// Variables to track if metadata and updates files are found
	bool hasMetadata = false;
	bool hasUpdates = false;

	// Iterate through each file in the extraction path
	foreach (string file in files)
	{
		// Check the file extension to determine if it's a metadata file (XML) or an update file
		string fileExtension = Path.GetExtension(file);
		if (string.Equals(fileExtension, ".xml", StringComparison.OrdinalIgnoreCase))
		{
			hasMetadata = true;
		}
		else if (string.Equals(fileExtension, ".msi", StringComparison.OrdinalIgnoreCase))
		{
			hasUpdates = true;
		}
	}

	// Print messages indicating if metadata and/or update files were found
	if (hasMetadata)
	{
		Console.WriteLine("Metadata files found in the extraction folder.");
	}
	else
	{
		Console.WriteLine("No metadata files found in the extraction folder.");
	}

	if (hasUpdates)
	{
		Console.WriteLine("Update files found in the extraction folder.");
	}
	else
	{
		Console.WriteLine("No update files found in the extraction folder.");
	}
}