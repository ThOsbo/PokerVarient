using System;

namespace CardGames
{
    class Program
    {
        static void Main(string[] args)
        {
            Poker poker = new Poker();

            poker.PlayPoker();

            Console.ReadKey();
        }
    }
}
