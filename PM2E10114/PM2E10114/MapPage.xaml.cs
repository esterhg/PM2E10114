using System;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Essentials;
using System.IO;

namespace PM2E10114
{
    public partial class MapPage : ContentPage
    {
        private double currentLatitude; 
        private double currentLongitude;
        private byte[] selectedImageBytes;

        public MapPage(double latitud, double longitud, string descripcion)
        {
            InitializeComponent();

            Task.Factory.StartNew(CheckLocationPermission);
            MoveToLocation(latitud, longitud, descripcion);
           
            Task.Factory.StartNew(CheckLocationPermission);

            currentLatitude = latitud; 
            currentLongitude = longitud; 

            MoveToLocation(latitud, longitud, descripcion);
        }

        private async void ShareImage(object sender, EventArgs e)
        {
            if (selectedImageBytes != null)
            {
                
                string tempFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tempImage.png");
                File.WriteAllBytes(tempFilePath, selectedImageBytes);

                
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Compartir Imagen",
                    File = new ShareFile(tempFilePath)
                });
            }
        }


        private async void CheckLocationPermission()
        {
        var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

        if (status != Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {

                    await DisplayAlert("Permiso necesario", "Necesitamos tu ubicación para mostrar el mapa.", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                status = results[Permission.Location];
            });
        }

        if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
        {
            
        }
        else if (status != Plugin.Permissions.Abstractions.PermissionStatus.Unknown)
        {
            await DisplayAlert("Permiso denegado", "Has denegado el permiso de ubicación. Es posible que el mapa no funcione correctamente.", "OK");

        }
    }


    private void MoveToLocation(double latitud, double longitud, string descripcion)
        {
            var mapPosition = new Position(latitud, longitud);

            Device.BeginInvokeOnMainThread(() =>
            {
                formMap.MoveToRegion(MapSpan.FromCenterAndRadius(mapPosition, Distance.FromMiles(3)));

                var mapPin = new Pin
                {
                    Type = PinType.Place,
                    Position = mapPosition,
                    Label = descripcion
                };

                formMap.Pins.Add(mapPin);
            });
        }

        private async void MoveToCurrentLocation(object sender, EventArgs e)
        {
            
        }
    
    }
}
