using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.Tiling;

namespace MapsuiMauiZoomToBoundingBoxSampleApp
{
  public partial class MainPage : ContentPage
  {
    // The two locations I want to zoom in on the map.
    private readonly Location _devinCastleGpsLocation = new Location(48.17442695182697, 16.97759876457116);
    private readonly (double X, double Y) _devinCastleMercatorLocation;
    private readonly Location _bratislavaCastleGpsLocation = new Location(48.142273232469634, 17.100320628008593);
    private readonly (double X, double Y) _bratislavaCastleMercatorLocation;


    public MainPage()
    {
      InitializeComponent();

      this._zoomToBoundsButton.Text = 
        $"Zoom to {this._devinCastleGpsLocation.Latitude},{this._devinCastleGpsLocation.Longitude} - " + 
        $"{this._bratislavaCastleGpsLocation.Latitude}, {this._bratislavaCastleGpsLocation.Longitude}";

      // Convert GPS to Mercator projection.
      this._devinCastleMercatorLocation = SphericalMercator.FromLonLat(this._devinCastleGpsLocation.Longitude, this._devinCastleGpsLocation.Latitude);
      this._bratislavaCastleMercatorLocation = SphericalMercator.FromLonLat(this._bratislavaCastleGpsLocation.Longitude, this._bratislavaCastleGpsLocation.Latitude);

      // Setup the map and location pins.
      this._mapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
      var pinsLayer = new GenericCollectionLayer<List<IFeature>>
      {
        Style = SymbolStyles.CreatePinStyle()
      };
      pinsLayer.Features.Add(new GeometryFeature
      {
        Geometry = new NetTopologySuite.Geometries.Point(this._devinCastleMercatorLocation.X, this._devinCastleMercatorLocation.Y)
      });
      pinsLayer.Features.Add(new GeometryFeature
      {
        Geometry = new NetTopologySuite.Geometries.Point(this._bratislavaCastleMercatorLocation.X, this._bratislavaCastleMercatorLocation.Y)
      });
      this._mapControl.Map.Layers.Add(pinsLayer);
    }


    private void _zoomToBoundsButton_Clicked(object sender, EventArgs e)
    {
      // Compute the bounding box containing the two locations.
      double minX = Math.Min(this._devinCastleMercatorLocation.X, this._devinCastleMercatorLocation.X);
      double maxX = Math.Max(this._bratislavaCastleMercatorLocation.X, this._bratislavaCastleMercatorLocation.X);
      double minY = Math.Min(_devinCastleMercatorLocation.Y, _devinCastleMercatorLocation.Y);
      double maxY = Math.Max(_bratislavaCastleMercatorLocation.Y, _bratislavaCastleMercatorLocation.Y);

      // Grow it by 500 meters to ensure the pins are visible.
      var bounds = new MRect(minX, minY, maxX, maxY).Grow(500);

      // Zoom the map to the bounds.
      this._mapControl.Map.Navigator.ZoomToBox(box: bounds, boxFit: MBoxFit.Fit);
    }
  }

}
