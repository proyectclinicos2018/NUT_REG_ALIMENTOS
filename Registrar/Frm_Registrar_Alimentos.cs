using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Text.RegularExpressions;


namespace Registrar_Alimentos
{
    public partial class Frm_Registrar_Alimentos : Form
    {

        #region Variables

        #region Variables Staticas

        static int cod_alimentos = 0;
        static int cod_distribucion = 0;


        static int cod_regimen = 0;
        static int cod_consistencia = 0;
        static int cod_digestabilidad = 0;
        static int cod_dulzor = 0;
        static int cod_lactosa = 0;
        static int cod_sal = 0;
        static int cod_temperatura = 0;
        static int cod_volumen = 0;
        static int cod_no_aplica = 0;


        static int cod_regimen_pref = 0;
        static int cod_consistencia_pref = 0;
        static int cod_digestabilidad_pref = 0;
        static int cod_dulzor_pref = 0;
        static int cod_lactosa_pref = 0;
        static int cod_sal_pref = 0;
        static int cod_temperatura_pref = 0;
        static int cod_volumen_pref = 0;


        static string  no_sal= "";
        static string  no_dulzor= "";
        static string  no_lactosa= "";

        #endregion

        #region Datatables

        DataTable dt = new DataTable();
        DataTable dt_componentes_alimentos = new DataTable();
        DataTable dt_distribucion_alimentos = new DataTable();

        #endregion

        #region Datos Conexion

        ConectarFalp CnnFalp;
        Configuration Config;
        string User = string.Empty;
        string[] Conexion = { "", "", "" };
        string PCK = "PCK_NUT001I";
        string PCK1 = "PCK_NUT001M";

        #endregion

        #endregion
        public Frm_Registrar_Alimentos()
        {
            InitializeComponent();
        }

        private void Frm_Registrar_Alimentos_Load(object sender, EventArgs e)
        {
            conectar();
            bloqueo();
            dt.Clear();
            Crear_Tbl_Distribucion();
            btn_guardar.Enabled = false;
  
        }

      
        #region Cargar

        #region Cargar Conexion

        protected void conectar()
        {

            if (!(CnnFalp != null))
            {

                ExeConfigurationFileMap FileMap = new ExeConfigurationFileMap();
                FileMap.ExeConfigFilename = Application.StartupPath + @"\..\WF.config";
                Config = ConfigurationManager.OpenMappedExeConfiguration(FileMap, ConfigurationUserLevel.None);

                CnnFalp = new ConectarFalp(Config.AppSettings.Settings["dbServer"].Value,//ConfigurationManager.AppSettings["dbServer"],
                                           Config.AppSettings.Settings["dbUser"].Value,//ConfigurationManager.AppSettings["dbUser"],
                                           Config.AppSettings.Settings["dbPass"].Value,//ConfigurationManager.AppSettings["dbPass"],
                                           ConectarFalp.TipoBase.Oracle);

                if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir(); // abre la conexion

                Conexion[0] = Config.AppSettings.Settings["dbServer"].Value;
                Conexion[2] = Config.AppSettings.Settings["dbUser"].Value;
                Conexion[1] = Config.AppSettings.Settings["dbPass"].Value;
            }



            // this.Text = this.Text + " [Versión: " + Application.ProductVersion + "] [Conectado: " + Conexion[0] + "]";
            // User = ValidaMenu.LeeUsuarioMenu();
            User = "SICI";
            LblUsuario.Text = "Usuario: " + User;
            //LblUsuario.Text = "Usuario: " + User;
        }

        #endregion

        #region Cargar Grilla

        #region Listar Grilla

        protected void Cargar_grilla()
        {

            if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

            CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK + ".P_CARGAR_RESULTADO_ALIMENTOS");

            CnnFalp.ParametroBD("PIN_DISTRIBUCION", cod_distribucion, DbType.Int64, ParameterDirection.Input);
            dt.Clear();
            dt.Load(CnnFalp.ExecuteReader());

            if (dt.Rows.Count > 0)
            {
                grilla_alimentos.AutoGenerateColumns = false;
                grilla_alimentos.DataSource = dt;
                agregarimagen();


            }
            else
            {

               /* txtmsg.Visible = true;
                txtmsg.Text = "Estimado Usuario, no existen Datos";*/

            }

            CnnFalp.Cerrar();
            ocultargrilla();
        }

        #endregion

        #region Agrupar

        #endregion

        #region Agregar Imagen

        protected void agregarimagen()
        {
            foreach (DataGridViewRow row in grilla_alimentos.Rows)
            {

                string ve = Convert.ToString(row.Cells["V"].Value);
                DataGridViewImageCell Imagen = row.Cells["Vigencia"] as DataGridViewImageCell;

                if (ve == "S")
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Alimentos.Properties.Resources.Check;
                }
                else
                {
                    Imagen.Value = (System.Drawing.Image)Registrar_Alimentos.Properties.Resources.Delete;

                }

            }



        }

        #endregion

        #region Ocultar Columnas

        protected void ocultargrilla()
        {
            grilla_alimentos.AutoResizeColumns();
            grilla_alimentos.Columns["Tipo_distribucion"].Visible = false;
            //grilla_alimentos.Columns["Vigencia"].Visible = false;


        }

        #endregion

        #region Ordenar Columnas

        #endregion

        #region Pintar Grilla

        private void grilla_alimentos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                e.PaintBackground(e.ClipBounds, false);
                Font drawFont = new Font("Trebuchet MS", 8, FontStyle.Bold);
                SolidBrush drawBrush = new SolidBrush(Color.White);
                StringFormat StrFormat = new StringFormat();
                StrFormat.Alignment = StringAlignment.Center;
                StrFormat.LineAlignment = StringAlignment.Center;

                e.Graphics.DrawImage(Properties.Resources.HeaderGV, e.CellBounds);
                e.Graphics.DrawString(grilla_alimentos.Columns[e.ColumnIndex].HeaderText, drawFont, drawBrush, e.CellBounds, StrFormat);

                e.Handled = true;
                drawBrush.Dispose();
            }
        }

        #endregion

        #region Pintar Extraer grilla

        #endregion

        #endregion

        #region Cargar DataTables

        #endregion


        #endregion

        #region Botones

        private void btn_guardar_Click(object sender, EventArgs e)
        {
            if(Validar_Campos_1() && Validar_Campos_2() && Validar_Campos_3())
            {

                DialogResult resp = MessageBox.Show("Estimado Usuario, Esta seguro de ingresar a la Distribución de alimento " + txtdistribucion.Text.ToUpper().Trim() + ", la descripción de alimento " + txtdescripcion.Text.ToUpper().Trim() + " ?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (resp == DialogResult.Yes)
                {

                if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

                try
                {
                CnnFalp.IniciarTransaccion();
                Guardar_parametros();
                recorrer_dt_det_componentes();
                recorrer_dt_det_distribucion();

                CnnFalp.ConfirmarTransaccion();
                CnnFalp.Cerrar();

                MessageBox.Show("Estimado usuario, El registro sea insertado correctamente.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                                                
                                                                                                
                Cargar_grilla();
     
                bloqueo();
                dt_componentes_alimentos.Clear();
                dt_distribucion_alimentos.Clear();
 
                txtdescripcion.ReadOnly = false;
                txtgr.ReadOnly = false;
                txtcc.ReadOnly = false;
   
                }
                catch (Exception ex)
                {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CnnFalp.ReversarTransaccion();

                }
                }
                                                                               
            }
            dt.Clear();
            txtdistribucion.Focus();
        }

        private void btn_limpiar_Click(object sender, EventArgs e)
        {
            txtdistribucion.Text = "";
            cod_distribucion = 0;
            bloqueo();
            dt.Clear();
        }

        private void btn_confirmar_Click(object sender, EventArgs e)
        
        {

            if(Validar_Campos_1())
            {
              DialogResult resp = MessageBox.Show("Estimado Usuario, Esta seguro  de ingresar en la Distribución " + txtdistribucion.Text.ToUpper().Trim() + ", el siguiente alimento " + txtdescripcion.Text.ToUpper().Trim() + "  ?", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

              if (resp == DialogResult.Yes)
              {
                  Crear_Tbl_Componentes();
                  txtregimen.Enabled = true;
                  btn_regimen.Enabled = true;
                  txtregimen.Focus();
                  btn_limpiar_componentes.Enabled = true;
                    
              }

              else
              {

              }
            }
               
        }

        private void btn_limpiar_menu_Click(object sender, EventArgs e)
        {
            txtdistribucion.Text = "";
            cod_distribucion = 0;
            limpiar_alimentos();
            dt.Clear();
        }

        private void btn_limpiar_componentes_Click(object sender, EventArgs e)
        {
            limpiar_componentes();
        }

        private void btn_limpiar_ingesta_Click(object sender, EventArgs e)
        {
            limpiar_ingesta();
        }

        private void btn_tipo_distribucion_Click(object sender, EventArgs e)
        {
            txtregimen.Text = "";
            Cargar_tipo_distribucion();
            Cargar_grilla();
            btn_limpiar_menu.Enabled = true;
          
        }

        private void btn_regimen_Click(object sender, EventArgs e)
        {
            txtregimen.Text = "";
          // Cargar_tipo_cobro();
            MessageBox.Show("Estimado usuario, Debe seleccionar Múltiples Opciones, para este Alimento.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);

               
            Cargar_tipo_cobro_2();

           if( cod_regimen==0)
           {
               txtregimen.Focus();
           }

           else
           {
              
               txtconsistencia.Enabled = true;
               btn_consistencia.Enabled = true;
               txtconsistencia.Focus();
           }

        }

        private void btn_consistencia_Click(object sender, EventArgs e)
        {
            txtconsistencia.Text = "";
            Cargar_tipo_consistencia();
            if (cod_consistencia_pref == 0)
            {
                txtdigestabilidad.Enabled = false;
                btn_digestabilidad.Enabled = false;
                txtconsistencia.Focus();

            }
            else
            {
               DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

               if (resp == DialogResult.Yes)
               {
                   Cargar_tipo_consistencia_2();
                   if (cod_consistencia == 0)
                   {
                       txtdigestabilidad.Enabled = false;
                       btn_digestabilidad.Enabled = false;
                       txtconsistencia.Focus();
                 
                   }
                   else
                   {
                       txtdigestabilidad.Enabled = true;
                       btn_digestabilidad.Enabled = true;
                       txtdigestabilidad.Focus();
                   }
               }
               else
               {
                 
                   txtdigestabilidad.Enabled = true;
                   btn_digestabilidad.Enabled = true;
                   txtdigestabilidad.Focus();
                   cod_consistencia = cod_consistencia_pref;
                   txtconsistencia.Text = txtconsistencia_pref.Text;
                   agregar_componentes(4, Convert.ToInt32(cod_consistencia));
               }
            }
        }

        private void btn_digestabilidad_Click(object sender, EventArgs e)
        {
            txtdigestabilidad.Text = "";
            Cargar_tipo_digestabilidad();

            if (cod_digestabilidad_pref == 0)
            {
                txtdulzor.Enabled = false;
                btn_dulzor.Enabled = false;
                txtdigestabilidad.Focus();
       
            }
            else
            {
                DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (resp == DialogResult.Yes)
                {
                    Cargar_tipo_digestabilidad_2();
                    if (cod_digestabilidad == 0)
                    {
                        txtdigestabilidad.Enabled = false;
                        txtconsistencia.Focus();

                    }
                    else
                    {
                        txtdulzor.Enabled = true;
                        btn_dulzor.Enabled = true;
                        txtdulzor.Focus();
                    }
                }
                else
                {
                    txtdulzor.Enabled = true;
                    btn_dulzor.Enabled = true;
                    txtdulzor.Focus();
                    cod_digestabilidad = cod_digestabilidad_pref;
                    txtdigestabilidad.Text = txtdigestabilidad_pref.Text;
                    agregar_componentes(5, Convert.ToInt32(cod_digestabilidad));
                }
            }
        }

        private void btn_dulzor_Click(object sender, EventArgs e)
        {
            txtdulzor.Text = "";
           
            valid_dulzor();
           
        }

        private void btn_lactosa_Click(object sender, EventArgs e)
        {
            txtlactosa.Text = "";
            valid_lactosa();
           
        }

        private void btn_sal_Click(object sender, EventArgs e)
        {
            txtsal.Text = "";
            valid_sal();
           
        }

        private void btn_temperatura_Click(object sender, EventArgs e)
        {
            txttemperatura.Text = "";
            valid_temperatura();
        }

        private void btn_volumen_Click(object sender, EventArgs e)
        {
            txtvolumen.Text = "";
            valid_volumen();
        }

    


        #endregion


        #region Metodos

        #region Limpiar


        protected void limpiar_alimentos()
        {
            cod_distribucion = 0;
            txtdistribucion.Text = "";
            txtdescripcion.Text = "";
            txtgr.Text="";
            txtcc.Text="";
 
          /*  btn_confirmar.Enabled = false;
            txtdistribucion.Enabled = true;
            btn_tipo_distribucion.Enabled = true;
            txtdescripcion.Enabled = false;
            txtgr.Enabled = false;
            txtcc.Enabled = false;*/
            txtdistribucion.Focus();
        }

        protected void limpiar_componentes()
        {

         txtregimen.Focus();
         cod_alimentos = 0;
         cod_distribucion = 0;
         cod_regimen = 0;
         cod_consistencia = 0;
         cod_digestabilidad = 0;
         cod_dulzor = 0;
         cod_lactosa = 0;
         cod_sal = 0;
         cod_temperatura = 0;
         cod_volumen = 0;
         no_lactosa = "";
         no_sal = "";
         no_dulzor = "";

         btn_regimen.Enabled = true;
         btn_consistencia.Enabled = false;
         btn_digestabilidad.Enabled = false;
         btn_lactosa.Enabled = false;
         btn_dulzor.Enabled = false;
         btn_sal.Enabled = false;
         btn_temperatura.Enabled = false;
         btn_volumen.Enabled = false;
     

         txtregimen.Enabled = true;
         txtconsistencia.Enabled = false;
         txtdigestabilidad.Enabled = false;
         txtlactosa.Enabled = false;
         txtdulzor.Enabled = false;
         txtsal.Enabled = false;
         txttemperatura.Enabled = false;
         txtvolumen.Enabled = false;

         txtregimen.Text = string.Empty;
         txtconsistencia.Text = string.Empty;
         txtdigestabilidad.Text = string.Empty;
         txtlactosa.Text = string.Empty;
         txtdulzor.Text = string.Empty;
         txtsal.Text = string.Empty;
         txttemperatura.Text = string.Empty;
         txtvolumen.Text = string.Empty;


         txtregimen_pref.Text = string.Empty;
         txtconsistencia_pref.Text = string.Empty;
         txtdigestabilidad_pref.Text = string.Empty;
         txtlactosa_pref.Text = string.Empty;
         txtdulzor_pref.Text = string.Empty;
         txtsal_pref.Text = string.Empty;
         txttemperatura_pref.Text = string.Empty;
         txtvolumen_pref.Text = string.Empty;
        }

        protected void  limpiar_ingesta()
        {
         txthyc.Text = string.Empty;
         txtproteinas.Text = string.Empty;
         txtlipidos.Text = string.Empty;
         txtfibra.Text = string.Empty;
         txtagmonoinsat.Text = string.Empty;
         txtagpoliinsat.Text = string.Empty;
         txtagsaturados.Text = string.Empty;
         txtcalcio.Text = string.Empty;
         txtcolesterol.Text = string.Empty;
         txtfosforo.Text = string.Empty;
         txthierro.Text = string.Empty;
         txtmagnesio.Text = string.Empty;
         txtn3.Text = string.Empty;
         txtn6.Text = string.Empty;
         txtpotacio.Text = string.Empty;
         txtsodio.Text = string.Empty;
         txtzinc.Text = string.Empty;
         txtcalorias.Text = string.Empty;

         txthyc.Enabled = false;
         txtproteinas.Enabled = true;
         txtlipidos.Enabled = false;
         txtfibra.Enabled = false;
         txtagmonoinsat.Enabled = false;
         txtagpoliinsat.Enabled = false;
         txtagsaturados.Enabled = false;
         txtcalcio.Enabled = false;
         txtcolesterol.Enabled = false;
         txtfosforo.Enabled = false;
         txthierro.Enabled = false;
         txtmagnesio.Enabled = false;
         txtn3.Enabled = false;
         txtn6.Enabled = false;
         txtpotacio.Enabled = false;
         txtsodio.Enabled = false;
         txtzinc.Enabled = false;
         txtcalorias.Enabled = false;
         txtproteinas.Focus();
         

        }


        #endregion

        #region Cargar Distribucion

        protected void Cargar_tipo_distribucion()
        {
            Cargar_datos_tipo_distribucion(ref Ayuda);

            if (!Ayuda.EOF())
            {
                cod_distribucion = Convert.ToInt32(Ayuda.Fields(0));
                txtdistribucion.Text = Ayuda.Fields(1);
                agregar_distribuciones(Convert.ToInt32(cod_distribucion));

                if (cod_distribucion == 0)
                {

                    txtdescripcion.Enabled = false;
                    txtdistribucion.Focus();
                 
                }
                else
                {
                    Cargar_grilla();
                    txtdescripcion.Focus();
                    txtdescripcion.Enabled = true;
                    btn_limpiar_menu.Enabled = true;
                }



            }
            else
            {
                if (cod_distribucion == 0)
                {
                    txtdistribucion.Text = "";
                }
            }


        }

        void Cargar_datos_tipo_distribucion(ref AyudaSpreadNet.AyudaSprNet Ayuda)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.User = Conexion[2];
            Ayuda.Pass = Conexion[1];
           
            Ayuda.TipoBase = 1;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar Tipo de Distribución";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_DISTRIBUCION";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txtdistribucion.Text, DbType.String, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        #endregion

        #region Cargar Cobro

        protected void Cargar_tipo_cobro()
        {
              Cargar_datos_Parametro(ref Ayuda, 3, txtregimen.Text);

              if (!Ayuda.EOF())
              {
                  cod_regimen = Convert.ToInt32(Ayuda.Fields(0));

                  txtregimen.Text = Ayuda.Fields(1);
              }

              else
              {
                  if (cod_regimen == 0)
                  {
                      txtregimen.Text = "";
                  }
              }
        }

        protected void Cargar_tipo_cobro_2()
        {
            Cargar_datos_2(ref Ayuda,0, 3, txtregimen.Text);

            int cont = 0;
            int cod = 3;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                   
                        cod_regimen = Convert.ToInt32(Ayuda.Fields(0));
                     
                            txtregimen.Text = Ayuda.Fields(1);
                            agregar_componentes(cod, Convert.ToInt32(cod_regimen));
                       
                }

                else
                {

                    cod_regimen = Convert.ToInt32(Ayuda.Fields(0));
                    txtregimen.Text = txtregimen.Text + "-" + Ayuda.Fields(1);
                    agregar_componentes(cod, Convert.ToInt32(cod_regimen));
                }

                Ayuda.MoveNext();
            }

            if (cod_regimen == 0)
            {
                txtregimen.Text = "";
            }

        }



        #endregion

        #region Cargar Consistencia

        protected void Cargar_tipo_consistencia()
        {
            Cargar_datos_Parametro(ref Ayuda, 4, txtconsistencia.Text);

            if (!Ayuda.EOF())
            {
                cod_consistencia_pref = Convert.ToInt32(Ayuda.Fields(0));
               
                   txtconsistencia_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
               
            }
            else
            {
                if (cod_consistencia_pref == 0)
                {
                    txtconsistencia.Text = "";
                }
            }


        }

        protected void Cargar_tipo_consistencia_2()
        {
            Cargar_datos_2(ref Ayuda,cod_consistencia_pref, 4, txtconsistencia.Text);
            int cont = 0;
            int cod = 4;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_consistencia = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_consistencia_pref != cod_consistencia )
                    {  
                        txtconsistencia.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_consistencia));
                    }
                }

                else
                {
                    cod_consistencia = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_consistencia_pref != cod_consistencia)
                    {                 
                        txtconsistencia.Text = txtconsistencia.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_consistencia));
                    }
                }

                Ayuda.MoveNext();
            }

            if (cod_consistencia == 0)
            {
                txtregimen.Text = "";
            }


        }


        #endregion

        #region Cargar Digestabilidad

        protected void Cargar_tipo_digestabilidad()
        {
            Cargar_datos_Parametro(ref Ayuda, 5, txtdigestabilidad.Text);

            if (!Ayuda.EOF())
            {
                cod_digestabilidad_pref = Convert.ToInt32(Ayuda.Fields(0));
              
                    txtdigestabilidad_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
               
            }
            else
            {
                if (cod_digestabilidad_pref == 0)
                {
                    txtdigestabilidad.Text = "";
                }
            }
        }

        protected void Cargar_tipo_digestabilidad_2()
        {
            Cargar_datos_2(ref Ayuda, cod_digestabilidad_pref,5, txtdigestabilidad.Text);

            int cont = 0;
            int cod = 5;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_digestabilidad = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_digestabilidad_pref != cod_digestabilidad )
                    {
                      
                        txtdigestabilidad.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_digestabilidad));
                    }
                }

                else
                {
                    cod_digestabilidad = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_digestabilidad_pref != cod_digestabilidad)
                    {
                       
                        txtdigestabilidad.Text = txtdigestabilidad.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_digestabilidad));
                    }
                }

                Ayuda.MoveNext();
            }

            if (cod_digestabilidad == 0)
            {
                txtdigestabilidad.Text = "";
            }
        }

  

        #endregion

        #region Cargar Sacarosa

        protected void Cargar_tipo_sacarosa()
        {
            Cargar_datos_Parametro(ref Ayuda, 7, txtdulzor.Text);
          

            if (!Ayuda.EOF())
            {
                cod_dulzor_pref = Convert.ToInt32(Ayuda.Fields(0));
              
                     txtdulzor_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
               
            }
            else
            {
                if (cod_dulzor_pref == 0)
                {
                    txtdulzor.Text = "";
                }
            }
        }

        protected void Cargar_tipo_sacarosa_2()
        {
            Cargar_datos_2(ref Ayuda, cod_dulzor_pref, 7, txtdulzor.Text);

            int cont = 0;
            int cod = 7;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_dulzor = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_dulzor_pref != cod_dulzor)
                    {
                       
                        txtdulzor.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_dulzor));
                    }
                }

                else
                {
                    cod_dulzor = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_dulzor_pref != cod_dulzor)
                    {
                       
                        txtdulzor.Text = txtdulzor.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_dulzor));
                    }
                }

                Ayuda.MoveNext();
            }
            if (cod_dulzor == 0)
            {
                txtconsistencia.Text = "";
            }
        }

        protected void Cargar_datos_Parametro(ref AyudaSpreadNet.AyudaSprNet Ayuda,Int64 Tipo, string Txt)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.Multi_Seleccion = false; 
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar Tipo de Sacarosa Preferencial";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_PARAMETROS";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", Txt.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.Generar_ParametroBD("PIN_TIPO", Tipo, DbType.Int64, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }

        protected void Cargar_datos_2(ref AyudaSpreadNet.AyudaSprNet Ayuda, int cod, int tipo, string txt)
        {
            string[] NomCol = { "Código", "Descripción" };
            int[] AnchoCol = { 80, 350 };
            Ayuda.Nombre_BD_Datos = Conexion[0];
            Ayuda.Pass = Conexion[1];
            Ayuda.User = Conexion[2];
            Ayuda.TipoBase = 1;
            Ayuda.Multi_Seleccion = true;
            Ayuda.NombreColumnas = NomCol;
            Ayuda.AnchoColumnas = AnchoCol;
            Ayuda.TituloConsulta = "Ingresar  Multiselección";
            Ayuda.Package = PCK;
            Ayuda.Procedimiento = "P_CARGAR_PARAMETROS_2";
            Ayuda.Generar_ParametroBD("PIN_DESCRIPCION", txt.ToUpper(), DbType.String, ParameterDirection.Input);
            Ayuda.Generar_ParametroBD("PIN_PREFERENCIAL", cod, DbType.Int64, ParameterDirection.Input);
            Ayuda.Generar_ParametroBD("PIN_TIPO", tipo, DbType.Int64, ParameterDirection.Input);
            Ayuda.EjecutarSql();

        }



        #endregion

        #region Cargar Lactosa

        protected void Cargar_tipo_lactosa()
        {
            Cargar_datos_Parametro(ref Ayuda, 8, txtlactosa.Text);

            if (!Ayuda.EOF())
            {
                cod_lactosa_pref = Convert.ToInt32(Ayuda.Fields(0));
               
                 txtlactosa_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
               
            }
            else
            {
                if (cod_lactosa_pref == 0)
                {
                    txtlactosa.Text = "";
                }
            }
        }


        protected void Cargar_tipo_lactosa_2()
        {
            Cargar_datos_2(ref Ayuda,cod_lactosa_pref, 8, txtlactosa.Text);

            int cont = 0;
            int cod = 8;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_lactosa = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_lactosa_pref != cod_lactosa)
                    {
                       
                        txtlactosa.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_lactosa));
                    }
                }

                else
                {
                    cod_lactosa = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_lactosa_pref != cod_lactosa)
                    {
                        cod_lactosa = Convert.ToInt32(Ayuda.Fields(0));
                        txtlactosa.Text = txtlactosa.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_lactosa));
                    }
                }

                Ayuda.MoveNext();
            }

            if (cod_lactosa == 0)
            {
                txtlactosa.Text = "";
            }
        }

    

  

        #endregion

        #region Cargar Sal

        protected void Cargar_tipo_sal()
        {
            Cargar_datos_Parametro(ref Ayuda, 6, txtsal.Text);

            if (!Ayuda.EOF())
            {
                cod_sal_pref = Convert.ToInt32(Ayuda.Fields(0));
               
                  txtsal_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
             
            }
            else
            {
                if (cod_sal_pref == 0)
                {
                    txtsal.Text = "";
                }
            }
        }

        protected void Cargar_tipo_sal_2()
        {
            Cargar_datos_2(ref Ayuda,cod_sal_pref, 6, txtsal.Text);

            int cont = 0;
            int cod = 6;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_sal = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_sal_pref != cod_sal)
                    {
                     
                      txtsal.Text = Ayuda.Fields(1);
                      agregar_componentes(cod, Convert.ToInt32(cod_sal));
                    }
                }

                else
                {
                    cod_sal = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_sal_pref != cod_sal)
                    {
                        
                        txtsal.Text = txtsal.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_sal));
                    }
                }

                Ayuda.MoveNext();
            }

            if (cod_sal == 0)
            {
                txtsal.Text = "";
            }
        }

 

        #endregion

        #region Cargar Temperatura

        protected void Cargar_tipo_temperatura()
        {
            Cargar_datos_Parametro(ref Ayuda, 10, txttemperatura.Text);

            if (!Ayuda.EOF())
            {
                cod_temperatura_pref = Convert.ToInt32(Ayuda.Fields(0));
              
                      txttemperatura_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
               
            }
            else
            {
                if (cod_temperatura_pref == 0)
                {
                    txttemperatura.Text = "";
                }
            }
        }

        protected void Cargar_tipo_temperatura_2()
        {
            Cargar_datos_2(ref Ayuda,cod_temperatura_pref, 10, txttemperatura.Text);

            int cont = 0;
            int cod = 10;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_temperatura = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_temperatura_pref != cod_temperatura)
                    {
                     
                        txttemperatura.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_temperatura));
                    }
                }

                else
                {
                    cod_temperatura = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_temperatura_pref != cod_temperatura)
                    {
                        txttemperatura.Text = txttemperatura.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_temperatura));
                    }
                }

                Ayuda.MoveNext();
            }

            if (cod_temperatura == 0)
            {
                txttemperatura.Text = "";
            }
        }


   

        #endregion

        #region Cargar Volumen

        protected void Cargar_tipo_volumen()
        {
            Cargar_datos_Parametro(ref Ayuda, 9, txtvolumen.Text);

            if (!Ayuda.EOF())
            {
                cod_volumen_pref = Convert.ToInt32(Ayuda.Fields(0));
                
                    txtvolumen_pref.Text = "Preferencial (" + Ayuda.Fields(1) + ")";
              
            }
            else
            {
                if (cod_volumen_pref == 0)
                {
                    txtvolumen.Text = "";
                }
            }

        }

        protected void Cargar_tipo_volumen_2()
        {
            Cargar_datos_2(ref Ayuda,cod_volumen_pref,9,txtvolumen.Text);

            int cont = 0;
            int cod = 9;
            while (!Ayuda.EOF())
            {
                cont++;
                if (cont == 1)
                {
                    cod_volumen = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_volumen_pref != cod_volumen)
                    {
                       
                        txtvolumen.Text = Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_volumen));
                    }
                }

                else
                {
                    cod_volumen = Convert.ToInt32(Ayuda.Fields(0));
                    if (cod_volumen_pref != cod_volumen)
                    {

                        txtvolumen.Text = txtvolumen.Text + "-" + Ayuda.Fields(1);
                        agregar_componentes(cod, Convert.ToInt32(cod_volumen));
                     }
                }

                Ayuda.MoveNext();
            }

            if (cod_volumen == 0)
            {
                txtvolumen.Text = "";
            }

        }


   
        #endregion


       #region  Agregar Componemtes

       private void Crear_Tbl_Componentes()
       {
           dt_componentes_alimentos.Columns.Clear();

           dt_componentes_alimentos.Columns.Add("cod_tipo_distribucion", typeof(int));
           dt_componentes_alimentos.Columns.Add("cod_seccion", typeof(int));
           dt_componentes_alimentos.Columns.Add("cod_componente", typeof(int));
       }

       protected void agregar_componentes(int cod_seccion, int cod_componente)
       {
           if (cod_seccion > 0 && cod_componente > 0 && cod_distribucion > 0)
           {
             
               DataRow Fila = dt_componentes_alimentos.NewRow();
               Fila["cod_tipo_distribucion"] = cod_distribucion;
               Fila["cod_seccion"] = cod_seccion;
               Fila["cod_componente"] = cod_componente;
               dt_componentes_alimentos.Rows.Add(Fila);
           }
       }


       #endregion

       #region  Guardar Alimentos

      protected  void Guardar_parametros()
       {
           double proteinas = txtproteinas.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtproteinas.Text);
           double hyc = txthyc.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txthyc.Text);
           double fibra = txtfibra.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtfibra.Text);
           double lipidos = txtlipidos.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtlipidos.Text);
           double agsaturados = txtagsaturados.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtagsaturados.Text);
           double agmonoinsat = txtagmonoinsat.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtagmonoinsat.Text);
           double agpoliinsat = txtagpoliinsat.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtagpoliinsat.Text);
           double colesterol = txtcolesterol.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtcolesterol.Text);
           double n6 = txtn6.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtn6.Text);
           double n3 = txtn3.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtn3.Text);
           double calcio = txtcalcio.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtcalcio.Text);
           double hierro = txthierro.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txthierro.Text);
           double magnesio = txtmagnesio.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtmagnesio.Text);
           double fosforo = txtfosforo.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtfosforo.Text);
           double potacio = txtpotacio.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtpotacio.Text);
           double sodio = txtsodio.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtsodio.Text);
           double zinc = txtzinc.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtzinc.Text);
           double calorias = txtcalorias.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtcalorias.Text);
           double gr = txtgr.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtgr.Text);
           double cc = txtcc.Text.Equals(string.Empty) ? 0 : Convert.ToDouble(txtcc.Text);


           string estado = "S";
           if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

           CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK +".P_REGISTRAR_ALIMENTOS");

           CnnFalp.ParametroBD("PIN_CODIGO", cod_distribucion, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_DESCRIPCION", txtdescripcion.Text.ToUpper().Trim().Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U"), DbType.String, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_USUARIO", User.ToUpper().Trim(), DbType.String, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_GR", gr, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_CC", cc, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_CONSISTENCIA", Convert.ToInt32(cod_consistencia_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_DIGESTABILIDAD", Convert.ToInt32(cod_digestabilidad_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_SACAROSA", Convert.ToInt32(cod_dulzor_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_LACTOSA", Convert.ToInt32(cod_lactosa_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_SAL", Convert.ToInt32(cod_sal_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_TEMPERATURA", Convert.ToInt32(cod_temperatura_pref), DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_VOLUMEN", Convert.ToInt32(cod_volumen_pref), DbType.Int64, ParameterDirection.Input);

           CnnFalp.ParametroBD("PIN_PROTEINAS", proteinas, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_HYC", hyc, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_FIBRA", fibra, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_LIPIDOS", lipidos, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_AGSATURADOS", agsaturados, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_AGMONOINSAT", agmonoinsat, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_AGPOLIINSAT", agpoliinsat, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_COLESTEROL", colesterol, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_N6", n6, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_N3", n3, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_CALCIO", calcio, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_HIERRO", hierro, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_MAGNESIO", magnesio, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_FOSFORO", fosforo, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_POTACIO", potacio, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_SODIO", sodio, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_ZINC", zinc, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_CALORIAS", calorias, DbType.Double, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_NO_SAL", no_sal, DbType.String, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_NO_DULZOR", no_dulzor, DbType.String, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_NO_LACTOSA", no_lactosa, DbType.String, ParameterDirection.Input);





           CnnFalp.ParametroBD("POUT_ALIMENTOS", 0, DbType.Int64, ParameterDirection.Output);

           int registro = CnnFalp.ExecuteNonQuery();

           cod_alimentos = Convert.ToInt32(CnnFalp.ParamValue("POUT_ALIMENTOS").ToString());

       
       }

      protected void Guardar_componentes(int seccion, int componente)
       {

     
           if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

           CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK + ".P_REGISTRAR_DET_ALIMENTOS");

           CnnFalp.ParametroBD("PIN_CODIGO", cod_alimentos, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_SECCION", seccion, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_COMPONENTE", componente, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_USUSARIO", User.ToUpper().Trim(), DbType.String, ParameterDirection.Input);


           int registro = CnnFalp.ExecuteNonQuery();

       }

      protected  void Guardar_distribucion(int distribucion, int alimento)
       {

           string estado = "S";
           if (CnnFalp.Estado == ConnectionState.Closed) CnnFalp.Abrir();

           CnnFalp.CrearCommand(CommandType.StoredProcedure, PCK +".P_REGISTRAR_DET_DISTR_ALIMEN");


           CnnFalp.ParametroBD("PIN_DISTRIBUCION", distribucion, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_ALIMENTO", cod_alimentos, DbType.Int64, ParameterDirection.Input);
           CnnFalp.ParametroBD("PIN_USUSARIO", User.ToUpper().Trim(), DbType.String, ParameterDirection.Input);


           int registro = CnnFalp.ExecuteNonQuery();


       }

       protected void recorrer_dt_det_componentes()
       {
           int seccion = 0;
           int componente = 0;
           int categoria = 0;

           foreach (DataRow miRow in dt_componentes_alimentos.Rows)
           {
               seccion = Convert.ToInt32(miRow["cod_seccion"].ToString());
               componente = Convert.ToInt32(miRow["cod_componente"].ToString());

               Guardar_componentes(seccion, componente);
           }


       }

       protected void recorrer_dt_det_distribucion()
       {
           int distribucion = 0;
           int alimento = 0;


           foreach (DataRow miRow in dt_distribucion_alimentos.Rows)
           {
               distribucion = Convert.ToInt32(miRow["cod_dist_distribucion"].ToString());
               alimento = Convert.ToInt32(miRow["cod_dist_alimento"].ToString());

               Guardar_distribucion(distribucion, alimento);
           }


       }

       protected void agregar_distribuciones(int cod_distribucion)
       {

           DataRow Fila = dt_distribucion_alimentos.NewRow();


           Fila["cod_dist_distribucion"] = cod_distribucion;
           Fila["cod_dist_alimento"] = cod_alimentos;

           dt_distribucion_alimentos.Rows.Add(Fila);
       }

       private void Crear_Tbl_Distribucion()
       {
           dt_distribucion_alimentos.Columns.Clear();

           dt_distribucion_alimentos.Columns.Add("cod_dist_distribucion", typeof(int));
           dt_distribucion_alimentos.Columns.Add("cod_dist_alimento", typeof(int));
       }

      #endregion

       #region Bloquear

       protected void bloqueo()
       {
           limpiar_alimentos();
           limpiar_componentes();
           txtregimen.Enabled = false;
           btn_regimen.Enabled = false;

           limpiar_ingesta();
           txtproteinas.Enabled = false;

           btn_limpiar_menu.Enabled = false;
           btn_limpiar_componentes.Enabled = false;
           btn_limpiar_ingesta.Enabled = false;
       }


       #endregion

       #region  No aplica

       protected void   agregar_no_aplica(int cod)
        {
            switch(cod)
            {

                case 1: no_sal = "S"; break;
                case 2: no_dulzor = "S"; break;
                case 3: no_lactosa = "S"; break;

            }

        }

       #endregion

        #endregion

       #region Validaciones

       protected Boolean Validar_Campos_1()
       {
           Boolean var = false;

           if (txtdistribucion.Text == "" && cod_distribucion == 0)
           {
               MessageBox.Show("Estimado usuario, El Campo Tipo Distribución se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
               txtdistribucion.Focus();
           }
           else
           {
               if (txtdescripcion.Text == "" )
               {
                   MessageBox.Show("Estimado usuario, El Campo Descripción se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   txtdescripcion.Focus();
               }
               else
               {
                   if (txtgr.Text == "")
                   {
                       MessageBox.Show("Estimado usuario, El Campo Gr se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       txtgr.Focus();
                   }
                   else
                   {
                       if (txtcc.Text == "")
                       {
                           MessageBox.Show("Estimado usuario, El Campo Cc se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           txtcc.Focus();
                       }
                       else
                       {

                           int cont = 0;
                           foreach (DataRow miRow in dt.Select("Descripcion = '" + txtdescripcion.Text.ToUpper() + "'"))
                           {
                               cont++;
                           }

                           if (cont == 0)
                           {

                               var = true;

                           }
                           else
                           {
                               MessageBox.Show("Estimado usuario, El Alimento ya se encuentra ingresado", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               var = false;
                               txtdescripcion.Text = "";
                               txtdescripcion.Focus();
                           }
                       }
                   }

               }
           }

           return var;
       }

       protected Boolean Validar_Campos_2()
       {
           Boolean var = false;

           if (txtregimen.Text == "" && cod_regimen == 0)
           {
               MessageBox.Show("Estimado usuario, El Campo Cobro se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
               txtdistribucion.Focus();
           }
           else
           {
               if (txtconsistencia.Text == "" && cod_consistencia == 0)
               {
                   MessageBox.Show("Estimado usuario, El Campo Consistencia se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   txtconsistencia.Focus();
               }
               else
               {
                   if (txtdigestabilidad.Text == "" && cod_digestabilidad == 0)
                   {
                       MessageBox.Show("Estimado usuario, El Campo Digestabilidad se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       txtdigestabilidad.Focus();
                   }
                   else
                   {
                       if (txtdulzor.Text == "" && cod_dulzor == 0 && cod_dulzor_pref!=8)
                       {
                           MessageBox.Show("Estimado usuario, El Campo Sacarosa se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           txtdulzor.Focus();
                       }
                       else
                       {
                           if (txtlactosa.Text == "" && cod_lactosa == 0  && cod_lactosa_pref!=8)
                           {
                               MessageBox.Show("Estimado usuario, El Campo Lactosa se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               txtlactosa.Focus();
                           }
                           else
                           {
                               if (txtsal.Text == "" && cod_sal == 0  && cod_sal_pref!=8)
                               {
                                   MessageBox.Show("Estimado usuario, El Campo Sal se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                   txtsal.Focus();
                               }
                               else
                               {

                                   if (txttemperatura.Text == "" && cod_temperatura == 0  && cod_temperatura_pref!=8)
                                   {
                                       MessageBox.Show("Estimado usuario, El Campo Temperatura se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                       txttemperatura.Focus();
                                   }
                                   else
                                   {
                                       if (txtvolumen.Text == "" && cod_volumen == 0  && cod_volumen_pref!=8)
                                       {
                                           MessageBox.Show("Estimado usuario, El Campo Volumen se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                           txtvolumen.Focus();
                                       }
                                       else
                                       {
                                           var = true;

                                       }
                                   }
                               }
                           }
                       }
                   }

               }
           }

           return var;
       }

       protected Boolean Validar_Campos_3()
       {
           Boolean var = false;

           if (txtproteinas.Text == "")
           {
               MessageBox.Show("Estimado usuario, El Campo Proteinas se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
               txtproteinas.Focus();
           }
           else
           {
               if (txtlipidos.Text == "" )
               {
                   MessageBox.Show("Estimado usuario, El Campo Lipidos se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                   txtlipidos.Focus();
               }
               else
               {
                   if (txtagpoliinsat.Text == "" )
                   {
                       MessageBox.Show("Estimado usuario, El Campo A.G Poliinsat se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                       txtagpoliinsat.Focus();
                   }
                   else
                   {
                       if (txtn3.Text == "" )
                       {
                           MessageBox.Show("Estimado usuario, El Campo N3 se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                           txtn3.Focus();
                       }
                       else
                       {
                           if (txtmagnesio.Text == "" )
                           {
                               MessageBox.Show("Estimado usuario, El Campo Magnesio se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                               txtmagnesio.Focus();
                           }
                           else
                           {
                               if (txtsodio.Text == "" )
                               {
                                   MessageBox.Show("Estimado usuario, El Campo Sodio se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                   txtsodio.Focus();
                               }
                               else
                               {

                                   if (txthyc.Text == "" )
                                   {
                                       MessageBox.Show("Estimado usuario, El Campo HYC se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                       txthyc.Focus();
                                   }
                                   else
                                   {
                                       if (txtagsaturados.Text == "")
                                       {
                                           MessageBox.Show("Estimado usuario, El Campo A.G Saturados se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                           txtagsaturados.Focus();
                                       }
                                       else
                                       {
                                           if (txtcolesterol.Text == "")
                                           {
                                               MessageBox.Show("Estimado usuario, El Campo Colesterol se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                               txtcolesterol.Focus();
                                           }
                                           else
                                           {
                                               if (txtcalcio.Text == "")
                                               {
                                                   MessageBox.Show("Estimado usuario, El Campo Calcio se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                   txtcalcio.Focus();
                                               }
                                               else
                                               {
                                                   if (txtfosforo.Text == "")
                                                   {
                                                       MessageBox.Show("Estimado usuario, El Campo Fosforo se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                       txtfosforo.Focus();
                                                   }
                                                   else
                                                   {
                                                       if (txtzinc.Text == "")
                                                       {
                                                           MessageBox.Show("Estimado usuario, El Campo Zinc se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                           txtzinc.Focus();
                                                       }
                                                       else
                                                       {
                                                           if (txtfibra.Text == "")
                                                           {
                                                               MessageBox.Show("Estimado usuario, El Campo Fibra se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                               txtfibra.Focus();
                                                           }
                                                           else
                                                           {
                                                               if (txtagmonoinsat.Text == "")
                                                               {
                                                                   MessageBox.Show("Estimado usuario, El Campo A.G MonoSaturados se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                   txtagmonoinsat.Focus();
                                                               }
                                                               else
                                                               {
                                                                   if (txtn6.Text == "")
                                                                   {
                                                                       MessageBox.Show("Estimado usuario, El Campo N6 se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                       txtn6.Focus();
                                                                   }
                                                                   else
                                                                   {
                                                                       if (txthierro.Text == "")
                                                                       {
                                                                           MessageBox.Show("Estimado usuario, El Campo Hierro se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                           txthierro.Focus();
                                                                       }
                                                                       else
                                                                       {
                                                                           if (txtpotacio.Text == "")
                                                                           {
                                                                               MessageBox.Show("Estimado usuario, El Campo Potasio se encuentra vacio", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                               txtpotacio.Focus();
                                                                           }
                                                                           else
                                                                           {
                                                                               var = true;

                                                                           }
                                                                       }
                                                                   }
                                                               }
                                                           }
                                                       }
                                                   }
                                               }
                                           }
                                       }
                                   }
                               }
                           }
                       }
                   }

               }
           }

           return var;
       }

       private void txtdistribucion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && (e.KeyChar != (char)Keys.Space))
            {

                e.Handled = true;
                return;
            }
            if (e.KeyChar == (char)13)
            {
            
                Cargar_tipo_distribucion();
             
                txtdescripcion.Enabled = true;
                if (cod_distribucion == 0)
                {
                    
                   // txtdescripcion.Enabled = false;
                    txtdistribucion.Focus();
                
                }
                else
                {

                    Cargar_grilla();
                    txtdescripcion.Focus();
                    txtdescripcion.Enabled = true;
                    btn_limpiar_menu.Enabled = true;
                }
            }
        }


        private void txtdescripcion_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && (e.KeyChar != (char)Keys.Space) && !(char.IsNumber(e.KeyChar)) && !(e.KeyChar == '/'))
                {
                 //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Handled = true;
                    return;
                }
                else
                {
                    if (e.KeyChar == (char)13)
                    {
                       
                        if (txtdescripcion.Text == "")
                        {

                          //  txtgr.Enabled = false;
                            txtdescripcion.Focus();
                        }
                        else
                        {
                         
                            txtgr.Enabled = true;
                            txtgr.Focus();
                        } 
                    }
            }
        }

        private void txtgr_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
                {
                  //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    e.Handled = true;
                    return;
                }
                else
                {
                    if (e.KeyChar == (char)13)
                    {


                        if (txtgr.Text == "")
                        {
                          //  txtcc.Enabled = false;
                            txtgr.Focus();
                        }
                        else
                        {
                            txtcc.Enabled = true;
                            txtcc.Focus();
                        }
                    }
                     
                
            }
        }

        private void txtcc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
          //      MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {
                if (txtcc.Text == "")
                {
               //     btn_confirmar.Enabled = false; 
                    txtcc.Focus();
                }
                else
                {
                    btn_confirmar.Enabled = true;
                    btn_confirmar.Focus();
                }
             
            }
        }

        private void txtregimen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {
                Cargar_tipo_cobro_2();

                if (cod_regimen == 0)
                {
                    txtconsistencia.Enabled = false;
                    btn_consistencia.Enabled = false;
                    txtregimen.Focus();
                
                }
                else
                {
                    txtconsistencia.Enabled = true;
                    btn_confirmar.Enabled = true;
                    txtconsistencia.Focus();
                }

           
            }
        }

        private void txtconsistencia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                Cargar_tipo_consistencia();
                if (cod_consistencia_pref == 0)
                {
                    txtdigestabilidad.Enabled = false;
                    btn_digestabilidad.Enabled = false;
                    txtconsistencia.Focus();
                
                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_consistencia_2();
                        if (cod_consistencia == 0)
                        {
                            txtdigestabilidad.Enabled = false;
                            btn_digestabilidad.Enabled = false;
                            txtconsistencia.Focus();
                            
                        }
                        else
                        {
                            txtdigestabilidad.Enabled = true;
                            btn_digestabilidad.Enabled = true;
                            txtdigestabilidad.Focus();
                        }
                    }
              
                else
                    {
                        txtdigestabilidad.Enabled = true;
                        btn_digestabilidad.Enabled = true;
                       txtdigestabilidad.Focus();
                       cod_consistencia = cod_consistencia_pref;
                       txtconsistencia.Text = txtconsistencia_pref.Text;
                       agregar_componentes(4, Convert.ToInt32(cod_consistencia));
                   

                    }
               }
            
            }
      
        }

        private void txtdigestabilidad_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
               // MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                Cargar_tipo_digestabilidad();

                if (cod_digestabilidad_pref == 0)
                {
                    txtdulzor.Enabled = false;
                    btn_dulzor.Enabled = false;
                    txtdigestabilidad.Focus();
                 
                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_digestabilidad_2();
                        if (cod_digestabilidad == 0)
                        {
                            txtdigestabilidad.Enabled = false;
                            txtconsistencia.Focus();

                          
                        }
                        else
                        {
                           
                            txtdulzor.Enabled = true;
                            btn_dulzor.Enabled = true;
                            txtdulzor.Focus();
                         
                        }
                    }

                    else
                    {
                        txtdulzor.Enabled = true;
                        btn_dulzor.Enabled = true;
                        txtdulzor.Focus();
                        cod_digestabilidad = cod_digestabilidad_pref;
                        txtdigestabilidad.Text = txtdigestabilidad_pref.Text;
                        agregar_componentes(5, Convert.ToInt32(cod_digestabilidad));

                    }


                }//
              
            }
         
        }

        private void txtdulzor_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                valid_dulzor();
            }
        }

        private void txtlactosa_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                valid_lactosa();
           
            }
        }

        private void txtsal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {
                valid_sal();
            }

               
        }

        private void txttemperatura_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                valid_temperatura();
            }
        }

        private void txtvolumen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsLetter(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
            //    MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;

                return;
            }
            if (e.KeyChar == (char)13)
            {

                valid_volumen();
            }
        }


    

        private void txtproteinas_KeyPress(object sender, KeyPressEventArgs e)

        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtproteinas.Text == "")
                    {
                        txtlipidos.Enabled = false;
                        txtproteinas.Focus();
                    }
                    else
                    {
                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {

                            // Si es válido se almacena el valor actual en la variable privada
                            txtlipidos.Text = textBox.Text;
                            txtlipidos.Enabled = true;
                            txtlipidos.Focus();
                        }
                        else
                        {
                            txtproteinas.Text = "";
                        }

                    }
                }
                
                
            }
        }

        private void txtlipidos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
               // MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtlipidos.Text == "")
                    {
                        txtagpoliinsat.Enabled = false;
                        txtlipidos.Focus();
                    }
                    else
                    {
                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {
                            // Si es válido se almacena el valor actual en la variable privada
                            txtagpoliinsat.Text = textBox.Text;
                            txtagpoliinsat.Enabled = true;
                            txtagpoliinsat.Focus();
                        }
                        else
                        {
                            txtlipidos.Text = "";
                        }
                   
                    }
                
                }
            }
        }

        private void txtagpoliinsat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtagpoliinsat.Text == "")
                    {
                        txtn3.Enabled = false;
                        txtagpoliinsat.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtn3.Enabled = true;
                             txtn3.Focus();
                         }
                         else
                         {
                             txtagpoliinsat.Text = "";
                         }
                    }
                }
            }
        }

        private void txtn3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() !=".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtn3.Text == "")
                    {
                        txtmagnesio.Enabled = false;
                        txtn3.Focus();
                    }
                    else
                    {
                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {
                            // Si es válido se almacena el valor actual en la variable privada
                            txtmagnesio.Text = textBox.Text;
                            txtmagnesio.Enabled = true;
                            txtmagnesio.Focus();
                        }
                        else
                        {
                            txtn3.Text = "";
                        }              
                    }
                 
                }
            }
        }

        private void txtmagnesio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtmagnesio.Text == "")
                    {
                        txtsodio.Enabled = false;
                        txtmagnesio.Focus();
                    }
                    else
                    {

                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {
                            // Si es válido se almacena el valor actual en la variable privada
                            txtsodio.Text = textBox.Text;
                            txtsodio.Enabled = true;
                            txtsodio.Focus();
                        }
                        else
                        {
                            txtmagnesio.Text = "";
                        }   
                      
                    }
                  
                }
            }
        }

        private void txtsodio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {

                    if (txtsodio.Text == "")
                    {
                        txthyc.Enabled = false;
                        txtsodio.Focus();
                    }
                    else
                    {
                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {
                            // Si es válido se almacena el valor actual en la variable privada
                            txthyc.Text = textBox.Text;
                            txthyc.Enabled = true;
                            txthyc.Focus();
                        }
                        else
                        {
                            txtsodio.Text = "";
                        }  
                   
                    }
                 
                  
                }
            }
        }

        private void txthyc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {

                    if (txthyc.Text == "")
                    {
                        txtagsaturados.Enabled = false;
                        txthyc.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtagsaturados.Enabled = true;
                             txtagsaturados.Focus();
                         }
                         else
                         {
                             txthyc.Text = "";
                         }
                    }
                   
                }
            }
        }

        private void txtagsaturados_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtagsaturados.Text == "")
                    {
                        txtcolesterol.Enabled = false;
                        txtagsaturados.Focus();
                    }
                    else
                    {
                        
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtcolesterol.Enabled = true;
                             txtcolesterol.Focus();
                         }
                         else
                         {
                             txtagsaturados.Text = "";
                         }
                    }

                
                }
            }
        }

        private void txtcolesterol_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtcolesterol.Text == "")
                    {
                        txtcalcio.Enabled = false;
                        txtcolesterol.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtcalcio.Enabled = true;
                             txtcalcio.Focus();
                         }
                         else
                         {
                             txtcolesterol.Text = "";
                         }
                    }

                 
                }
            }
        }

        private void txtcalcio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
               // MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtcalcio.Text == "")
                    {
                        txtfosforo.Enabled = false;
                        txtcalcio.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtfosforo.Enabled = true;
                             txtfosforo.Focus();
                         }
                         else
                         {
                             txtcalcio.Text = "";
                         }
                    }
                  
                }
            }
        }

        private void txtfosforo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {

                    if (txtfosforo.Text == "")
                    {
                        txtzinc.Enabled = false;
                        txtfosforo.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtzinc.Enabled = true;
                             txtzinc.Focus();
                         }
                         else
                         {
                             txtfosforo.Text = "";
                         }
                    }
                  
                }
            }
        }

        private void txtzinc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtzinc.Text == "")
                    {
                        txtfibra.Enabled = false;
                        txtzinc.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtfibra.Enabled = true;
                             txtfibra.Focus();
                         }
                         else
                         {
                             txtzinc.Text = "";
                         }
                    }
                  
                }
            }
        }

        private void txtfibra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtfibra.Text == "")
                    {
                        txtagmonoinsat.Enabled = false;
                        txtfibra.Focus();
                    }
                    else
                    {
                        var textBox = (TextBox)sender;
                        if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                        {
                            txtagmonoinsat.Enabled = true;
                            txtagmonoinsat.Focus();
                        }
                        else
                        {
                            txtfibra.Text = "";
                        }
                    }
            
                }
            }
        }

        private void txtagmonoinsat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtagmonoinsat.Text == "")
                    {
                        txtn6.Enabled = false;
                        txtagmonoinsat.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtn6.Enabled = true;
                             txtn6.Focus();
                         }
                         else
                         {
                             txtagmonoinsat.Text = "";
                         }
                    }
                 
                }
            }
        }

        private void txtn6_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtn6.Text == "")
                    {
                        txthierro.Enabled = false;
                        txtn6.Focus();
                    }
                    else
                    {
                          var textBox = (TextBox)sender;
                          if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                          {
                              txthierro.Enabled = true;
                              txthierro.Focus();
                          }
                          else
                          {
                              txtn6.Text = "";
                          }
                    }
                 
                }
            }
        }

        private void txthierro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txthierro.Text == "")
                    {
                        txtpotacio.Enabled = false;
                        txthierro.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtpotacio.Enabled = true;
                             txtpotacio.Focus();
                         }
                         else
                         {
                             txthierro.Text = "";
                         }
                    }
                 
                }
            }
        }

        private void txtpotacio_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
             //   MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtpotacio.Text == "")
                    {
                        txtcalorias.Enabled = false;
                        txtpotacio.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             txtcalorias.Enabled = true;
                             txtcalorias.Focus();
                         }
                         else
                         {
                             txtpotacio.Text = "";
                         }
                    }
               
                }
            }
        }


        private void txtcalorias_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(char.IsNumber(e.KeyChar)) && (e.KeyChar != (char)Keys.Back) && (e.KeyChar != (char)Keys.Enter) && ((e.KeyChar.ToString() != ".")))
            {
              //  MessageBox.Show("Información ingresa es incorrecta", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Handled = true;
                return;
            }
            else
            {
                if (e.KeyChar == (char)13)
                {
                    if (txtcalorias.Text == "")
                    {
                        btn_guardar.Enabled = false;
                        txtcalorias.Focus();
                    }
                    else
                    {
                         var textBox = (TextBox)sender;
                         if (Regex.IsMatch(textBox.Text, @"^(?:\d+\.?\d*)?$"))
                         {
                             btn_guardar.Enabled = true;
                             btn_guardar.Focus();
                         }
                         else
                         {
                             txtcalorias.Text = "";
                         }
                    }

                }
            }
        }

       


        #endregion

        private void CambiarBlanco_TextLeave(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.White;

        }

        private void CambiarColor_TextEnter(object sender, EventArgs e)
        {
            TextBox GB = (TextBox)sender;
            GB.BackColor = Color.FromArgb(255, 224, 192);
        }

        private void txtdistribucion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
	        {
               
	        }
               

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }



        protected void valid_dulzor()
        {
             Cargar_tipo_sacarosa();

             if (cod_dulzor_pref != 8)
             {
                 

                 if (cod_dulzor_pref == 0)
                 {
                     txtlactosa.Enabled = false;
                     btn_lactosa.Enabled = false;
                     txtdulzor.Focus();

                 }
                 else
                 {
                     DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                     if (resp == DialogResult.Yes)
                     {
                         Cargar_tipo_sacarosa_2();
                         if (cod_dulzor == 0)
                         {
                             txtlactosa.Enabled = false;
                             txtdulzor.Focus();

                         }
                         else
                         {
                             txtlactosa.Enabled = true;
                             btn_lactosa.Enabled = true;
                             txtlactosa.Focus();
                         }
                     }
                     else
                     {
                         txtlactosa.Enabled = true;
                         btn_lactosa.Enabled = true;
                         txtlactosa.Focus();
                         cod_dulzor = cod_dulzor_pref;
                         txtdulzor.Text = txtdulzor_pref.Text;
                         agregar_componentes(7, Convert.ToInt32(cod_dulzor));

                     }
                 }

             }
             else
             {
                 txtlactosa.Enabled = true;
                 btn_lactosa.Enabled = true;
                 txtlactosa.Focus();
             }
        }

        protected void valid_lactosa()
        {

            Cargar_tipo_lactosa();

            if (cod_lactosa_pref != 8)
            {
                if (cod_lactosa_pref == 0)
                {
                    txtsal.Enabled = false;
                    btn_sal.Enabled = false;
                    txtlactosa.Focus();

                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_lactosa_2();
                        if (cod_lactosa == 0)
                        {
                            txtsal.Enabled = false;
                            txtlactosa.Focus();

                        }
                        else
                        {
                            txtsal.Enabled = true;
                            btn_sal.Enabled = true;
                            txtsal.Focus();
                        }
                    }
                    else
                    {
                        txtsal.Enabled = true;
                        btn_sal.Enabled = true;
                        txtsal.Focus();
                        cod_lactosa = cod_lactosa_pref;
                        txtlactosa.Text = txtlactosa_pref.Text;
                        agregar_componentes(8, Convert.ToInt32(cod_lactosa));

                    }
                }
            }
            else
            {
                txtsal.Enabled = true;
                btn_sal.Enabled = true;
                txtsal.Focus();
            }
        }

        protected void valid_sal()
        {
            Cargar_tipo_sal();

            if (cod_sal_pref != 8)
            {
                if (cod_sal_pref == 0)
                {
                    txttemperatura.Enabled = false;
                    btn_temperatura.Enabled = false;
                    txtsal.Focus();

                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_sal_2();
                        if (cod_sal == 0)
                        {
                            txttemperatura.Enabled = false;
                            btn_temperatura.Enabled = false;
                            txtsal.Focus();

                        }
                        else
                        {
                            txttemperatura.Enabled = true;
                            btn_temperatura.Enabled = true;
                            txttemperatura.Focus();
                        }
                    }

                    else
                    {
                        txttemperatura.Enabled = true;
                        btn_temperatura.Enabled = true;
                        txttemperatura.Focus();
                        cod_sal = cod_sal_pref;
                        txtsal.Text = txtsal_pref.Text;
                        agregar_componentes(6, Convert.ToInt32(cod_sal));
                    }
                }
            }
            else
            {
                txttemperatura.Enabled = true;
                btn_temperatura.Enabled = true;
                txttemperatura.Focus();

            }
        }

        protected void valid_temperatura()
        {
            Cargar_tipo_temperatura();
            if (cod_temperatura_pref != 8)
            {

                if (cod_temperatura_pref == 0)
                {
                    txtvolumen.Enabled = false;
                    btn_volumen.Enabled = false;
                    txttemperatura.Focus();

                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_temperatura_2();
                        if (cod_temperatura == 0)
                        {
                            txtvolumen.Enabled = false;
                            btn_volumen.Enabled = false;
                            txttemperatura.Focus();

                        }
                        else
                        {
                            txtvolumen.Enabled = true;
                            btn_volumen.Enabled = true;
                            txtvolumen.Focus();
                        }
                    }
                    else
                    {
                        txtvolumen.Enabled = true;
                        btn_volumen.Enabled = true;
                        txtvolumen.Focus();
                        cod_temperatura = cod_temperatura_pref;
                        txttemperatura.Text = txttemperatura_pref.Text;
                        agregar_componentes(10, Convert.ToInt32(cod_temperatura));

                    }
                }
            }
            else
            {
                txtvolumen.Enabled = true;
                btn_volumen.Enabled = true;
                txtvolumen.Focus();
            }

           
             
             
        }

        protected void valid_volumen()
        {
            Cargar_tipo_volumen();

            if (cod_volumen_pref != 8)
            {
                if (cod_volumen_pref == 0)
                {
                    txtproteinas.Enabled = false;
                    txtvolumen.Focus();

                }
                else
                {
                    DialogResult resp = MessageBox.Show("Estimado Usuario, Desea seleccionar Múltiples Opciones", "Información", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (resp == DialogResult.Yes)
                    {
                        Cargar_tipo_volumen_2();
                        if (cod_volumen == 0)
                        {
                            txtproteinas.Enabled = false;
                            txtvolumen.Focus();

                        }
                        else
                        {
                            txtproteinas.Enabled = true;
                            txtproteinas.Focus();
                        }
                    }
                    else
                    {
                        txtvolumen.Enabled = true;
                        txtvolumen.Focus();
                        btn_volumen.Enabled = true;
                        cod_volumen = cod_volumen_pref;
                        txtvolumen.Text = txtvolumen_pref.Text;
                        agregar_componentes(9, Convert.ToInt32(cod_volumen));
                    }
                }
            }
            else
            {
                txtproteinas.Enabled = true;
                txtproteinas.Focus();
            }
        }

        private void txtlactosa_TextChanged(object sender, EventArgs e)
        {

        }

    }
}
