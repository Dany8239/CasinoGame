static void Slot(int money)
{
    // Pocatecni nastaveni
    Console.WriteLine("Vítej v casinu!\n ----------\n\n");
    Random rnd = new Random();
    int spinCount = 0;

    // Hlavni loop
    while (money > 0)
    {
        // Hrac zada sazku a range
        Thread.Sleep(650);
        Console.WriteLine($"\nAktuálně máš {money} korun");
        Console.WriteLine("Zadej sázku");

        if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0)
        {
            Console.WriteLine("Neplatná hodnota");
            continue;
        }
        else if (bet > money)
        {
            Console.WriteLine("Lol broke ass bitch zadej sázku na kterou atually máš bez prodeje orgánů");
            continue;
        }

        Console.WriteLine("Zadej počet čísel v automatu, minimum je 5(psst... od sedmi máš bonus)");
        if (!int.TryParse(Console.ReadLine(), out int range) || range < 5)
        {
            Console.WriteLine("Neplatná hodnota");
            continue;
        }

        money -= bet;
        int slot1, slot2, slot3;

        // Zarucena vyhra prvni 3 spiny
        spinCount++;

        if (spinCount <= 3)
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
        // Normalni vyhodnoceni
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

        Console.Write("\r" + slot1 + " " + slot2 + " " + slot3);
        Thread.Sleep(500);

        // Vyhodnoceni
        // Jackpot (3 matching)
        if (slot1 == slot2 && slot2 == slot3)
        {
            int win = bet * 10;

            if (range < 8)
            {
                win = (int)(win * ((double)range / 10 + 0.3));
            }

            Console.WriteLine($"\nJackpot! Výhral jsi {win} korun!");
            money += win;
        }
        // Big win (2 matching)
        else if (slot1 == slot2 || slot1 == slot3 || slot2 == slot3)
        {
            money = 1000;

            int win = bet * 2;

            if (range < 8)
            {
                win = (int)(win * ((double)range / 10 + 0.3));
            }
            else
            {
                win += range * 10;
            }

            Console.WriteLine($"\nBig win! Výhral jsi {win} korun!");
            money += win;
        }
        // Prohra, womp womp
        else
        {
            Console.WriteLine("\nBohužel jsi prohrál, ale zkus to znovu, tentokrát to určitě vyjde!");
        }
    }

    Thread.Sleep(650);
    Console.WriteLine("Bohužel ti došly peníze, končíš");
    return;
}
