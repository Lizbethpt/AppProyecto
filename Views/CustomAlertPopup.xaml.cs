using CommunityToolkit.Maui.Views;
namespace AppProyecto.Views
{
    public partial class CustomAlertPopup : Popup
    {
        public CustomAlertPopup(string message, bool esError)
        {
            InitializeComponent();
            MessageLabel.Text = message;
            if (esError)
            {
                FrameContainer.BackgroundColor = Colors.Red;
                MessageLabel.TextColor = Colors.White;
            }
            else
            {
                FrameContainer.BackgroundColor = Colors.LightGreen;
                MessageLabel.TextColor = Colors.Black;
            }
        }
        void OnAceptarClicked(object sender, EventArgs e)
        {
            Close();
        }
    }
}