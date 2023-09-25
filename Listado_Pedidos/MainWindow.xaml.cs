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
using System.Data;
using System.Data.SqlClient;
namespace Listado_Pedidos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string connectionString = "Data Source=DESKTOP-DKU7QGM\\SQLEXPRESS;Initial Catalog=Neptuno;User ID=jhon;Password=123456";
        public MainWindow()
        {
            InitializeComponent();
            DateTime fechaInicio = new DateTime(1994,08,04); // Cambia esta fecha según tus necesidades
            DateTime fechaFin = new DateTime(1994,09,01); // Cambia esta fecha según tus necesidades

            Pedidos pedidos = ListarPedidos(fechaInicio, fechaFin);

            // Asigna la lista de detalles de pedido a tu DataGrid
            McDataGrid.ItemsSource = pedidos.Detalles;
        }
        private static Pedidos ListarPedidos(DateTime fechaInicio, DateTime fechaFin)
        {
            Pedidos pedidos = new Pedidos();
            pedidos.FechaInicio = fechaInicio;
            pedidos.FechaFin = fechaFin;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "usp_ListarDetallesPedidosPorIntervaloFechas";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                DetallePedido detalle = new DetallePedido
                                {
                                    IdPedido = reader.GetInt32(reader.GetOrdinal("idpedido")),
                                    IdProducto = reader.GetInt32(reader.GetOrdinal("idproducto")),
                                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("preciounidad")),
                                    Cantidad = reader.GetInt32(reader.GetOrdinal("cantidad")),
                                    Descuento = reader.GetDecimal(reader.GetOrdinal("descuento"))
                                };
                                pedidos.Detalles.Add(detalle);
                            }
                        }
                    }
                }
                connection.Close();
            }
            return pedidos;
        }
    }
}