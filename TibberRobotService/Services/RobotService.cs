using System;
using System.Collections.Generic;
using System.Diagnostics;
using IntervalTree;
using TibberRobotService.Models;
using Newtonsoft.Json;
using Advanced.Algorithms.Geometry;
namespace TibberRobotService.Services
{
    public class RobotService : IRobotService
    {
        public Execution CalculatePathAndClean(PathInputModel startCommand)
        {
            var stopwatch = Stopwatch.StartNew();


            (int X, int Y) currentPoint = (startCommand.Start.X, startCommand.Start.Y);

            var lines = new List<Line>();
            var edges = new HashSet<(int, int)>
    {
        (currentPoint.X, currentPoint.Y)
    };

            int count = 1;
            foreach (var command in startCommand.Commands)
            {
                // Calculate the movement
                int dx = 0, dy = 0;
                switch (command.Direction.ToLower())
                {
                    case "east":
                        dx = 1;
                        break;
                    case "west":
                        dx = -1;
                        break;
                    case "north":
                        dy = 1;
                        break;
                    case "south":
                        dy = -1;
                        break;
                    default:
                        throw new ArgumentException($"Invalid direction: {command.Direction}");
                }

                int endX = currentPoint.X + (dx * command.Steps);
                int endY = currentPoint.Y + (dy * command.Steps);

                (int X, int Y) nextPoint = (endX, endY);

                var newLine = new Line(new Point(currentPoint.X, currentPoint.Y), new Point(nextPoint.X, nextPoint.Y));
                if (newLine.IsHorizontal == lines.LastOrDefault()?.IsHorizontal || newLine.IsVertical == lines.LastOrDefault()?.IsVertical)
                {
                    // Get the last line
                    var lastLine = lines.LastOrDefault();

                    // Check if the lines overlap
                    if (lastLine != null && newLine.IsHorizontal && newLine.Left.Y == lastLine.Left.Y)
                    {
                        // The lines are horizontal and on the same line, check if their x-ranges overlap
                        if (Math.Max(newLine.Left.X, lastLine.Left.X) <= Math.Min(newLine.Right.X, lastLine.Right.X))
                        {
                            // The x-ranges overlap, so calculate the length of the non-overlapping part of the new line
                            int nonOverlappingLength = Math.Abs((int)(newLine.Right.X - Math.Max(newLine.Left.X, lastLine.Right.X)));
                            count += nonOverlappingLength;
                        }
                    }
                    else if (lastLine != null && newLine.IsVertical && newLine.Left.X == lastLine.Left.X)
                    {
                        // The lines are vertical and on the same line, check if their y-ranges overlap
                        if (Math.Max(newLine.Left.Y, lastLine.Left.Y) <= Math.Min(newLine.Right.Y, lastLine.Right.Y))
                        {
                            // The y-ranges overlap, so calculate the length of the non-overlapping part of the new line
                            int nonOverlappingLength = Math.Abs((int)(newLine.Right.Y - Math.Max(newLine.Left.Y, lastLine.Right.Y)));
                            count += nonOverlappingLength;
                        }
                    }
                } else {
                    if (edges.Contains((nextPoint.X, nextPoint.Y)))
                    {
                        count--;
                    }
                    count += Math.Abs((int)(newLine.Right.X - newLine.Left.X)) + Math.Abs((int)(newLine.Right.Y - newLine.Left.Y));
                }

                currentPoint = nextPoint;

                edges.Add((nextPoint.X, nextPoint.Y));
                lines.Add(newLine);
            }

            // offset all points to positive
            var min = (edges.Min(x => x.Item1), edges.Min(x => x.Item2));
            int offset = 0;
            if (min.Item1 < 0 || min.Item2 < 0)
            {
                offset = Math.Abs(min.Item1) > Math.Abs(min.Item2) ? Math.Abs(min.Item1) : Math.Abs(min.Item2);
                edges = new HashSet<(int, int)>(edges.Select(x => (x.Item1 + offset, x.Item2 + offset)));
                lines = new List<Line>(lines.Select(x => new Line(new Point(x.Left.X + offset, x.Left.Y + offset), new Point(x.Right.X + offset, x.Right.Y + offset))));
            }

            var bentleyOttmanAlgorithm = new BentleyOttmann();
            var intersections = bentleyOttmanAlgorithm.FindIntersections(lines.ToArray())
            .Where((p, _) => !edges.Contains(((int)p.Key.X, (int)p.Key.Y))).ToList();

            count -= intersections.Count;
            stopwatch.Stop();

            return new Execution
            {
                Commands = startCommand.Commands.Count,
                Duration = stopwatch.Elapsed.TotalSeconds,
                Result = count
            };
        }

    }
}
