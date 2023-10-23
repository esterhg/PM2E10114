using Plugin.Geolocator;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using PM2E10114.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static SQLite.SQLite3;

namespace PM2E10114
{
    public partial class MainPage : ContentPage
    {
        string base64Image = "";

        Plugin.Media.Abstractions.MediaFile photo = null;

        public MainPage()
        {
            InitializeComponent();
            this.Title = "IMAGEN";
            localizar();
            CheckAndRequestLocationPermission(); 
       

        }

        private async void Button_agregar(object sender, EventArgs e)
        {
            if (validarDatos())
            {
                byte[] imagenBytes = ConvertImageToByteArray();
                Sitios sitios = new Sitios
                {
                    ImagenBytes = imagenBytes,
                    Latitud = double.Parse(latitud.Text),
                    Longitud = double.Parse(longitud.Text),
                    Descripcion= descrip.Text

                };
                await App.SQLiteDB.Agreagar(sitios);
                descrip.Text = ""; 
                Img.Source = null;
                await DisplayAlert("Registro", "Se ha agregado de manera exitosa", "OK");
                



            }
            else
            {
                await DisplayAlert("Advertencia", "INGRESE TODOS LOS DATOS","OK");
            }
        }

        public bool validarDatos()
        {
            bool respuesta;
            if (string.IsNullOrEmpty(descrip.Text))
            {
                respuesta = false;
            }
            if (string.IsNullOrEmpty(Img.Source?.ToString()))
            {
                return false;
            }
            else
            {
                return true;
            }

            return respuesta;
        }
        private void Button_lista(object sender, EventArgs e)
        {
           
            Navigation.PushAsync(new ListaSitiosPage());
           
        }
        private void Button_salir(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }
        private async void Button_tomarFoto(object sender, EventArgs e)
        {
            try
            {
                photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Front,
                    Name = DateTime.Now.ToString(),
                    Directory = "ImgSitio",
                    SaveToAlbum = true,
                    PhotoSize = PhotoSize.Medium
                });

                if (photo != null)
                {

                    Img.Source = ImageSource.FromStream(() => { return photo.GetStream(); });
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", ex.Message.ToString(), "OK");
            }

        }
        private byte[] ConvertImageToByteArray()
        {
            
            if (photo != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    photo.GetStream().CopyTo(stream);
                    return stream.ToArray();
                }
            }
            return null; 
        }
        private async void CheckAndRequestLocationPermission()
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);

            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Permiso necesario", "Necesitamos tu ubicación para mostrar el mapa.", "OK");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
                status = results[Permission.Location];
                localizar();
            }

            if (status == PermissionStatus.Granted)
            {
                localizar();
            }
            else if (status != PermissionStatus.Unknown)
            {
                await DisplayAlert("Permiso denegado", "Has denegado el permiso de ubicación. Es posible que el mapa no funcione correctamente.", "OK");

            }
        }

        private async void localizar()
        {
            var locator = CrossGeolocator.Current;
            locator.DesiredAccuracy = 50;

            if (locator.IsGeolocationAvailable && locator.IsGeolocationEnabled)
            {
                if (!locator.IsListening)
                {
                    await locator.StartListeningAsync(TimeSpan.FromSeconds(1), 5);
                }
                locator.PositionChanged += (sender, args) =>
                {
                    var loc = args.Position;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        latitud.Text = loc.Latitude.ToString();
                        longitud.Text = loc.Longitude.ToString(); 
                       
                    });
                };
            }
        }
    }

}