using System;
using System.IO;
using System.Diagnostics;

// TODO: Save/load dictionary to file so tags persist between runs

// One-off personal tagging tool – not hardened, not user-friendly. MVP and nothing more.
class Program
{
    const string ClipPath = @"C:\Users\jacob\Videos\Krunker\Clips";
    static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>(); 
    static void Main(string[] args)
    {
        bool mainLoop = true;
        do
        {
            Console.WriteLine("1: Create base dictionary\n" +
                              "2: List entries\n" +
                              "3: Begin tagging\n" +
                              "e: Exit\n");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1": CreateBaseDictionary(); break;
                case "2": ListEntries(); break;
                case "3": BeginTagging(); break;
                case "e": mainLoop = false; break;
                default: Console.WriteLine("Invalid input"); break;
            }
        }
        while (mainLoop);
    }

    static void BeginTagging()
    {
        List<string> clips = dictionary.Keys.ToList();
        bool exit = false;
        Console.WriteLine("Where to begin?\n");
        string clipI = Console.ReadLine();
        if (!int.TryParse(clipI, out int i))
        {
            Console.WriteLine("Invalid number, starting at 0.");
            i = 0;
        }
        while (i < clips.Count && i >= 0)
        {
            Console.WriteLine("Currently watching clip " + i);
            string key = clips[i];
            bool taggingLoop = true;
            while (taggingLoop)
            {
                Console.WriteLine("1: View clip\n" +
                                  "2: Next clip\n" +
                                  "3: List clip tags\n" +
                                  "4: Previous clip\n" +
                                  "e: Exit\n" +
                                  "Enter anything else to tag the current clip\n");
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1": PlayClip(key); break;
                    case "2":
                        taggingLoop = false;
                        i++;
                        break;
                    case "3": ListClipTags(key); break;
                    case "4": i = Math.Max(0, i - 1);
                        taggingLoop = false; break;
                    case "e":
                        taggingLoop = false;
                        exit = true;
                        break;
                    default: dictionary[key].Add(input); break;
                }
            }

            if (exit) break;
        }
    }

    static void ListClipTags(string key)
    {
        foreach (string tag in dictionary[key])
        {
            Console.WriteLine(tag);
        }
    }

    static void PlayClip(string clip)
    {
        Process.Start(
            new ProcessStartInfo
            {
                FileName = clip, 
                UseShellExecute = true
            });
    }
    static void CreateBaseDictionary()
    {
        string[] clips = Directory.GetFiles(ClipPath, "*.mkv");
        foreach (string clip in clips)
        {
            dictionary[clip] = new List<string>();
        }
    }
    
    static void ListEntries()
    {
        foreach (string clip in dictionary.Keys)
        {
            Console.WriteLine(clip);
        }
    }
}
