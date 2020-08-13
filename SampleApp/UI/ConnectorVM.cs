using OrthogonalConnectorRouting;
using OrthogonalConnectorRouting.Enums;
using OrthogonalConnectorRouting.Models;
using System;
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

        public ObservableCollection<OrthogonalConnectorRouting.Models.Point> Intersections { get; private set; }

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
            this.Intersections = new ObservableCollection<OrthogonalConnectorRouting.Models.Point>();
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

            var results = this._orthogonalPathFinder.OrthogonalPath(DesignerItems.ToList(), 800, 450, SearchAlgorithm.Dijkstra, connector);

            this.Connections = new ObservableCollection<Connection>(results.Connections);
            this.Intersections = new ObservableCollection<OrthogonalConnectorRouting.Models.Point>(results.Intersections);

            this.DrawPath(connector);

            sw.Stop();
            Console.WriteLine($"Algorithm time: {sw.ElapsedMilliseconds} ms");
            this.RunTime = sw.ElapsedMilliseconds;
        }

        private void DrawPath(Connector connector)
        {
            foreach (var path in connector.ConnectorPath)
            {
                this.Paths.Add(path);
            }
        }

        private void AddConnector(object param)
        {
            var rect = (System.Windows.Shapes.Rectangle)param;
            var mousePosition = Mouse.GetPosition((IInputElement)param);
            var relativeCoords = new OrthogonalConnectorRouting.Models.Point(mousePosition.X, mousePosition.Y);

            if (_lastConnector == null)
            {
                _lastConnector = new Connector
                {
                    Source = rect.DataContext as IInput
                };

                _lastConnector.SourceOrientation = this._orthogonalPathFinder.CalculateOrientation(_lastConnector.Source, relativeCoords);
            }
            else
            {
                _lastConnector.Destinaton = rect.DataContext as IInput;
                _lastConnector.DestinationOrientation = this._orthogonalPathFinder.CalculateOrientation(_lastConnector.Destinaton, relativeCoords);
                this._orthogonalPathFinder.CalculatePathForConnector(_lastConnector, SearchAlgorithm.Dijkstra);
                this.DrawPath(_lastConnector);
                _lastConnector = null;
            }
        }

        private void NotifyPropertyChanged(String propertyName = "")
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
