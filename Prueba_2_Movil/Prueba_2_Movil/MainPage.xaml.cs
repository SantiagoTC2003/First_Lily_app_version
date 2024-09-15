using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Exceptions;


namespace Prueba_2_Movil
{
    public partial class MainPage : ContentPage
    {
        private readonly IBluetoothLE ble;
        private readonly IAdapter _adaptador;                                  // Clase del adaptador de bluetooth
        private readonly ObservableCollection<IDevice> devicesList;            //Lista para almacenar dispositivos encontrados en el escaneo
        private IDevice _dispositivoSelecionado;                               //Dispositivo para almacenar la informacion del dispositivo seleccionado

        public MainPage()
        {
            InitializeComponent();
            ble = CrossBluetoothLE.Current;
            _adaptador = CrossBluetoothLE.Current.Adapter;                     // Point _bluetoothAdapter to the current adapter on the phone
            devicesList = new ObservableCollection<IDevice>();
            _adaptador.DeviceDiscovered += (sender, args) =>
            {
                if (args.Device != null)   //&& !String.IsNullOrEmpty(args.Device.Name)
                {
                    devicesList.Add(args.Device);
                }
            };
        }

        private async Task<bool> PermissionsGrantedAsync()      // Function to make sure that all the appropriate approvals are in place
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }
            return status == PermissionStatus.Granted;
        }

        public async void NavigateTo(object sender, EventArgs e)
        {
            LabelA.Text = "Connecting...";
            _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(Boton_Escaneo.IsEnabled = false);        // Swith the Isbusy Indicator on
            if (_dispositivoSelecionado.State == DeviceState.Connected)                                                // Check first if we are already connected to the BLE Device 
            {
                await Navigation.PushAsync(new NavigationPage(new Menu()));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(true, true);
                    await _adaptador.ConnectToDeviceAsync(_dispositivoSelecionado, connectParameters);          // if we are not connected, then try to connect to the BLE Device selected
                    await Navigation.PushAsync(new NavigationPage(new Menu()));
                    _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(Boton_Escaneo.IsEnabled = true);
                }
                catch (DeviceConnectionException ex)
                {
                    await DisplayAlert("Error connecting", ex.Message.ToString(), "Retry");       // give an error message if it is not possible to connect
                    _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(Boton_Escaneo.IsEnabled = true);         // Switch off the busy indicator
                    LabelA.Text = "Connection failed";
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            LabelA.Text = "Searching for devices";
            _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(Boton_Escaneo.IsEnabled = false);        // Swith the Isbusy Indicator on
            ListaDispositivos.ItemsSource = null;                                                     // Empty the list of found BLE devices (in the GUI)
            if (!await PermissionsGrantedAsync())                                                           // Make sure there is permission to use Bluetooth
            {
                await DisplayAlert("Permission required", "Application needs location permission", "OK");
                _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(_Indicador_Ocupado.IsEnabled = true);
                return;
            }
            devicesList.Clear();                                                                           // Also clear the _gattDevices list
            if (!_adaptador.IsScanning)                                                              // Make sure that the Bluetooth adapter is scanning for devices
            {
                await _adaptador.StartScanningForDevicesAsync();
            }
            foreach (var device in _adaptador.ConnectedDevices)                                      // Make sure BLE devices are added to the _gattDevices list
                devicesList.Add(device);
            ListaDispositivos.ItemsSource = devicesList;
            await _adaptador.StopScanningForDevicesAsync();
            _Indicador_Ocupado.IsVisible = _Indicador_Ocupado.IsRunning = !(Boton_Escaneo.IsEnabled = true);         // Switch off the busy indicator
            LabelA.Text = "Please select Lily's device";
        }

        private async void ListaDispositivosEncontradosListView_ItemTapped(object sender, ItemTappedEventArgs e)   // Function that is run whenever a detected BLE device is selected
        {
            _dispositivoSelecionado = e.Item as IDevice;                                                       // The item selected is an IDevice (detected BLE device). Therefore we have to cast the selected item to an IDevice
            lbl_Dispositvo_seleccionado.Text = Convert.ToString(_dispositivoSelecionado.NativeDevice + " " + _dispositivoSelecionado.IsConnectable);
        }
    }
}
