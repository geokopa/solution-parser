using System.Xml;

const string solutionPath = @"C:\Users\gkopa\source\repos\pangea-it\Pangea.RegistrationService\src\Pangea.RegistrationService.sln";
const string targetFilePath = @"CustomActionCodeSigning.targets";
try
{
    var projectPaths = GetProjectPathsFromSolution(solutionPath);

    foreach (var projectPath in projectPaths)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.Load(projectPath);

        if (xmlDoc.DocumentElement != null)
        {
            XmlElement root = xmlDoc.DocumentElement;
            
            string element = "ItemGroup";
            var lastItemGroup = root.SelectNodes($"//{element}").OfType<XmlElement>().LastOrDefault();
            
            if (lastItemGroup is not null)
            {
                XmlElement newTargetNode =
                    xmlDoc.CreateElement("Import");
                newTargetNode.SetAttribute("Project", targetFilePath);
            
                root.InsertAfter(newTargetNode, lastItemGroup);
                xmlDoc.Save(projectPath);
            }
        }
    }
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
    List<string> projectPaths = new List<string>();
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