
using _2___Servicios.Interfaces;
using _2___Servicios.Services;
using _2___Servicios.Services.ShopStatServices;
using _3_Presenters.ViewModels;
using CortezosWorkshop.Configuracion;
using CortezosWorkshop.Estadisticas;
using CortezosWorkshop.Maps;
using CortezosWorkshop.Precios;
using Microsoft.Extensions.DependencyInjection;
using System.Media;

namespace CortezosWorkshop
{
    public partial class FormMain : Form
    {
        #region Constructor

        private readonly IServiceProvider _serviceProvider;
        private readonly ConfigurationService _configuration;
        private readonly GetFundsByNameService<FundsViewModel> _getFundsByNameService;
        private readonly ISoundSystem _soundSystem;
        private readonly ILogger _logger;

        private IEnumerable<string> _logEntries;
        public FormMain(IServiceProvider serviceProvider,
            ConfigurationService configuration,
            GetFundsByNameService<FundsViewModel> getFundsByNameService,
            ISoundSystem soundSystem,
            ILogger logger)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _getFundsByNameService = getFundsByNameService;
            _soundSystem = soundSystem;
            _logger = logger;
        }
        #endregion

        #region CargarDatos

        // -------------------------------------------------------------------------------------------------------
        // --------------------------------------------- CARGAR DATOS --------------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private async void FormMain_Load(object sender, EventArgs e)
        {
            await CheckForLogWarnings();
            await Load_Funds();
        }

        private async Task CheckForLogWarnings()
        {
            _logEntries = await _logger.GetLogEntriesAsync();

            if (_logEntries.Any(entry => entry.Contains("WRN:")))
            {
                SystemSounds.Hand.Play();
                MessageBox.Show("Se ha encontrado una entrada de datos sospechosamente sospechosa. Avisa a Vargath para que revise los logs. " +
                    "\n\nPuedes continuar usando el programa sin miedo.", "- - -  AVISO  - - -");
            }
        }

        private async Task Load_Funds()
        {
            var fundsViewModel = await _getFundsByNameService.ExecuteAsync(
                _configuration.Configuration["Constants:_SHOPSTAT_CAJA_FUERTE"]);
            UpdateLabelText(lbl_Oro, fundsViewModel.Funds, lbl_Tesoreria);
        }

        private void UpdateLabelText(Label label, string funds, Label referenceLabel)
        {
            label.Text = funds;
            label.Left = referenceLabel.Left + (referenceLabel.Width - label.Width) / 2;
        }

        #endregion

        #region NavegacionBotones

        // -------------------------------------------------------------------------------------------------------
        // ----------------------------------------------- BOTONES -----------------------------------------------
        // -------------------------------------------------------------------------------------------------------
        private async void btn_mapas_Click(object sender, EventArgs e)
        {
            this.Hide();

            var frm = _serviceProvider.GetRequiredService<FormMapsMain>();
            frm.Location = new Point(this.Location.X, this.Location.Y);
            frm.ShowDialog();

            await Load_Funds();
            this.Show();
        }

        private async void btn_precios_Click(object sender, EventArgs e)
        {
            this.Hide();

            var frm = _serviceProvider.GetRequiredService<FormPreciosMain>();
            frm.Location = new Point(this.Location.X, this.Location.Y);
            frm.ShowDialog();

            await Load_Funds();
            this.Show();
        }

        private async void btn_config_Click(object sender, EventArgs e)
        {
            this.Hide();

            var frm = _serviceProvider.GetRequiredService<FormConfigMain>();
            frm.Location = new Point(this.Location.X, this.Location.Y);
            frm.ShowDialog();

            await Load_Funds();
            this.Show();
        }

        private async void btn_estadisticas_Click(object sender, EventArgs e)
        {
            this.Hide();

            var frm = _serviceProvider.GetRequiredService<FormEstadisticasMain>();
            frm.Location = new Point(this.Location.X, this.Location.Y);
            frm.ShowDialog();

            await Load_Funds();
            this.Show();
        }

        private void btn_addFunds_Click(object sender, EventArgs e)
        {
            var frm = _serviceProvider.GetRequiredService<FormMainEditFunds>();
            lbl_Oro.Text = "???";
            frm.ShowDialog();

            Load_Funds();
        }

        private void btn_beneficio_Click(object sender, EventArgs e)
        {
            var frm = _serviceProvider.GetRequiredService<FormMainBeneficio>();
            lbl_Oro.Text = "???";
            frm.ShowDialog();

            Load_Funds();
        }
        #endregion

        private void btn_portapapeles_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(_configuration.Configuration["Constants:_NOMBRE_RUNA_TIENDA"]);

            _soundSystem.PlaySound(_configuration.Configuration["Constants:_SOUND_MARK_RUNE"]);
        }
    }
}
