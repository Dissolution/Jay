using System.Diagnostics;

namespace ConsoleSandbox;

public class MontyHall
{
    public int Iterations { get; } = 1_000_000;

    public Random Random { get; } = new Random();
    
    
    public MontyHall()
    {
        
    }

    private int NotDoor(int door)
    {
        int r;
        while ((r = Random.Next(0,3)) == door) { }
        Debug.Assert(r != door);
        return r;
    }
    
    private int NotDoors(int guessDoor, int openDoor)
    {
        var lastDoor = (0 + 1 + 2) - (guessDoor) - (openDoor);
        Debug.Assert(lastDoor != guessDoor);
        Debug.Assert(lastDoor != openDoor);
        return lastDoor;
    }

    private int EmptyDoor(int guessDoor, int prizeDoor)
    {
        int emptyDoor;
        if (guessDoor == prizeDoor)
        {
            emptyDoor = NotDoor(guessDoor);
        }
        else
        {
            emptyDoor = NotDoors(guessDoor, prizeDoor);
        }
        Debug.Assert(emptyDoor != guessDoor);
        Debug.Assert(emptyDoor != prizeDoor);
        return emptyDoor;
    }
    
    public void Test()
    {
        var random = Random;
        long stayWins = 0L;
        long switchWins = 0L;
        long houseWins = 0L;
        double div;
        
        for (var i = 0; i < Iterations; i++)
        {
            // Determine the prize door (0,1,2) at random
            var prizeDoor = random.Next(0, 3);
            
            // start with a random guess
            var guess = random.Next(0, 3);
            
            // open a non-prize door
            var openDoor = EmptyDoor(guess, prizeDoor);
           
            // Are we a stay or a switch?
            bool stay = random.Next(0, 2) == 0;
            int guessDoor;
            if (stay)
            {
                guessDoor = guess;
            }
            else
            {
                guessDoor = NotDoors(guess, openDoor);
            }

            if (guessDoor == prizeDoor)
            {
                if (stay) stayWins++;
                else switchWins++;
            }
            else
            {
                // House wins (in the end)
                houseWins++;
            }

            // Every 100, print the percentages
            if (i % 100 == 0)
            {
                div = (double)i;
                Console.WriteLine($"Stay: {stayWins/div:P2}%  |  Switch: {switchWins/div:P2}%  |  House: {houseWins/div:P2}");
            }
        }

        div = (double)Iterations;
        Console.WriteLine("Finished Totals:");
        Console.WriteLine($"Stay: {stayWins/div:P0}%  |  Switch: {switchWins/div:P0}%");
    }
}