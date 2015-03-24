using GraphX;
using GraphX.Controls;
using GraphX.GraphSharp.Algorithms.Layout.Simple.Tree;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DComposer
{
    /// <summary>
    /// Interaction logic for DialogWindow.xaml
    /// </summary>
    public partial class DialogWindow : Window
    {
        Graph mGraph;
        DataVertex mRootNode;
        string mOpenFile = "";
        public DialogWindow()
        {
            InitializeComponent();

            ZoomControl.SetViewFinderVisibility(zoomctrl, Visibility.Visible);
            //Set Fill zooming strategy so whole graph will be always visible
            zoomctrl.ZoomToFill();
           
            SetupGraph();

            Loaded += DialogWindow_Loaded;

           


        }

        void DialogWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var json = args[1];
                Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(json));
                this.Title += " " + System.IO.Path.GetFileName(json);



                if (File.Exists(json))
                {

                    Area.ClearLayout();
                    mGraph.Clear();
                    var page = DialogLoader.Load(json);
                    BuildGraph(page, null);
                    mRootNode = page.Vertex;

                    Area.GenerateGraph(true, true);
                    zoomctrl.ZoomToFill();
                    Area.RelayoutGraph();
                }
                else
                {
                    SetupGraph();


                    DialogPage page = new DialogPage(this);
                    mRootNode = createNewVertex(page);
                    mRootNode.Page.Label = "Starting Point";
                    AddNode(mRootNode);



                    Area.GenerateGraph(true, true);
                    zoomctrl.ZoomToFill();
                    Area.RelayoutGraph();
                }

                mOpenFile = json;

            }
        }

        private void MenuItem_New_Click(object sender, RoutedEventArgs e)
        {
            SetupGraph();


            DialogPage page = new DialogPage(this);
            mRootNode = createNewVertex(page);
            mRootNode.Page.Label = "Starting Point";          
            AddNode(mRootNode);



            Area.GenerateGraph(true, true);
            zoomctrl.ZoomToFill();
            Area.RelayoutGraph();
        }

        private void BuildGraph(DialogPage page, DialogLabel parent)
        {
            page.setOwner(this);
            DataVertex dv;
            if (parent != null)
            {
                if (parent is DialogOption)
                {
                    dv = ((DialogOption)parent).Target.Vertex;
                }
                else
                {
                    dv = parent.Vertex;
                }
            }
            else
            {
                dv = createNewVertex(page);
                AddNode(dv);
            }

            foreach(var option in page.Options)
            {
                if (option.Target != null)
                {
                    var optionDv = createNewVertex(option.Target);
                    AddNode(optionDv);
                    LinkNodes(dv, optionDv);
                    option.Enabled = true;
                    option.Target.OptionOwner = option;
                    option.Target.Vertex = optionDv;
                    option.Target.PageOwner = page;
                    option.Target.Label = option.Label;
                    option.Parent = page;
                    option.Command = option.Command;

                    BuildGraph(option.Target, option);
                }
            }
        }


        private void MenuItem_Load_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.DefaultExt = ".json";
            openDlg.Filter = "Dialog files (*.json)|*.json";

            Nullable<bool> result = openDlg.ShowDialog();

            if (result == true)
            {
                Area.ClearLayout();
                mGraph.Clear();

                mOpenFile = openDlg.FileName;
                Directory.SetCurrentDirectory(System.IO.Path.GetDirectoryName(openDlg.FileName));
                var page = DialogLoader.Load(openDlg.FileName);
                BuildGraph(page, null);
                mRootNode = page.Vertex;

                Area.GenerateGraph(true, true);
                zoomctrl.ZoomToFill();
                Area.RelayoutGraph();
            }
        
        }

        private void MenuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            if ( !String.IsNullOrWhiteSpace(mOpenFile) )
            {
                var page = mRootNode.Page as DialogPage;
                DialogLoader.Save(page, mOpenFile);
            }
            else
            {
                MenuItem_SaveAs_Click(sender, e);
            }
        }

        private void MenuItem_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            var page = mRootNode.Page as DialogPage;

            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.FileName = mOpenFile;
            saveDlg.DefaultExt = ".json";
            saveDlg.Filter = "Dialog files (*.json)|*.json";

            Nullable<bool> result = saveDlg.ShowDialog();
            if (result == true)
            {
                DialogLoader.Save(page, saveDlg.FileName);
                mOpenFile = saveDlg.FileName;
            }
        }
        
        private void onNodeClick(object node)
        {
            DataVertex dv = node as DataVertex;
            var page = dv.Page;

            PageEditor pe = new PageEditor();
            pe.Owner = this;

            if (page is DialogPage)
            {
                DialogPage dp = page as DialogPage;

                if (dp.isSubTreeCommand)
                {
                    var jsonFile = dp.OptionOwner.Command;

                    string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                    string path = Uri.UnescapeDataString(new UriBuilder(codeBase).Path);

                    var file = System.IO.Path.Combine(Directory.GetCurrentDirectory(), jsonFile);
                    Process.Start(new ProcessStartInfo(path, "\"" + file + "\""));

                    return;
                }

                pe.DataContext = page;
            }
            else
            {
                var option = dv.Page as DialogOption;

                if (option.Target == null)
                {
                    option.Target = new DialogPage(this);
                    option.Target.Vertex = option.Vertex;
                }
                pe.DataContext = option.Target;
            }

            pe.ShowDialog();

            Area.RelayoutGraph();
            Area.GenerateGraph(true, true);
        }

        public DataVertex createNewVertex(DialogLabel page)
        {
            DataVertex dv = new DataVertex();           
            page.Vertex = dv;
            dv.Page = page;
            page.onClick = new RelayCommand(onNodeClick);
            return dv;
        }

        public void AddNode(DataVertex dv)
        {
            mGraph.AddVertex(dv);
        }

        public void LinkNodes(DataVertex dv1, DataVertex dv2)
        {
            try
            {
                mGraph.AddEdge(new DataEdge(dv1, dv2));

            }
            catch (Exception)
            {
                
                throw;
            }
        }


        private Graph CreateGraph()
        {
            mGraph = new Graph();
            return mGraph;
        }
        private void SetupGraph()
        {
            Area.ClearLayout(true);

            if (mGraph != null)
                mGraph.Clear();

            //Lets create logic core and filled data graph with edges and vertices
            var logicCore = new GXLogicCore() { Graph = CreateGraph() };

            logicCore.DefaultLayoutAlgorithm = LayoutAlgorithmTypeEnum.Tree;
            logicCore.DefaultLayoutAlgorithmParams = logicCore.AlgorithmFactory.CreateLayoutParameters(GraphX.LayoutAlgorithmTypeEnum.Tree);
            ((SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).Direction = GraphX.GraphSharp.Algorithms.Layout.LayoutDirection.TopToBottom;
            ((SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).SpanningTreeGeneration = SpanningTreeGeneration.DFS;
            ((SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).LayerGap = 50;
            ((SimpleTreeLayoutParameters)logicCore.DefaultLayoutAlgorithmParams).VertexGap = 50;

            //This property sets vertex overlap removal algorithm.
            //Such algorithms help to arrange vertices in the layout so no one overlaps each other.
            logicCore.DefaultOverlapRemovalAlgorithm = OverlapRemovalAlgorithmTypeEnum.FSA;
            //Default parameters are created automaticaly when new default algorithm is set and previous params were NULL
            logicCore.DefaultOverlapRemovalAlgorithmParams.HorizontalGap = 50;
            logicCore.DefaultOverlapRemovalAlgorithmParams.VerticalGap = 50;

            //This property sets edge routing algorithm that is used to build route paths according to algorithm logic.
            //For ex., SimpleER algorithm will try to set edge paths around vertices so no edge will intersect any vertex.
            //Bundling algorithm will try to tie different edges that follows same direction to a single channel making complex graphs more appealing.
            logicCore.DefaultEdgeRoutingAlgorithm = EdgeRoutingAlgorithmTypeEnum.SimpleER;

            //This property sets async algorithms computation so methods like: Area.RelayoutGraph() and Area.GenerateGraph()
            //will run async with the UI thread. Completion of the specified methods can be catched by corresponding events:
            //Area.RelayoutFinished and Area.GenerateGraphFinished.
            logicCore.AsyncAlgorithmCompute = false;

            //Finally assign logic core to GraphArea object
            Area.LogicCore = logicCore;// as IGXLogicCore<DataVertex, DataEdge, BidirectionalGraph<DataVertex, DataEdge>>;
        }


        public void Rebind()
        {
            var page =mRootNode.Page;

            Area.ClearLayout();
            mGraph.Clear();
            BuildGraph(page as DialogPage, null);
            mRootNode = page.Vertex;

            Area.GenerateGraph(true, true);

        }
        public void Redraw()
        {
            Area.GenerateGraph(true, true);
        }
    }

    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Func<bool> _canExecute;

        public RelayCommand(Action execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = p => execute();
            _canExecute = canExecute;
        }

        public RelayCommand(Action<object> execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
