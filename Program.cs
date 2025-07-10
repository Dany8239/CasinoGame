using System;
using System.Threading;

class Program
{
    static int WaitForKeyPress()
    {
        Console.WriteLine("Stiskni Enter pro pokračování...");
        while (Console.ReadKey(true).Key != ConsoleKey.Enter) { }
        return 0;
    }

    static int ShowMenu(string[] options, int money)
    {
        int currentSelection = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();

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
        string[] options = { "Dobít kredity", "Hrát", "Exit(Cashout)" };

        while (true)
        {
            if (credits <= 0)
            {
                options = new string[] { "Dobít kredity", "\x1b[38;2;105;105;105mHrát dál", "\x1b[0mExit(Cashout)" };
            }
            else if (money <= 0 && credits <= 0)
            {
                Console.WriteLine("Došly ti peníze i kredity, návrat do hlavní nabídky");
                Thread.Sleep(800);
                return money;
            }
            else
            {
                options = new string[] { "Dobít kredity", "Hrát", "Exit(Cashout)" };
            }

            // Vyber moznosti z menu
            int choice = ShowMenu(options, money);
            switch (choice)
            {
                // Dobijeni kreditu
                case 0:
                    Console.WriteLine("Kolik chceš dobít kreditů? (minimálně 100)");
                    int topUp;
                    if (!int.TryParse(Console.ReadLine(), out topUp) || topUp < 100)
                    {
                        Console.WriteLine("Neplatná hodnota, zadej částku alespoň 100 korun");
                        break;
                    }
                    if (topUp > money)
                    {
                        Console.WriteLine("Lol broke ass na to nemáš, buď si sežeň zam*stnání(fuj), nebo zvaž prodej orgánů na černém trhu");
                    }
                    credits += topUp;
                    money -= topUp;
                    Console.WriteLine($"Dobito {topUp} korun, aktuální stav: {credits} kreditů");
                    WaitForKeyPress();
                    break;

                case 1:
                    // Hlavni gameplay loop
                    while (credits > 0)
                    {
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

                        // Rig, prvni 3 spiny jsou zarucena vyhra    
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

                        // Animace spinu
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
                    }
                    Thread.Sleep(500);
                    Console.WriteLine("Došly ti peníze");
                    WaitForKeyPress();
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

            // Ukazat posledni pokus
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
    bool getBetJackTry = false;
    int betJack = 0;
    bool gameOverBlack = false;

    void BlackJackGameOver()
    {
        Console.WriteLine("Prohráli jste, váš součet karet přesáhl 21");
    }

    void BlackJackGameOverTurns()
    {
        Console.WriteLine("Prohráli jste. Zdrželi jste se hry a váš soupeř nedosáhl 21.");
    }

    void BlackJackGameWin()
    {
        Console.WriteLine("Vyhráli jste, součet karet soupeře přesáhl 21");
        money += (int)Math.Round(betJack * 1.5);
    }

    Random rndBlack = new Random();
    int[] cards = {1,2,3,4,5,6,7,8,9,10,10,10,10,11};
    List<int> playerCards = new List<int>();
    List<int> aiCards = new List<int>();

    do
    {
        Console.Write($"Zbývající peníze: {money}\nVyberte sázku: ");
        getBetJackTry = int.TryParse(Console.ReadLine(), out betJack);
    }
    while (!getBetJackTry || betJack > money);

    money -= betJack;

    for (int i = 0; i < 2; i++)
    {
        playerCards.Add(cards[rndBlack.Next(cards.Length)]);
        aiCards.Add(cards[rndBlack.Next(cards.Length)]);
    }

    while (!gameOverBlack)
    {
        int totalPlayer = playerCards.Sum();
        int totalAI = aiCards.Sum();

        Console.Clear();
        Console.WriteLine($"\n//Vy//\nZbývající peníze: {money}\nVaše sázka: {betJack}$");
        Console.WriteLine($"Máte: {playerCards.Count} karet, jejich hodnota: {totalPlayer}");
        Console.WriteLine($"\n//Soupeř//\nMá: {aiCards.Count} karet, jejich hodnota: {totalAI}");

        if (totalPlayer > 21) { gameOverBlack = true; BlackJackGameOver(); break; }
        if (totalAI > 21) { gameOverBlack = true; BlackJackGameWin(); break; }

        Console.WriteLine("\nVyberte akci:\n1 -> další karta\n2 -> zdržet se\n3 -> odejít");
        if (!int.TryParse(Console.ReadLine(), out int action)) continue;

        if (action == 1)
        {
            playerCards.Add(cards[rndBlack.Next(cards.Length)]);
            if (rndBlack.Next(1,4) == 1 && totalAI > 18)
            {
                aiCards.Add(cards[rndBlack.Next(cards.Length)]);
            }
        }
        else if (action == 2)
        {
            if (totalAI > 21) { gameOverBlack = true; BlackJackGameWin(); }
            else { gameOverBlack = true; BlackJackGameOverTurns(); }
        }
        else if (action == 3)
        {
            Console.WriteLine("Opustil jste hru. Přišel jste o vsazené peníze.");
            gameOverBlack = true;
        }

        Console.WriteLine("Zmáčkněte jakékoliv tlačítko pro pokračování...");
        Console.ReadKey();
    }

    return money;
}

    static void Main()
    {
        int money = 1000;
        bool firstSession = true;
        while (money > 0)
        {
            string[] options = { "Automat", "Hádání čísel", "BlackJack", "Ukončit" };
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
                    return;
            }
        }
    }
}
