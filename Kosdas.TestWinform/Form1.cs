#region
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
#endregion

namespace Kosdas.TestWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            StockLoader.Instance.ProgressChanged += StockLoader_ProgressChanged;
            StockLoader.Instance.RequestSending += StockLoader_RequestSending;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            StockLoader.Instance.ProgressChanged -= StockLoader_ProgressChanged;
            StockLoader.Instance.RequestSending -= StockLoader_RequestSending;

            base.OnClosing(e);
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            prbProgress.Value = 0;
            await StockLoader.Instance.LoadAsync();
            
            MessageBox.Show($"Finished with {StockLoader.Instance.Count():N0} stocks");
        }

        private void StockLoader_ProgressChanged(object sender, StockLoader.ProgressChangedEventArgs e)
        {
            prbProgress.Invoke(new Action(() => prbProgress.Value = (int) e.Percent));
        }

        private void StockLoader_RequestSending(object sender, StockLoader.RequestSendingEventArgs e)
        {
            if (e.Market == Market.KQ)
            {
                e.Cancel = true;
                return;
            }

            Invoke(new Action(() => Text = $"{e.Market}, {e.Page}, {e.Url}"));
        }
    }
}