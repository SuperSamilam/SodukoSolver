public static class Settings
{
    public static Dictionary<string, List<string>> localizor = new Dictionary<string, List<string>>()
    {
        {"SE", new List<string>(){"Ändra dina inställningar", "Ändra språk", "Ändra max letrådar", "Spela med misstag alert", "Gå tillbaka", "Språket har ändrats", "Fel, skriv såhär: #opperation #språk", "Max letrådar är nu ", "Fel, Skriv såhär: #opperation #nummer", "Spela med misstags alert är", "Fel, skriv såhär: #opperation, #true_false"}},
        {"EN", new List<string>(){"Change your settings", "Change Language", "Change max hints", "Play with mistake alerter", "Go back", "Language have changed", "Syntax error, please follow this syntax: #operation #language", "Max hint value is now ", "Syntax error, please follow this syntax: #operation #number", "Play with mistake alerts are", "Syntax error, please follow this syntax: #operation #true_or_false"}},
        {"FI", new List<string>(){}}
    };
    public static string lang { get; set; } 

    public static int hints { get; set; }

    //Edits the settings
    public static void EditSettings()
    {
        while (true)
        {
            Thread.Sleep(3000);
            Console.Clear();

            Console.WriteLine(localizor[lang][0]); //Change ur settings
            Console.WriteLine("1: " + localizor[lang][1]); //Change Language
            Console.WriteLine("2: " + localizor[lang][2]); //Change Max hints avaible
            Console.WriteLine("3: " + localizor[lang][4]); //Quit

            //edits the choosen setting
            string resp = Console.ReadLine() ?? "";
            if (resp[0] == '1')
                if (localizor.Keys.Contains(resp.Substring(resp.IndexOf(' ') + 1).ToUpper()))
                {
                    lang = resp.Substring(resp.IndexOf(' ') + 1).ToUpper();
                    Console.WriteLine(localizor[lang][5]); //Language have now changed
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(localizor[lang][6]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            else if (resp[0] == '2')
                if (char.IsDigit(resp[resp.IndexOf(' ') + 1]))
                {
                    hints = (int)char.GetNumericValue(resp[resp.IndexOf(' ') + 1]);
                    Console.WriteLine(localizor[lang][7] + hints);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(localizor[lang][7]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
            else if (resp[0] == '3')
                return;
        }
    }
}