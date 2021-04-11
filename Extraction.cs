using System;
using System.IO;
using System.Diagnostics;
#nullable enable
namespace ChromosomeExtraction
{
    class Extraction
    {
        const string startString = "ttggtaccat";
        const string endString = "TAATTGCAGT";
        const string endStringLowered = "taattgcagt";
        public static void Extract(int startNo, int endNo, bool shouldOpen = false)
        {

            string filePath;
            string pathInProject = "../../../GRCh38_latest_genomic.fna";
            string pathRoot = "./GRCh38_latest_genomic.fna";
            bool startFound = false;
            bool useConsole = false;
            int startBaseNo = -1;
            int endBaseNo = -1;
            string? line;
            
            filePath = File.Exists(pathInProject) ? pathInProject : pathRoot;
            
            using StreamReader reader = new StreamReader(filePath);
            _ = reader.ReadLine();
            int baseNumber;
            for (baseNumber = 0; baseNumber < startNo; baseNumber++)
            {
                _ = reader.ReadLine();
            }

            for (; baseNumber < endNo; baseNumber++)
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    Debug.WriteLine("reached end of original file");
                    break;
                }
                // mark start position
                // condition should be taken only once
                if (line.Contains(startString))
                {
                    startBaseNo = baseNumber;
                    startFound = true;
                }
                else if (startFound && line.Contains(endString))
                {
                    endBaseNo = baseNumber;
                    break;
                }
            }
            Debug.WriteLine("start:" + startBaseNo + "end:" + endBaseNo);
            // OK

            // found start and end
            // now read again and output the desired parts
            using (StreamWriter writer = useConsole ?
                new(Console.OpenStandardOutput()) : new(@"SParts" + startNo + "_" + endNo + ".txt") )
            {
                line = "";
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
                if (startBaseNo == -1 || endBaseNo == -1)
                {
                    throw new Exception();
                }
                int lineNo;
                // skipped unwanted parts
                for (lineNo = 0; lineNo < startBaseNo; lineNo++)
                {
                    _ = reader.ReadLine();
                }
                for (; lineNo < endBaseNo+1; lineNo++)
                {
                    line = reader.ReadLine();
                    if (line == null)
                    {
                        Debug.WriteLine("reached EOF");
                        break;
                    }
                    writer.WriteLine(line.ToLower());
                }
                Debug.WriteLine(line!.ToLower());
            }// end writing SParts
            using StreamReader sReader = new(@"SParts" + startNo + "_" + endNo + ".txt"); 
            using StreamWriter sWriter = new(@"S." + startNo + "_" + endNo + ".txt");
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
            if (line == null)
            {
                Debug.WriteLine("Should not reach EOF");
                throw new Exception();
            }
            if(!useConsole && shouldOpen)
            {
                string editorPath = File.Exists(@"F:\Apps\Microsoft VS Code\Code.exe") ?
                    @"F:\Apps\Microsoft VS Code\Code.exe" : @"C:\Program Files\Microsoft VS Code\Code.exe";
                try
                {
                    //Process.Start(editorPath, @"output.txt");
                    Process.Start(editorPath, @"SParts"+startNo+"_"+endNo+".txt");
                    Process.Start(editorPath, @"S." + startNo + "_" + endNo + ".txt");
                }
                catch(Exception ex)
                {
                    Console.WriteLine("find files by your self!");
                }
            }
        }// end main
    }
}
