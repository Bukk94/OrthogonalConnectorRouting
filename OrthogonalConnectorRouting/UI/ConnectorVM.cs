using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class ConnectorVM
    {
        public ObservableCollection<DesignerItem> AllItems { get; private set; }

        public ObservableCollection<Connection> Connections { get; private set; }

        public ObservableCollection<Point> Intersections { get; private set; }

        public ConnectorVM()
        {
            AllItems = new ObservableCollection<DesignerItem>();
            Connections = new ObservableCollection<Connection>();
            Intersections = new ObservableCollection<Point>();

            AllItems.Add(new DesignerItem { X = 50, Y = 150, Height = 50, Width = 130 });
            AllItems.Add(new DesignerItem { X = 400, Y = 130, Height = 50, Width = 130 });
            AllItems.Add(new DesignerItem { X = 440, Y = 50, Height = 50, Width = 150 });
            AllItems.Add(new DesignerItem { X = 420, Y = 270, Height = 50, Width = 130 });
            AllItems.Add(new DesignerItem { X = 270, Y = 110, Height = 150, Width = 50 });

            OrthogonalPathFinder pathFinder = new OrthogonalPathFinder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Connections = new ObservableCollection<Connection> (pathFinder.CreateLeadLines(AllItems.ToList(), 800, 450));

            foreach (var conn in Connections)
            {
                foreach (var conn2 in Connections)
                {
                    if (conn == conn2) continue;

                    var intersection = pathFinder.FindIntersection(conn, conn2);
                    if (intersection.HasValue && !Intersections.Contains(intersection.Value))
                    {
                        Intersections.Add(intersection.Value);
                    }
                }
            }

            pathFinder.ConstructGraph(Intersections.ToList().OrderBy(x => x.X).ThenBy(y => y.Y).ToList());
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }
    }
}
