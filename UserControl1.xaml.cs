using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Microsoft.AnalysisServices.AdomdClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        private Dictionary<string, List<string>> dimensiones;
        private string cuboInscripcion = "[CuboInscripcion]";
        private string cuboCursoProg = "[CuboCursoProg]";
        private List<UIElement> currentElements = new List<UIElement>();
        private int fontSizeConst = 16;
        public UserControl1()
        {
            List<ComboBoxItem> itemsFecha = new List<ComboBoxItem>();
            InitializeComponent();
            PantallaInicio();
            //cboFecha.ItemsSource = itemsFecha;
            dimensiones = new Dictionary<string, List<string>>
            {
                {"[Dim Alumnos]", new List<string>{ "[Alumno Genero]", "[Alumno Nombre Completo]", "[Alumnos Edad]"} },
                {"[Dim Curso]", new List<string>{ "[Curso Costo Contado]","[Curso Descripcion]","[Curso Modulo Costo]","[Curso Modulo Descripcion]"} },
                {"[Dim Fecha]", new List<string>{ "[Fecha Anho]","[Fecha Dia]","[Fecha Hora]","[Fecha Mes]","[Fecha Minuto]"} },
                {"[Dim Profesor]", new List<string>{ "[Porfesor Nombre]","[Profesor Apellido]","[Profesor Especialidad]","[Profesor Fecha Nac]"} }
            };

            foreach (string atributos in dimensiones["[Dim Fecha]"])
            {
                ComboBoxItem cboFec = new ComboBoxItem();
                cboFec.Content = atributos;
                itemsFecha.Add(cboFec);
            }

            
        }

        private void PantallaInicio()
        {
            UIElementCollection col = grdMain.Children;
            foreach (UIElement element in currentElements)
            {
                grdMain.Children.Remove(element);
            }
            currentElements.Clear();
            this.PieChart();

            AlumnosPorMesLineChart();

            DoughnutChart();
            CargaHorariaDocenteRowsChart();
        }

        private void BtnInicio_Click(object sender, RoutedEventArgs e)
        {
            PantallaInicio();
        }

        private void Inscripciones_Click(object sender, RoutedEventArgs e)
        {
            UIElementCollection col = grdMain.Children;
            foreach (UIElement element in currentElements)
            {
                grdMain.Children.Remove(element);
            }
            currentElements.Clear();
            TendeciaAlumnosXDocentesBarra();
            AlumnosPorMesLineChart();
            HorariosMasSolicitadosPieChart();
            PieChart();
        }

        private void BtnCursos_Click(object sender, RoutedEventArgs e)
        {
            UIElementCollection col = grdMain.Children;
            foreach (UIElement element in currentElements)
            {
                grdMain.Children.Remove(element);
            }
            currentElements.Clear();
            CargaHorariaDocenteRowsChart();
            DoughnutChart();
            AlumnosPorYearLineChart();
            CursosPorYear();
        }

        //Cartesian chart
        public Func<double, string> yFormatter { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        public string[] Labels { get; set; }

        // pie chart
        public Func<ChartPoint, string> PointLabel { get; set; }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double altura = grdMain.ActualHeight;
            double ancho = grdMain.ActualWidth;
            double alturaMW = mainWindow.ActualHeight;
            double anchoMW = mainWindow.ActualWidth;
            //lblSize.Content = altura.ToString() + " " + ancho.ToString();
            //lblSize2.Content = alturaMW.ToString() + " " + anchoMW.ToString();
        }

        private Dictionary<string, object> CodigoDeAdomd1Dimension(string query)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            AdomdConnection con = new AdomdConnection("Data Source=DESKTOP-MF82JHU;catalog=FullOlapCube");
            con.Open();
            AdomdCommand cmd = new AdomdCommand(query, con);
            CellSet cs = cmd.ExecuteCellSet();
            TupleCollection tuplesOnColumns = cs.Axes[0].Set.Tuples;

            Microsoft.AnalysisServices.AdomdClient.Tuple tp = cs.Axes[1].Set.Tuples[1];
            string tituloX = tp.Members[0].ParentLevel.Caption;
            string tituloY = "";

            foreach (Microsoft.AnalysisServices.AdomdClient.Tuple column in tuplesOnColumns)
            {
                tituloY = column.Members[0].Caption;
            }

            TupleCollection tuplesOnRows = cs.Axes[1].Set.Tuples;
            List<string> listaNombres = new List<string>();
            List<double> listaValores = new List<double>();
            for (int row = 0; row < tuplesOnRows.Count; row++)
            {
                for (int members = 0; members < tuplesOnRows[row].Members.Count; members++)
                {
                    listaNombres.Add(tuplesOnRows[row].Members[members].Caption);
                    listaValores.Add(Convert.ToDouble(cs.Cells[0, row].FormattedValue));
                }
            }
            con.Close();
            resultado.Add("listaNombres", listaNombres);
            resultado.Add("listaValores", listaValores);
            resultado.Add("tituloX", tituloX);
            resultado.Add("tituloY", tituloY);
            return resultado;
        }


        private Dictionary<string, object> CodigoDeAdomd2Dimension(string query)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            AdomdConnection con = new AdomdConnection("Data Source=DESKTOP-MF82JHU;catalog=FullOlapCube");
            con.Open();
            AdomdCommand cmd = new AdomdCommand(query, con);
            CellSet cs = cmd.ExecuteCellSet();
            TupleCollection tuplesOnColumns = cs.Axes[0].Set.Tuples;

            Microsoft.AnalysisServices.AdomdClient.Tuple tp = cs.Axes[1].Set.Tuples[1];
            string tituloX = tp.Members[0].ParentLevel.Caption;
            string tituloY = "";

            foreach (Microsoft.AnalysisServices.AdomdClient.Tuple column in tuplesOnColumns)
            {
                tituloY = column.Members[0].Caption;
            }

            TupleCollection tuplesOnRows = cs.Axes[1].Set.Tuples;
            //List<string> listaNombres = new List<string>();
            //List<double> listaValores = new List<double>();
            int row = 0;
            int rowAux = 0;
            List<LineSeries> seriesList = new List<LineSeries>();
            List<string> seriesLabel = new List<string>();

            //Convert cvv = new Convert();
            while (row < tuplesOnRows.Count)
            {
                string firstCategory = tuplesOnRows[rowAux].Members[0].Caption;
                LineSeries ls = new LineSeries();
                ls.Title = firstCategory;
                ChartValues<int> cv = new ChartValues<int>();

                while (tuplesOnRows[row].Members[0].Caption == firstCategory)
                {
                    //for (int members = 0; members < tuplesOnRows[row].Members.Count; members++)
                    //{
                    seriesLabel.Add(tuplesOnRows[row].Members[1].Caption);
                    //}
                    for (int col = 0; col < tuplesOnColumns.Count; col++)
                    {
                        cv.Add(Convert.ToInt32(cs.Cells[col, row].FormattedValue));
                        //listaValores.Add(Convert.ToDouble(cs.Cells[col, row].FormattedValue));

                    }
                    row++;
                    if (row == tuplesOnRows.Count)
                    {
                        break;
                    }
                }
                ls.Values = cv;
                rowAux = row;
                seriesList.Add(ls);
            }



            con.Close();
            resultado.Add("listaNombres", seriesLabel);
            resultado.Add("listaValores", seriesList);
            resultado.Add("tituloX", tituloX);
            resultado.Add("tituloY", tituloY);
            return resultado;
        }

        private Dictionary<string, object> CodigoDeAdomd2DimensionSA(string query)
        {
            Dictionary<string, object> resultado = new Dictionary<string, object>();
            AdomdConnection con = new AdomdConnection("Data Source=DESKTOP-MF82JHU;catalog=FullOlapCube");
            con.Open();
            AdomdCommand cmd = new AdomdCommand(query, con);
            CellSet cs = cmd.ExecuteCellSet();
            TupleCollection tuplesOnColumns = cs.Axes[0].Set.Tuples;

            Microsoft.AnalysisServices.AdomdClient.Tuple tp = cs.Axes[1].Set.Tuples[1];
            string tituloX = tp.Members[0].ParentLevel.Caption;
            string tituloY = "";

            foreach (Microsoft.AnalysisServices.AdomdClient.Tuple column in tuplesOnColumns)
            {
                tituloY = column.Members[0].Caption;
            }

            TupleCollection tuplesOnRows = cs.Axes[1].Set.Tuples;
            //List<string> listaNombres = new List<string>();
            //List<double> listaValores = new List<double>();
            int row = 0;
            int rowAux = 0;
            List<LineSeries> seriesList = new List<LineSeries>();
            List<string> seriesLabel = new List<string>();

            //Convert cvv = new Convert();
            while (row < tuplesOnRows.Count)
            {
                string firstCategory = tuplesOnRows[rowAux].Members[0].Caption;
                StackedAreaSeries ls = new StackedAreaSeries();
                ls.Title = firstCategory;
                ChartValues<DateTimePoint> cv = new ChartValues<DateTimePoint>();

                while (tuplesOnRows[row].Members[0].Caption == firstCategory)
                {
                    //for (int members = 0; members < tuplesOnRows[row].Members.Count; members++)
                    //{
                    DateTimePoint dtp = new DateTimePoint();
                    dtp.DateTime = new DateTime(Convert.ToInt32(tuplesOnRows[row].Members[1].Caption), 1, 1);
                    seriesLabel.Add(tuplesOnRows[row].Members[1].Caption);
                    //}
                    for (int col = 0; col < tuplesOnColumns.Count; col++)
                    {
                        dtp.Value = Convert.ToDouble(cs.Cells[col, row].FormattedValue);
                        cv.Add(dtp);
                        //listaValores.Add(Convert.ToDouble(cs.Cells[col, row].FormattedValue));

                    }
                    row++;
                    if (row == tuplesOnRows.Count)
                    {
                        break;
                    }
                }
                ls.Values = cv;
                rowAux = row;
                seriesList.Add(ls);
            }



            con.Close();
            resultado.Add("listaNombres", seriesLabel);
            resultado.Add("listaValores", seriesList);
            resultado.Add("tituloX", tituloX);
            resultado.Add("tituloY", tituloY);
            return resultado;
        }
        #region Analisis de cursos

        public void CursosPorYear()
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd2DimensionSA(@" SELECT NON EMPTY { [Measures].[Hecho Curso Prog Count] } ON COLUMNS, 
            NON EMPTY { ([Dim Curso].[Curso Descripcion].[Curso Descripcion].ALLMEMBERS * [Dim Fecha].[Fecha Anho].[Fecha Anho].ALLMEMBERS  ) } 
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM ( SELECT ( -{ [Dim Curso].[Curso Descripcion].[All].UNKNOWNMEMBER }
            ) ON COLUMNS FROM [CuboCursoProg]) CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, 
            FONT_FLAGS");

            Func<double, string> yFormateador;
            Func<double, string> xFormateador;
            yFormateador = val => val.ToString("N");
            xFormateador = val => new DateTime((long)val).ToString("yyyy");

            SeriesCollection sCollection = new SeriesCollection();
            //List<string> labels = (List<string>)datosQuery["listaNombres"];

            LiveCharts.Wpf.Axis axisY = new LiveCharts.Wpf.Axis();
            axisY.Title = (string)datosQuery["tituloX"];

            axisY.LabelFormatter = yFormateador;
            LiveCharts.Wpf.Axis axisX = new LiveCharts.Wpf.Axis();
            axisX.Title = (string)datosQuery["tituloY"];
            axisX.LabelFormatter = xFormateador;



            sCollection.AddRange((List<LineSeries>)datosQuery["listaValores"]);
            CartesianChart cartesianChart = new CartesianChart();
            cartesianChart.Series = sCollection;
            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.Height = 250;
            cartesianChart.VerticalAlignment = VerticalAlignment.Center;
            cartesianChart.AxisX.Add(axisX);
            cartesianChart.AxisY.Add(axisY);

            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Cursos Por Año";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 106;
            margin.Top = 676;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = 800;
            crd.Height = 300;

            grd.Children.Add(cartesianChart);
            crd.Content = grd;
            //crd.Content = cartesianChart;
            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }


        public void AlumnosPorYearLineChart() // inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd2Dimension(@"  SELECT NON EMPTY { [Measures].[Hecho Inscripcion Count] } ON COLUMNS, NON EMPTY { ([Dim Curso].[Curso Descripcion].[Curso Descripcion].ALLMEMBERS * [Dim Fecha].[Fecha Anho].[Fecha Anho].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM ( SELECT ( { [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 COTILLON], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 DECORACIONES BASICAS Y TORTAS COMERCIALES], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 DISEÑO DE UÑAS], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 INTRODUCCION A LA BARBERIA], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 INTRODUCCION A LA GASTRONOMIA], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 INTRODUCION A LA GASTRONOMIA], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 PANADERIA COMERCIAL Y ARTESANAL], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 PASTELERIA COMERCIAL I], [Dim Curso].[Curso Modulo Descripcion].&[MODULO 1 COCINA SALUDABLE BASICA Y COMIDA RAPIDA] } ) ON COLUMNS FROM ( SELECT ( -{ [Dim Curso].[Curso Descripcion].&[Matricula] } ) ON COLUMNS FROM ( SELECT ( -{ [Dim Fecha].[Fecha Anho].&[2009] } ) ON COLUMNS FROM [CuboInscripcion]))) WHERE ( [Dim Curso].[Curso Modulo Descripcion].CurrentMember ) CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS");
            Func<double, string> yFormateador;
            yFormateador = value => value.ToString("N");
            SeriesCollection sCollection = new SeriesCollection();
            List<string> labels = (List<string>)datosQuery["listaNombres"];

            LiveCharts.Wpf.Axis axisY = new LiveCharts.Wpf.Axis();
            axisY.Title = (string)datosQuery["tituloY"];

            axisY.LabelFormatter = yFormateador;
            LiveCharts.Wpf.Axis axisX = new LiveCharts.Wpf.Axis();
            axisX.Title = (string)datosQuery["tituloX"];
            axisX.Labels = labels;



            sCollection.AddRange((List<LineSeries>)datosQuery["listaValores"]);


            CartesianChart cartesianChart = new CartesianChart();
            cartesianChart.Series = sCollection;
            cartesianChart.Tag = "Inscripciones por anho";
            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.Height = 168;
            cartesianChart.VerticalAlignment = VerticalAlignment.Center;
            cartesianChart.AxisX.Add(axisX);
            cartesianChart.AxisY.Add(axisY);

            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Alumnos Por Año";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 105;
            margin.Top = 86;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = 1150;
            crd.Height = 225;
            crd.Tag = "Inscripciones por Year";

            grd.Children.Add(cartesianChart);
            crd.Content = grd;
            //crd.Content = cartesianChart;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        private void CargaHorariaDocenteRowsChart() //Cursos
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension("SELECT NON EMPTY { [Measures].[Duracion Horario] } " +
                "ON COLUMNS, NON EMPTY { ([Dim Profesor].[Porfesor Nombre].[Porfesor Nombre].ALLMEMBERS ) }" +
                " DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [CuboCursoProg] " +
                "CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME," +
                " FONT_SIZE, FONT_FLAGS");
            SeriesCollection SeriesCollectionc;
            List<string> Labelsc = (List<string>)datosQuery["listaNombres"];
            Func<double, string> Formatter = value => value.ToString("N");

            LiveCharts.Wpf.Axis axisY = new LiveCharts.Wpf.Axis();
            axisY.Title = (string)datosQuery["tituloY"];

            axisY.LabelFormatter = Formatter;
            LiveCharts.Wpf.Axis axisX = new LiveCharts.Wpf.Axis();
            axisX.Title = (string)datosQuery["tituloX"];
            axisX.Labels = Labelsc;

            ChartValues<double> myChartValues = new ChartValues<double>();
            List<double> listaValores = (List<double>)datosQuery["listaValores"];
            foreach (double value in listaValores)
            {
                myChartValues.Add(value);
            }
            SeriesCollectionc = new SeriesCollection
            {
                new RowSeries
                {
                    Values = myChartValues
                }
            };
            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Cargar Horarioa del Docente";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            CartesianChart cartesianChart = new CartesianChart();
            cartesianChart.Series = SeriesCollectionc;
            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.Height = 250;
            cartesianChart.VerticalAlignment = VerticalAlignment.Center;
            cartesianChart.AxisX.Add(axisY);
            cartesianChart.AxisY.Add(axisX);

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 106;
            margin.Top = 325;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = 800;
            crd.Height = 325;

            grd.Children.Add(cartesianChart);
            crd.Content = grd;
            //crd.Content = cartesianChart;
            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        private void DoughnutChart() //Cursos
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@"SELECT NON EMPTY { [Measures].[Hecho Inscripcion Count] } ON COLUMNS, 
            NON EMPTY { ([Dim Fecha].[Fecha Hora].[Fecha Hora].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, 
            FONT_FLAGS");
            int width = 350;
            int height = 250;
            PointLabel = ChartPoint => string.Format("({1:P})", ChartPoint.Y, ChartPoint.Participation);
            PieChart pc = new PieChart();


            int contador = 0;
            List<string> listaNombres = (List<string>)datosQuery["listaNombres"];
            var ValoresPie = new PieSeries[listaNombres.Count];
            foreach (var cv in (List<double>)datosQuery["listaValores"])
            {
                ValoresPie[contador] = new PieSeries { Title = listaNombres.ElementAt(contador), Values = new ChartValues<double>() { cv }, LabelPoint = PointLabel, DataLabels = true };
                contador++;
            }
            SeriesCollection sc = new SeriesCollection();
            sc.AddRange(ValoresPie);

            pc.Series = sc;
            pc.InnerRadius = 100;
            pc.LegendLocation = LegendLocation.Right;

            pc.Width = 280;
            pc.Height = 80;
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 10;
            margin.Top = 100;
            
            pc.Margin = margin;
            pc.VerticalAlignment = VerticalAlignment.Top;
            pc.HorizontalAlignment = HorizontalAlignment.Left;
           

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();

            margin.Left = 923;
            margin.Top = 400;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = width;
            crd.Height = height;
            


           

            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Horarios Mas Concurridos";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //System.Windows.Thickness margin = new Thickness();
           
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            grd.Children.Add(pc);
            crd.Content = grd;
            //crd.Content = pc;
            //crd.Padding = padding;
            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            crd.Uid = "dcWEBA";
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        #endregion


        #region Charts de inscripcion
        private void TendeciaAlumnosXDocentesBarra() //Inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@" SELECT NON EMPTY { [Measures].[Hecho Inscripcion Count] } ON COLUMNS, 
            NON EMPTY { ([Dim Profesor].[Porfesor Nombre].[Porfesor Nombre].ALLMEMBERS ) } DIMENSION PROPERTIES 
            MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, 
            FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS");
            SeriesCollection SeriesCollectionc;
            List<string> Labelsc = (List<string>)datosQuery["listaNombres"];
            Func<double, string> Formatter = value => value.ToString("N");

            LiveCharts.Wpf.Axis axisY = new LiveCharts.Wpf.Axis();
            axisY.Title = (string)datosQuery["tituloY"];

            axisY.LabelFormatter = Formatter;
            LiveCharts.Wpf.Axis axisX = new LiveCharts.Wpf.Axis();
            axisX.Title = (string)datosQuery["tituloX"];
            axisX.Labels = Labelsc;



            ChartValues<double> myChartValues = new ChartValues<double>();
            List<double> listaValores = (List<double>)datosQuery["listaValores"];
            foreach (double value in listaValores)
            {
                myChartValues.Add(value);
            }
            SeriesCollectionc = new SeriesCollection
            {
                new ColumnSeries
                {
                    Values = myChartValues
                }
            };

            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Tendencia de Alumnos Por Docentes";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            CartesianChart cartesianChart = new CartesianChart();
            cartesianChart.Series = SeriesCollectionc;

            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.Height = 250;
            cartesianChart.VerticalAlignment = VerticalAlignment.Center;
            cartesianChart.AxisX.Add(axisX);
            cartesianChart.AxisY.Add(axisY);


            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 105;
            margin.Top = 350;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = 800;
            crd.Height = 300;

            grd.Children.Add(cartesianChart);
            crd.Content = grd;
            //crd.Content = cartesianChart;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;

        }

        private void HorariosMasSolicitadosPieChart() //Inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@"  SELECT NON EMPTY { [Measures].[Hecho Inscripcion Count] } 
            ON COLUMNS, NON EMPTY { ([Dim Fecha].[Fecha Hora].[Fecha Hora].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME 
            ON ROWS FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS");

            int width = 350;
            int height = 300;
            PointLabel = ChartPoint => string.Format("{0}({1:P})", ChartPoint.Y, ChartPoint.Participation);
            PieChart pc = new PieChart();


            int contador = 0;
            List<string> listaNombres = (List<string>)datosQuery["listaNombres"];
            var ValoresPie = new PieSeries[listaNombres.Count];
            foreach (var cv in (List<double>)datosQuery["listaValores"])
            {
                ValoresPie[contador] = new PieSeries { Title = listaNombres.ElementAt(contador), Values = new ChartValues<double>() { cv }, LabelPoint = PointLabel, DataLabels = true };
                contador++;
            }
            SeriesCollection sc = new SeriesCollection();
            sc.AddRange(ValoresPie);

            pc.LegendLocation = LegendLocation.Bottom;
            pc.Series = sc;
            pc.Width = width;
            pc.Height = height;


            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Horarios mas solicitados";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            System.Windows.Thickness margin = new Thickness();
            margin.Top = 20;
            pc.Margin = margin;
            lbl.FontSize = fontSizeConst;
            //lbl.Margin = margin;
            grd.Children.Add(lbl);
            
            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            
            margin.Left = 923;
            margin.Top = 400;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = width;
            crd.Height = height;
            crd.Tag = "Utilidades por curso";
            
            grd.Children.Add(pc);
            crd.Content = grd;
            //crd.Content = pc;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        public void PieChart() //analisis de inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@"SELECT NON EMPTY { [Measures].[Metrica Pago] } ON COLUMNS, 
            NON EMPTY { ([Dim Curso].[Curso Descripcion].[Curso Descripcion].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, 
            MEMBER_UNIQUE_NAME ON ROWS FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, 
            FONT_NAME, FONT_SIZE, FONT_FLAGS");

            int width = 350;
            int height = 300;
            PointLabel = ChartPoint => string.Format("({1:P})", ChartPoint.Y, ChartPoint.Participation);
            PieChart pc = new PieChart();

            int contador = 0;
            List<string> listaNombres = (List<string>)datosQuery["listaNombres"];
            var ValoresPie = new PieSeries[listaNombres.Count];
            foreach (var cv in (List<double>)datosQuery["listaValores"])
            {
                ValoresPie[contador] = new PieSeries { Title = listaNombres.ElementAt(contador), Values = new ChartValues<double>() { cv }, LabelPoint = PointLabel, DataLabels = true };
                contador++;
            }
            SeriesCollection sc = new SeriesCollection();
            sc.AddRange(ValoresPie);

            pc.LegendLocation = LegendLocation.Bottom;
            pc.Series = sc;
            pc.Width = width;
            pc.Height = height;
            object tmp = pc.ChartLegend;


            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Utilidades por curso";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            System.Windows.Thickness margin = new Thickness();
            margin.Top = 20;
            pc.Margin = margin;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            //System.Windows.Thickness margin = new Thickness();
            margin.Left = 923;
            margin.Top = 86;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = width;
            crd.Height = height;
            crd.Tag = "Utilidades por curso";

            grd.Children.Add(pc);
            crd.Content = grd;
            //crd.Content = pc;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        public void CursosMasBaratosPieChart() //analisis de inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@"SELECT NON EMPTY { [Measures].[Metrica Pago] } ON COLUMNS, 
            NON EMPTY { ([Dim Curso].[Curso Descripcion].[Curso Descripcion].ALLMEMBERS ) } DIMENSION PROPERTIES MEMBER_CAPTION, 
            MEMBER_UNIQUE_NAME ON ROWS FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, 
            FONT_NAME, FONT_SIZE, FONT_FLAGS");

            int width = 350;
            int height = 300;
            PointLabel = ChartPoint => string.Format("({1:P})", ChartPoint.Y, ChartPoint.Participation);
            PieChart pc = new PieChart();

            int contador = 0;
            List<string> listaNombres = (List<string>)datosQuery["listaNombres"];
            var ValoresPie = new PieSeries[listaNombres.Count];
            foreach (var cv in (List<double>)datosQuery["listaValores"])
            {
                ValoresPie[contador] = new PieSeries { Title = listaNombres.ElementAt(contador), Values = new ChartValues<double>() { cv }, LabelPoint = PointLabel, DataLabels = true };
                contador++;
            }
            SeriesCollection sc = new SeriesCollection();
            sc.AddRange(ValoresPie);

            pc.LegendLocation = LegendLocation.Bottom;
            pc.Series = sc;
            pc.Width = width;
            pc.Height = height;
            object tmp = pc.ChartLegend;


            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Curso mas barato";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            System.Windows.Thickness margin = new Thickness();
            margin.Top = 20;
            pc.Margin = margin;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            //System.Windows.Thickness margin = new Thickness();
            margin.Left = 923;
            margin.Top = 86;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = width;
            crd.Height = height;
            //crd.Tag = "Utilidades por curso";

            grd.Children.Add(pc);
            crd.Content = grd;
            //crd.Content = pc;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }

        public void AlumnosPorMesLineChart() // inscripciones
        {
            Dictionary<string, object> datosQuery = CodigoDeAdomd1Dimension(@"SELECT NON EMPTY { [Measures].[Hecho Inscripcion Count] } ON COLUMNS, 
            NON EMPTY ORDER( ([Dim Fecha].[Fecha Mes].[Fecha Mes].ALLMEMBERS ), [Dim Fecha].[Fecha Mes].CurrentMember.MEMBERValue, ASC)
            DIMENSION PROPERTIES MEMBER_CAPTION, MEMBER_UNIQUE_NAME ON ROWS
            FROM [CuboInscripcion] CELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS");

            Func<double, string> yFormateador;
            yFormateador = value => value.ToString("N");
            SeriesCollection sCollection = new SeriesCollection();
            List<string> labels = (List<string>)datosQuery["listaNombres"];

            LiveCharts.Wpf.Axis axisY = new LiveCharts.Wpf.Axis();
            axisY.Title = (string)datosQuery["tituloY"];

            axisY.LabelFormatter = yFormateador;
            LiveCharts.Wpf.Axis axisX = new LiveCharts.Wpf.Axis();
            axisX.Title = (string)datosQuery["tituloX"];
            axisX.Labels = labels;
            ChartValues<double> myChartValues = new ChartValues<double>();
            List<double> listaValores = (List<double>)datosQuery["listaValores"];
            foreach (double value in listaValores)
            {
                myChartValues.Add(value);
            }
            sCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Inscripciones", Values = myChartValues
                }

            };

            Grid grd = new Grid();
            Label lbl = new Label();
            lbl.Content = "Inscripciones por mes";
            lbl.HorizontalAlignment = HorizontalAlignment.Center;
            //MaterialDesignThemes.Wpf.MaterialDesignFontExtension font = new MaterialDesignThemes.Wpf.MaterialDesignFontExtension();
            //lbl.Style = font.;
            lbl.FontSize = fontSizeConst;
            grd.Children.Add(lbl);

            CartesianChart cartesianChart = new CartesianChart();
            cartesianChart.Series = sCollection;
            cartesianChart.Tag = "Inscripciones por mes";
            cartesianChart.LegendLocation = LegendLocation.Right;
            cartesianChart.Height = 168;
            cartesianChart.VerticalAlignment = VerticalAlignment.Center;
            cartesianChart.AxisX.Add(axisX);
            cartesianChart.AxisY.Add(axisY);


            MaterialDesignThemes.Wpf.Card crd = new MaterialDesignThemes.Wpf.Card();
            System.Windows.Thickness margin = new Thickness();
            margin.Left = 105;
            margin.Top = 86;
            margin.Right = 0;
            margin.Bottom = 0;
            crd.Margin = margin;
            crd.Width = 800;
            crd.Height = 225;
            crd.Tag = "Inscripciones por mes";

            grd.Children.Add(cartesianChart);
            crd.Content = grd;
            //crd.Content = cartesianChart;

            crd.HorizontalAlignment = HorizontalAlignment.Left;
            crd.VerticalAlignment = VerticalAlignment.Top;
            grdMain.Children.Add(crd);
            currentElements.Add(crd);
            DataContext = this;
        }


        #endregion

        private void BtnPersonalzado_Click(object sender, RoutedEventArgs e)
        {

            foreach (UIElement element in currentElements)
            {
                grdMain.Children.Remove(element);
            }
            currentElements.Clear();
            ComboBox cbFiltro = new ComboBox();
            cbFiltro.Width = 50;



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //ComboBoxItem value = (ComboBoxItem)cboFecha.SelectedItem;

            //MessageBox.Show(value.Content.ToString());
        }
    }
}
