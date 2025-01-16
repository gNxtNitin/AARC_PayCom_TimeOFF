using Microsoft.Extensions.Configuration;
using PaycomTimeOffData;
using PaycomTimeOffData.BusinessLogic;

try
{
    //WebDriverHandler.CreateLog("F:\\Console\\PayComAPILogFile.txt", "Calling the Program.cs");

    var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", false);

    IConfiguration config = builder.Build();

    string url = config["PayComUrls:LoginUrl"];
    if (url != null)
    {
        GetTimeOffRecords.ExtractTimeOffRecords(url, config);
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message.ToString());
    Console.WriteLine(ex.StackTrace);
    Console.ReadLine();
}
