using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Prueba_2_Movil
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class Resultados : ContentPage
	{
        public Resultados()
        {
            InitializeComponent();

            string stream = "$$03928029111192831923871428031111111111##";

            string Temperatura = stream.Substring(2, 4);
            lblTemperatura.Text = "Temperature: " + Temperatura;

            string Humedad = stream.Substring(6, 4);
            lblHumedad.Text = "Humidity: " + Humedad;

            string Audio = stream.Substring(11, 1);
            if (Audio.Equals("1"))
            {
                lblAudio.Text = "Audio: activated";
            }
            else
            {
                lblAudio.Text = "Audio: desactited";
            }


            string Bluetooth = stream.Substring(12, 1);
            if(Bluetooth.Equals("1"))
            {
                lblBluetooth.Text = "Bluetooth: activated";
            }
            else
            {
                lblBluetooth.Text = "Bluetooth: desactivated";
            }

            string Wifi = stream.Substring(13, 1);
            if(Wifi.Equals("1")) 
            { 
                lblWifi.Text = "Wifi: activated";
            }
            else
            {
                lblWifi.Text = "Wifi: desactivated";
            }

            string Led = stream.Substring(30, 10);
            lblLed.Text = "Led: 10/10 activated";

        }

		public string resultado { get; set; }

        private void Button_Clicked(object sender, EventArgs e)
        {
			Navigation.PopAsync();
        }
    }
}