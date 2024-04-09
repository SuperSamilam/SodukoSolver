public static class Settings
{
    public static int hints { get; set; }

    //Edits the settings
    public static void EditSettings()
    {
        while (true)
        {
            Thread.Sleep(3000);
            Console.Clear();

            Console.WriteLine("Change your settings"); 
            Console.WriteLine("1: Change max hints"); 
            Console.WriteLine("2: Back"); 

            //edits the choosen setting
            string resp = Console.ReadLine() ?? "";
            if (resp[0] == '1')
                if (char.IsDigit(resp[resp.IndexOf(' ') + 1]))
                {
                    hints = (int)char.GetNumericValue(resp[resp.IndexOf(' ') + 1]);
                    Console.WriteLine("Amount of hints: " + hints);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("IDK what this does exactly");
                    Console.ForegroundColor = ConsoleColor.White;
                }
            else if (resp[0] == '2')
                return;
        }
    }
}