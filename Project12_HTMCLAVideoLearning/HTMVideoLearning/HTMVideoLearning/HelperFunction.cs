﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HTMVideoLearning
{
    public class HelperFunction
    {
        /// <summary>
        /// Print a line in Console with color and/or hightlight
        /// </summary>
        /// <param name="str">string to print</param>
        /// <param name="foregroundColor">Text color</param>
        /// <param name="backgroundColor">Hightlight Color</param>
        public static void WriteLineColor(
            string str, 
            ConsoleColor foregroundColor = ConsoleColor.White, 
            ConsoleColor backgroundColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = foregroundColor;
            Console.BackgroundColor = backgroundColor;
            Console.WriteLine(str);
            Console.ResetColor();
        }
        public static string[] GetVideoSetPaths(string trainingFolderPath)
        {
            // remove the two outer quotation marks
            trainingFolderPath = trainingFolderPath.Replace("\"", "");
            string[] videoSetPaths = { };
            string testDir;
            if (Directory.Exists(trainingFolderPath))
            {
                testDir = trainingFolderPath;
                HelperFunction.WriteLineColor($"Inserted Path is a folder", ConsoleColor.Green);
                Console.WriteLine($"Begin reading directory: {trainingFolderPath} ...");
            }
            else
            {
                string currentDir = Directory.GetCurrentDirectory();
                HelperFunction.WriteLineColor($"The inserted path for the training folder is invalid. " +
                    $"If you have trouble adding the path, copy your training folder with name TrainingVideos to {currentDir}",ConsoleColor.Yellow);
                // Get the root path of training videos.
                testDir = $"{currentDir}\\TrainingVideos";
            }
            // Get all the folders that contain video sets under TrainingVideos/
            try
            {
                videoSetPaths = Directory.GetDirectories(testDir, "*", SearchOption.TopDirectoryOnly);
                HelperFunction.WriteLineColor("Complete reading directory ...");
                return videoSetPaths;
            }
            catch(Exception e)
            {
                WriteLineColor("=========== Caught exception ============", ConsoleColor.Magenta);
                WriteLineColor(e.Message, ConsoleColor.Magenta);
                return videoSetPaths;
            }
        }
    }
}
