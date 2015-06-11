﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using Helper;
using readConfiguracion;

namespace PagoElectronico
{
    public partial class ABMCliente : Form
    {

        public MenuPrincipal padre_mp;
       
        public ABM_Cliente.BuscarCliente padre_buscar;
        public string evento;
        public string tipoDoc = "";
        public int nroDoc = 0;
        public int id = 0;
        int ban;
        string user;
        ABMS abm = new ABMS();
        public DateTime fecha = DateTime.ParseExact(readConfiguracion.Configuracion.fechaSystem(), "yyyy-dd-MM", System.Globalization.CultureInfo.InvariantCulture);
        
        ErrorProvider errorProvider1 = new ErrorProvider();
        public bool crear;
        

        public ABMCliente(string ev,string usuario)
        {
            InitializeComponent();
            evento = ev;
            user = usuario;
            boxDatosCliente.Enabled = false;
            txtNombre.Enabled = false;
            btnModificar.Enabled = false;
            btnGrabar.Enabled = false;
            btnEliminar.Enabled = false;
            btnLimpiar.Enabled = false;
            btnSalir.Enabled = true;
            btnBuscar.Enabled = true;
            btnNuevo.Enabled = true;
            txtUsuario.Enabled = false;


            lblMailExistente.Visible = false;
            // Conectar a DB
            Conexion con1 = new Conexion();
            Conexion conDomicilio = new Conexion();
            con1.cnn.Open();

            // Pregunto todos los TipoDoc de la DB
            cbID.Items.Add("");
            string query = "SELECT tipo_descr FROM LPP.TIPO_DOCS";

            SqlCommand command = new SqlCommand(query, con1.cnn);
            SqlDataReader lector1 = command.ExecuteReader();
            while (lector1.Read())
            {
                // Cargo la descripciones en la lista
                cbID.Items.Add(lector1.GetString(0));
            }

            con1.cnn.Close();

            if(evento != "A")
            {
                
                string query1 = "SELECT DISTINCT id_tipo_doc, num_doc, nombre, apellido, " +
                                "nacionalidad, fecha_nac, habilitado,id_domicilio,username " +
                                "FROM LPP.Cliente " +
                                "WHERE Mail = '" + evento + "'";
                              
                con1.cnn.Open();
                SqlCommand command1 = new SqlCommand(query1, con1.cnn);
                SqlDataReader lector = command1.ExecuteReader();
                
                lector.Read();
                cbID.Text = lector.GetString(0);
                txtNumeroID.Text = lector.GetDecimal(1).ToString();
                txtNombre.Text = lector.GetString(2);
                txtApellido.Text = lector.GetString(3);
                txtNacionalidad.Text = lector.GetString(4);
                fechaNacimiento.Value = Convert.ToDateTime(lector.GetDateTime(5));
                chkHabilitado.Checked = lector.GetBoolean(6);
                int id_domicilio = Convert.ToInt32(lector.GetDecimal(7));
                txtMail.Text = evento;
                txtUsuario.Text = lector.GetString(8);
                txtUsuario.Enabled = false;
                con1.cnn.Close();

                //Consulto Domicilio
                string queryDomicilio = "SELECT DISTINCT calle,num,depto,piso,localidad " +
                                        "FROM LPP.Cliente " +
                                        "WHERE id_domicilio = '+ id_domicilio +'";

                conDomicilio.cnn.Open();
                SqlCommand commandDomicilio = new SqlCommand(queryDomicilio, conDomicilio.cnn);
                SqlDataReader lectorDomicilio = commandDomicilio.ExecuteReader();

                lectorDomicilio.Read();


                //Cargo Datos Domicilio
                txtDomicilio.Text = lectorDomicilio.GetString(0);
                txtNumeroCalle.Text = lectorDomicilio.GetDecimal(1).ToString();
                txtDepto.Text = lectorDomicilio.GetString(2);
                txtPiso.Text = lectorDomicilio.GetDecimal(3).ToString();
                txtLocalidad.Text = lectorDomicilio.GetString(4);
                
                txtMail.Enabled = false;
                btnModificar.Enabled = true;
                btnNuevo.Enabled = false;
                btnEliminar.Enabled = true;
            }
            else
            {
                fechaNacimiento.Value = Convert.ToDateTime("2000-01-01");
                chkHabilitado.Visible = false;
            }
            if (user != "U")

            {
                txtUsuario.Text = user;
            
            }
        
        }

       

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            
            ban = 1;
            chkMail.Enabled = false;
            txtNombre.Enabled = true;
            lblNombre.Enabled = true;
            boxDatosCliente.Enabled = true;
            btnLimpiar.Enabled = true;
            btnGrabar.Enabled = true;
            btnNuevo.Enabled = false;
            btnModificar.Enabled = false;
            btnEliminar.Enabled = false;
            btnBuscar.Enabled = false;
            
           
        }


        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtNombre_Validating(object sender, CancelEventArgs e)
        {
            
            if (txtNombre.Text == "")
            {

                errorProvider1.SetError(txtNombre,"No ingreso nombre");
                e.Cancel = true;
                return;
                
              
            }
            else
            {
               errorProvider1.Clear();
            }
        }

        private void txtApellido_Validating(object sender, CancelEventArgs e)
        {
            if (txtApellido.Text == "")
            {
                errorProvider1.SetError(txtApellido, "No ingreso Apellido");
                e.Cancel = true;
                return;
            }
            else
            {
               errorProvider1.Clear();
            }
        }


        private void txtMail_Validating(object sender, CancelEventArgs e)
        {
            Regex r = new Regex(@"^(.+\@.+\.[a-z]{1,3}.+)$");
            if (r.IsMatch(txtMail.Text))
            { errorProvider1.Clear(); 
            }
            else
            {
                errorProvider1.SetError(txtMail, "Mail Inválido");
                e.Cancel = true;
            }
        }

        private void txtNacionalidad_Validating(object sender, CancelEventArgs e)
        {
            if (txtNacionalidad.Text == "")
            {
                errorProvider1.SetError(txtNacionalidad, "No ingreso Pais");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void txtDireccion_Validating(object sender, CancelEventArgs e)
        {
            if (txtDomicilio.Text == "")
            {
                errorProvider1.SetError(txtDomicilio, "No ingreso Direccion");
                e.Cancel = true;
            }
            else
            {
                errorProvider1.Clear();
            }
        }

       
      
        private bool fechaMayorAHoy(int fecha)
        {
            bool ret;
            int hoy;
            DateTime fechaInicio = DateTime.Today;
            hoy = Convert.ToInt32(fechaInicio.ToString("yyyy-MM-dd").Replace('-', '0'));

            if (fecha >= hoy)
            {
                ret = true;
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        private void btnGrabar_Click(object sender, EventArgs e)
        {
            Conexion con1 = new Conexion();
            

            int temp;

            DateTime fecha = DateTime.ParseExact(readConfiguracion.Configuracion.fechaSystem(), "yyyy-dd-MM", System.Globalization.CultureInfo.InvariantCulture);

            if (!((DateTime.Compare(fechaNacimiento.Value, fecha)) < 0))
            {

                MessageBox.Show("La fecha de nacimiento debe ser menor a" + fecha);
                return;
            }
            else
            {

                
                if (cbID.Text!="")
                {
                    string query = "SELECT 1 FROM LPP.TIPO_DOCS WHERE tipo_descr = '" + cbID.Text + "'";
                    con1.cnn.Open();
                    SqlCommand command = new SqlCommand(query, con1.cnn);
                    SqlDataReader lector = command.ExecuteReader();
                    if (!lector.Read())
                    {
                        MessageBox.Show("Tipo de documento Inválido");
                        con1.cnn.Close();
                        return;
                    }
                   con1.cnn.Close();
                }
               
                  
                    // Con este try y este catch verifico que no tengan letras ciertos campos
                    try
                    {
                        if (txtTelefono.Text != "")
                            temp = Convert.ToInt32(txtTelefono.Text);
                        
                        if (txtNumeroID.Text != "")
                            temp = Convert.ToInt32(txtNumeroID.Text);
                    }
                    catch (Exception h)
                    {
                        MessageBox.Show("Numero de ID y Telefono solo pueden contener numeros",h.ToString());
                        return;
                    }

                    
                    this.grabarEnTabla();
                    
                

            }
        }
        private void grabarEnTabla()
        {
            // Conectar a DB
            Conexion con1 = new Conexion();


            // Inserto Cliente
            if (txtNombre.Text != "" && txtApellido.Text != "" && cbID.Text != "" && txtNumeroID.Text != "" && txtMail.Text != "" && txtTelefono.Text != "" && txtNacionalidad.Text != "" && txtLocalidad.Text != "" && fechaNacimiento.Value != null)
            {
                if (ban == 1)
                {
                    if (abm.clienteRegistrado(cbID.Text, Convert.ToInt32(txtNumeroID.Text)) == 0)
                    {

                        //Compruebo que no se repitan los mails
                        string query2 = "SELECT 1 FROM LPP.CLIENTES WHERE mail = '" + txtMail.Text + "' ";
                        con1.cnn.Open();
                        SqlCommand command2 = new SqlCommand(query2, con1.cnn);
                        SqlDataReader lector2 = command2.ExecuteReader();


                        if (lector2.Read())
                        {

                            MessageBox.Show("Mail existente en la Base de Datos, modifiquelo");
                            lblMailExistente.Visible = true;
                            return;
                        }

                        con1.cnn.Close();
                        //int piso = 0;

                        if (txtPiso.Text == "")
                        {
                            txtPiso.Text = "0";
                        }
                        if (txtDepto.Text == "")
                        {
                            txtDepto.Text = "NULO";
                        }

                        //Inserto en la Tabla Domicilio



                        int id_domicilio = abm.insertarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text);
                        string salida = abm.insertarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text,fechaNacimiento.Value, txtNacionalidad.Text, chkHabilitado.Checked,id_domicilio,txtUsuario.Text);
                        MessageBox.Show("" + salida);
                        tipoDoc = cbID.Text;
                        nroDoc = Convert.ToInt32(txtNumeroID.Text);
                        boxDatosCliente.Enabled = false;
                        cbID.Text = "Elija una opcion";
                        chkHabilitado.Checked = false;
                        fechaNacimiento.Value = DateTime.Today;
                        lblNombre.Enabled = false;
                        txtNombre.Enabled = false;
                        btnLimpiar.Enabled = true;
                        btnGrabar.Enabled = false;
                        btnBuscar.Enabled = true;
                        btnNuevo.Enabled = true;
                        btnSalir.Enabled = true;
                        txtNombre.Text = "";
                        txtApellido.Text = "";
                        txtLocalidad.Text = "";
                        txtNacionalidad.Text = "";
                        txtNumeroID.Text = "";
                        txtMail.Text = "";
                        txtDomicilio.Text = "";
                        txtTelefono.Text = "";
                        txtNumeroCalle.Text = "";
                        txtPiso.Text = "";
                        
                    }
                    //Modifico al Cliente
                    if (ban == 2)
                    {
                        //Saco id_domicilio para modificar
                        Conexion con3 = new Conexion();
                        string query3 = "SELECT id_domicilio FROM LPP.CLIENTES WHERE id_tipo_doc = '" + cbID.Text + "' AND num_doc= " + Convert.ToInt32(txtNumeroID.Text) + " ";
                        con3.cnn.Open();
                        SqlCommand command3 = new SqlCommand(query3, con3.cnn);
                        SqlDataReader lector3 = command3.ExecuteReader();
                        lector3.Read();
                        int id_domicilio = lector3.GetInt32(0);
                        con3.cnn.Close();

                        if (chkMail.Checked == true)
                        {
                           //Compruebo que no se repitan los mails
                            Conexion con2 = new Conexion();

                            string query2 = "SELECT 1 FROM LPP.CLIENTES WHERE mail = '" + txtMail.Text + "' ";
                            con2.cnn.Open();
                            SqlCommand command2 = new SqlCommand(query2, con2.cnn);
                            SqlDataReader lector2 = command2.ExecuteReader();

                            if (lector2.Read())
                            {

                                MessageBox.Show("Mail existente en la Base de Datos, modifiquelo");
                                lblMailExistente.Visible = true;
                                return;
                            }

                            con2.cnn.Close();

                            //Modifico en la Tabla Domicilio y CLiente
                            abm.modificarDomicilio(txtDomicilio.Text,Convert.ToInt32(txtNumeroCalle.Text),Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text,id_domicilio);
                            string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNacimiento.Value, txtNacionalidad.Text, chkHabilitado.Checked);
                            MessageBox.Show("" + salida);
                        }

                        else
                        {
                            //Modifico en la Tabla Domicilio y Cliente
                            abm.modificarDomicilio(txtDomicilio.Text, Convert.ToInt32(txtNumeroCalle.Text), Convert.ToInt32(txtPiso.Text), txtDepto.Text, txtLocalidad.Text, id_domicilio);
                            string salida = abm.modificarCliente(txtNombre.Text, txtApellido.Text, cbID.Text, Convert.ToInt32(txtNumeroID.Text), txtMail.Text, fechaNacimiento.Value, txtNacionalidad.Text, chkHabilitado.Checked);
                            MessageBox.Show("" + salida);

                        }

                        boxDatosCliente.Enabled = false;
                        cbID.Text = "";
                        chkHabilitado.Checked = false;
                        fechaNacimiento.Value = fecha;
                        lblNombre.Enabled = false;
                        txtNombre.Enabled = false;
                        btnLimpiar.Enabled = true;
                        btnGrabar.Enabled = false;
                        btnBuscar.Enabled = true;
                        btnNuevo.Enabled = true;
                        btnSalir.Enabled = true;
                        txtNombre.Text = "";
                        txtApellido.Text = "";
                        txtLocalidad.Text = "";
                        txtNacionalidad.Text = "";
                        txtNumeroID.Text = "";
                        txtMail.Text = "";
                        txtDomicilio.Text = "";
                        txtTelefono.Text = "";
                        txtNumeroCalle.Text = "";
                        txtPiso.Text = "";

                    }
                }
                else
                {

                    MessageBox.Show("Faltan Ingresar Datos");
                }

           }
        }


        private void btnBuscar_Click(object sender, EventArgs e)
        {
            btnNuevo.Enabled = true;
            btnSalir.Enabled = true;
            btnUsuario.Enabled = false;
            txtUsuario.Enabled = false;
            ABM_Cliente.BuscarCliente bc = new ABM_Cliente.BuscarCliente();
            this.Close();
            bc.Show();
        }

        private void btnModificar_Click(object sender, EventArgs e)
        {
            txtNombre.Focus();
            btnGrabar.Enabled = true;
            ban = 2;
            chkMail.Enabled = true;
            lblNombre.Enabled = true;
            txtNombre.Enabled = true;
            boxDatosCliente.Enabled = true;
            btnModificar.Enabled = false;
            btnNuevo.Enabled = false;
            btnBuscar.Enabled = false;
            btnEliminar.Enabled = false;
            
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtNombre.Text = "";
            txtApellido.Text = "";
            txtLocalidad.Text = "";
            txtNacionalidad.Text = "";
            txtNumeroID.Text = "";
            txtMail.Text = "";
            txtDomicilio.Text = "";
            txtTelefono.Text = "";
            txtNumeroCalle.Text = "";
            fechaNacimiento.Value = fecha;
            txtPiso.Text = "";
            txtNombre.Focus();
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {

            Conexion con1 = new Conexion();
            con1.cnn.Open();
            //Baja logica del domicilio

            string salida = abm.eliminarCliente(cbID.Text, Convert.ToInt32(txtNumeroID.Text));
            MessageBox.Show(""+salida);
            con1.cnn.Close();
           
           txtNombre.Text = "";
           txtApellido.Text = "";
           txtLocalidad.Text = "";
           txtNacionalidad.Text = "";
           txtNumeroID.Text = "";
           txtMail.Text = "";
           txtDomicilio.Text = "";
           txtTelefono.Text = "";
           txtPiso.Text = "";
           txtNumeroCalle.Text = "";
           fechaNacimiento.Value = fecha;
           cbID.Text = "Elija una opción";
           chkHabilitado.Checked = false;
           txtNombre.Focus();
           boxDatosCliente.Enabled = false;
           btnEliminar.Enabled = false;
           btnGrabar.Enabled = false;
           btnGrabar.Enabled = false;
           btnNuevo.Enabled = true;
           btnModificar.Enabled = false;
       }

        
        private void txtNombre_TextChanged(object sender, EventArgs e)
        {
            if (txtNombre.Text != " ")
            {
                btnGrabar.Enabled = true;

            }
        }

        private void txtNumeroCalle_Validating(object sender, CancelEventArgs e)
        {
            if (txtNumeroCalle.Text == "")
            {

                errorProvider1.SetError(txtNumeroCalle, "No ingreso Numero de Calle");
                e.Cancel = true;

            }
            else
            {
                errorProvider1.Clear();
            }
        }

      private void chkMail_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMail.Checked==true)
            {
                txtMail.Enabled = true;
            }
            else
            {
                txtMail.Enabled = false;
            }
        }

      private void btnUsuario_Click(object sender, EventArgs e)
      {
          btnNuevo.Enabled = true;
          btnSalir.Enabled = true;
          ABM_de_Usuario.BuscarUsuario bu = new ABM_de_Usuario.BuscarUsuario(0);
          bu.Show();
      }

      private void ABMCliente_Load(object sender, EventArgs e)
      {

      }

       

       

      

      
      

       

       

       
    }
}
