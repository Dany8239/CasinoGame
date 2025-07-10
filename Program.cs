using System;
using System.Threading;

class Program
{
    static int WaitForKeyPress()
    {
        Console.WriteLine("Stiskni libovolnou klávesu pro pokračování...");
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

        while (money > 0)
        {
            Console.WriteLine("Hrací automat!\n ------------\n");
            Thread.Sleep(350);
            Console.WriteLine($"\nAktuálně máš {money} korun");
            // Kontrola vstupu pro sazku
            while (true)
            {
                Console.WriteLine("Zadej sázku:");
                if (!int.TryParse(Console.ReadLine(), out bet) || bet <= 0)
                {
                    Console.WriteLine("Neplatná hodnota");
                    Thread.Sleep(800);
                    continue;
                }
                else if (bet > money)
                {
                    Console.WriteLine("Lol broke ass bitch zadej sázku na kterou actually máš bez prodeje orgánů");
                    Thread.Sleep(800);
                    continue;
                }
                Console.WriteLine("Zadej počet čísel v automatu, minimum je 5(psst... od osmi máš bonus):");
                if (!int.TryParse(Console.ReadLine(), out range) || range < 5)
                {
                    Console.WriteLine("Neplatná hodnota");
                    Thread.Sleep(800);
                    continue;
                }

                break;
            }

            money -= bet;
            int slot1, slot2, slot3;

            // Rig, prvni 3 spiny jsou garantovana vyhra
            spinCount++;
            if (spinCount <= 3 && firstSession == true && range <= 15)
            {
                slot1 = slot2 = rnd.Next(1, range + 1);
                if (spinCount == 2 && bet < 2000 && range < 10)
                {
                    slot3 = slot1;
                }
                else
                {
                    slot3 = rnd.Next(1, range + 1);
                }
            }
            else
            {
                slot1 = rnd.Next(1, range + 1);
                slot2 = rnd.Next(1, range + 1);
                slot3 = rnd.Next(1, range + 1);
            }

            Console.WriteLine("\n");

            // Animace spinu
            for (int i = 0; i < 40; i++)
            {
                Console.Write("\r" + rnd.Next(1, range + 1) + " " + rnd.Next(1, range + 1) + " " + rnd.Next(1, range + 1) + "           ");
                Thread.Sleep(100);
            }

            // Vyhodnoceni vyhry
            // Jackpot
            if (slot1 == slot2 && slot2 == slot3)
            {
                int win = bet * 6;
                if (range < 8)
                    win = (int)(win * 0.5);
                else if (range >= 8)
                    win = (int)(win * 1.1);
                win += bet;
                Console.Write("\x1b[38;2;0;255;0m");
                Console.Write("\r" + slot1 + " " + slot2 + " " + slot3);
                Thread.Sleep(500);
                Console.WriteLine($"\nJackpot! Výhral jsi {win - bet} korun!");
                money += win;
            }

            // Big win
            else if (slot1 == slot2 || slot1 == slot3 || slot2 == slot3)
            {
                int win = (int)(bet * 1.3);

                if (range < 8)
                    win = (int)(win * 0.5);  if (spinCount <= 3 && firstSession == true && range <= 15) " + slot2 + " " + slot3);
                Thread.Sleep(800);
                Console.WriteLine($"\nBig win! Výhral jsi {win - bet} korun!");
                money += win;
            }

            //Prohra, womp womp
            else
            {
                Console.Write("\x1b[38;2;255;0;0m");
                Console.Write("\r" + slot1 + " " + slot2 + " " + slot3);
                Thread.Sleep(800);
                Console.WriteLine("\nBohužel jsi prohrál, ale zkus to znovu, tentokrát to určitě vyjde!");
            }
            Console.ResetColor();
            WaitForKeyPress();
            Console.Clear();

        }

        Thread.Sleep(500);
        Console.WriteLine("Bohužel ti došly peníze, končíš");
        Thread.Sleep(650);
        return money;

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
            reward = (int)(bet * Math.Pow(1.6, (10 - attempts) / 2.0));
            reward += bet;
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

    static void Main()
    {
        int money = 1000;
        bool firstSession = true;
        while(money > 0)
        {
            string[] options = { "Automat", "Hádání čísel", "Ukončit" };
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
                    return;
            }
        }
    }
}
