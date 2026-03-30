using System.Diagnostics;

async Task<string> SimulateDownloadAsync(string fileName, int ms)
{

  if (fileName.Contains("fail", StringComparison.CurrentCultureIgnoreCase))
  {
    throw new InvalidOperationException($"Download failed for {fileName}");
  }


  Console.WriteLine($"Starting {fileName}...");
  await Task.Delay(ms);
  Console.WriteLine($"Finished {fileName}!");

  return $"Content of {fileName}";
}

async Task SpinnerUntilCompleted(Task t)
{
  char[] frames = ['|', '/', '-', '\\'];
  var i = 0;

  while (!t.IsCompleted)
  {
    int currentFrame = i % frames.Length;
    Console.WriteLine($"\rWorking {frames[currentFrame]}");
    await Task.Delay(100);
    i++;
  }
  Console.WriteLine("\rDone\t\t");
}

var stopwatch = Stopwatch.StartNew();

Console.WriteLine("== Sequential downloads ==");
var fileA = await SimulateDownloadAsync("fileA.txt", 1000);
var fileB = await SimulateDownloadAsync("fileB.txt", 1200);
stopwatch.Stop();
Console.WriteLine($"Sequential elapsed: {stopwatch.ElapsedMilliseconds} ms\n");


Console.WriteLine();
Console.WriteLine("== Parallel downloads (Task.WhenAll) ==");
stopwatch.Restart();
// var failedTask = SimulateDownloadAsync("fail.txt", 1000);
var task1 = SimulateDownloadAsync("fileC.txt", 1000);
var task2 = SimulateDownloadAsync("fileD.txt", 1200);
var taskResults = await Task.WhenAll(/*failedTask,*/ task1, task2);

foreach (var result in taskResults) Console.WriteLine($"Result: {result}");
stopwatch.Stop();
Console.WriteLine($"Parallel elapsed: {stopwatch.ElapsedMilliseconds} ms\n");

// var myTasks = new[]
// {
//   SimulateDownloadAsync("fileC.txt", 1200),
//   SimulateDownloadAsync("fileD.txt", 1200)
// };
// await Task.WhenAll(myTasks);


Console.WriteLine("== Error handling with async/await ==");
try
{
  await SimulateDownloadAsync("fail.txt", 1000);
  // var failedTask = SimulateDownloadAsync("fail.txt", 1000);
  // var task1 = SimulateDownloadAsync("fileC.txt", 1000);
  // var task2 = SimulateDownloadAsync("fileD.txt", 1200);
  // var taskResults = await Task.WhenAll(failedTask, task1, task2);
  // try Task.WhenAny() for granular control

  // foreach (var result in taskResults) Console.WriteLine($"Result: {result}");
}
catch (Exception ex)
{
  Console.WriteLine($"Caught error: {ex.Message}\n");
}

Console.WriteLine("== Non-blocking spinner while downloads run ==");
var longTask = Task.WhenAll(
                        SimulateDownloadAsync("fileE.txt", 2200),
                        SimulateDownloadAsync("fileF.txt", 2000));

var spinner = SpinnerUntilCompleted(longTask);

await Task.WhenAll(longTask, spinner);

Console.WriteLine("All done");