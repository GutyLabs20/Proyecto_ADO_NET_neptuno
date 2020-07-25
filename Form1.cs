using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data; //Contiene propiedades comunes de ADO, .NET, DataSet, DataTable, etc.
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;    // permite usar SQLConnection, SQLCommand, etc.
using System.Configuration; //permitir leer el app.config

namespace Proyecto_ADO_NET_neptuno
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Recuperar la cadena de conexion desde el app.config
        String cad_cn = ConfigurationManager.ConnectionStrings["cn1"].ConnectionString;

        //Traer las categorias
        void Traer_Categorias()
        {
            SqlDataAdapter adapCat = new SqlDataAdapter("Listar_Categorias", cad_cn);
            //
            adapCat.SelectCommand.CommandType = CommandType.StoredProcedure;
            //
            DataTable dtabla = new DataTable();
            adapCat.Fill(dtabla);
            adapCat.Dispose();

            //Enlazar el datatable como origen de datos del combobox
            cbocategoria.DataSource = dtabla;
            //indicar la columna del datatable que mostrara sus datos
            cbocategoria.DisplayMember = "NombreCategoria";
            //Indica la columna que almacenara el combobox
        }

        //Listar Productos
        void Traer_Productos()
        {
            SqlDataAdapter adapProd = new SqlDataAdapter("Listar_Producto", cad_cn);
            //
            adapProd.SelectCommand.CommandType = CommandType.StoredProcedure;
            //
            adapProd.SelectCommand.Parameters.AddWithValue("@Eliminado", "NO");
            //
            DataTable dtabla = new DataTable();
            adapProd.Fill(dtabla);
            adapProd.Dispose();

            grilla.DataSource = dtabla;
            grilla.AutoResizeColumns();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Traer_Categorias();

            Traer_Productos();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                int xcodigo = int.Parse(txtcodigo.Text);
                SqlConnection cnx = new SqlConnection(cad_cn);
                cnx.Open();

                SqlCommand cmd = new SqlCommand("Eliminar_Producto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@codprod", xcodigo);
                cmd.ExecuteNonQuery(); //Ejecuta instrucciones que no sea un SELECT

                cnx.Close();

                MessageBox.Show("Producto Eliminado");

                Traer_Productos();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            try
            {
                //para recuperar el valor almacenado por la columna indicada en la propiedad
                // ValueMember de un combobox o listbox, realizamos lo siguiente:
                int xcodcat = cbocategoria.SelectedIndex;

                SqlConnection cnx = new SqlConnection(cad_cn);
                cnx.Open();

                SqlCommand cmd = new SqlCommand("Grabar_Producto", cnx);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nomprod", txtnombre.Text);
                cmd.Parameters.AddWithValue("@precio", decimal.Parse(txtprecio.Text));
                cmd.Parameters.AddWithValue("@stock", short.Parse(txtstock.Text));
                cmd.Parameters.AddWithValue("@codcat", xcodcat);
                cmd.Parameters.AddWithValue("@fechaprod", dateFecha.Value);

                txtcodigo.Text = cmd.ExecuteScalar().ToString();//Ejecuta una linea select

                cnx.Close();

                MessageBox.Show("Nuevo producto grabado correctamente");

                Traer_Productos();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
