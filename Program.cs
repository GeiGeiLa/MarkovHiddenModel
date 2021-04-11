using System;
using System.IO;
using System.Diagnostics;
#nullable enable
namespace ChromosomeExtraction
{
    class Program
    {
        const string startString = "ttggtaccat";
        const string endString = "TAATTGCAGT";
        const string endStringLowered = "taattgcagt";
        static void Main(string[] args)
        {

            string filePath;
            string pathInProject = "../../../GRCh38_latest_genomic.fna";
            string pathRoot = "./GRCh38_latest_genomic.fna";
            bool startFound = false;
            bool useConsole = false;
            int startPosition = -1;
            int endPosition = -1;
            string? line;
            if (args.Length == 0)
            {
                filePath = File.Exists(pathInProject) ? pathInProject : pathRoot;
            }
            else
            {
                filePath = args[0];
            }
            using StreamReader reader = new StreamReader(filePath);
            using (StreamWriter writer = useConsole ?
                new(Console.OpenStandardOutput()) : new("output.txt"))
            {
                writer.WriteLine(reader.ReadLine() + "\nPress \'Enter\' to read next line, '\'c\' to break");
                if (useConsole)
                {
                    writer.AutoFlush = true;
                    Console.SetOut(writer);
                }
                int baseNumber;
                for (baseNumber = 0; baseNumber < 100000; baseNumber++)
                {
                    _ = reader.ReadLine();
                }

                for (; baseNumber < 1099999; baseNumber++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        Debug.WriteLine("reached end of file");
                        break;
                    }
                    // write specified range of data to output.txt
                    writer.WriteLine(line.ToLower());
                    // mark start position
                    // condition should be taken only once
                    if (!startFound && line.Contains(startString))
                    {
                        startPosition = baseNumber;
                        startFound = true;
                    }
                    else if (startFound && line.Contains(endString))
                    {
                        endPosition = baseNumber;
                    }
                }
            }
            Debug.WriteLine("start:" + startPosition + "end:" + endPosition);

            // found start and end
            // now read again and output the desired parts
            using (StreamWriter writer = useConsole ?
                new(Console.OpenStandardOutput()) : new("SParts.txt") )
            {
                if (useConsole)
                {
                    writer.AutoFlush = true;
                    Console.SetOut(writer);
                }
                // return to the beginning of file
                reader.DiscardBufferedData();
                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                reader.BaseStream.Position = 0;
                // skip the title line
                Debug.WriteLine(reader.ReadLine() );
                if (startPosition == -1 || endPosition == -1)
                {
                    throw new Exception();
                }
                int lineNo;
                // skipped unwanted parts
                for (lineNo = 0; lineNo < startPosition; lineNo++)
                {
                    _ = reader.ReadLine();
                }
                for (; lineNo < endPosition; lineNo++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        Debug.WriteLine("reached EOF");
                        break;
                    }
                    writer.WriteLine(line.ToLower());
                }
            }// end writing SParts
            using StreamReader sReader = new(@"SParts.txt"); 
            using StreamWriter sWriter = new(@"S.txt");
            line = sReader.ReadLine();
            int startColumnNo = line!.IndexOf(startString);
            sWriter.WriteLine(line.Substring(startColumnNo));
            while( (line = sReader.ReadLine() ) != null )
            {
                if( line.Contains(endStringLowered) )
                {
                    Debug.WriteLine("reached end");
                    int endColumnNo = line.IndexOf(endStringLowered);
                    sWriter.WriteLine(line.Substring(0, endColumnNo + endString.Length));
                    break;
                }
                sWriter.WriteLine(line);
            }
            if (line == null) throw new NullReferenceException();
            if(!useConsole)
            {
                Process.Start(@"C:\Program Files\Microsoft VS Code\Code.exe", @"output.txt");
                Process.Start(@"C:\Program Files\Microsoft VS Code\Code.exe", @"SParts.txt");
                Process.Start(@"C:\Program Files\Microsoft VS Code\Code.exe", @"S.txt");
            }
        }// end main
    }
}
