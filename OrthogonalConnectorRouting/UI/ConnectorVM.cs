using OrthogonalConnectorRouting.Graph;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OrthogonalConnectorRouting
{
    public class ConnectorVM : INotifyPropertyChanged
    {
        private double _algorithmTime = 0;
        private IOrthogonalPathFinder _orthogonalPathFinder;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<DesignerItem> DesignerItems { get; private set; }

        public ObservableCollection<Connection> Connections { get; private set; }

        public ObservableCollection<Point> Intersections { get; private set; }

        public ObservableCollection<Connection> Paths { get; private set; }

        public double RunTime
        {
            get { return this._algorithmTime; }
            set
            {
                this._algorithmTime = value;
                this.NotifyPropertyChanged("RunTime");
            }
        }

        public ConnectorVM()
        {
            this.DesignerItems = new ObservableCollection<DesignerItem>();
            this.Connections = new ObservableCollection<Connection>();
            this.Intersections = new ObservableCollection<Point>();
            this.Paths = new ObservableCollection<Connection>();

            // Add testing blocks
            this.DesignerItems.Add(new DesignerItem { X = 50, Y = 150, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 400, Y = 130, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 440, Y = 50, Height = 50, Width = 150 });
            this.DesignerItems.Add(new DesignerItem { X = 420, Y = 270, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 270, Y = 110, Height = 150, Width = 50 });

            this._orthogonalPathFinder = new OrthogonalPathFinder();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            this.Connections = new ObservableCollection<Connection>(this._orthogonalPathFinder.CreateLeadLines(
                                                                                DesignerItems.ToList(), 800, 450));
            foreach (var conn in this.Connections)
            {
                foreach (var conn2 in this.Connections)
                {
                    if (conn == conn2)
                    {
                        continue;
                    }

                    var intersection = this._orthogonalPathFinder.FindIntersection(conn, conn2);
                    if (intersection.HasValue && !this.Intersections.Contains(intersection.Value))
                    {
                        this.Intersections.Add(intersection.Value);
                    }
                }
            }

            this._orthogonalPathFinder.ConstructGraph(this.Intersections.ToList().OrderBy(x => x.X).ThenBy(y => y.Y).ToList());
            var shortestPath = this._orthogonalPathFinder.ShortestPath(new Node(50, 175),
                                                                       new Node(550, 295));


            foreach (var pathEdge in shortestPath.pathEdges)
            {
                this.Paths.Add(new Connection(pathEdge.Source.Position, pathEdge.Destination.Position));
            }

            sw.Stop();
            Console.WriteLine($"Algorithm time: {sw.ElapsedMilliseconds} ms");
            this.RunTime = sw.ElapsedMilliseconds;
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
