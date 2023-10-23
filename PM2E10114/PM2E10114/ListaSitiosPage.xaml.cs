using Plugin.Geolocator.Abstractions;
using PM2E10114.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace PM2E10114
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListaSitiosPage : ContentPage
    {
        private object locationPin;
        private byte[] selectedImageBytes;


        public ListaSitiosPage()
        {
            InitializeComponent();
            LoadData();
        }
        private async void LoadData()
        {
            var sitios = await App.SQLiteDB.GetSitiosAsync();

            lstSitios.ItemsSource = sitios;


        }

        private async void Is_ItemSelect(object sender, SelectedItemChangedEventArgs e)
        {
            var obj = (Sitios)e.SelectedItem;
            btnEliminar.IsVisible = false;
            btnMapa.IsVisible = false;

            if (!string.IsNullOrEmpty(obj.ID.ToString()))
            {
                var sitio = await App.SQLiteDB.GetSitiosByIdAsync(obj.ID);
                if (sitio != null)
                {
                    btnEliminar.IsVisible = true;
                    btnMapa.IsVisible = true;

                   
                    selectedImageBytes = sitio.ImagenBytes;
                }
            }
        }

        private async void Button_eliminar(object sender, EventArgs e)
        {
            var selectedSitio = lstSitios.SelectedItem as Sitios;

            if (selectedSitio != null)
            {
                await App.SQLiteDB.DeleteSitioAsync(selectedSitio.ID);
                LoadData();

               
               
            }
        }
        private async void OnItemSelect(object sender, SelectedItemChangedEventArgs e)
        {
           
                if (e.SelectedItem is Sitios sitioSeleccionado)
                {
                    await Navigation.PushAsync(new MapPage(sitioSeleccionado.Latitud, sitioSeleccionado.Longitud, sitioSeleccionado.Descripcion));
                }
        }

        private async void Ver_mapa(object sender, ItemTappedEventArgs e)
        {
            
        }
    }


}