using System;
using System.Threading;
using System.IO;

class Program
{
    static int win = 0;
    static int[] redNums = {3, 12, 7, 18, 9, 14, 1, 16, 5, 23, 30, 36, 27, 34, 25, 21, 19, 32};
    static int[] blackNums = {26, 35, 28, 29, 22, 31, 20, 33, 24, 10, 8, 11, 13, 6, 17, 2, 4, 15};
    static List<List<int>> insideBets = new List<List<int>>();

    static int WaitForKeyPress()
    {
        Console.WriteLine("Stiskni Enter pro pokračování...");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        return 0;
    }

    // Nacteni anon klice z .env, mozna bude nevyuzite pokud se mi napodari nastavit automatizace
    static void LoadEnv(string filePath)
    {
        if (!File.Exists(filePath)) return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) 
            {
                continue;
            }

            var parts = line.Split('=', 2);
            if (parts.Length != 2) continue;

            Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
        }
    }


static int ShowMenu(string[] options, int money, string header = "")
{
    int currentSelection = 0;
    ConsoleKey key;

    do
    {
        Console.Clear();

        if (!string.IsNullOrEmpty(header))
        {
            Console.WriteLine(header);
            Console.WriteLine(); // extra blank line after header
        }

        for (int i = 0; i < options.Length; i++)
        {
            if (i == currentSelection)
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.Black;
            }
            else
            {
                Console.ResetColor();
            }
            Console.WriteLine(options[i]);
        }
        Console.ResetColor();
        Console.WriteLine($"\nMáš {money} korun");

        var keyInfo = Console.ReadKey(true);
        key = keyInfo.Key;

        if (key == ConsoleKey.UpArrow)
        {
            currentSelection--;
            if (currentSelection < 0)
                currentSelection = options.Length - 1;
        }
        else if (key == ConsoleKey.DownArrow)
        {
            currentSelection++;
            if (currentSelection >= options.Length)
                currentSelection = 0;
        }

    } while (key != ConsoleKey.Enter);

    return currentSelection;
}

    static int Slot(int money, bool firstSession)
    {
        Thread.Sleep(200);
        Console.Clear();
        Random rnd = new Random();
        int spinCount = 0;
        int bet = 0;
        int range = 0;
        int credits = 0;
        int slot1 = 0, slot2 = 0, slot3 = 0;
        
        string[] mainOptions = { "Dobít kredity", "Hrát", "Exit(Cashout)" };
        string[] spinOptions = { "Dobít kredity", "Hrát další spin", "Exit(Cashout)" };

        while (true)
        {
            if (credits <= 0)
            {
                mainOptions = new string[] { "Dobít kredity", "\x1b[38;2;105;105;105mHrát dál", "\x1b[0mExit(Cashout)" };
            }
            else if (money <= 0 && credits <= 0)
            {
                Console.WriteLine("Došly ti peníze i kredity, návrat do hlavní nabídky");
                Thread.Sleep(800);
                return money;
            }
            else
            {
                mainOptions = new string[] { "Dobít kredity", "Hrát", "Exit(Cashout)" };
            }

            // If no credits, show main menu
            int choice;
            if (credits <= 0)
            {
                choice = ShowMenu(mainOptions, money);
            }
            else
            {
                // When player has credits, show spin menu after each spin
                choice = ShowMenu(spinOptions, money);
            }

            switch (choice)
            {
                // Dobijeni kreditu
                case 0:
                    Console.WriteLine("Kolik chceš dobít kreditů? (minimálně 100)");
                    int topUp;
                    if (!int.TryParse(Console.ReadLine(), out topUp) || topUp < 100)
                    {
                        Console.WriteLine("Neplatná hodnota, zadej částku alespoň 100 korun");
                        WaitForKeyPress();
                        break;
                    }
                    if (topUp > money)
                    {
                        Console.WriteLine("Lol broke ass na to nemáš, buď si sežeň zam*stnání(fuj), nebo zvaž prodej orgánů na černém trhu");
                        WaitForKeyPress();
                        break;
                    }
                    credits += topUp;
                    money -= topUp;
                    Console.WriteLine($"Dobito {topUp} korun, aktuální stav: {credits} kreditů");
                    WaitForKeyPress();
                    break;

                case 1: // Spin (play)
                    if (credits <= 0)
                    {
                        Console.WriteLine("Nemáš dostatek kreditů na hraní.");
                        WaitForKeyPress();
                        break;
                    }

                    Console.WriteLine("Hrací automat!\n ------------\n");
                    Thread.Sleep(350);
                    Console.WriteLine($"\nMáš {credits} kreditů");

                    while (true)
                    {
                        Console.WriteLine("Zadej sázku:");
                        if (!int.TryParse(Console.ReadLine(), out bet) || bet <= 0)
                        {
                            Console.WriteLine("Neplatná hodnota");
                            Thread.Sleep(800);
                            continue;
                        }
                        else if (bet > credits)
                        {
                            Console.WriteLine("Nemáš dostatek peněz na sázku");
                            Thread.Sleep(800);
                            continue;
                        }
                        Console.WriteLine("Zadej počet čísel v automatu, minimum je 5, maximum 15:");
                        if (!int.TryParse(Console.ReadLine(), out range) || range < 5 || range > 15)
                        {
                            Console.WriteLine("Neplatná hodnota");
                            Thread.Sleep(800);
                            continue;
                        }
                        credits -= bet;
                        break;
                    }

                    spinCount++;
                    if (spinCount <= 3 && firstSession == true && range <= 15)
                    {
                        slot1 = slot2 = rnd.Next(1, range + 1);
                        slot3 = (spinCount == 2 && bet < 2000 && range < 10) ? slot1 : rnd.Next(1, range + 1);
                    }
                    else
                    {
                        slot1 = rnd.Next(1, range + 1);
                        slot2 = rnd.Next(1, range + 1);
                        slot3 = rnd.Next(1, range + 1);
                    }

                    Thread.Sleep(800);
                    Console.Clear();
                    Console.WriteLine("Hrací automat!\n ------------\n");

                    for (int i = 0; i < 40; i++)
                    {
                        Console.Write("\r" + rnd.Next(1, range + 1) + " " + rnd.Next(1, range + 1) + " " + rnd.Next(1, range + 1) + "           ");
                        Thread.Sleep(100);
                    }

                    if (slot1 == slot2 && slot2 == slot3)
                    {
                        int win = bet * 6;
                        win = range < 8 ? (int)(win * 0.5) : (int)(win * 1.1);
                        win += bet;
                        Console.Write("\x1b[38;2;0;255;0m");
                        Console.Write($"\r{slot1} {slot2} {slot3}");
                        Thread.Sleep(500);
                        Console.WriteLine($"\nJackpot! Výhral jsi {win - bet} korun!");
                        credits += win;
                    }
                    else if (slot1 == slot2 || slot1 == slot3 || slot2 == slot3)
                    {
                        int win = (int)(bet * 1.3);
                        win = range < 8 ? (int)(win * 0.5) : (int)(win * 1.1);
                        win += bet;
                        Console.Write("\x1b[38;2;0;255;0m");
                        Console.Write($"\r{slot1} {slot2} {slot3}");
                        Thread.Sleep(800);
                        Console.WriteLine($"\nBig win! Výhral jsi {win - bet} korun!");
                        credits += win;
                    }
                    else
                    {
                        Console.Write("\x1b[38;2;255;0;0m");
                        Console.Write($"\r{slot1} {slot2} {slot3}");
                        Thread.Sleep(800);
                        Console.WriteLine("\nBohužel jsi prohrál, ale zkus to znovu.");
                    }
                    Console.ResetColor();
                    WaitForKeyPress();
                    Console.Clear();

                    if (credits <= 0)
                    {
                        Console.WriteLine("Došly ti peníze");
                        WaitForKeyPress();
                    }
                    break;

                case 2:
                    Thread.Sleep(650);
                    money += credits;
                    return money;
            }
        }
    }


    static int GuessingGame(int money)
    {
        Random rng = new Random();
        string[] options = {"Hrát znovu", "Exit"};
        int number = rng.Next(1, 101);
        int playerGuess = 0;
        int attempts = 0;
        int bet = 0;
        int reward = 0;
        bool isSmaller = false;
        bool hasGuessed = false;
        while (playerGuess != number && attempts < 10)
        {
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine("Uhodni číslo\n----------");
            attempts++;

            if(isSmaller && hasGuessed)
            {
                Console.Write("\x1b[38;2;255;0;0m");
                Console.WriteLine(playerGuess);
                Console.WriteLine("To není ono, zkus menší číslo");
            }
            else if(!isSmaller && hasGuessed)
            {
                Console.Write("\x1b[38;2;255;0;0m");
                Console.WriteLine(playerGuess);
                Console.WriteLine("To není ono, zkus větší číslo");
            }

            // Zadani sazky
            if(!hasGuessed)
            {
                Thread.Sleep(400);
                Console.WriteLine("Zadej svoji sázku, máš 10 pokusů:");
                if(!int.TryParse(Console.ReadLine(), out bet))
                {
                    Console.WriteLine("Neplatný vstup");
                    continue;
                }
                money -= bet;
            }

            // Hlavni gameplay loop
            Console.ResetColor();
            Console.WriteLine($"Pokus {attempts}/10");
            Console.WriteLine("Zadej svůj tip:");
            hasGuessed = true;

            if (!int.TryParse(Console.ReadLine(), out playerGuess))
            {
                Console.WriteLine("Neplatná hodnota!");
                continue;
            }

            if (playerGuess > number)
            {
                Console.Write("\x1b[38;2;255;0;0m");
                Console.WriteLine("To není ono, zkus menší číslo");
                isSmaller = true;
            }
            else if (playerGuess < number)
            {
                Console.Write("\x1b[38;2;255;0;0m");
                Console.WriteLine("To není ono, zkus větší číslo");
                isSmaller = false;
            }
            Thread.Sleep(1400);
        }
        if(attempts <= 10) 
        {
            // Vypocet rewardu
            double maxLog = Math.Log(11);
            double currentLog = Math.Log(11 - attempts + 1);
            double scale = currentLog / maxLog;
            scale /= 2;
            reward = (int)(bet + bet * scale * 2.0);


            money += reward;

            Console.Write("\x1b[38;2;0;255;0m");
            Console.WriteLine($"Gratulujeme! Uhádl jsi číslo {number}, zabralo ti to {attempts} pokusů a vyhrál jsi {reward} korun");
            Thread.Sleep(500);
            WaitForKeyPress();
        }
        else
        {
            Console.Write("\x1b[38;2;255;0;0m");
            Console.WriteLine($"Bohužel jsi prohrál, číslo bylo {number}.");
            WaitForKeyPress();
        }
        
        isSmaller = false;
        int choice = ShowMenu(options, money);
        
        if(choice == 0)
        {
            GuessingGame(money);
            return money;
        }
        else
        {
            return money;
        }
    }

static int BlackJack(int money)
{
    Random rndBlack = new Random();
    int[] cards = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };
    List<int> playerCards = new List<int> { cards[rndBlack.Next(cards.Length)] };
    List<int> aiCards = new List<int> { cards[rndBlack.Next(cards.Length)] };
    int betJack = 0;
    bool gameOverBlack = false;

    Console.Clear();
    Console.WriteLine("Zadej svoji sázku:");
    if (!int.TryParse(Console.ReadLine(), out betJack) || betJack <= 0 || betJack > money)
    {
        Console.WriteLine("Neplatná sázka!");
        Thread.Sleep(1000);
        return money;
    }
    money -= betJack;

    while (!gameOverBlack)
    {
        int totalPlayer = playerCards.Sum();
        int totalAI = aiCards.Sum();

        string header = $"// Vy //\nZbývající peníze: {money}\nSázka: {betJack}\n" +
                        $"Vaše karty ({playerCards.Count}): {string.Join(", ", playerCards)} (součet: {totalPlayer})\n\n" +
                        $"// Soupeř //\nKarty soupeře ({aiCards.Count}): {string.Join(", ", aiCards)} (součet: {totalAI})";

        if (totalPlayer > 21)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(header);
            Console.WriteLine("\nProhráli jste, součet vašich karet přesáhl 21.");
            Console.ResetColor();
            gameOverBlack = true;
            break;
        }
        if (totalAI > 21)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(header);
            Console.WriteLine("\nVyhráli jste, soupeř překročil 21.");
            money += (int)(betJack * 2.5);
            Console.ResetColor();
            gameOverBlack = true;
            break;
        }

        string[] options = { "další karta", "zdržet se", "odejít" };
        int action = ShowMenu(options, money, header);

        if (action == 0)
        {
            playerCards.Add(cards[rndBlack.Next(cards.Length)]);
            if (rndBlack.Next(1, 4) == 1 && totalAI < 19)
            {
                aiCards.Add(cards[rndBlack.Next(cards.Length)]);
            }
        }
        else if (action == 1)
        {
            if (totalAI > totalPlayer && totalAI <= 21)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(header);
                Console.WriteLine("\nProhráli jste. Soupeř má vyšší skóre.");
            }
            else if (totalAI == totalPlayer)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(header);
                Console.WriteLine("\nRemíza. Sázka vám je vrácena.");
                money += betJack;
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(header);
                Console.WriteLine("\nVyhráli jste! Máte vyšší skóre než soupeř.");
                money += betJack * 2;
            }
            Console.ResetColor();
            gameOverBlack = true;
        }
        else if (action == 2)
        {
            Console.Clear();
            Console.WriteLine(header);
            Console.WriteLine("\nOpustil jste hru, přišli jste o vsazené peníze.");
            gameOverBlack = true;
        }

        if (!gameOverBlack)
        {
            Console.WriteLine("\nStiskněte libovolnou klávesu pro pokračování...");
            Console.ReadKey(true);
        }
    }

    // Added final win/lose message consistent with UI style
    Thread.Sleep(1000);
    Console.WriteLine("\n------------------------------------");

    if (money > 0)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Skvělá práce! Máš ještě peníze, můžeš pokračovat v hazardu.");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Bohužel jsi přišel o všechny peníze. Zkus štěstí příště!");
    }

    Console.ResetColor();
    WaitForKeyPress();

    return money;
}

    static void pleaseEnterValidInput(int money)
    {
        string[] options3 = { "Prosím vlože validní inptut" };
        int selected3 = ShowMenu(options3, money);
        switch (selected3)
        {
            case 0:
                break;
        }
    }

    static void addInsideBets(int howManyTimes, ref int money)
    {
        Console.Clear();
        bool repeat = true;
        while(repeat == true)
        {
            repeat = false;

            Console.WriteLine($"Kolik si chcete vsadit. Bilance {money}");
            int bet;                            
            if (! int.TryParse(Console.ReadLine(), out bet) || bet > money || bet <= 0)
            {
                pleaseEnterValidInput(money);
                repeat = true;
                continue;
            }

            insideBets.Add(new List<int> {Math.Abs(bet), -1});
            money -= Math.Abs(bet);

            int rowCount = insideBets.Count - 1;

            for (int i = 0; i <= howManyTimes - 1; i++)
            {
                Console.Clear();

                Console.WriteLine(i);
                Console.WriteLine("Vyberte si číslo na které chcete vsadit (0 - 36)");

                int num;                            
                if (! int.TryParse(Console.ReadLine(), out num))
                {
                    i -= 1;
                    pleaseEnterValidInput(money);
                }
                else
                {
                    if  (num >= 0 && num <= 36)
                    {
                        insideBets[rowCount].Add(num);
                    }
                    else
                    {
                        i -= 1;
                        pleaseEnterValidInput(money);
                    }
                }
            }                        
        }
    }

    static void addOutsideBetsBets(char type, ref int money)
    {
        Console.Clear();
        bool repeat = true;
        while(repeat == true)
        {
            repeat = false;

            Console.WriteLine($"Kolik si chcete vsadit. Bilance {money}");
            int bet;                            
            if (! int.TryParse(Console.ReadLine(), out bet) || bet > money || bet <= 0)
            {
                pleaseEnterValidInput(money);
                repeat = true;
                continue;
            }

            insideBets.Add(new List<int> {Math.Abs(bet)});
            money -= Math.Abs(bet);

            int rowCount = insideBets.Count - 1;

            Console.Clear();

            switch (type)
            {
                case 'r':
                    insideBets[rowCount].Add(-2);
                    for (int i = 0; i < redNums.Length; i++)
                    {
                        insideBets[rowCount].Add(redNums[i]);
                    }
                    break;

                case 'b':
                    insideBets[rowCount].Add(-3);
                    for (int i = 0; i < blackNums.Length; i++)
                    {
                        insideBets[rowCount].Add(blackNums[i]);
                    }
                    break;

                case 'l':
                    insideBets[rowCount].Add(-4);
                    for (int i = 1; i < 19; i++)
                    {
                        insideBets[rowCount].Add(i);
                    }
                    break;

                case 'h':
                    insideBets[rowCount].Add(-5);
                    for (int i = 19; i < 37; i++)
                    {
                        insideBets[rowCount].Add(i);
                    }
                    break;

                case 's':
                    insideBets[rowCount].Add(-6);
                    for (int i = 1; i < 37; i++)
                    {
                        if (i % 2 == 0)
                        {
                            insideBets[rowCount].Add(i);
                        }
                    }
                    break;

                case 'o':
                    insideBets[rowCount].Add(-7);
                    for (int i = 1; i < 37; i++)
                    {
                        if (i % 2 != 0)
                        {
                            insideBets[rowCount].Add(i);
                        }
                    }
                    break;
            }
        }
    }

    static void RuleteBet(ref int money)
    {
        bool repeat = true;

        while (repeat == true)
        {
            Console.WriteLine("Jakou by jste chtěli sázku?");
            string[] options = { "Vnitřní sázka ->   Sázení na specifické čísla neb číselné skupiny", "Vnější sázka  ->   Sázíte pouze na skupiny, např. barvy nebo lichý či sudý", "exit"};
            int selected = ShowMenu(options, money);
            switch (selected)
            {
                case 0:
                    Console.Clear();
                    Console.WriteLine("Na co chcete vsadit?");

                    string[] options1 = { "Přímá sázka   ->   Jedno číslo", "Rozdělené     ->   Dvě čísla", "Ulice         ->   Tři čísla", "Rohová sázka  ->   Čtiři čísla", "Dvojitá ulice ->   Šest čísel" };
                    int selected1 = ShowMenu(options1, money);
                    switch (selected1)
                    {
                        case 0:
                            addInsideBets(1, ref money);
                            break;
                        case 1:
                            addInsideBets(2, ref money);
                            break;
                        case 2:
                            addInsideBets(3, ref money);
                            break;
                        case 3:
                            addInsideBets(4, ref money);
                            break;
                        case 4:
                            addInsideBets(6, ref money);
                            break;
                    }                         
                    break;

                case 1:
                    Console.Clear();
                    Console.WriteLine("Na co chcete vsadit?");

                    string[] options2 = { "Červená ->   Vsadíte na červenou barvu", "Černá   ->   Vsadíte na černou barvu", "Nízká   ->   Vsadíte na čísla 1 - 18", "Vysoká  ->   Vsadíte na čísla 19 - 36", "Sudé    ->   Vsadíte na to, že padne sudé číslo", "Liché   ->   Vsadíte na to, že padne sudé číslo" };
                    int selected2 = ShowMenu(options2, money);
                    switch (selected2)
                    {
                        case 0:
                            addOutsideBetsBets('r', ref money);
                            break;
                        case 1:
                            addOutsideBetsBets('b', ref money);
                            break;
                        case 2:
                            addOutsideBetsBets('l', ref money);
                            break;
                        case 3:
                            addOutsideBetsBets('h', ref money);
                            break;
                        case 4:
                            addOutsideBetsBets('s', ref money);
                            break;
                        case 5:
                            addOutsideBetsBets('o', ref money);
                            break;
                    }                                             
                    break;

                case 2:
                    return;
            }

            Console.Clear();
            string[] options3 = { "Chci ještě něco VSADIT!", "už jdeme hrát", "exit"};
            int selected3 = ShowMenu(options3, money);
            switch (selected3)
            {
                case 0:
                    repeat = true;
                    break;
                case 1:
                    repeat = false;
                    break;
                case 2:
                    return;
            }
        }
    }

    static void animationRoling(int winningNumber)
    {
        int current = 0;
        
        // Fast spinning phase
        for (int i = 0; i < 50; i++)
        {
            Console.SetCursorPosition(0, 0);
            
            // Print all numbers with highlight
            for (int num = 0; num <= 36; num++)
            {
                if (num == current)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write($"{num}");
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else
                {
                    Console.Write($"{num} ");
                }
            }
            
            current = (current + 1) % 37; // Loop 0-36
            Thread.Sleep(100);
        }
        
        // Slow down and stop on winning number
        while (current != winningNumber)
        {
            current = (current + 1) % 37;
            
            Console.SetCursorPosition(0, 0);
            
            for (int num = 0; num <= 36; num++)
            {
                if (num == current)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write($"{num}");
                    Console.ResetColor();
                    Console.Write(" ");
                }
                else
                {
                    Console.Write($"{num} ");
                }
            }
            
            Thread.Sleep(200); // Slower
        }

        Console.WriteLine("\n");
        Thread.Sleep(1000);
    }

    static int RuleteGame(int money)
    {
        win = 0; // RESET WIN AT THE START
        bool repeat = true;
        while (repeat == true)
        {
            repeat = false;
            Console.Clear();

            RuleteBet(ref money);
            Console.Clear();
            
            Random rnd = new Random();
            int winNum = (rnd.Next(0, 37));

            animationRoling(winNum);

            string winNumColour;
            int winPosInRed = Array.IndexOf(redNums, winNum);
            if (winNum == 0)
            {
                winNumColour = "zelená"; //green
            }
            else if (winPosInRed == -1)
            {
                winNumColour = "černá"; //black
            }
            else
            {
                winNumColour = "červená"; //red
            }

            Console.WriteLine($"Vítězné číslo je {winNumColour} {winNum}!");
            
            for (int i = 0; i < insideBets.Count; i++)
            {
                for (int y = 2; y < insideBets[i].Count; y++)
                {
                    if (insideBets[i][y] == winNum)
                    {
                        int x;
                        switch (insideBets[i].Count - 2)
                        {
                            case 1:
                                x = insideBets[i][0] * 36;
                                win += x;
                                Console.WriteLine($"Vyhrál jste {x} za správný tip příjmé sázky.");
                                break;
                            case 2:
                                x = insideBets[i][0] * 18;
                                win += x;
                                Console.WriteLine($"Vyhrál jste {x} za správný tip rozdělené sázky.");
                                break;
                            case 3:
                                x = insideBets[i][0] * 12;
                                win += x;
                                Console.WriteLine($"Vyhrál jste {x} za správný tip ulice.");
                                break;
                            case 4:
                                x = insideBets[i][0] * 9;
                                win += x;
                                Console.WriteLine($"Vyhrál jste {x} za správný tip rohu.");
                                break;
                            case 6:
                                x = insideBets[i][0] * 6;
                                win += x;
                                Console.WriteLine($"Vyhrál jste {x} za správný tip dvojité ulice.");
                                break;
                            case 18:
                                switch (insideBets[i][1])
                                {
                                    case -2:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude červené.");
                                        break;
                                    case -3:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude černé.");
                                        break;
                                    case -4:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude nízké.");
                                        break;
                                    case -5:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude vysoké.");
                                        break;
                                    case -6:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude sudé.");
                                        break;
                                    case -7:
                                        x = insideBets[i][0] * 2;
                                        win += x;
                                        Console.WriteLine($"Vyhrál jste {x} za správný tip že vítězné číslo bude liché.");
                                        break;       
                                }
                                break;
                        }
                    }
                }
            }

            if (win == 0)
            {
                Console.WriteLine("Bohužel jset tentokrát neměli štěstí");
            }

            money += win;

            Console.WriteLine("Zmáčkněte libovolnou klávesu pro pokračování.");
            Console.ReadKey();

            string[] options4 = { "Chci hrát ZNOVU", "exit" };
            int selected4 = ShowMenu(options4, money);
            switch (selected4)
            {
                case 0:
                    repeat = true;
                    insideBets.Clear(); // Clear bets for new game
                    break;
                case 1:
                    break;
            }
        }
        return money;

    }

    static void Main()
    {
        int money = 1000;
        bool firstSession = true;
        //LoadEnv(".env");
        //string supabaseKey = Environment.GetEnvironmentVariable("SUPABASE_ANON_KEY");
        //string supabaseUrl = Environment.GetEnvironmentVariable("SUPABASE_URL");


        while (money > 0)
        {
            string[] options = { "Automat", "Hádání čísel", "BlackJack", "Rulete", "Ukončit" };
            int choice = ShowMenu(options, money);

            switch (choice)
            {
                case 0:
                    money = Slot(money, firstSession);
                    firstSession = false;
                    break;
                case 1:
                    money = GuessingGame(money);
                    break;
                case 2:
                    money = BlackJack(money);
                    break;
                case 3:
                    money = RuleteGame(money);
                    break;
                case 4:
                    return;
            }
        }
    }
}

