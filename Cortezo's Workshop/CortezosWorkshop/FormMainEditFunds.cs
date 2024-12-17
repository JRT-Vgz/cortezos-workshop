﻿
using _2___Servicios.Services;
using _2___Servicios.Services.ShopStatServices;
using _3_Presenters.ViewModels;

namespace CortezosWorkshop
{
    public partial class FormMainEditFunds : Form
    {
        #region Constructor
        private readonly ConfigurationService _configuration;
        private readonly GetFundsByNameService<FundsViewModel> _getFundsByNameService;
        private readonly SumToCajaFuerteService _sumToCajaFuerteService;

        private const int _MAX_LENGTH_TEXTBOX = 8;

        public FormMainEditFunds(ConfigurationService configuration,
            GetFundsByNameService<FundsViewModel> getFundsByNameService,
            SumToCajaFuerteService sumToCajaFuerteService)
        {
            InitializeComponent();
            _configuration = configuration;
            _getFundsByNameService = getFundsByNameService;
            _sumToCajaFuerteService = sumToCajaFuerteService;
        }
        #endregion

        #region CargarDatos

        // -------------------------------------------------------------------------------------------------------
        // -------------------------------------------- CARGAR DATOS ---------------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private async void FormMainEditFunds_Load(object sender, EventArgs e)
        {
            this.Location = new Point(this.Location.X - 350, this.Location.Y + 165);
            await Load_Funds();
        }
        private async Task Load_Funds()
        {
            var fundsViewModel = await _getFundsByNameService.ExecuteAsync(
                _configuration.Configuration["Constants:_SHOPSTAT_CAJA_FUERTE"]);
            lbl_Oro.Text = fundsViewModel.Funds;
        }

        private void FormMainEditFunds_Shown(object sender, EventArgs e)
            => Reset_TextBox();

        private void Reset_TextBox()
        {
            txtBox.Text = "";
            txtBox.Focus();
        }
        #endregion

        #region RellenarTextboxNumerico

        // -------------------------------------------------------------------------------------------------------
        // ------------------------------------- RELLENAR TEXTBOX NUMERICO ---------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private void txtBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var textBox = (sender as TextBox);
                if (textBox.Text.Length == 0) { btn_Back_Click(sender, e); return; }

                int parsedValue;
                if (int.TryParse(textBox.Text, out parsedValue)) 
                { 
                    if (parsedValue == 0) { textBox.Text = ""; return; }
                }

                btn_Save_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }
        private void txtBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = (sender as TextBox);
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                if (e.KeyChar == '-' && textBox.SelectionStart == 0) { return; }

                e.Handled = true;
            }

            if (textBox.Text.Length > _MAX_LENGTH_TEXTBOX && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        #endregion

        #region GuardarDatos

        // -------------------------------------------------------------------------------------------------------
        // -------------------------------------- GUARDAR EN BASE DE DATOS ---------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private async void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                var txtBoxQuantity = int.Parse(txtBox.Text);
                await _sumToCajaFuerteService.ExecuteAsync(txtBoxQuantity);
            }
            catch (Exception) { MessageBox.Show("Ha ocurrido un error inesperado. Prueba otra vez."); }

            Reset_TextBox();
            await Load_Funds();
        }
        #endregion

        // -------------------------------------------------------------------------------------------------------
        // ----------------------------------------------- CERRAR ------------------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private void btn_Back_Click(object sender, EventArgs e)
            => this.Close();
    }
}
