Queue<string?> requestQueue = new();

int availableSeats = 10;

Lock @lock = new();

// 2. Start a thread to monitor the queue and process requests
Thread monitoringThread = new(MonitorQueue);
monitoringThread.Start();

// 1. Enqueue some requests
Console.WriteLine("Server is running. \r\nType 'b' to Book. \r\nType 'c' to Cancel. \r\nType 'exit' to quit:");
while (true)
{
    string? input = Console.ReadLine();
    if(input?.ToLower() == "exit")
    {
        break;
    }

    requestQueue.Enqueue(input);
}

void MonitorQueue()
{
    while (true)
    {
        if (requestQueue.Count > 0)
        {
            string? input = requestQueue.Dequeue();
            Thread processingThread = new Thread(() => ProcessBooking(input));
            processingThread.Start();
        }
        Thread.Sleep(100); // Sleep to prevent busy waiting
    }
}

// 3. Method to process requests
void ProcessBooking(string? input)
{
    if (input != null)
    {
        Console.WriteLine($"Processing request: {input}");
        // Simulate processing time
        Thread.Sleep(2000);

        lock (@lock)
        {
            if (input == "b")
            {
                if (availableSeats > 0)
                {
                    availableSeats--;
                    Console.WriteLine("Booking successful. Remaining seats: " + availableSeats);
                }
                else
                {
                    Console.WriteLine($"Tickets are not available.");
                }
            }
            else if (input == "c")
            {
                if (availableSeats < 10)
                {
                    availableSeats++;
                    Console.WriteLine("Cancellation successful. Remaining seats: " + availableSeats);
                }
                else
                {
                    Console.WriteLine($"Error. You cannot cancel a booking at this time.");
                }
            }
        }

        Console.WriteLine($"Finished processing...");
    }
}
