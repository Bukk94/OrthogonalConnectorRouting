﻿using OrthogonalConnectorRouting;
using OrthogonalConnectorRouting.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace SampleApp
{
    public class ConnectorVM : INotifyPropertyChanged
    {
        private readonly IOrthogonalPathFinder _orthogonalPathFinder;
        private Connector _lastConnector;
        private double _algorithmTime = 0;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IInput> DesignerItems { get; private set; }

        public ObservableCollection<Connection> Connections { get; private set; }

        public ObservableCollection<Point> Intersections { get; private set; }

        public ObservableCollection<Connection> Paths { get; private set; }

        public ICommand AddConnectorCommand { get; set; }

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
            this.DesignerItems = new ObservableCollection<IInput>();
            this.Connections = new ObservableCollection<Connection>();
            this.Intersections = new ObservableCollection<Point>();
            this.Paths = new ObservableCollection<Connection>();

            this._orthogonalPathFinder = new OrthogonalPathFinder();

            // Register Commands
            this.AddConnectorCommand = new RelayParamCommand((param) => this.AddConnector(param));

            this.RunSample();
        }

        private void RunSample()
        {
            // Add testing blocks
            this.DesignerItems.Add(new DesignerItem { X = 50, Y = 150, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 400, Y = 130, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 440, Y = 50, Height = 50, Width = 150 });
            this.DesignerItems.Add(new DesignerItem { X = 420, Y = 270, Height = 50, Width = 130 });
            this.DesignerItems.Add(new DesignerItem { X = 270, Y = 110, Height = 150, Width = 50 });

            Stopwatch sw = new Stopwatch();
            sw.Start();

            // Add test route
            var connector = new Connector()
            {
                Source = this.DesignerItems[0],
                SourceOrientation = ConnectorOrientation.Left,
                Destinaton = this.DesignerItems[3],
                DestinationOrientation = ConnectorOrientation.Right
            };

            var results = this._orthogonalPathFinder.OrthogonalPath(DesignerItems.ToList(), 800, 450, connector);

            this.Connections = new ObservableCollection<Connection>(results.Connections);
            this.Intersections = new ObservableCollection<Point>(results.Intersections);

            this.DrawPath(connector);

            sw.Stop();
            Console.WriteLine($"Algorithm time: {sw.ElapsedMilliseconds} ms");
            this.RunTime = sw.ElapsedMilliseconds;
        }

        #region Draw methods
        private void DrawPath(Connector connector)
        {
            foreach (var path in connector.ConnectorPath)
            {
                this.Paths.Add(path);
            }
        }
        #endregion

        private void AddConnector(object param)
        {
            var rect = (System.Windows.Shapes.Rectangle)param;
            var relativeCoords = Mouse.GetPosition((IInputElement)param);
            if (_lastConnector == null)
            {
                _lastConnector = new Connector
                {
                    Source = rect.DataContext as IInput
                };

                _lastConnector.SourceOrientation = this.CalculateOrientation(_lastConnector.Source, relativeCoords);
            }
            else
            {
                _lastConnector.Destinaton = rect.DataContext as IInput;
                _lastConnector.DestinationOrientation = this.CalculateOrientation(_lastConnector.Destinaton, relativeCoords);
                this._orthogonalPathFinder.CalculatePathForConnector(_lastConnector);
                this.DrawPath(_lastConnector);
                _lastConnector = null;
            }
        }

        private ConnectorOrientation CalculateOrientation(IInput item, Point relativeCoords)
        {
            var coords = new List<Point>
            {
                new Point(0, item.Height / 2), // LEFT
                new Point(item.Width / 2, 0),   // TOP
                new Point(item.Width, item.Height / 2),  // RIGHT
                new Point(item.Width / 2, item.Height) // BOTTOM
            };

            Point closestPoint = new Point();
            double closestDistanceSquared = double.MaxValue;
            foreach (var point in coords)
            {
                var distanceSquared = Math.Pow(point.X - relativeCoords.X, 2) + Math.Pow(point.Y - relativeCoords.Y, 2);

                if (distanceSquared < closestDistanceSquared)
                {
                    closestDistanceSquared = distanceSquared;
                    closestPoint = point;
                }
            }

            var orientation = coords.IndexOf(closestPoint);
            return (ConnectorOrientation)Enum.Parse(typeof(ConnectorOrientation), orientation.ToString());
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
