using System;
using System.Text.Json;
using System.IO;
using System.Diagnostics;

// One-off personal tagging tool – not hardened, not user-friendly. MVP and nothing more.
class Program
{
    static string clipPath = @"";
    static Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>(); 
    static string savePath = Path.Combine(AppContext.BaseDirectory, "save");
    static void Main(string[] args)
    {
        Load();
        bool mainLoop = true;
        do
        {
            Console.WriteLine("1: Manage dictionary\n" +
                              "2: Save\n" +
                              "3: Begin tagging\n" +
                              "e: Exit\n");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1": ManageDictionary(); break;
                case "2": Save(); break;
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
                FileName = Path.Combine(clipPath, clip),
                UseShellExecute = true
            });
    }

    static void ManageDictionary()
    {
        bool dictionaryLoop = true;
        do
        {
            Console.WriteLine("1: New file path\n" +
                              "2: Create base dictionary (from file path)\n" +
                              "3: List entries\n" +
                              "e: Exit\n");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1": EditPath(); break;
                case "2": CreateBaseDictionary(); break;
                case "3": ListEntries(); break;
                case "e": dictionaryLoop = false; break;
                default: Console.WriteLine("Invalid input"); break;
            }
        }
        while (dictionaryLoop);
    }
    static void EditPath()
    {
        Console.WriteLine("Please input the path to the clips folder\n"+
                          "This will overwrite any already created dictionary, input \"e\" to exit\n"+
                          "Expected format: C:\\your\\file\\path\\\n");
        string input = Console.ReadLine();
        if (input != "e" && input != "")
        {
            clipPath = input;
        } //else exit
    }
    static void CreateBaseDictionary()
    {
        string[] clips = Directory.GetFiles(clipPath, "*.mkv");
        if (clipPath != "" && clips.Length > 0)
        {
            foreach (string clip in clips)
            {
                string fileName = Path.GetFileName(clip);
                dictionary[fileName] = new List<string>();
            }
            Console.WriteLine("The length of the dictionary is: " + dictionary.Count);
            Console.WriteLine("The length of the clips is: " + clips.Length);
        }
    }

    static void Save()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true 
        };
        
        string json = JsonSerializer.Serialize(dictionary, options);
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        File.WriteAllText(Path.Combine(savePath, "tags.json"), json);
        File.WriteAllText(Path.Combine(savePath, "clip-path.json"), clipPath);
    }

    static void Load()
    {
        string tags = Path.Combine(savePath, "tags.json");
        if (File.Exists(tags))
        {
            string json = File.ReadAllText(tags);
            dictionary = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(json);
        }
        string clips = Path.Combine(savePath, "clip-path.json");
        if (File.Exists(clips))
        {
            clipPath = File.ReadAllText(clips);
        }
    }
    
    static void ListEntries()
    {
        foreach (string clip in dictionary.Keys)
        {
            string tags = "";
            foreach (string tag in dictionary[clip])
            {
                tags += tag + ", ";
            }
            Console.WriteLine(clip + ": " + tags);
        }
    }
}