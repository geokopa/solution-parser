using System.Diagnostics;

using System.Xml;



const string solutionPath = @"C:\Users\gkopadze\source\repos\Congree\Congree.sln";

const string targetFilePath = @"..\Targets\CodeSigning.targets";

try

{

	//var projectPaths = GetProjectPathsFromSolution(solutionPath);

	var projectPaths = GetProjectPathsFromSolutionExcludeCoreProjects(solutionPath);



	var filtered = projectPaths.Where(x => !x.Contains("Test") || !x.Contains("Tests")).ToList();



	var timer = Stopwatch.StartNew();

	int successCount = 0;

	foreach (var projectPath in filtered)

	{

		try

		{

			var xmlDoc = new XmlDocument();

			xmlDoc.Load(projectPath);



			if (xmlDoc.DocumentElement != null)

			{

				XmlElement root = xmlDoc.DocumentElement;



				XmlElement newTargetNode =

						xmlDoc.CreateElement("Import", "http://schemas.microsoft.com/developer/msbuild/2003");

				newTargetNode.SetAttribute("Project", targetFilePath);



				root.InsertAfter(newTargetNode, root.LastChild);

				xmlDoc.Save(projectPath);

				successCount++;



				Console.WriteLine($"Info: Project {projectPath} has successfully modified!");

			}

		}

		catch (Exception ex)

		{

			Console.ForegroundColor = ConsoleColor.Red;

			Console.WriteLine($"Warning: Project {projectPath} has thrown exception: \n{ex}");

			Console.ResetColor();

			continue;

		}

	}



	timer.Stop();

	var ellapsed = timer.Elapsed.TotalSeconds;



	Console.ForegroundColor = ConsoleColor.Green;

	Console.WriteLine($"\nElapsed total seconds: {ellapsed}. Total Projects Modified: {successCount}");

	Console.ResetColor();



	Console.ReadLine();

}

catch (Exception e)

{

	Console.WriteLine(e);

}







string[] GetProjectPathsFromDirectory(string directoryPath)

{

	return Directory.GetFiles(directoryPath, "*.csproj", SearchOption.AllDirectories);

}





List<string> GetProjectPathsFromSolution(string solutionPath)

{

	List<string> projectPaths = new();

	string[] lines = File.ReadAllLines(solutionPath);



	if (lines.Length > 2)

	{

		for (int i = 2; i < lines.Length; i++)

		{

			string line = lines[i].Trim();

			int index = line.IndexOf(",", StringComparison.Ordinal);



			if (index >= 0)

			{

				string projectPath = line.Split(",")[1].Trim('"').Remove(0, 2);



				if (projectPath.EndsWith(".csproj"))

				{

					string fullPath = Path.Combine(Path.GetDirectoryName(solutionPath), projectPath);



					if (File.Exists(fullPath))

					{

						projectPaths.Add(fullPath);

					}

					else

					{

						Console.WriteLine($"Warning: Project file not found {fullPath}");

					}

				}

			}

		}

	}

	else

	{

		Console.WriteLine("Error: Solution file has invalid format.");

	}



	return projectPaths.ToList();

}







List<string> GetProjectPathsFromSolutionExcludeCoreProjects(string solutionPath)

{

	List<string> projectPaths = new();

	string[] lines = File.ReadAllLines(solutionPath);



	if (lines.Length > 2)

	{

		for (int i = 2; i < lines.Length; i++)

		{

			string line = lines[i].Trim();

			int index = line.IndexOf(",", StringComparison.Ordinal);



			if (index >= 0)

			{

				string projectPath = line.Split(",")[1].Trim('"').Remove(0, 2);



				if (projectPath.EndsWith(".csproj"))

				{

					string fullPath = Path.Combine(Path.GetDirectoryName(solutionPath), projectPath);



					try

					{

						var xmlDoc = new XmlDocument();

						xmlDoc.Load(fullPath);



						if (xmlDoc.DocumentElement != null)

						{

							var sdkAttribute = xmlDoc.DocumentElement.GetAttribute("Sdk");

							if (sdkAttribute != "Microsoft.NET.Sdk")

							{

								projectPaths.Add(fullPath);

							}

							else

							{

								Console.WriteLine($"Info: Project {fullPath} has Sdk=\"Microsoft.NET.Sdk\" and will be skipped.");

							}

						}

					}

					catch (Exception ex)

					{

						Console.ForegroundColor = ConsoleColor.Red;

						Console.WriteLine($"Warning: Project {fullPath} has thrown exception: \n{ex}");

						Console.ResetColor();

						continue;

					}

				}

			}

		}

	}

	else

	{

		Console.WriteLine("Error: Solution file has invalid format.");

	}



	return projectPaths.ToList();

}
